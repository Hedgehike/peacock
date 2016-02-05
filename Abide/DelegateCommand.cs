using System;
using System.Windows.Input;

namespace Abide
{
    public class DelegateCommand : ICommand
    {
        public DelegateCommand(Action action)
        {
            Action = action;
        }

        public Action Action { get; set; }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            Action.Invoke();
            ;
        }

        public event EventHandler CanExecuteChanged;
    }
}