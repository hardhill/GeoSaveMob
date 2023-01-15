using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LCadLau2.Classes
{
    public class RelayCommand : ICommand
    {
        private readonly WeakAction _execute;

        private readonly WeakFunc<bool> _canExecute;

        //
        // Сводка:
        //     Occurs when changes occur that affect whether the command should execute.
        public event EventHandler CanExecuteChanged;

        //
        // Сводка:
        //     Initializes a new instance of the RelayCommand class that can always execute.
        //
        // Параметры:
        //   execute:
        //     The execution logic. IMPORTANT: If the action causes a closure, you must set
        //     keepTargetAlive to true to avoid side effects.
        //
        //   keepTargetAlive:
        //     If true, the target of the Action will be kept as a hard reference, which might
        //     cause a memory leak. You should only set this parameter to true if the action
        //     is causing a closure. See http://galasoft.ch/s/mvvmweakaction.
        //
        // Исключения:
        //   T:System.ArgumentNullException:
        //     If the execute argument is null.
        public RelayCommand(Action execute, bool keepTargetAlive = false)
            : this(execute, null, keepTargetAlive)
        {
        }

        //
        // Сводка:
        //     Initializes a new instance of the RelayCommand class.
        //
        // Параметры:
        //   execute:
        //     The execution logic. IMPORTANT: If the action causes a closure, you must set
        //     keepTargetAlive to true to avoid side effects.
        //
        //   canExecute:
        //     The execution status logic. IMPORTANT: If the func causes a closure, you must
        //     set keepTargetAlive to true to avoid side effects.
        //
        //   keepTargetAlive:
        //     If true, the target of the Action will be kept as a hard reference, which might
        //     cause a memory leak. You should only set this parameter to true if the action
        //     is causing a closures. See http://galasoft.ch/s/mvvmweakaction.
        //
        // Исключения:
        //   T:System.ArgumentNullException:
        //     If the execute argument is null.
        public RelayCommand(Action execute, Func<bool> canExecute, bool keepTargetAlive = false)
        {
            if (execute == null)
            {
                throw new ArgumentNullException("execute");
            }

            _execute = new WeakAction(execute, keepTargetAlive);
            if (canExecute != null)
            {
                _canExecute = new WeakFunc<bool>(canExecute, keepTargetAlive);
            }
        }

        //
        // Сводка:
        //     Raises the GalaSoft.MvvmLight.Command.RelayCommand.CanExecuteChanged event.
        public void RaiseCanExecuteChanged()
        {
            this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        //
        // Сводка:
        //     Defines the method that determines whether the command can execute in its current
        //     state.
        //
        // Параметры:
        //   parameter:
        //     This parameter will always be ignored.
        //
        // Возврат:
        //     true if this command can be executed; otherwise, false.
        public bool CanExecute(object parameter)
        {
            if (_canExecute != null)
            {
                if (_canExecute.IsStatic || _canExecute.IsAlive)
                {
                    return _canExecute.Execute();
                }

                return false;
            }

            return true;
        }

        //
        // Сводка:
        //     Defines the method to be called when the command is invoked.
        //
        // Параметры:
        //   parameter:
        //     This parameter will always be ignored.
        public virtual void Execute(object parameter)
        {
            if (CanExecute(parameter) && _execute != null && (_execute.IsStatic || _execute.IsAlive))
            {
                _execute.Execute();
            }
        }
    }
}
