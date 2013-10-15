using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scripting.Editor
{
	class Document
	{
		public const string _defaultDocumentName = "document1.lua";

		public string DocumentName
		{
			get
			{
				return System.IO.Path.GetFileName(FileName);
			}
		}

		public string FileName { get; private set; }

		public bool IsChanged { get; private set; }

		public bool IsPathAssigned { get; private set; }

		public void DocumentChanged()
		{
			IsChanged = true;
		}

		public void SaveAs(string fileName, string content)
		{
			FileName = fileName;
			IsPathAssigned = true;

			Save(content);
		}

		public void Save(string content)
		{
			System.IO.File.WriteAllText(FileName, content);
			IsChanged = false;
		}

		public string Load(string fileName)
		{
			string result = System.IO.File.ReadAllText(fileName);

			FileName = fileName;
			IsPathAssigned = true;
			IsChanged = false;

			return result;
		}

		public void NewFile()
		{
			IsPathAssigned = false;
			IsChanged = false;
			FileName = _defaultDocumentName;
		}

		public Document()
		{
			FileName = _defaultDocumentName;
		}
	}
}
