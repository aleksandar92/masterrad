using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFSParserOWL2.Parser
{
	interface IHandler
	{
		void StartDocument(string filePath);
		void StartElement(string localName, string qName, SortedList<string, string> atts);
		void EndElement(string localName, string qName);
		void StartPrefixMapping(string prefix, string uri);
		void Characters(string text);
		void EndDocument();
	}
}
