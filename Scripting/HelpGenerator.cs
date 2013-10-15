using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scripting
{
	class HelpGenerator
	{
		public Dictionary<string, string[]> HelpBase { get; private set; }

		public void Refresh(Type[] moduleTypes)
		{
			HelpBase = new Dictionary<string, string[]>();

			foreach (Type t in moduleTypes)
			{
				try
				{
					using (ScriptingSDK.ModuleBase mod = Activator.CreateInstance(t) as ScriptingSDK.ModuleBase)
					{
						HelpBase.Add(mod.Namespace, ModuleLoader.GetFunctionsHelp(mod));
					}
				}
				catch (Exception)
				{
				}
			}
		}

		public HelpGenerator()
		{

		}

		public HelpGenerator(Type[] moduleTypes)
			: this()
		{
			Refresh(moduleTypes);
		}

		public void FillListBox(System.Windows.Controls.ListBox HelpList)
		{
			List<string> fullHelpDump = new List<string>();

			foreach (string[] mod in HelpBase.Values)
			{
				fullHelpDump.AddRange(mod);
			}

			HelpList.Items.Clear();
			HelpList.ItemsSource = fullHelpDump;
		}
	}
}
