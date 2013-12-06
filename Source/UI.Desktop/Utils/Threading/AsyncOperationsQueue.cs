using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;

namespace UI.Desktop.Utils
{
	public class AsyncOperationsQueue<TOperationResult>
	{
		#region OperationArg
		private class OperationArg
		{
			private readonly object _completeCallback;
			public object CompleteCallback
			{
				get
				{
					return _completeCallback;
				}
			}

			private readonly CancelationToken _cancelationToken;
			public CancelationToken CancelationToken
			{
				get
				{
					return _cancelationToken;
				}
			}

			private readonly Func<TOperationResult> _operation;
			public Func<TOperationResult> Operation
			{
				get
				{
					return _operation;
				}
			}

			private readonly bool _useAsyncCallback;
			public bool UseAsyncCallback
			{
				get
				{
					return _useAsyncCallback;
				}
			}

			public OperationArg(object completeCallback, Func<TOperationResult> operation, 
				CancelationToken cancelationToken, bool useAsyncCallback)
			{
				_operation = operation;
				_completeCallback = completeCallback;
				_cancelationToken = cancelationToken;
				_useAsyncCallback = useAsyncCallback;
			}
		}
		#endregion

		private int _threadsLimit;
		private volatile bool _stop;
		private Dispatcher _dispatcher;
		private JobQueue<OperationArg> _jobQueue; 

		public CancelationToken EnqueueOperation(Action<TOperationResult> operationCompleteCallback, Func<TOperationResult> operation)
		{
			return InternalEnqueueOperation(operationCompleteCallback, operation, true);
		}

		public CancelationToken EnqueueOperation(Action<TOperationResult, CancelationToken> operationCompleteCallback, Func<TOperationResult> operation,
			bool useAsyncCallback)
		{
			return InternalEnqueueOperation(operationCompleteCallback, operation, useAsyncCallback);
		}

		public CancelationToken EnqueueOperation(Action<TOperationResult> operationCompleteCallback, Func<TOperationResult> operation,
				bool useAsyncCallback)
		{
			return InternalEnqueueOperation(operationCompleteCallback, operation, useAsyncCallback);
		}


		private CancelationToken InternalEnqueueOperation(object operationCompleteCallback, Func<TOperationResult> operation, bool useAsyncCallback)
		{
			if (operationCompleteCallback == null)
			{
				throw new ArgumentNullException("operationCompleteCallback");
			}
			if (operationCompleteCallback is Action<TOperationResult, CancelationToken> && !useAsyncCallback)
			{
				throw new ArgumentException("Passing CancelationToken to operationCompleteCallback supported only with useAsyncCallback = True!");
			}

			CancelationToken cancelationToken = new CancelationToken();
			OperationArg arg = new OperationArg(operationCompleteCallback, operation, cancelationToken, useAsyncCallback);
			if (_threadsLimit > 0)
			{
				_jobQueue.AddJob(arg);
			}
			else
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(RunOperation), arg);
			}
			return cancelationToken;
		}


		private void RunOperation(object state)
		{
			if (_stop)
			{
				return;
			}

			OperationArg arg = (OperationArg)state;
			TOperationResult result = arg.Operation();

			if (!arg.CancelationToken.IsCanceled)
			{
				if (arg.UseAsyncCallback)
				{
					if (arg.CompleteCallback is Action<TOperationResult>)
					{
						_dispatcher.BeginInvoke((Action<TOperationResult>)arg.CompleteCallback, result);
					}
					else if (arg.CompleteCallback is Action<TOperationResult, CancelationToken>)
					{
						_dispatcher.BeginInvoke((Action<TOperationResult, CancelationToken>)arg.CompleteCallback, result, arg.CancelationToken);
					}
				}
				else
				{
					_dispatcher.Invoke((Action<TOperationResult>)arg.CompleteCallback, result);
				}
			}
		}

		private void RunJobQueue()
		{
			for (int threads = 0; threads < _threadsLimit; threads++)
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(new Action<object>(o =>
				{
					while (!_stop)
					{
						OperationArg jobArgs = _jobQueue.WaitForJob();
						RunOperation(jobArgs);
					}
				})), null);
			}
		}

		public AsyncOperationsQueue()
			: this(0)
		{
		}

		public AsyncOperationsQueue(int threadsLimit)
		{
			_threadsLimit = threadsLimit;
			_dispatcher = Dispatcher.CurrentDispatcher;

			if (_threadsLimit > 0)
			{
				_jobQueue = new JobQueue<OperationArg>();
				RunJobQueue();
			}
		}


		public void Stop()
		{
			if (_threadsLimit > 0)
			{
				_stop = true;
				_jobQueue.Clear();
			}
		}
	}
}
