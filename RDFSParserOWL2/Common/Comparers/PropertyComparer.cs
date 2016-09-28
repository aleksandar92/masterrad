using RDFSParserOWL2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFSParserOWL2.Common.Comparers
{
	public class PropertyComparer:IEqualityComparer<Property>
	{
		public bool Equals(Property x, Property y)
		{
			if (object.ReferenceEquals(x, y))
			{
				return true;
			}
			if (object.ReferenceEquals(x, null) ||
				object.ReferenceEquals(y, null))
			{
				return false;
			}
			return x.URI.Equals(y.URI);
			//throw new NotImplementedException();
		}

		public int GetHashCode(Property obj)
		{
			if (obj == null)
			{
				return 0;
			}
			return obj.URI.GetHashCode();
		}


	}
}
