using RDFSParserOWL2.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFSParserOWL2.Reporter.Interfaces
{
	/// <summary>
	/// Generating for  profile report
	/// </summary>
	public interface IProfileReport:IReporter
	{
		/// <summary>
		/// Property for profile for which report is being generated
		/// </summary>
		Profile Profile { get; set; } 
		
	}
}
