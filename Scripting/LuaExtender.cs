using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptingSDK;
using System.Reflection;

namespace Scripting
{
	class LuaExtender : LuaInterface.Lua
	{
		private bool _isDisposed;

		private ModuleBase[] _modules;

		private void RegisterModule(ModuleBase module)
		{
			string nameSpace = module.Namespace;

			this.NewTable(nameSpace);

			foreach (var method in ModuleLoader.GetFunctionList(module))
			{
				string path = string.Format("{0}.{1}", nameSpace, method.Name);
				RegisterFunction(path, module, method);
			}
		}

		public LuaExtender(Type[] modules)
			: base()
		{
			_isDisposed = false;

			_modules = new ModuleBase[modules.Length];

			for (int i = 0; i < modules.Length; i++)
			{
				_modules[i] = Activator.CreateInstance(modules[i]) as ModuleBase;
				RegisterModule(_modules[i]);
			}
		}

		public override void Dispose()
		{
			if (_isDisposed == false)
			{
				for (int i = 0; i < _modules.Length; i++)
				{
					try
					{
						_modules[i].Dispose();
					}
					catch (Exception)
					{
					}

					_modules[i] = null;
				}

				_isDisposed = true;
			}

			base.Dispose();
		}

		public void AddModule(ModuleBase module)
		{
			RegisterModule(module);
		}
	}
}
