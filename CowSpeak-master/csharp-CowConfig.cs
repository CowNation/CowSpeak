using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.ComponentModel;
using System.Globalization;
using System.Text;

namespace CowConfig
{
	class ReadConfig
	{
		private List< string > Lines = new List< string >();

		private void ReadAllLines(string FileName)
		{
			Lines = File.ReadAllLines(FileName).ToList();
		}

		private int FindElement(string Section, string offsetText)
		{
			Section = "[" + Section + "]";
			bool SectionFound = false;
			for (int i = 0; i < Lines.Count(); i++)
			{
				if (Lines[i] == Section || Section == "[]")
					SectionFound = true;
				else if (SectionFound && Lines[i].IndexOf("[") != -1 && Lines[i].IndexOf("]") != -1)
					break;
				if (SectionFound && Lines[i].IndexOf(offsetText) != -1)
					return i;
			}
			return -1;
		}

		private static T TryParse<T>(string inValue)
		{
			TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
			return (T)converter.ConvertFromString(null, CultureInfo.InvariantCulture, inValue);
		}

		public ReadConfig(string fileName)
		{
			if (!File.Exists(fileName))
				throw new Exception(fileName + " does not exist");

			ReadAllLines(fileName);
		}

		public List< string > GetLines()
		{
			return Lines;
		}

		public T Read<T>(string Section, string offsetText)
		{
			int lineIndex = FindElement(Section, offsetText);
			if (lineIndex == -1)
				return default(T);

			try
			{
				string temp = Lines[lineIndex];
				temp = temp.Replace(offsetText, "");
				return TryParse<T>(temp);
			}
			catch
			{
				return default(T);
			}
		}
	}

	class WriteConfig
	{
		private FileStream writeStream;

		public WriteConfig(string fileName)
		{
			if (!File.Exists(fileName))
				File.Create(fileName).Close();

			writeStream = File.OpenWrite(fileName);
		}

		~WriteConfig()
		{
			Close();
		}

		public void Close()
		{
			writeStream.Close();
		}

		private static void AddText(FileStream fileStream, string value)
		{
			byte[] info = new UTF8Encoding(true).GetBytes(value);
			fileStream.Write(info, 0, info.Length);
		}

		public void Section(string Text)
		{
			AddText(writeStream, "[" + Text + "]\n");
		}

		public void WriteLine(string offsetText, string Item)
		{
			AddText(writeStream, offsetText + Item + "\n");
		}
	}
}