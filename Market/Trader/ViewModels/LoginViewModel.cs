using System;
using System.Windows.Input;
using Trader.Common;

namespace Trader.ViewModels
{
    public class LoginViewModel
    {
        public ICommand LoginCommand => new CommandHandler(Login);

        public void Login()
        {
            // call login service to authenticate...
        }
    }
}
