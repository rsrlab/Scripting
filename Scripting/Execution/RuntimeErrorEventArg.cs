using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scripting.Execution
{
	class RuntimeErrorEventArg : EventArgs
	{
		public Exception RuntimeException { get; private set; }

		public RuntimeErrorEventArg(Exception exc)
		{
			RuntimeException = exc;
		}
	}
}
