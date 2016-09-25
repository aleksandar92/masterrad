using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFSParserOWL2.Parser.Handler
{
	public class WordsXMLHandler:IHandler
	{
		private List<string> words;
		private string value;

		public List<string> Words
		{
			get { return words; }
			set { words = value; }
		}


		public void StartDocument(string filePath)
		{
			words = new List<string>();
		}

		public void StartElement(string localName, string qName, SortedList<string, string> atts)
		{
			//throw new NotImplementedException();
		}

		public void EndElement(string localName, string qName)
		{
			if (localName.Equals("word"))
			{
				words.Add(value);
			}
			value = string.Empty;
			//throw new NotImplementedException();
		}

		public void StartPrefixMapping(string prefix, string uri)
		{
			throw new NotImplementedException();
		}

		public void Characters(string text)
		{
			value = text;
			//throw new NotImplementedException();
		}

		public void EndDocument()
		{
			//throw new NotImplementedException();
		}
	}
}
