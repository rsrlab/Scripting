using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scripting.Execution
{
	class AppStateChangeEventArgs : EventArgs
	{
		public AppState ApplicationState { get; private set; }

		public int LastLine { get; private set; }

		public AppStateChangeEventArgs(AppState state, int line)
			: this(state)
		{
			LastLine = line;
		}

		public AppStateChangeEventArgs(AppState state)
		{
			ApplicationState = state;
		}
	}
}
