using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Core.BLL;
using UI.Desktop.Utils;
using UI.Desktop.Views;
using System.Threading;
using Core.BLL.Common;

namespace UI.Desktop.Commands
{
	public class ExportCommand : Command
	{
		public override void Execute(object parameter)
		{
            if (parameter.ToString() == "JSON")
            {
                FolderBrowserDialog dialog = new FolderBrowserDialog();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    Managers.ExportManager.ExportToJson(dialog.SelectedPath);
                    MessageBox.Show("Export Completed.", "Ad Collector");
                }
            }
            else if (parameter.ToString() == "SQL")
            {
                ProgressViewModel.ExecuteAsyncOperation(Managers.ExportManager.ExportToSqlAsync);
            }
            else if (parameter.ToString() == "Binary")
            {
                ProgressViewModel.ExecuteAsyncOperation(Managers.ExportManager.ExportToBinaryAsync);
            }
		}
	}
}
