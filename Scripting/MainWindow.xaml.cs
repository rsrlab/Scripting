using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Microsoft.Windows.Controls.Ribbon;
using System.IO;
using Microsoft.Win32;
using Scripting.Execution;
using ICSharpCode.AvalonEdit.CodeCompletion;
using Scripting.Editor;
using System.Xml;

namespace Scripting
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : RibbonWindow, IMainWindow
	{
		ControllerMain _ctr;
		CsvReport _report;
		BuildInMod.BasicModule _basicModule;

		Editor.BreakLineHighlight _breakLineHighlight;

		HelpGenerator _helpGen;

		CompletionWindow _completionWindow;

		private void LoadSyntaxHighlight()
		{
			using (Stream s = new MemoryStream(Properties.Resources.LuaSyntax, false))
			{
				using (XmlTextReader reader = new XmlTextReader(s))
				{
					textScript.SyntaxHighlighting = ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load
					  (reader, ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance);
				}
			}
		}

		public MainWindow()
		{
			InitializeComponent();

			App.Current.Exit += new ExitEventHandler(Current_Exit);

			// Insert code required on object creation below this point.

			_breakLineHighlight = new Editor.BreakLineHighlight();
			textScript.TextArea.TextView.LineTransformers.Add(_breakLineHighlight);
			textScript.TextArea.TextEntered += new TextCompositionEventHandler(TextArea_TextEntered);
			textScript.TextArea.TextEntering += new TextCompositionEventHandler(TextArea_TextEntering);

			LoadSyntaxHighlight();

			_report = new CsvReport(DataOutput);
			_basicModule = new BuildInMod.BasicModule(WriteOutputFunc, _report);
			_ctr = new ControllerMain(this, _basicModule);

			textScript.TextChanged += new EventHandler(textScript_TextChanged);

			ButtonNew.Command = _ctr.CmdNewFile;
			ButtonOpen.Command = _ctr.CmdLoadFile;
			ButtonSave.Command = _ctr.CmdSaveFile;
			ButtonSaveAs.Command = _ctr.CmdSaveAsFile;

			ButtonRun.Command = _ctr.CmdPlay;
			ButtonPause.Command = _ctr.CmdPause;
			ButtonStop.Command = _ctr.CmdStop;


			_helpGen = _ctr.CreateHelp();
		}

		void Current_Exit(object sender, ExitEventArgs e)
		{
			Properties.Settings.Default.Save();
		}

		void TextArea_TextEntering(object sender, TextCompositionEventArgs e)
		{
			if (e.Text.Length > 0 && _completionWindow != null)
			{
				if (!char.IsLetterOrDigit(e.Text[0]))
				{
					// Whenever a non-letter is typed while the completion window is open,
					// insert the currently selected element.
					_completionWindow.CompletionList.RequestInsertion(e);
				}
			}
		}

		private string getlastword(string text)
		{
			int start = 0;
			for (int i = text.Length - 1; i > 0; i--)
			{
				if (text[i] == ' ')
					start = i;
			}

			return text.Substring(start, text.Length - start - 1).Trim();
		}

		private void TextArea_TextEntered(object sender, TextCompositionEventArgs e)
		{
			ICSharpCode.AvalonEdit.Editing.TextArea textArea = sender as ICSharpCode.AvalonEdit.Editing.TextArea;

			if (e.Text == ".")
			{
				int line = textArea.Caret.Line;
				int offset = textArea.Document.GetOffset(line, 0);
				int length = textArea.Caret.Offset - offset;
				string lineText = textArea.Document.GetText(offset, length);

				string comletionWord = getlastword(lineText);

				if (Char.IsNumber(comletionWord, 0) == false)
				{
					_completionWindow = new CompletionWindow(textArea);
					_completionWindow.Width = Math.Max(textScript.ActualWidth / 2, 250);
					IList<ICompletionData> data = _completionWindow.CompletionList.CompletionData;

					FillCompletionData(comletionWord, _completionWindow.CompletionList.CompletionData);

					if (data.Count > 0)
					{
						_completionWindow.Show();
						_completionWindow.Closed += delegate
						{
							_completionWindow = null;
						};
					}
				}
			}
		}

		private void FillCompletionData(string comletionWord, IList<ICompletionData> list)
		{
			if (_helpGen.HelpBase.ContainsKey(comletionWord))
			{
				foreach (string i in _helpGen.HelpBase[comletionWord])
				{
					list.Add(new CompletionDataFunction(i));
				}
			}
		}

		void textScript_TextChanged(object sender, EventArgs e)
		{
			_ctr.TextChanged();
		}

		private void WriteOutputFunc(string msg)
		{
			(this as Control).EnsureThread(() =>
			{
				AddLogString(msg);
			});
		}

		public void AddLogString(string output)
		{
			(this as Control).EnsureThread(() =>
			{
				ListBoxItem item = new ListBoxItem();
				item.Content = output;

				int idx = listOutput.Items.Add(item);
				listOutput.ScrollIntoView(item);
			});
		}

		private void ButtonClearLog_Click(object sender, RoutedEventArgs e)
		{
			listOutput.Items.Clear();
		}

		private void ButtonCopyLog_Click(object sender, RoutedEventArgs e)
		{
			StringBuilder result = new StringBuilder();

			foreach (var i in listOutput.Items)
			{
				result.AppendLine((i as ListBoxItem).Content.ToString());
			}

			Clipboard.SetText(result.ToString());
		}

		private void MenuExit_Click(object sender, RoutedEventArgs e)
		{
			this.Close();
		}

		private void Ribbon_SelectionChanged(object sender, SelectionChangedEventArgs e)
		{
			if (Ribbon.SelectedItem == HomeTab)
			{
				GridEditor.Visibility = System.Windows.Visibility.Visible;
				GridProtocol.Visibility = System.Windows.Visibility.Collapsed;
			}
			else
			{
				GridEditor.Visibility = System.Windows.Visibility.Collapsed;
				GridProtocol.Visibility = System.Windows.Visibility.Visible;
			}
		}

		private void ButtonSaveProtocol_Click(object sender, RoutedEventArgs e)
		{
			// REPORT

			SaveFileDialog save = new SaveFileDialog();
			save.Filter = "CSV file (*.csv)|*.csv";
			if (save.ShowDialog() == true)
			{
				_report.Save(save.FileName);
			}
		}

		private void ButtonSaveProtocolDump_Click(object sender, RoutedEventArgs e)
		{
			_report.Dump(".");
		}

		#region IMainWindow Members

		public string GetEditorContent()
		{
			return textScript.Text;
		}

		public void SetEditorContent(string content)
		{
			textScript.Text = content;
		}

		public void InvokeProc(Action threadCriticalAction)
		{
			this.EnsureThread(threadCriticalAction);
		}

		public void HighLightCodeLine(int line, bool isEnabled)
		{
			(this as Control).EnsureThread(() =>
			{
				_breakLineHighlight.IsEnabled = isEnabled;
				_breakLineHighlight.LineNumber = line;

				if (isEnabled)
				{
					textScript.ScrollTo(line, 0);
				}

				textScript.TextArea.TextView.Redraw();
			});
		}

		#endregion
	}
}
