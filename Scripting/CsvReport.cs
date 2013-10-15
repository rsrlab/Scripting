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
		private System.Windows.Controls.DataGrid _listView;

		private int _colCount;

		DataTable _dataTable;

		public CsvReport(System.Windows.Controls.DataGrid listView)
		{
			_dataTable = new DataTable("report");
			_listView = listView;
			_listView.ItemsSource = _dataTable.DefaultView;
		}

		public void RefreshTable()
		{
			_listView.Items.Refresh();
		}

		public void ResetReport(int columns)
		{
			_listView.EnsureThread(() =>
			{
				_dataTable.Rows.Clear();
				_dataTable.Columns.Clear();
				_colCount = columns;

				for (int i = 0; i < _colCount; i++)
				{
					string colName = string.Format("Col{0}", i);
					DataColumn col = new DataColumn(colName);
					col.Caption = string.Format("Col {0}", i);
					col.DataType = typeof(string);
					col.DefaultValue = string.Empty;

					_dataTable.Columns.Add(col);
				}
			});
		}

		public void WriteValue(int row, int col, object value)
		{
			_listView.EnsureThread(() =>
			{
				while (row >= _dataTable.Rows.Count)
				{
					_dataTable.Rows.Add(new string[_colCount]);
				}

				_dataTable.Rows[row][col] = value.ToString();

				RefreshTable();
			});
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
			StringBuilder result = new StringBuilder();

			for (int line = 0; line < _dataTable.Rows.Count; line++)
			{
				for (int col = 0; col < _dataTable.Columns.Count; col++)
				{
					result.Append(_dataTable.Rows[line][col].ToString());
					if (col < (_dataTable.Columns.Count - 1))
					{
						result.Append(";");
					}
				}

				result.AppendLine();
			}

			System.IO.File.WriteAllText(fileName, result.ToString());
		}
	}
}
