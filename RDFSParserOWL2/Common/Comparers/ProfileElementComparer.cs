using RDFSParserOWL2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFSParserOWL2.Common.Comparers
{
	public  class ProfileElementComparer: IEqualityComparer<ProfileElement>
	{
		public bool Equals(ProfileElement x, ProfileElement y)
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
			return x.URI.Trim().Equals(y.URI.Trim());
			//throw new NotImplementedException();
		}

		public int GetHashCode(ProfileElement obj)
		{
			if (obj == null)
			{
				return 0;
			}
			return obj.URI.GetHashCode();
		}
	}
}
