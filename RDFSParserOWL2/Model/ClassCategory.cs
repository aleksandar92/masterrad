using RDFSParserOWL2.Generator.Helper;
using RDFSParserOWL2.Model.Settings;
using RDFSParserOWL2.Reporter.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace RDFSParserOWL2.Model
{
	public class ClassCategory : ProfileElement
	{

		protected string belongsToCategory;
		protected List<ProfileElement> membersOfClassCategory;
		protected ClassCategory belongsToCategoryAsObject;
        protected bool isBasePackage;


		public ClassCategory()
			: base("From Derived")
		{

		}

        #region Methods for handling equivalence with another element

        public override void EquivalenceWithAnotherElement(string uriOfEquivalent)
        {
            return;
        }


        public override bool EquvialenceWithAnotherElementWithStereotype(string baseUriOfElement, string stereotype) 
        {
            return false;
        }


        #endregion


		#region Class category specifics

		public string BelongsToCategory
		{
			get
			{
				return belongsToCategory;
			}
			set
			{
				belongsToCategory = value;
			}
		}

		/// <summary>
		/// Class category Specific property.
		/// <para>Gets and sets the list of all members of this class category.</para>
		/// </summary>
		//[BrowsableAttribute(false)]
		public List<ProfileElement> MembersOfClassCategory
		{
			get
			{
				return membersOfClassCategory;
			}
			set
			{
				membersOfClassCategory = value;
			}
		}

		/// <summary>
		/// Class category specific property.
		/// <para>Gets the number of all members of this class category.</para>
		/// <para>Memeber of class category can be another class category or class.</para>
		/// </summary>
		public int CountMembersOfClassCategory
		{
			get
			{
				int count = 0;
				if (membersOfClassCategory != null)
				{
					count = membersOfClassCategory.Count;
				}
				return count;
			}
		}

		public ClassCategory BelongsToCategoryAsObject
		{
			get
			{
				return belongsToCategoryAsObject;
			}
			set
			{
				belongsToCategoryAsObject = value;
			}
		}

		#endregion Class category specifics

		/// <summary>
		/// Method adds given ProfileElement to the MembersOfClassCategory list. 
		/// 
		/// TODO: added so that sets the subClassOf attribute
		/// </summary>
		/// <param fullName="member"></param>
		public void AddToMembersOfClassCategory(ProfileElement member)
		{
			if (membersOfClassCategory == null)
			{
				membersOfClassCategory = new List<ProfileElement>();
			}

			if (!membersOfClassCategory.Contains(member))
			{
				membersOfClassCategory.Add(member);
				//  membersOfClassCategory.Sort(CIMComparer.ProfileElementComparer);
			}
		}

        public bool IsBasePackage
        {
            get { return isBasePackage; }
            set { isBasePackage = value; }
        }

        public override bool ContainsStereotype(string stereotype)
        {
            return false;
        }

        #region RDFXML representation

        protected override void WriteEquivalenceToRDFXML(XmlWriter writer, string baseAdress)
        {
            return;
        }

        protected override bool IsNotToBeWritten()
        {
            return isBasePackage ;
        }

        protected override string OWLRDFXMLType() 
        {
            return OWL2Namespace.NamedIndividual;
        }

        protected override void WriteSpecificElementsToRDFXML(XmlWriter writer, string baseAddress, EnumRepresentationKindOWL2 enumOpt) 
        {
            WriteTypeToRDFXML(writer);
        }

        private void WriteTypeToRDFXML(XmlWriter writer) 
        {
            if (!string.IsNullOrEmpty(Type))
            {
                writer.WriteStartElement(OWL2Namespace.rdfPrefix, OWL2Namespace.Type, null);
                writer.WriteAttributeString(OWL2Namespace.rdfPrefix, OWL2Namespace.rdfResource, null, Type);
                writer.WriteEndElement();
            }
        }

        public override List<EntityTypesGeneratorReporter> StatusForOWL2()
        {
            List<EntityTypesGeneratorReporter> reportStatus = new List<EntityTypesGeneratorReporter>();
            if (IsBlankNode)
            {
                reportStatus.Add(EntityTypesGeneratorReporter.BlankId);
            }

            if (IsNotToBeWritten())
            {
                reportStatus.Add(EntityTypesGeneratorReporter.NotGenerated);
            }
            else
            {
                reportStatus.Add(EntityTypesGeneratorReporter.NamedIndividual);
            }

            return reportStatus;
        }

        # endregion

	}
}


