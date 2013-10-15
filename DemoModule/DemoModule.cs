using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptingSDK;

namespace DemoModule
{
	public class DemoModule : ModuleBase
	{
		public override string Namespace
		{
			get
			{
				return "DemoModule";
			}
		}

		[ModuleMethod()]
		public double GetValue(double input)
		{
			return 42.42 + input;
		}

		public override void Dispose()
		{
			
		}
	}
}
