using RDFSParserOWL2.Model;
using RDFSParserOWL2.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RDFSParserOWL2
{
	 public class ParserToOWL2
	{
		private Profile profile;

		public Profile Profile
		{
			get { return profile; }
			set { profile = value; }
		}

		 public void ParseDocument(string path)
		 {
			 RDFSXMLReaderHandler handler = new RDFSXMLReaderHandler();
             using (FileStream fs = new FileStream(@"E:\Aleksandar Popovic\rdfs\GeographicalLocationProfileRDFSAugmented-v2_4_15-7Aug2014.rdf", FileMode.Open))
			 {
				 bool succes;
				 TimeSpan ts;
				XMLParser.DoParse(handler,fs,"GeographicalLocationProfileRDFSAugmented-v2_4_15-7Aug2014",out succes,out ts);
				Profile = handler.Profile;
			 
			 }
			 
			 //XmlReaderSettings settings = new XmlReaderSettings();
			 //settings.IgnoreComments = true;
			 //settings.IgnoreWhitespace = true;
			 //XmlTextReader readertext = new XmlTextReader(path);
			 //RDFSXMLReaderHandler handle = new RDFSXMLReaderHandler();
			 //using (XmlReader reader =  XmlReader.Create(readertext,settings))
			 //{
			 //	string localName = string.Empty;
			 //	string name = string.Empty;
			 //	bool isEmptyElement = false;
			 //	SortedList<string, string> atts = null;
			 //	handle.StartDocument(path);
			 //	while (reader.Read())
			 //	{
			 //		reader.MoveToElement();
			 //		switch (reader.NodeType)
			 //		{
			 //			case XmlNodeType.Element:
			 //				{
			 //					atts = new SortedList<string, string>();
			 //					localName = reader.LocalName;
			 //					name = reader.Name;
			 //					isEmptyElement = reader.IsEmptyElement;
			 //					if (reader.HasAttributes)
			 //					{
			 //						atts = GetAttributes(reader);
			 //					}
			 //					handle.StartElement(localName, name, atts);
			 //					if (isEmptyElement)
			 //					{
			 //					   handle.EndElement(localName, name);
			 //					}
			 //					break;
			 //				}
			 //			case XmlNodeType.EndElement:
			 //				{
			 //					handle.EndElement(reader.LocalName, reader.Name);
			 //					break;
			 //				}
			 //			case XmlNodeType.Text:
			 //				{
			 //					handle.Characters(reader.Value.ToString());
			 //					break;
			 //				}
			 //		}
			 //	}
			 //	handle.EndDocument();
			 //}

		 }


		 //private static SortedList<string, string> GetAttributes(XmlReader reader)
		 //{
		 //	SortedList<string, string> atts = new SortedList<string, string>();
		 //	if (reader.HasAttributes)
		 //	{
		 //		reader.MoveToFirstAttribute();
		 //		do
		 //		{
		 //			if (!atts.ContainsKey(reader.Name))
		 //			{
		 //				atts.Add(reader.Name, reader.Value.ToString());
		 //			}
		 //		} while (reader.MoveToNextAttribute());
		 //	}
		 //	return atts;
		 //}
	}
}
