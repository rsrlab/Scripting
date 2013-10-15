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
using System.Windows.Shapes;

namespace Scripting
{
	/// <summary>
	/// Interaction logic for WindowErrorMsg.xaml
	/// </summary>
	public partial class WindowErrorMsg : Window
	{
		public WindowErrorMsg(string msg)
		{
			InitializeComponent();

			textError.Text = msg;
		}

		public static void ShowError(Exception exc)
		{
			ShowError(exc.Message);
		}

		public static void ShowError(string msg)
		{
			WindowErrorMsg errWnd = new WindowErrorMsg(msg);
			errWnd.ShowDialog();
		}
	}
}
