using System;
using System.Windows.Input;

namespace StoryCanvas.Shared.ViewTools
{
	public class RelayCommand<T> : ICommand
	{
		protected readonly Action<T> _execute;          // Action: 戻り値void
		protected readonly Predicate<T> _canExecute;        // Predicate: Funcの特別な形。戻り値bool

		public RelayCommand(Action<T> execute) : this(execute, null)
		{
		}

		public RelayCommand(Action<T> execute, Predicate<T> canExecute)
		{
			if (execute == null)
			{
				throw new ArgumentNullException("canExecute is null - " + this.GetType());
			}
			this._execute = execute;
			this._canExecute = canExecute;
		}

		public virtual void Execute(object parameter)
		{
			if (parameter is T)
			{
				this._execute((T)parameter);
			}
		}

		public virtual bool CanExecute(object parameter)
		{
			if (this._canExecute != null)
			{
				if (parameter is T)
				{
					return this._canExecute((T)parameter);
				}
				else
				{
					return false;
				}
			}

			// canExecuteがそもそも設定されていない
			return true;
		}

		public event EventHandler CanExecuteChanged = delegate { };

		public void OnCanExecuteChanged()
		{
			this.CanExecuteChanged(this, new EventArgs());
		}
	}

	public class RelayCommand : RelayCommand<object>
	{

		public RelayCommand(Action<object> execute) : base(execute, null)
		{
		}

		public RelayCommand(Action<object> execute, Predicate<object> canExecute) : base(execute, canExecute)
		{
		}

		public override void Execute(object parameter)
		{
			this._execute(parameter);
		}

		public override bool CanExecute(object parameter)
		{
			if (this._canExecute != null)
			{
				return this._canExecute(parameter);
			}

			// canExecuteがそもそも設定されていない
			return true;
		}
	}
}
