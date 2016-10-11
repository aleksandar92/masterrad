using RDFSParserOWL2.Model;
using RDFSParserOWL2.Reporter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFSParserOWL2.Reporter
{
	public  class ProfileReporter:IProfileReport
	{


		#region FIELDS

		private Profile profile;

		#endregion FIELDS

		public ProfileReporter(Profile profile) 
		{
			this.profile = profile;
		}

		public ProfileReporter()
		{
			//this.profile = profile;
		}



		public Profile Profile
		{
			get
			{
				return profile;
			}
			set
			{
				profile = value;
			}
		}

		public string GenerateReport()
		{
			StringBuilder report = new StringBuilder();
			DateTime time = new DateTime();
			if (profile != null)
			{
				report.AppendLine(String.Format("Report generated at:{0}",time.ToString()));
				report.AppendLine(String.Format("Total number of elements:{0}",profile.TotalElementsCount));
				report.AppendLine(String.Format("Number of classes:{0} ,from which:",profile.ClassCount));
				report.AppendLine(String.Format("\t\t\tNumber of enum classes:{0}", profile.EnumClassesCount));
				report.AppendLine(String.Format("Number of properties:{0} ,from which:", profile.PropertyCount));
				report.AppendLine(String.Format("\t\t\tNumber of object properties:{0}", profile.ObjectPropertyCount));
				report.AppendLine(String.Format("\t\t\tNumber of datatype properties:{0}", profile.DatatypePropertyCount));
				report.AppendLine(String.Format("Total number of enum elements:{0}", profile.EnumMembersCount));
				report.AppendLine(String.Format("Total number of unknown elements:{0}", profile.UnkownElementsCount));
			}
			return report.ToString();
		}
	}
}
