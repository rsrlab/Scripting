using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;

namespace Scripting
{
	public static class WindowCrossThreadHelper
	{
		public static void EnsureThread(this Control control, Action threadCriticalAction)
		{
			if (System.Windows.Threading.Dispatcher.CurrentDispatcher != control.Dispatcher)
			{
				control.Dispatcher.Invoke(threadCriticalAction);
			}
			else
			{
				threadCriticalAction();
			}
		}
	}
}
