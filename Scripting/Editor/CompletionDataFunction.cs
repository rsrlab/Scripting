using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.CodeCompletion;

namespace Scripting.Editor
{
	class CompletionDataFunction : ICompletionData
	{
		#region ICompletionData Members

		public CompletionDataFunction(string text)
		{
			Text = text;
		}

		public void Complete(ICSharpCode.AvalonEdit.Editing.TextArea textArea, ICSharpCode.AvalonEdit.Document.ISegment completionSegment, EventArgs insertionRequestEventArgs)
		{
			textArea.Document.Replace(completionSegment, this.Text);
		}

		public object Content
		{
			get { return Text; }
		}

		public object Description
		{
			get { return string.Empty; }
		}

		public System.Windows.Media.ImageSource Image
		{
			get { return null; }
		}

		public double Priority
		{
			get { return 0; }
		}

		public string Text
		{
			get;
			private set;
		}

		#endregion
	}
}
