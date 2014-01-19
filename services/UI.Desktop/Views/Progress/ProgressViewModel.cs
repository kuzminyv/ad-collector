using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Core.Entities;
using System.Windows.Input;
using UI.Desktop.Commands;
using System.Web;
using Core.BLL.Common;
using System.Threading;

namespace UI.Desktop.Views
{
	public class ProgressViewModel : ViewModel
	{
        private Action _cancelCallback;

        private int _progress;
        public int Progress        
        {
            get
            {
                return _progress;
            }
            set
            {
                if (_progress != value)
                {
                    _progress = value;
                    OnPropertyChanged("Progress");
                }
            }
        }

        private string _progressText;
        public string ProgressText
        {
            get
            {
                return _progressText;
            }
            set
            {
                if (_progressText != value)
                {
                    _progressText = value;
                    OnPropertyChanged("ProgressText");
                }
            }
        }

        private OperationState _operationState;
        public OperationState OperationState
        {
            get
            {
                return _operationState;
            }
            set
            {
                _operationState = value;
                ProgressText = string.Format("{0} of {1} {2}", 
                    _operationState.Progress, 
                    _operationState.ProgressTotal, 
                    _operationState.Description);
                Progress = (int)_operationState.ProgressPercentage;
            }
        }

        private bool _isCancelRequested;
        private DelegateCommand _cancelCommand;
		public ICommand CancelCommand
		{
			get
			{
                return _cancelCommand;
			}
		}


        public ProgressViewModel(Action cancelCallback)
		{
            _cancelCallback = cancelCallback;
            _cancelCommand = new DelegateCommand(CancelCommand_Execute, CancelCommand_CanExecute);
		}

        private void CancelCommand_Execute(object o)
        {
            _isCancelRequested = true;
            _cancelCommand.RaiseCanExecuteChanged();
            _cancelCallback();
        }

        private bool CancelCommand_CanExecute(object o)
        {
            return !_isCancelRequested;
        }

        public static void ExecuteAsyncOperation(Func<Action<OperationState>, Action, CancellationTokenSource> asyncOperation)
        {
            CancellationTokenSource cancelationToken = null; ;
            ProgressViewModel model = new ProgressViewModel(() =>
            {
                if (cancelationToken != null)
                {
                    cancelationToken.Cancel();
                }
            });
            ProgressWindowView progressWindow = new ProgressWindowView(model);
            progressWindow.Loaded += (s, e) =>
            {
                cancelationToken = asyncOperation(
                    state => App.AppContext.Dispatcher.BeginInvoke(new Action(() => 
                    {
                        if (state.Exception != null) {
                            progressWindow.Close();
                            throw new Exception("Exception in background thread", state.Exception);
                        }
                        model.OperationState = state; 
                    })),
                    () => App.AppContext.Dispatcher.BeginInvoke(new Action(progressWindow.Close)));
            };
            progressWindow.SetCurrentWindowAsOwner();
            progressWindow.ShowDialog();
        }
	}
}
