using RDFSParserOWL2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFSParserOWL2.Parser.Handler
{
	public  class XMLNamespaceReaderHandler:IHandler
	{
		private List<Namespace> namespaces;


		private string id;
		private string value;
		private bool generated;
		private bool relative;
		private bool def;
		private string ns;
		private string prefix;

		public List<Namespace> Namespaces
		{
			get { return namespaces; }
			set { namespaces = value; }
		}

		public void StartDocument(string filePath)
		{
			namespaces = new List<Namespace>();
			//throw new NotImplementedException();
		}

		public void StartElement(string localName, string qName, SortedList<string, string> atts)
		{
			//atts.TryGetValue("id",out id);
			foreach(KeyValuePair<string,string> kv in atts) 
			{
				if (kv.Key.Equals("default")) 
				{
					def = Boolean.Parse(kv.Value.Trim());
				}else if(kv.Key.Equals("generate")) 
				{
					generated = Boolean.Parse(kv.Value.Trim());
				}else if(kv.Key.Equals("relative")) 
				{
					relative = Boolean.Parse(kv.Value.Trim());
				}else if(kv.Key.Equals("ns"))
				{
					ns=kv.Value.Trim();
				}else if(kv.Key.Equals("prefix")) 
				{
					prefix = kv.Value.Trim();
				}
			}
		}

		public void EndElement(string localName, string qName)
		{
			if (localName.Equals("namespace")) 
			{
				namespaces.Add(new Namespace(value,generated,relative,def,ns,prefix));
				//namespaces.Add(id,new Namespace(value,generated,relative,def));
			}
			id = string.Empty;
			value = string.Empty;
			generated = false;
			relative = false;
			def = false;
			prefix = string.Empty;
			ns = string.Empty;
		}

		public void StartPrefixMapping(string prefix, string uri)
		{
			throw new NotImplementedException();
		}

		public void Characters(string text)
		{
			value = text.Trim();
			//throw new NotImplementedException();
		}

		public void EndDocument()
		{
			//throw new NotImplementedException();
		}
	}
}
