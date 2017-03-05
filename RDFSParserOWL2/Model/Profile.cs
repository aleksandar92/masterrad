using RDFSParserOWL2.Common;
using RDFSParserOWL2.Common.Comparers;
using RDFSParserOWL2.Generator.Helper;
using RDFSParserOWL2.Generator.Interfaces;
using RDFSParserOWL2.Manager;
using RDFSParserOWL2.Model.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RDFSParserOWL2.Model
{

    /// <summary>
    /// Profile class represents the CIM profile loaded from RDFS source file.
    /// <para>@author: Stanislava Selena</para>
    /// </summary>
    public class Profile
    {

        /// <summary> 
        /// List of common stereotypes for profile elements which can be extended
        /// by adding newly identified stereotypes in profile processing process.
        /// <para>See also: <seealso cref="M:FindOrCreateStereotypeForName(string fullStereotypeName)"/></para>
        /// </summary>
        public static List<ProfileElementStereotype> StereotypeList;


        private SortedDictionary<ProfileElementTypes, List<ProfileElement>> profileMap;
        private string sourcePath;
        private string fileName;
        private string baseNS;
        private double fileSizeMB = 0;
        private DateTime? lastModificationTime;

        private StringBuilder processingReport;
        private List<string> importNamespaces;
        private EnumRepresentationKindOWL2 enumKind;
        private List<Namespace> usedNamespaces;
        private bool isReportToBeWrittenToFile;




        private bool loaderErrorOcurred = false;


        static Profile()
        {
            StereotypeList = new List<ProfileElementStereotype>();
            // init common stereotypes list
            int index = 0;
            //StereotypeList.Add(new ProfileElementStereotype(index++, ProfileElementStereotype.StereotypeConcrete));
            //StereotypeList.Add(new ProfileElementStereotype(index++, ProfileElementStereotype.StereotypeCompound));
            //StereotypeList.Add(new ProfileElementStereotype(index++, ProfileElementStereotype.StereotypeAttribute));
            //StereotypeList.Add(new ProfileElementStereotype(index++, ProfileElementStereotype.StereotypeEnumeration));
            //StereotypeList.Add(new ProfileElementStereotype(index++, ProfileElementStereotype.StereotypeByReference));
            //StereotypeList.Add(new ProfileElementStereotype(index++, ProfileElementStereotype.StereotypeAggregateOf));
            //StereotypeList.Add(new ProfileElementStereotype(index++, ProfileElementStereotype.StereotypeOfAggregate));
            //StereotypeList.Add(new ProfileElementStereotype(index++, ProfileElementStereotype.StereotypeCompositeOf));
            //StereotypeList.Add(new ProfileElementStereotype(index++, ProfileElementStereotype.StereotypeEntsoe));
            //StereotypeList.Add(new ProfileElementStereotype(index++, ProfileElementStereotype.StereotypePrimitive));
        }


        public Profile()
        {
            profileMap = new SortedDictionary<ProfileElementTypes, List<ProfileElement>>();
            usedNamespaces = new List<Namespace>();
            processingReport = new StringBuilder();
        }

        public Profile(string sourcePath)
        {
            profileMap = new SortedDictionary<ProfileElementTypes, List<ProfileElement>>();
            SourcePath = sourcePath;
            usedNamespaces = new List<Namespace>();
            processingReport = new StringBuilder();
        }

        public List<string> ImportNamespaces
        {
            get { return importNamespaces; }
            set { importNamespaces = value; }
        }


        public EnumRepresentationKindOWL2 EnumKind
        {
            get { return enumKind; }
            set { enumKind = value; }
        }

        public List<Namespace> UsedNamespaces
        {
            get { return usedNamespaces; }
            set { usedNamespaces = value; }
        }


        /// <summary>
        /// Gets and sets the map which projectModels the profile.        
        /// </summary>
        public SortedDictionary<ProfileElementTypes, List<ProfileElement>> ProfileMap
        {
            get
            {
                return profileMap;
            }
            set
            {
                profileMap = value;
                SortElementsInMap();
            }
        }

        /// <summary>
        /// Gets and sets the absolute file path to source file of profile.
        /// </summary>
        public string SourcePath
        {
            set
            {
                sourcePath = value;
                fileName = string.Empty;
                fileSizeMB = 0;
                if (sourcePath != null)
                {
                    fileName = System.IO.Path.GetFileName(sourcePath);
                }
            }
            get
            {
                return sourcePath;
            }
        }

        /// <summary>
        /// Gets the file name of profile's source file (with extension).
        /// </summary>
        public string FileName
        {
            get
            {
                return fileName;
            }

                       set
            {
                fileName = value ;
            }
        }

        public StringBuilder ProcessingReport
        {
            get { return processingReport; }
            set { processingReport = value; }
        }

        public string BaseNS
        {
            set
            {
                baseNS = value;
            }

            get
            {
                return baseNS;
            }
        }


        /// <summary>
        /// Gets the size of profile's source file (in MB).
        /// </summary>
        public double FileSizeMB
        {
            get
            {
                return fileSizeMB;
            }
        }

        /// <summary>
        /// Gets and sets the time of last modification of source file.
        /// </summary>
        public DateTime? LastModificationTime
        {
            get
            {
                return lastModificationTime;
            }
            set
            {
                lastModificationTime = value;
            }
        }

        public bool IsReportToBeWrittenToFile
        {
            get { return isReportToBeWrittenToFile; }
            set { isReportToBeWrittenToFile = value; }
        }
            

        /// <summary>
        /// Get and sets the indication if there was serious errors during initial loading of project file.
        /// <para>This error is reported by LoaderManager's LoadCIMProject() method.</para>
        /// </summary>
        public bool LoaderErrorOcurred
        {
            get
            {
                return loaderErrorOcurred;
            }
            set
            {
                loaderErrorOcurred = value;
                if (loaderErrorOcurred && (profileMap != null))
                {
                    profileMap.Clear();
                }
            }
        }

        /// <summary>
        /// Gets the number of all ProfileElementTypes.ClassCategory in this profile.
        /// </summary>
        public int PackageCount
        {
            get
            {
                int count = 0;
                if (profileMap != null)
                {
                    List<ProfileElement> elements = null;
                    profileMap.TryGetValue(ProfileElementTypes.ClassCategory, out elements);
                    if (elements != null)
                    {
                        count = elements.Count;
                    }
                }
                return count;
            }
        }


        public int ElementsBlankNodeCount
        {
            get
            {
                int count = 0;
                if (profileMap != null)
                {
                    foreach (ProfileElementTypes pet in profileMap.Keys)
                    {
                        List<ProfileElement> elements = null;
                        profileMap.TryGetValue(pet, out elements);
                        if (elements != null)
                        {
                            count += elements.Where(x => x.URI.Contains(StringManipulationManager.SeparatorBlankNode)).Count();
                        }
                    }
                }
                return count;
            }
        }

        /// <summary>
        /// Gets the number of all ProfileElementTypes.Class in this profile.
        /// </summary>
        public int ClassCount
        {
            get
            {
                int count = 0;
                if (profileMap != null)
                {
                    List<ProfileElement> elements = null;
                    profileMap.TryGetValue(ProfileElementTypes.Class, out elements);
                    if (elements != null)
                    {
                        count = elements.Count;
                    }
                }
                return count;
            }
        }



        /// <summary>
        /// Gets number of datatype properties in profile
        /// </summary>
        public int DatatypePropertyCount
        {
            get
            {
                int count = 0;
                if (profileMap != null)
                {
                    List<ProfileElement> elements = null;
                    profileMap.TryGetValue(ProfileElementTypes.Property, out elements);
                    if (elements != null)
                    {
                        count = elements.Cast<Property>().ToList().Where(x => !x.IsObjectProperty()).Count();
                    }
                }
                return count;
            }

        }

        /// <summary>
        /// Gets number of object properties in profile 
        /// </summary>
        public int ObjectPropertyCount
        {
            get
            {
                int count = 0;
                if (profileMap != null)
                {
                    count = PropertyCount - DatatypePropertyCount;
                }
                return count;
            }
        }

        public void AddToReport(string report) 
        {
            ProcessingReport.AppendLine(report);
        }

        /// <summary>
        /// Get number of enumeration elements in profile
        /// </summary>
        public int EnumMembersCount
        {
            get
            {
                int count = 0;
                if (profileMap != null)
                {
                    List<ProfileElement> elements = null;
                    profileMap.TryGetValue(ProfileElementTypes.EnumerationElement, out elements);
                    if (elements != null)
                    {
                        count = elements.Count;
                    }
                }
                return count;
            }
        }

        /// <summary>
        /// Returns number of enum classes in profile 
        /// </summary>
        public int EnumClassesCount
        {
            get
            {
                int count = 0;
                if (profileMap != null)
                {
                    List<ProfileElement> elements = null;
                    profileMap.TryGetValue(ProfileElementTypes.Class, out elements);
                    if (elements != null)
                    {
                        count = elements.Cast<Class>().ToList().Where(x => x.IsEnumeration).Count();
                    }
                }
                return count;
            }
        }

        /// <summary>
        /// Returns number of unknown elements in profile 
        /// </summary>
        public int UnkownElementsCount
        {
            get
            {
                int count = 0;
                if (profileMap != null)
                {
                    List<ProfileElement> elements = null;
                    profileMap.TryGetValue(ProfileElementTypes.Unknown, out elements);
                    if (elements != null)
                    {
                        count = elements.Count;
                    }
                }
                return count;
            }
        }


        /// <summary>
        /// Returns number of unknown elements in profile 
        /// </summary>
        public int TotalElementsCount
        {
            get
            {
                int count = 0;
                count = UnkownElementsCount + EnumMembersCount + PropertyCount + ClassCount;
                return count;
            }
        }






        /// <summary>
        /// Gets the number of all ProfileElementTypes.Property in this profile.
        /// </summary>
        public int PropertyCount
        {
            get
            {
                int count = 0;
                if (profileMap != null)
                {
                    List<ProfileElement> elements = null;
                    profileMap.TryGetValue(ProfileElementTypes.Property, out elements);
                    if (elements != null)
                    {
                        count = elements.Count;
                    }
                }
                return count;
            }
        }

        


        /// <summary>
        /// Gets the number of all ProfileElementTypes.Stereotype in this profile.
        /// </summary>
        public int StereotypeCount
        {
            get
            {
                int count = 0;
                if (profileMap != null)
                {
                    List<ProfileElement> elements = null;
                    profileMap.TryGetValue(ProfileElementTypes.Stereotype, out elements);
                    if (elements != null)
                    {
                        count = elements.Count;
                    }
                }
                return count;
            }
        }


        /// <summary>
        /// Method first tries to find the ProfileElementStereotype object with given fullName isInside the
        /// (static) StereotypeList, and if it doesn't find it, it creates new stereotype objects 
        /// and adds it to this list.
        /// <remarks>If the fullStereotypeName is null, method will return null.</remarks>
        /// </summary>
        /// <param fullName="fullStereotypeName">full fullName of stereotype which is being searched</param>
        /// <returns>ProfileElementStereotype object with given fullName founded (or added) in StereotypeList</returns>
        public static ProfileElementStereotype FindOrCreateStereotypeForName(string fullStereotypeName)
        {
            ProfileElementStereotype stereotype = null;
            if (!string.IsNullOrEmpty(fullStereotypeName))
            {
                string shortName = StringManipulationManager.ExtractShortestName(fullStereotypeName, StringManipulationManager.SeparatorSharp);

                foreach (ProfileElementStereotype existingStereotype in Profile.StereotypeList)
                {
                    if (existingStereotype.Name.Equals(fullStereotypeName) || existingStereotype.Name.Equals(shortName))
                    {
                        stereotype = existingStereotype;
                        break;
                    }
                }

                if (stereotype == null)
                {
                    stereotype = new ProfileElementStereotype(Profile.StereotypeList.Count, fullStereotypeName);
                    Profile.StereotypeList.Add(stereotype);
                }
            }
            return stereotype;
        }


        public static List<ProfileElement> RepackProperties(List<ProfileElement> elements)
        {
            List<Property> properties = elements.Cast<Property>().ToList();


            List<Property> objectProperties = properties.Where(x => x.IsObjectProperty() == true && !x.HasStereotype(OWL2Namespace.Entsoe)).ToList();
            List<Property> entsoObjectProperties = properties.Where(x => x.IsObjectProperty() && x.HasStereotype(OWL2Namespace.Entsoe)).ToList();
            List<Property> datatypeProperties = properties.Where(x => x.IsObjectProperty() != true && !x.HasStereotype(OWL2Namespace.Entsoe)).ToList();
            List<Property> entsoDatatypesProperties = properties.Where(x => x.IsObjectProperty() != true && x.HasStereotype(OWL2Namespace.Entsoe)).ToList();

            entsoObjectProperties.AddRange(objectProperties);
            entsoObjectProperties.AddRange(entsoDatatypesProperties);
            entsoObjectProperties.AddRange(datatypeProperties);
            return entsoObjectProperties.Cast<ProfileElement>().ToList();
        }


        /// <summary>
        /// Removes properties that contains one or more items from stereotypes list
        /// </summary>
        /// <param name="stereotypes"></param>
        private void RemovePropertiesWithStereotypes(List<string> stereotypes)
        {
            List<Property> properties = null;
            if (profileMap != null && profileMap.ContainsKey(ProfileElementTypes.Property))
            {
                properties = GetAllProfileElementsOfType(ProfileElementTypes.Property).Cast<Property>().ToList();
                foreach (string s in stereotypes)
                {
                    if (properties != null)
                        properties = properties.Where(x => !x.HasStereotype(s)).ToList();
                }

                if (properties != null)
                    profileMap[ProfileElementTypes.Property] = properties.Cast<ProfileElement>().ToList();
            }

        }


        public void RemoveDuplicates()
        {
            foreach (ProfileElementTypes pet in Enum.GetValues(typeof(ProfileElementTypes)))
            {
                List<ProfileElement> heleprCollection = null;
                profileMap.TryGetValue(pet, out heleprCollection);
                if (heleprCollection != null)
                    profileMap[pet] = new List<ProfileElement>(new HashSet<ProfileElement>(heleprCollection, new ProfileElementComparer()));
            }
        }


        private int MarkPropertiesWithStereotypes(List<string> stereotypes)
        {
            List<Property> properties = null;
            int count = 0;
            if (profileMap != null && profileMap.ContainsKey(ProfileElementTypes.Property))
            {
                properties = GetAllProfileElementsOfType(ProfileElementTypes.Property).Cast<Property>().ToList();
                foreach (string s in stereotypes)
                {
                    if (properties != null)
                    {
                        properties = properties.Where(x => x.HasStereotype(s)).ToList();
                        count = properties.Count;
                        foreach (Property p in properties)
                        {
                            p.IsNotToBeGenerated = true;
                        }
                    }
                }

            }
            return count;

        }



        private void RemoveClassesWithStereotypes(List<string> stereotypes)
        {
            if (profileMap != null && profileMap.ContainsKey(ProfileElementTypes.Class))
            {
                List<Class> classes = GetAllProfileElementsOfType(ProfileElementTypes.Class).Cast<Class>().ToList();
                foreach (string s in stereotypes)
                {
                    if (classes != null)
                        classes = classes.Where(x => !x.HasStereotype(s)).ToList();
                }

                if (classes != null)
                    profileMap[ProfileElementTypes.Class] = classes.Cast<ProfileElement>().ToList();


            }

        }

        public void AddNamespace(Namespace ns) 
        {
            if(usedNamespaces==null)
                usedNamespaces=new List<Namespace>();
            usedNamespaces.Add(ns);

        }


        private int MarkClassesWithStereotypes(List<string> stereotypes)
        {
            int count = 0;
            if (profileMap != null && profileMap.ContainsKey(ProfileElementTypes.Class))
            {
                List<Class> classes = GetAllProfileElementsOfType(ProfileElementTypes.Class).Cast<Class>().ToList();
                foreach (string s in stereotypes)
                {
                    if (classes != null)
                    {
                        classes = classes.Where(x => x.HasStereotype(s)).ToList();
                        count = classes.Count;
                        foreach (Class cls in classes)
                        {
                            cls.IsNotToBeGenerated = true;
                        }
                    }
                }



            }
            return count;

        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="specialOntologyProfile"></param>
        /// <param name="nameOfStereotype"></param>
        /// <returns></returns>
        public bool ProcessElementsWithSpecialStereotype(Profile specialOntologyProfile, string nameOfStereotype)
        {
            bool changed = false;
            if (specialOntologyProfile != null)
            {

                foreach (ProfileElementTypes type in profileMap.Keys)
                {
                    switch (type)
                    {
                        case ProfileElementTypes.Class:
                            CalculateDifferencessInClassesBeetweenTwoProfiles(specialOntologyProfile, ref changed, nameOfStereotype);
                            break;

                        case ProfileElementTypes.Property:
                            List<Property> propertiesToAdd = null;
                            List<Property> entsoProperties = null;
                            List<Property> properties = null;

                            if (specialOntologyProfile.ProfileMap.ContainsKey(type))
                            {
                                entsoProperties = specialOntologyProfile.ProfileMap[type].Cast<Property>().ToList();
                            }

                            if (profileMap.ContainsKey(type))
                            {
                                properties = profileMap[type].Cast<Property>().Where(x => x.HasStereotype(nameOfStereotype)).ToList();
                            }


                            DifferenceBeetwenListOfProperties(ref entsoProperties, properties, out propertiesToAdd);

                            if (propertiesToAdd != null && propertiesToAdd.Count > 0)
                            {
                                if (entsoProperties == null)
                                {
                                    entsoProperties = new List<Property>();
                                }

                                entsoProperties.AddRange(propertiesToAdd);
                                specialOntologyProfile.ProfileMap[type] = entsoProperties.Cast<ProfileElement>().ToList();
                                changed = true;
                            }

                            break;


                    }
                }

            }

            return changed;


        }


        private void CalculateDifferencessInClassesBeetweenTwoProfiles(Profile specialOntologyProfile, ref bool changed, string nameOfStereotype)
        {
            List<Class> classesToAdd = null;
            List<Class> classes = null;
            List<Class> soClasses = null;
            ProfileElementTypes type = ProfileElementTypes.Class;

            if (specialOntologyProfile.ProfileMap.ContainsKey(type))
            {
                soClasses = specialOntologyProfile.ProfileMap[type].Cast<Class>().ToList();
            }

            if (profileMap.ContainsKey(type))
            {
                classes = profileMap[type].Cast<Class>().Where(x => x.HasStereotype(nameOfStereotype)).ToList();
            }

            if (soClasses == null && classes != null)
            {
                classesToAdd = new List<Class>();
                classesToAdd.AddRange(classes);
                soClasses = new List<Class>();
            }
            else if (soClasses != null && classes != null)
            {
                classesToAdd = classes.Except(soClasses, new ClassComparer()).ToList();
                List<Class> classesToChange = classes.Intersect(soClasses, new ClassComparer()).ToList();
                foreach (Class cls in soClasses)
                {

                    Class secCLs = classes.Where(x => x.URI.Equals(cls.URI)).SingleOrDefault();
                    if (secCLs != null)
                    {
                        List<ProfileElement> enumsClassToAdd = null;
                        List<ProfileElement> enumMembersSoClass = cls.MyEnumerationMembers;
                        List<ProfileElement> enumMembersClass = secCLs.MyEnumerationMembers;
                        DifferenceBeetwenListOfProfileElements(ref enumMembersSoClass, enumMembersClass, out enumsClassToAdd);
                        if (enumsClassToAdd != null && enumsClassToAdd.Count > 0)
                        {
                            if (enumMembersSoClass == null)
                            {
                                enumMembersSoClass = new List<ProfileElement>();
                            }

                            enumMembersSoClass.AddRange(enumsClassToAdd);
                            cls.MyEnumerationMembers = enumMembersSoClass;
                            //specialOntologyProfile.ProfileMap[type] = entsoProperties.Cast<ProfileElement>().ToList();
                            changed = true;
                        }
                        //if (enumMembersSoClass == null && enumMembersClass!=null) 
                        //{
                        //	propertiesClassToAdd = new List<ProfileElement>();
                        //	propertiesClassToAdd.AddRange(enumMembersClass);
                        //}else if() 
                    }
                    //DifferenceBeetwenListOfProperties(,,);
                }
            }

            if (classesToAdd != null && classesToAdd.Count > 0)
            {
                if (soClasses == null)
                {
                    soClasses = new List<Class>();
                }

                soClasses.AddRange(classesToAdd);
                specialOntologyProfile.ProfileMap[type] = soClasses.Cast<ProfileElement>().ToList();
                changed = true;
            }


            foreach (Class cls in classes)
            {
                List<Property> propToAdd = null;
                List<Property> soProp = null;
                List<Property> prop = null;

                if (specialOntologyProfile.ProfileMap.ContainsKey(ProfileElementTypes.Property))
                {
                    soProp = specialOntologyProfile.ProfileMap[ProfileElementTypes.Property].Cast<Property>().ToList();
                }

                if (cls.MyProperties != null)
                {
                    prop = cls.MyProperties.Cast<Property>().ToList();
                }


                DifferenceBeetwenListOfProperties(ref soProp, prop, out propToAdd);

                if (prop != null && propToAdd.Count > 0)
                {
                    if (soProp == null)
                    {
                        soProp = new List<Property>();
                    }

                    soProp.AddRange(propToAdd);
                    specialOntologyProfile.ProfileMap[ProfileElementTypes.Property] = soProp.Cast<ProfileElement>().ToList();
                    changed = true;
                }

                if (cls.IsEnumeration)
                {
                    DifferenceBeetwenEnumsInClass(ref specialOntologyProfile, cls, ref changed);
                }


            }

        }

        private void DifferenceBeetweenListOfSameClassesFromDifferentProfile()
        {


        }


        /// <summary>
        /// Method for calculating deifferences between  two lists of properties
        /// </summary>
        /// <param name="firstList"> </param>
        /// <param name="secondList"></param>
        /// <param name="propertiesToAdd"></param>
        private void DifferenceBeetwenListOfProperties(ref List<Property> firstList, List<Property> secondList, out List<Property> propertiesToAdd)
        {
            propertiesToAdd = null;

            if (firstList == null && secondList != null)
            {
                firstList = new List<Property>();
                propertiesToAdd = new List<Property>();
                propertiesToAdd.AddRange(secondList);
            }
            else if (firstList != null && secondList != null)
            {
                propertiesToAdd = secondList.Except(firstList, new PropertyComparer()).ToList();

            }
        }


        /// <summary>
        /// Method for calculating deifferences between  two lists of profile elements
        /// </summary>
        /// <param name="firstList"> </param>
        /// <param name="secondList"></param>
        /// <param name="propertiesToAdd"></param>
        private void DifferenceBeetwenListOfProfileElements(ref List<ProfileElement> firstList, List<ProfileElement> secondList, out List<ProfileElement> propertiesToAdd)
        {
            propertiesToAdd = null;

            if (firstList == null && secondList != null)
            {
                firstList = new List<ProfileElement>();
                propertiesToAdd = new List<ProfileElement>();
                propertiesToAdd.AddRange(secondList);
            }
            else if (firstList != null && secondList != null)
            {
                propertiesToAdd = secondList.Except(firstList, new ProfileElementComparer()).ToList();

            }
        }

        public void PopulateClassCategoryReferences(Profile metaProfile)
        {
            List<Class> classes = GetAllProfileElementsOfType(ProfileElementTypes.Class).Cast<Class>().ToList();
            List<ProfileElement> metaPackages = metaProfile.GetAllProfileElementsOfType(ProfileElementTypes.ClassCategory);
            foreach (Class c in classes)
            {
                if (c.BelongsToCategoryAsObject == null)
                {
                    ProfileElement pe = metaPackages.Where(x => x.URI.Equals(c.BelongsToCategory)).SingleOrDefault();
                    if (pe != null)
                    {
                        ClassCategory cc = pe as ClassCategory;
                        cc.IsBasePackage = true;
                        c.BelongsToCategoryAsObject = cc;
                    }


                }
            }

        }

        public void MarkBasePackages(Profile metaProfile, string metaUri)
        {
            if (metaProfile != null)
            {
                List<ProfileElement> metaPackages = metaProfile.GetAllProfileElementsOfType(ProfileElementTypes.ClassCategory);
                List<ProfileElement> packages = GetAllProfileElementsOfType(ProfileElementTypes.ClassCategory);
                if (metaPackages != null && packages != null)
                {
                    List<ClassCategory> categories = packages.Cast<ClassCategory>().ToList();
                    foreach (ClassCategory clsCat in categories) 
                    {
                        clsCat.Type = metaUri + StringManipulationManager.ExtractShortestName(clsCat.Type,StringManipulationManager.SeparatorSharp);
                    } 

                    List<ClassCategory> categoriesFromMeta = packages.Intersect(metaPackages, new ProfileElementComparer()).Cast<ClassCategory>().ToList();
                    foreach (ClassCategory cat in categoriesFromMeta)
                    {
                        cat.IsBasePackage = true;
                        cat.URI = StringManipulationManager.TrimAfterLastSeparator(metaUri,StringManipulationManager.SeparatorSharp) + cat.URI;
                    }

                }
            }

        }


        private void DifferenceBeetwenListOfEnums(ref List<EnumMember> firstList, List<EnumMember> secondList, out List<EnumMember> enumsToAdd)
        {
            enumsToAdd = null;

            if (firstList == null && secondList != null)
            {
                firstList = new List<EnumMember>();
                enumsToAdd = new List<EnumMember>();
                enumsToAdd.AddRange(secondList);
            }
            else if (firstList != null && secondList != null)
            {
                enumsToAdd = secondList.Except(firstList, new EnumMemberComparer()).ToList();

            }
        }


        private void DifferenceBeetwenEnumsInClass(ref Profile specialOntologyProfile, Class cls, ref bool changed)
        {
            List<EnumMember> enumToAdd = null;
            List<EnumMember> soEnum = null;
            List<EnumMember> enums = null;

            if (specialOntologyProfile.ProfileMap.ContainsKey(ProfileElementTypes.EnumerationElement))
            {
                soEnum = specialOntologyProfile.ProfileMap[ProfileElementTypes.EnumerationElement].Cast<EnumMember>().ToList();
            }

            if (cls.MyEnumerationMembers != null)
            {
                enums = cls.MyEnumerationMembers.Cast<EnumMember>().ToList();
            }


            DifferenceBeetwenListOfEnums(ref soEnum, enums, out enumToAdd);

            if (enums != null && enumToAdd.Count > 0)
            {
                if (soEnum == null)
                {
                    soEnum = new List<EnumMember>();
                }

                soEnum.AddRange(enumToAdd);
                specialOntologyProfile.ProfileMap[ProfileElementTypes.EnumerationElement] = soEnum.Cast<ProfileElement>().ToList();
                changed = true;
            }


        }


        public List<ProfileElement> GetAllProfileElementsOfType(ProfileElementTypes type)
        {
            List<ProfileElement> elementsOfType = new List<ProfileElement>();
            if (profileMap != null)
            {
                profileMap.TryGetValue(type, out elementsOfType);
            }
            return elementsOfType;
        }


        public ProfileElement FindProfileElementByUri(string uri)
        {
            ProfileElement element = null;

            if (!string.IsNullOrEmpty(uri) && (profileMap != null))
            {
                foreach (ProfileElementTypes type in profileMap.Keys)
                {
                    //List<ProfileElement> list = profileMap[type];
                    //element = list.Where(x=>x.URI.Equals(uri)).SingleOrDefault();
                    ////Dictionary<string, ProfileElement> elements = profileMap[type].ToDictionary(x=>x.URI);
                    ////if (elements.ContainsKey(uri)) 
                    ////{
                    ////	element = elements[uri];
                    ////}
                    List<ProfileElement> list = profileMap[type];
                    //HashSet<ProfileElement> hashSet = new HashSet<ProfileElement>(list);
                    //ProfileElement pe = new Class();
                    //pe.URI = uri;
                    //if (hashSet.Contains(pe, new ProfileElementComparer()))
                    //{
                    //{
                    //element = hashSet.Where(x=>x.URI.Equals(uri)).SingleOrDefault();
                    //}
                    foreach (ProfileElement elem in list)
                    {
                        if (uri.Equals(elem.URI))
                        {
                            element = elem;
                            break;
                        }
                    }
                    //}
                    if (element != null)
                    {
                        break;
                    }
                }
            }
            return element;
        }

        public List<ProfileElement> GetAllElements()
        {
            List<ProfileElement> list = new List<ProfileElement>();
            List<ProfileElement> result;
            foreach (ProfileElementTypes profileElement in Enum.GetValues(typeof(ProfileElementTypes)))
            {
                profileMap.TryGetValue(profileElement, out result);
                if (result != null)
                    list.AddRange(result);
            }

            return list;
        }


        public ProfileElement FindProfileElementByType(ProfileElementTypes type, string uri)
        {
            ProfileElement pe = null;
            List<ProfileElement> listOfElements;
            profileMap.TryGetValue(type, out listOfElements);
            if (!string.IsNullOrEmpty(uri) && profileMap != null && listOfElements != null)
            {
                foreach (ProfileElement elem in listOfElements)
                {
                    if (uri.Equals(elem.URI))
                    {
                        pe = elem;
                        break;
                    }
                }
            }

            return pe;

        }

        public ProfileElement FindProfileElementByShortUri(string shortUri)
        {
            ProfileElement element = null;

            if (!string.IsNullOrEmpty(shortUri) && (profileMap != null))
            {
                foreach (ProfileElementTypes type in profileMap.Keys)
                {
                    List<ProfileElement> list = profileMap[type];
                    foreach (ProfileElement elem in list)
                    {
                        if (shortUri.Equals(elem.UniqueName))
                        {
                            element = elem;
                            break;
                        }
                    }
                    if (element != null)
                    {
                        break;
                    }
                }
            }
            return element;
        }


        public ProfileElement FindProfileElementByName(string name)
        {
            ProfileElement element = null;

            if (!string.IsNullOrEmpty(name) && (profileMap != null))
            {
                foreach (ProfileElementTypes type in profileMap.Keys)
                {
                    List<ProfileElement> list = profileMap[type];
                    foreach (ProfileElement elem in list)
                    {
                        if (name.Equals(elem.Name))
                        {
                            element = elem;
                            break;
                        }
                    }
                    if (element != null)
                    {
                        break;
                    }
                }
            }
            return element;
        }



        public void ProcessElementsWithCIMDatatypeStereotype(CIMDatatypePresentation presentation) 
        {
            if (presentation == CIMDatatypePresentation.SIMPLE) 
            {
                List<Class> classes = GetAllProfileElementsOfType(ProfileElementTypes.Class).Cast<Class>().ToList();
                foreach (Class cls in classes)
                {
                    if (cls.ContainsStereotype("CIMDatatype"))
                    {
                        cls.IsNotToBeGenerated = true;
                        if (cls.MyProperties != null)
                        {
                            foreach (ProfileElement profileElement in cls.MyProperties)
                            {

                                profileElement.IsNotToBeGenerated = true;
                            }
                        }

                        if (cls.MyEnumerationMembers != null)
                        {
                            foreach (ProfileElement profileElement in cls.MyEnumerationMembers)
                            {
                                profileElement.IsNotToBeGenerated = true;
                            }
                        }

                    }
                }

                    List<Property> properties = GetAllProfileElementsOfType(ProfileElementTypes.Property).Cast<Property>().ToList();
                    if (properties != null) 
                    {
                        foreach (Property property in properties) 
                        {
                            //if (!property.IsObjectProperty()) 
                            //{
                                if (property.DataTypeAsComplexObject != null) 
                                {
                                    Class cl = property.DataTypeAsComplexObject as Class;
                                    if (cl != null && cl.MyProperties!=null && cl.ContainsStereotype("CIMDatatype")) 
                                    {
                                        ProfileElement pe = cl.MyProperties.Where(x => x.Name.StartsWith("value")).SingleOrDefault() ;
                                        Property datatypeValueProperty = pe as Property;
                                        if (datatypeValueProperty != null) 
                                        {
                                            property.DataType = datatypeValueProperty.DataType;
                                            property.DataTypeAsComplexObject = null; 
                                        }
                                    }
                                }
                           // }
                        }
                    
                    }
                    // 
                
            }
            //foreach () {}
        }


        /*
        public string FullPrintingString()
        {
            StringBuilder toStringBuilder = new StringBuilder("Profile: \n");
            if (profileMap != null)
            {
                if (profileMap.ContainsKey(ProfileElementTypes.ClassCategory))
                {
                    foreach (ProfileElement package in profileMap[ProfileElementTypes.ClassCategory])
                    {
                        toStringBuilder.Append("* members of ");
                        toStringBuilder.AppendLine(package.UniqueName);
                        List<ProfileElement> list = package.MembersOfClassCategory;
                        if (list != null)
                        {
                            foreach (ProfileElement elem in list)
                            {
                                if (elem.TypeAsEnumValue == ProfileElementTypes.Class)
                                {
                                    toStringBuilder.Append("\t ");
                                    toStringBuilder.AppendLine(elem.URI);
                                    toStringBuilder.Append("\t\t type = ");
                                    toStringBuilder.AppendLine(elem.TypeAsEnumValue.ToString());
                                    toStringBuilder.Append("\t\t label = ");
                                    toStringBuilder.AppendLine(elem.Label);

                                    if (elem.Stereotypes != null)
                                    {
                                        toStringBuilder.AppendLine("\t\t has Stereotypes : ");
                                        foreach (ProfileElementStereotype stereotype in elem.Stereotypes)
                                        {
                                            toStringBuilder.Append("\t\t\t");
                                            toStringBuilder.AppendLine(stereotype.ToString());
                                        }
                                    }

                                    toStringBuilder.Append("\t\t subClassOf = ");
                                    toStringBuilder.AppendLine(elem.SubClassOf);
                                    toStringBuilder.Append("\t\t belongsToCategory = ");
                                    toStringBuilder.AppendLine(elem.BelongsToCategory);
                                    if (elem.MyProperties != null)
                                    {
                                        toStringBuilder.AppendLine("\t\t has Properties : ");
                                        foreach (ProfileElement property in elem.MyProperties)
                                        {
                                            toStringBuilder.Append("\t\t\t");
                                            toStringBuilder.Append(property.UniqueName);
                                            toStringBuilder.Append(" ( ");
                                            toStringBuilder.Append(property.URI);
                                            toStringBuilder.AppendLine(" )");
                                            toStringBuilder.Append("\t\t\t\t label = ");
                                            toStringBuilder.AppendLine(property.Label);
                                            toStringBuilder.Append("\t\t\t\t dataType = ");
                                            toStringBuilder.AppendLine(property.DataType);
                                            toStringBuilder.Append("\t\t\t\t range = ");
                                            toStringBuilder.AppendLine(property.Range);
                                            toStringBuilder.Append("\t\t\t\t multiplicity = ");
                                            toStringBuilder.AppendLine(property.Multiplicity.ToString());
                                            toStringBuilder.Append("\t\t\t\t inverseRoleName = ");
                                            toStringBuilder.AppendLine(property.InverseRoleName);

                                            if (property.Stereotypes != null)
                                            {
                                                toStringBuilder.AppendLine("\t\t\t\t has Stereotypes : ");
                                                foreach (ProfileElementStereotype stereotype in property.Stereotypes)
                                                {
                                                    toStringBuilder.Append("\t\t\t\t\t");
                                                    toStringBuilder.AppendLine(stereotype.ToString());
                                                }
                                            }

                                            toStringBuilder.AppendLine();
                                        }
                                    }

                                    if (elem.MyEnumerationMembers != null)
                                    {
                                        toStringBuilder.AppendLine("\t\t\t has enum members : ");
                                        foreach (ProfileElement enumMember in elem.MyEnumerationMembers)
                                        {
                                            toStringBuilder.Append("\t\t\t\t");
                                            toStringBuilder.AppendLine(enumMember.UniqueName);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return toStringBuilder.ToString();
        }
        */

        public void PopulateObjectReferences()
        {
            PopulateRangeAsObjectProperties();
            PopulateDomainAsObjectProperties();
            PopulateSubclassOfClasses();
            ClearEquivalences();
            ClearBelongsToCategoryAsObject();
            ClearPackages();
        }

        private void ClearPackages() 
        {
            if (profileMap.ContainsKey(ProfileElementTypes.ClassCategory)) 
            {
                profileMap[ProfileElementTypes.ClassCategory] = new List<ProfileElement>();
            } 
        }

        private void ClearEquivalences() 
        {
            foreach (ProfileElement pe in GetAllElements()) 
            {
                pe.ClearEquivalence();
            }
        }

        private void ClearBelongsToCategoryAsObject() 
        {
            if (profileMap.ContainsKey(ProfileElementTypes.Class))
            {
               List<Class>  classes = profileMap[ProfileElementTypes.Class].Cast<Class>().ToList();
                foreach (Class cls in classes)
                    cls.BelongsToCategoryAsObject = null;
            }

        }

        private void PopulateSubclassOfClasses()
        {
            if (profileMap != null && profileMap != null)
            {
                //List<Property> properties = null;
                List<Class> classes = null;


                if (profileMap.ContainsKey(ProfileElementTypes.Class))
                {
                    classes = profileMap[ProfileElementTypes.Class].Cast<Class>().ToList();
                }

                if (classes != null)
                {
                    foreach (Class c in classes)
                    {
                        Class cls = classes.Where(x => x.URI.Equals(c.SubClassOf)).SingleOrDefault();
                        c.SubClassOfAsObject = cls;

                    }
                }


            }

        }


        private void PopulateRangeAsObjectProperties()
        {
            if (profileMap != null)
            {
                List<Property> properties = null;
                List<Class> classes = null;


                if (profileMap.ContainsKey(ProfileElementTypes.Class))
                {
                    classes = profileMap[ProfileElementTypes.Class].Cast<Class>().ToList();
                }


                if (profileMap.ContainsKey(ProfileElementTypes.Property))
                {
                    properties = profileMap[ProfileElementTypes.Property].Cast<Property>().ToList();
                }

                if (properties != null && classes != null)
                {
                    foreach (Property p in properties)
                    {
                        Class cls = classes.Where(x => x.URI.Equals(p.Range)).SingleOrDefault();
                        p.RangeAsObject = cls;

                    }
                }

                else if (classes == null)
                {
                    if (properties != null)
                    {
                        foreach (Property p in properties)
                        {
                            p.DomainAsObject = null;
                            p.RangeAsObject = null;
                        }
                    }

                }

            }

        }


        private void PopulateDomainAsObjectProperties()
        {
            if (profileMap != null)
            {
                List<Property> properties = null;
                List<Class> classes = null;


                if (profileMap.ContainsKey(ProfileElementTypes.Class))
                {
                    classes = profileMap[ProfileElementTypes.Class].Cast<Class>().ToList();
                }


                if (profileMap.ContainsKey(ProfileElementTypes.Property))
                {
                    properties = profileMap[ProfileElementTypes.Property].Cast<Property>().ToList();
                }

                if (properties != null && classes != null)
                {
                    foreach (Property p in properties)
                    {
                        Class cls = classes.Where(x => x.URI.Equals(p.Domain)).SingleOrDefault();
                        p.DomainAsObject = cls;
                    }
                }

                else if (classes == null)
                {
                    if (properties != null)
                    {
                        foreach (Property p in properties)
                        {
                            p.DomainAsObject = null;
                        }
                    }
                }

            }


        }

        /// <summary>
        /// Remove lements with certain stereotypes
        /// </summary>
        /// <param name="stereotypes"> Steroetypes to be removed </param>
        public void MarkElementsWithStereotypes(List<string> stereotypes)
        {
            if (profileMap != null && stereotypes != null)
            {
                MarkClassesWithStereotypes(stereotypes);
                MarkPropertiesWithStereotypes(stereotypes);
            }
        }

        public void ProcessProfileInformationAfterParsing(string baseAddressOfDocument, List<string> importNamespaces, EnumRepresentationKindOWL2 enumRepresentation)
        {
            EnumKind = enumRepresentation;
            string filename;
            StringManipulationManager.GenerateNameForFile(FileName, InputOutput.LoadWordsToSkip(), out filename);
            IsReportToBeWrittenToFile = true;
            BaseNS = baseAddressOfDocument + filename;
            ImportNamespaces = importNamespaces;
        }

       

        private void SortElementsInMap()
        {
            if ((profileMap != null) && (profileMap.Count > 0))
            {
                foreach (ProfileElementTypes profileType in profileMap.Keys)
                {
                    if ((profileMap[profileType] != null) && (profileMap[profileType].Count > 0))
                    {
                        //  profileMap[profileType].Sort(CIMComparer.ProfileElementComparer);
                    }
                }
            }
        }


        #region methods for equivalence

        public void ProcessEquivalenceForProfileElementsForCommon(string uriOfCommon)
        {
            if (!string.IsNullOrEmpty(uriOfCommon))
            {
                foreach (ProfileElement pe in GetAllElements())
                {
                    pe.EquivalenceWithAnotherElement(uriOfCommon);
                }
            }
        }

        public void ProcessEquivalenceForProfileElementsForExtensionAndCommon(string extensionBaseAdress, string stereotype, string commonBaseAdress)
        {

            foreach (ProfileElement pe in GetAllElements())
            {
                pe.EquivalenceForAnotherElementCombined(commonBaseAdress, extensionBaseAdress, stereotype);
            }
        }

        public void ProcessEquivalenceForProfileElementsForExtension(string extensionUri, string stereotype)
        {

                foreach (ProfileElement pe in GetAllElements())
                {
                    pe.EquvialenceWithAnotherElementWithStereotype(extensionUri,stereotype);
                }
        }

        #endregion


        #region Method for writing profile to document

        //public void WriteDocumentToFile(IDocumentGenerator generator)
        //{
        //    StringManipulationManager.GenerateNameForFile(FileName, InputOutput.LoadWordsToSkip(), out fileName);
        //    writer.WriteStartDocument(baseAddress + fileName, fileName, importNamespace);
        //    foreach (ProfileElement pe in GetAllElements())
        //    {
        //        writer.WriteElement(pe, baseAddress + fileName, enumOption);
        //    }
        //    writer.WriteEndDocument();
        //}


        #endregion



    }
}