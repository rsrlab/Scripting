using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScriptingSDK
{
	public abstract class ModuleBase : IDisposable
	{
		public abstract string Namespace { get; }

		public abstract void Dispose();
	}
}
