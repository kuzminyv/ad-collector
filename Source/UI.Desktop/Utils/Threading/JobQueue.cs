using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace UI.Desktop.Utils
{
	public class JobQueue<TJob>
	{
		private readonly object _lock = new object();
		private readonly Queue<TJob> _jobQueue = new Queue<TJob>();

		public TJob WaitForJob()
		{
			lock (_lock)
			{
				while (_jobQueue.Count == 0)
				{
					Monitor.Wait(_lock); 
				}

				return _jobQueue.Dequeue();
			}
		}

		public void AddJob(TJob action)
		{
			lock (_lock)
			{
				_jobQueue.Enqueue(action);
				Monitor.Pulse(_lock); 
			}
		}

		internal void Clear()
		{
			lock (_lock)
			{
				_jobQueue.Clear();
			}
		}
	}
}
