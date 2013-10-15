using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScriptingSDK;

namespace Scripting.BuildInMod
{
	class BasicModule : ModuleBase
	{
		public delegate void PrintFuncType(string msg);
		public delegate void CsvFuncType(int row, int col, object value);

		private PrintFuncType _print;
		private BuildInMod.ICsvReport _csvFunc;
		
		private System.Random _rnd = new Random();

		public BasicModule(PrintFuncType printFunc,
			BuildInMod.ICsvReport csvFunc)
		{
			_print = printFunc;
			_csvFunc = csvFunc;
		}

		public BasicModule()
		{
			_print = null;
			_csvFunc = null;
		}

		public override string Namespace
		{
			get { return "Base"; }
		}

		[ModuleMethod]
		public void MessageBox(string message, string title)
		{
			System.Windows.MessageBox.Show(message, title);
		}

		[ModuleMethod]
		public void Print(string message)
		{
			_print(message);
		}

		[ModuleMethod]
		public double Random()
		{
			
			return _rnd.NextDouble();
		}

		[ModuleMethod]
		public void DelayMs(int ms)
		{
			System.Threading.Thread.Sleep(ms);
		}

		[ModuleMethod]
		public void ResetCSV(int col)
		{
			_csvFunc.ResetReport(col);
		}

		[ModuleMethod]
		public void WriteCSV(int row, int col, object value)
		{
			_csvFunc.WriteValue(row, col, value);
		}

		public override void Dispose()
		{
			
		}
	}
}
