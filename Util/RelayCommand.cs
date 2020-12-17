using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace BlitFlashNet.Util
{
    /// <summary>
    /// Relay command used to bind model actions to interface
    /// </summary>
    public class RelayCommand : ICommand
    {
        /// <summary>
        /// The execute.
        /// </summary>
        private Action<object> execute;

        /// <summary>
        /// The can execute.
        /// </summary>
        private Predicate<object> canExecute;

        /// <summary>
        /// The can execute changed internal.
        /// </summary>
        private event EventHandler CanExecuteChangedInternal;

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">
        /// The execute.
        /// </param>
        public RelayCommand(Action<object> execute)
            : this(execute, DefaultCanExecute)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RelayCommand"/> class.
        /// </summary>
        /// <param name="execute">
        /// The execute.
        /// </param>
        /// <param name="canExecute">
        /// The can execute.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
            {
                throw new ArgumentNullException(nameof(execute));
            }

            if (canExecute == null)
            {
                throw new ArgumentNullException(nameof(canExecute));
            }

            this.execute = execute;
            this.canExecute = canExecute;
        }

        //public RelayCommand(Action saveTodo)
        //{
        //    this.saveTodo = saveTodo;
        //}

        /// <summary>
        /// The can execute changed.
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                CommandManager.RequerySuggested += value;
                this.CanExecuteChangedInternal += value;
            }

            remove
            {
                CommandManager.RequerySuggested -= value;
                this.CanExecuteChangedInternal -= value;
            }
        }

        /// <summary>
        /// The can execute.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CanExecute(object parameter)
        {
            return this.canExecute != null && this.canExecute(parameter);
        }

        /// <summary>
        /// The execute.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        public void Execute(object parameter)
        {
            this.execute(parameter);
        }

        /// <summary>
        /// The on can execute changed.
        /// </summary>
        public void OnCanExecuteChanged()
        {
            EventHandler handler = this.CanExecuteChangedInternal;
            handler?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// The destroy.
        /// </summary>
        public void Destroy()
        {
            this.canExecute = _ => false;
            this.execute = _ => { return; };
        }

        /// <summary>
        /// The default can execute.
        /// </summary>
        /// <param name="parameter">
        /// The parameter.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        private static bool DefaultCanExecute(object parameter)
        {
            return true;
        }
    }
}
