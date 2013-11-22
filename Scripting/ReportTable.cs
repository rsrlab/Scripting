using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Data;

namespace Scripting
{
	class ReportTable : DataTable
	{
		const string defaultTableName = "CSVReportTable";
		const string columnNameFormat = "COL{0}";

		public ReportTable()
			: base(defaultTableName)
		{
			InitializeColumns(0);
		}

		public void InitializeColumns(int columns)
		{
			this.Rows.Clear();
			this.Columns.Clear();

			for(int i = 0; i < columns; i++)
			{
				this.Columns.Add(string.Format(columnNameFormat, i), typeof(string));
			}
		}

		public void WriteValue(int row, int col, object value)
		{
			if (col < Columns.Count)
			{
				while (row >= Rows.Count)
				{
					this.Rows.Add(new string[this.Columns.Count]);
				}

				this.Rows[row][col] = value.ToString();
			}
			else
			{
				// column not in range
				return;
			}
		}

		public void Save(string fileName, Encoding encoding)
		{
			using (StreamWriter writer = new StreamWriter(fileName, false, encoding))
			{
				for (int line = 0; line < Rows.Count; line++)
				{
					for (int col = 0; col < Columns.Count; col++)
					{
						writer.Write(Rows[line][col] ?? string.Empty);

						if (col < (Columns.Count - 1))
						{
							writer.Write(";");
						}
					}

					writer.WriteLine();
				}
			}
		}
	}
}
