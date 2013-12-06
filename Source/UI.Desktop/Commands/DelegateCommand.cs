using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UI.Desktop.Commands
{
    public class DelegateCommand : Command
    {
        private readonly Action<object> _execute;
        private readonly Func<object, bool> _canExcute;

        public override void Execute(object parameter)
        {
            _execute(parameter);
        }

        public override bool CanExecute(object parameter)
        {
            if (_canExcute == null)
            {
                return base.CanExecute(parameter);
            }
            return _canExcute(parameter);
        }

        public DelegateCommand(Action<object> execute, Func<object, bool> canExecute)
        {
            _execute = execute;
            _canExcute = canExecute;
        }

        public DelegateCommand(Action action)
            : this(o => action(), null)
        { 
        }
    }
}
