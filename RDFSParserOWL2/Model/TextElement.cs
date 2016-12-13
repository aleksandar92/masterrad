using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFSParserOWL2.Model
{
	public class TextElement
	{
		//protected string name;
		private string value;

		private Dictionary<string, string> attributes;



		public TextElement() 
		{                                        
			attributes = new Dictionary<string, string>();
            value = String.Empty;
		}

		public TextElement(string value)
		{
			attributes = new Dictionary<string, string>();
			this.value = value;
		}

		public TextElement(string value,Dictionary<string,string> attr) 
		{
		    this.attributes = new Dictionary<string,string>(attr);
			this.value = value;		
		}


		public string Value
		{
			get { return this.value; }
			set { this.value = value; }
		}

		public Dictionary<string, string> Attributes
		{
			get { return attributes; }
			set { attributes = value; }
		}

	}
}
