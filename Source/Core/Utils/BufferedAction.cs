using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Core.Utils
{
    public class BufferedAction
    {
        private static Dictionary<Delegate, Timer> timers =
            new Dictionary<Delegate, Timer>();

        private static object _lockObject = new object();
        private static List<Thread> _threadsInProgress = new List<Thread>();

        public static void DelayAction(Action action, int delayMilleseconds)
        {
            DelayAction(action, new TimeSpan(0, 0, 0, 0, delayMilleseconds));
        }

        public static void DelayAction(Action action, TimeSpan delay)
        {
            lock (_lockObject)
            {
                Timer timer;
                if (!timers.TryGetValue(action, out timer))
                {
                    timer = new Timer(EventTimerCallback, action,
                        Timeout.Infinite,
                        Timeout.Infinite);
                    timers.Add(action, timer);
                }
                timer.Change(delay, TimeSpan.FromMilliseconds(-1));
            }
        }

        public static void WaitAll()
        {
            List<Action> actionsToWait;
            lock (_lockObject)
            {
                actionsToWait = timers.Select(kvp => (Action)kvp.Key).ToList();

                foreach (var timer in timers.Values)
                {
                    timer.Dispose();
                }
                timers.Clear();
            }

            foreach (var action in actionsToWait)
            {
                action();
            }

            List<Thread> threadsToWait;
            lock (_threadsInProgress)
            {
                threadsToWait = _threadsInProgress.ToList();
            }

            foreach (var thread in threadsToWait)
            {
                thread.Join();
            }
        }

        private static void EventTimerCallback(object state)
        {
            var action = (Action)state;
            bool timerExists;
            lock (_lockObject)
            {
                Timer timer;
                timerExists = timers.TryGetValue(action, out timer);
                if (timerExists)
                {
                    timers.Remove(action);
                    timer.Dispose();
                }
            }

            if (timerExists)
            {
                lock (_threadsInProgress)
                {
                    _threadsInProgress.Add(Thread.CurrentThread);
                }
                try
                {
                    action();
                }
                finally
                {
                    lock (_threadsInProgress)
                    {
                        _threadsInProgress.Remove(Thread.CurrentThread);
                    }
                }
            }
        }
    }
}
