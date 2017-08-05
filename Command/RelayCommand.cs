using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace first_mvvm.Command
{
    class RelayCommand :ICommand
    {
        Action<object> executeMethod;
        Func<object, bool> canexecuteMethod;
        //bool canExecuteCache;


        public RelayCommand(Action<object> executeMethod, Func<object, bool> canexecuteMethod /*, bool canExecuteCache*/)
        {
            this.executeMethod = executeMethod;
            this.canexecuteMethod = canexecuteMethod;
            //canExecuteCache = canExecuteCache;
        }

        public bool CanExecute(object parameter)
        {
            if(canexecuteMethod == null)
                return true;
            else
                return canexecuteMethod(parameter);
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                    CommandManager.RequerySuggested += value;
            }
            remove
            {
                    CommandManager.RequerySuggested -= value;
            }
        }

        public void Execute(object parameter)
        {
            executeMethod(parameter);
        }


    }
}
