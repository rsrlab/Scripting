using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Data;
using System.Windows.Data;

namespace Scripting
{
	class CsvReport : BuildInMod.ICsvReport
	{
		private System.Windows.Controls.ListView _listView;

		ReportTable _table;

		public CsvReport(System.Windows.Controls.ListView listView)
		{
			_table = new ReportTable();
			_listView = listView;
			_listView.ItemsSource = _table.DefaultView;
		}

		public void RefreshTable()
		{
			_listView.EnsureThread(() =>
			{
				_listView.Items.Refresh();
			});
		}

		public void ResetReport(int columns)
		{
			_listView.EnsureThread(() =>
			{
				GridView view = (_listView.View as GridView);
		
				_table.InitializeColumns(columns);

				view.Columns.Clear();
				for (int i = 0; i < columns; i++)
				{
					GridViewColumn col = new GridViewColumn();
					col.Header = string.Format("Col {0}", i);
					col.DisplayMemberBinding = new Binding(string.Format("[{0}]", i));

					view.Columns.Add(col);
				}
			});
		}

		public void WriteValue(int row, int col, object value)
		{
			lock (_table)
			{
				_table.WriteValue(row, col, value);
			}

			RefreshTable();
		}

		public void Dump(string path)
		{
			string fileName;

			do
			{
				DateTime now = DateTime.Now;
				string name = string.Format("{0:D2}.{1:D2}.{2:D4}_{3:D2}{4:D2}{5:D2}.csv",
					now.Day, now.Month, now.Year, now.Hour, now.Minute, now.Second);

				fileName = System.IO.Path.Combine(path, name);

			} while (System.IO.File.Exists(fileName) == true);

			Save(fileName);
		}

		public void Save(string fileName)
		{
			lock (_table)
			{
				_table.Save(fileName, Encoding.UTF8);
			}
		}
	}
}
