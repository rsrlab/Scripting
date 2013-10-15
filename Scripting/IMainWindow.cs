using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scripting
{
	interface IMainWindow
	{
		string GetEditorContent();
		void SetEditorContent(string content);
		void HighLightCodeLine(int line, bool isEnabled);
		void AddLogString(string msg);
		void InvokeProc(Action threadCriticalAction);
	}
}
