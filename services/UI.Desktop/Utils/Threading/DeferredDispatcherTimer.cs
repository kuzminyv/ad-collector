using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Threading;

namespace UI.Desktop.Utils
{
	public class DeferredDispatcherTimer : DeferredTimer
	{
		#region Fields
		private Dispatcher _dispatcher;
		private Action _timerElapsedCallback;
		#endregion

		#region Constructors
		public DeferredDispatcherTimer(int interval, Action timerElapsedCallback)
			: base (interval)
		{
			if (timerElapsedCallback == null)
			{
				throw new ArgumentNullException("timerElapsedCallback");
			}

			_timerElapsedCallback = timerElapsedCallback;
			_dispatcher = Dispatcher.CurrentDispatcher;
		}
		#endregion

		#region Methods
		protected override void OnDeferredTimerElapsed(object o)
		{
			_dispatcher.Invoke(new Action(() =>
				{
					base.OnDeferredTimerElapsed(o);
					_timerElapsedCallback();
				}));
		}
		#endregion
	}
}
