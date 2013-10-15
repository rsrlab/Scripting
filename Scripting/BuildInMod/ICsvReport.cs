using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scripting.BuildInMod
{
	interface ICsvReport
	{
		void ResetReport(int columns);
		void WriteValue(int row, int col, object value);
	}
}
