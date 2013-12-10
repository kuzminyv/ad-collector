using Core.BLL;
using Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UI.Desktop.Utils;
using UI.Desktop.Views;

namespace UI.Desktop.Commands
{
    public class RecreateAutoLinksCommand : Command
    {
        public override void Execute(object parameter)
        {
            ProgressViewModel.ExecuteAsyncOperation(Managers.AdHistoryManager.CreateHistoryAsync);
        }

        public RecreateAutoLinksCommand()
        {
        }
    }
}
