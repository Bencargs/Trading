using System;
using System.Windows.Input;

namespace Trader.Common
{
    public class CommandHandler : ICommand
    {
        public event EventHandler CanExecuteChanged;
        private readonly bool _canExecute;
        private readonly Action _action;


        public CommandHandler(Action action, bool canExecute = true)
        {
            _action = action;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute;
        }

        public void Execute(object parameter)
        {
            _action();
        }
    }
}
