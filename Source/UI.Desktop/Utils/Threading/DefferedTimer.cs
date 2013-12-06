using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace UI.Desktop.Utils
{
	public class DeferredTimer : IDisposable
	{
		#region Fields
		private Timer _timer;
		private readonly int _interval;
		#endregion

		#region Events
		public event EventHandler Elapsed;
		#endregion

		#region Constructors
		public DeferredTimer(int interval)
		{
			_interval = interval;
		}
		#endregion

		#region Methods
		protected virtual void OnDeferredTimerElapsed(object o)
		{
			if (Elapsed != null)
			{
				Elapsed(this, EventArgs.Empty);
			}
		}

		public void Defer()
		{
			if (_timer == null)
			{
				_timer = new Timer(new TimerCallback(OnDeferredTimerElapsed));
			}
			_timer.Change(_interval, Timeout.Infinite);
		}

		public void Stop()
		{
			if (_timer != null)
			{
				_timer.Dispose();
				_timer = null;
			}
		}
		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			Stop();
		}

		#endregion
	}

}
