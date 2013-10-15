using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace Scripting
{
	class CmdWrapper : ICommand
	{
		private Action<object> _command;
		private Func<object, bool> _isEnabled;

		private static bool alwaysEnabledStub(object p)
		{
			return true;
		}

		public CmdWrapper(Action<object> command)
			: this(command, alwaysEnabledStub)
		{
		}

		public CmdWrapper(Action<object> command, Func<object, bool> isEnabled)
		{
			_command = command;
			_isEnabled = isEnabled;
		}

		public void RiseCanExecuteChanged()
		{
			if (CanExecuteChanged != null)
			{
				CanExecuteChanged(this, new EventArgs());
			}
		}

		#region ICommand Members

		public bool CanExecute(object parameter)
		{
			return _isEnabled(parameter);
		}

		public event EventHandler CanExecuteChanged;

		public void Execute(object parameter)
		{
			_command(parameter);
		}

		#endregion
	}
}
