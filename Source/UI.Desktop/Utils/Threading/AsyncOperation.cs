using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Threading;
using System.ComponentModel;
using System.Windows.Threading;

namespace UI.Desktop.Utils
{
	public class AsyncOperation<TOperationArgument, TOperationResult>
	{
		#region Fields
		private Dispatcher _dispatcher;
		private DeferredTimer _deferredTimer;
		private List<CancelationToken> _cancelationTokens;
		private object _cancelationTokensSync;
		private TOperationArgument _deferredOperationArgument;
		private readonly Func<TOperationArgument, CancelationToken, TOperationResult> _operation;
		private readonly Action<TOperationResult> _operationCompleteCallback;
		private readonly bool _transferExceptionToUiThread;
		private readonly bool _runOperationInSTAThread;
		#endregion

		#region Events
		public event EventHandler<AsyncOperationResultEventArgs<TOperationResult>> Complete;
		#endregion

		#region OperationArg
		private class OperationArg<TArgument>
		{
			private readonly CancelationToken _cancelationToken;
			public CancelationToken CancelationToken
			{
				get
				{
					return _cancelationToken;
				}
			}

			private readonly TArgument _argument;
			public TArgument Argument
			{
				get
				{
					return _argument;
				}
			}

			public OperationArg(TArgument argument, CancelationToken cancelationToken)
			{
				_argument = argument;
				_cancelationToken = cancelationToken;
			}
		}
		#endregion

		private void OnComplete(TOperationResult result)
		{
			if (_operationCompleteCallback != null)
			{
				_dispatcher.Invoke(new Action(() =>
				{
					_operationCompleteCallback(result);
				}));
			}

			EventHandler<AsyncOperationResultEventArgs<TOperationResult>> complete = Complete;
			if (complete != null)
			{
				_dispatcher.Invoke(new Action(() =>
					complete(this, new AsyncOperationResultEventArgs<TOperationResult>(result))));
			}
		}

		public void RunAsync(TOperationArgument argument)
		{
			CancelAll();
			if (_deferredTimer == null)
			{
				RunImmediatly(argument);
			}
			else
			{
				RunDeferred(argument);
			}
		}

		public void RunSync(TOperationArgument argument)
		{
			TOperationResult result = _operation(argument, new CancelationToken());
			OnComplete(result);
		}

		private void RunDeferred(TOperationArgument argument)
		{
			_deferredOperationArgument = argument;
			_deferredTimer.Defer();
		}

		private void RunImmediatly(TOperationArgument argument)
		{
			CancelationToken token = new CancelationToken();
			lock (_cancelationTokensSync)
			{
				_cancelationTokens.Add(token);
			}

			if (_runOperationInSTAThread)
			{
				Thread thread = new Thread(new ParameterizedThreadStart(RunOperationAsync));
				thread.SetApartmentState(ApartmentState.STA);
				thread.Start(new OperationArg<TOperationArgument>(argument, token));
			}
			else
			{
				ThreadPool.QueueUserWorkItem(new WaitCallback(RunOperationAsync),
					new OperationArg<TOperationArgument>(argument, token));
			}
		}

		private void CancelAll()
		{
			_cancelationTokens.ForEach(t => t.IsCanceled = true);
		}

		private void deferredTimer_Elapsed(object sender, EventArgs e)
		{
			RunImmediatly(_deferredOperationArgument);
		}

		private void RunOperationAsync(object stateInfo)
		{
			OperationArg<TOperationArgument> arg = (OperationArg<TOperationArgument>)stateInfo;
			TOperationResult result = default(TOperationResult);
			try
			{
				result = _operation(arg.Argument, arg.CancelationToken);
			}
			catch (Exception ex)
			{
				if (_transferExceptionToUiThread)
				{
					_dispatcher.Invoke(new Action(() => { throw ex; }));
				}
				else
				{
					throw;
				}
			}
			finally
			{
				lock (_cancelationTokensSync)
				{
					_cancelationTokens.Remove(arg.CancelationToken);
				}
			}
			if (!arg.CancelationToken.IsCanceled)
			{
				OnComplete(result);
			}
		}

		public AsyncOperation(Func<TOperationArgument, CancelationToken, TOperationResult> operation, Action<TOperationResult> operationCompleteCallback = null,
			int operationDelay = 0, bool transferExceptionToUiThread = false, bool runOperationInSTAThread = false)
		{
			if (operation == null)
			{
				throw new ArgumentNullException("operation");
			}
			_dispatcher = Dispatcher.CurrentDispatcher;
			_operation = operation;
			_cancelationTokensSync = new object();
			_cancelationTokens = new List<CancelationToken>();
			_operationCompleteCallback = operationCompleteCallback;
			_transferExceptionToUiThread = transferExceptionToUiThread;
			_runOperationInSTAThread = runOperationInSTAThread;

			if (operationDelay > 0)
			{
				_deferredTimer = new DeferredTimer(operationDelay);
				_deferredTimer.Elapsed += deferredTimer_Elapsed;
			}
		}
	}
}
