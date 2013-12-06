using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.BLL.Common
{
    public static class TaskHelper
    {
        public static CancellationTokenSource ExecuteAsync(Action<Action<OperationState>, Action, CancellationToken> func, 
            Action<OperationState> stateChangedCallback, Action completedCallback)
        {
            var tokenSource = new CancellationTokenSource();
            var t = Task.Factory.StartNew(() => 
            {
                try
                {
                    func(stateChangedCallback, completedCallback, tokenSource.Token);
                }
                catch (Exception e)
                {
                    stateChangedCallback(new OperationState() { Exception = e });
                    Managers.LogEntriesManager.AddItem(Entities.SeverityLevel.Error, e.Message, e.StackTrace);
                }
            });
            return tokenSource;
        }
    }
}
