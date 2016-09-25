using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFSParserOWL2.Model
{
	public class ComplexTag
	{
		//protected string name;
		private string value;

		private Dictionary<string, string> attributes;



		public ComplexTag() 
		{
			attributes = new Dictionary<string, string>();
            value = String.Empty;
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
