using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFSParserOWL2.Model
{
	public class Namespace
	{
		private string value;		
		//private string id;
		private bool isToBeGenerated;
		private bool isToBeRelative;
		private bool isToBeDefault;
        private bool isNotToBeWritten;

		private string prefix;
		private string ns;

		public Namespace(string value,bool generate,bool relative,bool def,string ns,string prefix,bool isNotToBeWr) 
		{
			this.Value = value;
			this.IsToBeGenerated = generate;
			this.IsToBeRelative = relative;
			this.IsToBeDefault = def;
			this.ns = ns;
			this.prefix = prefix;
            this.isNotToBeWritten = isNotToBeWr;
		}

		public Namespace(string value,string prefix) 
		{
			this.Value = value;
			this.Prefix = prefix;
		}


        public bool IsNotToBeWritten
        {
            get { return isNotToBeWritten; }
            set { isNotToBeWritten = value; }
        }

		public void GenerateNamespace(string def,string localName) 
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(def).Append(localName);
			if (isToBeRelative == true) 
			{
				sb.Append("#");
			}
			value = sb.ToString();
		}

		//public  Namespace(string def, string localName) 
		//{
		//	this.prefix = localName;
		//	this.value = def+localName;
		//}

		public string Value
		{
			get { return this.value; }
			set { this.value = value; }
		}

		//public string Id
		//{
		//	get { return id; }
		//	set { id = value; }
		//}

		public bool IsToBeGenerated
		{
			get { return isToBeGenerated; }
			set { isToBeGenerated = value; }
		}

		public bool IsToBeRelative
		{
			get { return isToBeRelative; }
			set { isToBeRelative = value; }
		}

		public bool IsToBeDefault
		{
			get { return isToBeDefault; }
			set { isToBeDefault = value; }
		}

		public string Prefix
		{
			get { return prefix; }
			set { prefix = value; }
		}


		public string Ns
		{
			get { return ns; }
			set { ns = value; }
		}

	}
}
