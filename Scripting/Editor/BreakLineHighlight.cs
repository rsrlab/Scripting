using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSharpCode.AvalonEdit.Rendering;
using System.Windows.Media;

namespace Scripting.Editor
{
	class BreakLineHighlight : DocumentColorizingTransformer
	{
		public bool IsEnabled { get; set; }

		public int LineNumber { get; set; }

		public BreakLineHighlight()
			: base()
		{
		}

		protected override void ColorizeLine(ICSharpCode.AvalonEdit.Document.DocumentLine line)
		{
			if ((line.LineNumber == LineNumber) && IsEnabled)
			{
				ChangeLinePart(line.Offset, line.EndOffset, element => element.TextRunProperties.SetBackgroundBrush(Brushes.Yellow));
			}
		}
	}
}
