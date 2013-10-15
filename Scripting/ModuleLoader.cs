using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptingSDK;
using System.Reflection;

namespace Scripting
{
	static class ModuleLoader
	{
        public static ModuleBase[] CreateInstances(Type[] types)
        {
            var result = from item in types
                         where (item.BaseType == typeof(ModuleBase))
                         select ((ModuleBase)Activator.CreateInstance(item));

            return result.ToArray();
        }

		public static IEnumerable<MethodInfo> GetFunctionList(ModuleBase module)
		{
			var methods = from item in module.GetType().GetMethods()
						  where (item.GetCustomAttributes(typeof(ModuleMethod), true).Length > 0)
						  select item;

			return methods;
		}

		public static Type[] LoadAllModules(string path)
		{
			List<Type> result = new List<Type>();

			try
			{

				foreach (string fileName in System.IO.Directory.EnumerateFiles(path, "*.dll"))
				{
					Assembly asm = Assembly.LoadFile(fileName);
					Type[] types = asm.GetTypes();

					var modules = from item in types
								  where (item.BaseType == typeof(ModuleBase))
								  select item;

					result.AddRange(modules);
				}
			}
			catch(Exception)
			{

			}

			return result.ToArray();
		}

		private static string GetShortTypeName(Type t)
		{
			if (t == typeof(Int32))
			{
				return "int";
			}
			else if (t == typeof(string))
			{
				return "string";
			}
			else if (t == typeof(void))
			{
				return "void";
			}
			else
			{
				return t.Name;
			}
		}

		public static string GetFunctionHelp(MethodInfo methodInfo, string nameSpace)
		{
			StringBuilder result = new StringBuilder();

			result.AppendFormat("{0}(", methodInfo.Name);

			ParameterInfo[] parameters = methodInfo.GetParameters();
			for(int i = 0; i < parameters.Length; i++)
			{
				bool isPeriod = (i < (parameters.Length - 1));

				result.AppendFormat(isPeriod ? "{0} {1}, " : "{0} {1}",
					GetShortTypeName(parameters[i].ParameterType),
					parameters[i].Name);
			}

			result.AppendFormat(") : {0}", GetShortTypeName(methodInfo.ReturnType));

			return result.ToString();
		}

		public static string[] GetFunctionsHelp(IEnumerable<MethodInfo> methods, string nameSpace)
		{
			var result = from item in methods
						 select GetFunctionHelp(item, nameSpace);

			return result.ToArray();
		}

		public static string[] GetFunctionsHelp(ModuleBase module)
		{
			var methods = GetFunctionList(module);
			return GetFunctionsHelp(methods, module.Namespace);
		}
	}
}
