using RDFSParserOWL2.Common.Comparers;
using RDFSParserOWL2.Generator.Helper;
using RDFSParserOWL2.Manager;
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

		private bool loaderErrorOcurred = false;


		static Profile()
		{
			StereotypeList = new List<ProfileElementStereotype>();
			// init common stereotypes list
			int index = 0;
			StereotypeList.Add(new ProfileElementStereotype(index++, ProfileElementStereotype.StereotypeConcrete));
			StereotypeList.Add(new ProfileElementStereotype(index++, ProfileElementStereotype.StereotypeCompound));
			StereotypeList.Add(new ProfileElementStereotype(index++, ProfileElementStereotype.StereotypeAttribute));
			StereotypeList.Add(new ProfileElementStereotype(index++, ProfileElementStereotype.StereotypeEnumeration));
			StereotypeList.Add(new ProfileElementStereotype(index++, ProfileElementStereotype.StereotypeByReference));
			StereotypeList.Add(new ProfileElementStereotype(index++, ProfileElementStereotype.StereotypeAggregateOf));
			StereotypeList.Add(new ProfileElementStereotype(index++, ProfileElementStereotype.StereotypeOfAggregate));
			StereotypeList.Add(new ProfileElementStereotype(index++, ProfileElementStereotype.StereotypeCompositeOf));
			StereotypeList.Add(new ProfileElementStereotype(index++, ProfileElementStereotype.StereotypeEntsoe));
			StereotypeList.Add(new ProfileElementStereotype(index++, ProfileElementStereotype.StereotypePrimitive));
		}


		public Profile()
		{
			profileMap = new SortedDictionary<ProfileElementTypes, List<ProfileElement>>();
		}

		public Profile(string sourcePath)
		{
			profileMap = new SortedDictionary<ProfileElementTypes, List<ProfileElement>>();
			SourcePath = sourcePath;
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



		public bool ProcessEntsoeElements(Profile entsoProfile)
		{
			bool changed = false;
			if (entsoProfile != null)
			{

				foreach (ProfileElementTypes type in profileMap.Keys)
				{
					switch (type)
					{
						case ProfileElementTypes.Class:
							List<Class> classesToAdd = null;
							List<Class> classes = null;
							List<Class> entsoClasses = null;

							if (entsoProfile.ProfileMap.ContainsKey(type))
							{
								entsoClasses = entsoProfile.ProfileMap[type].Cast<Class>().ToList();
							}

							if (profileMap.ContainsKey(type))
							{
								classes = profileMap[type].Cast<Class>().Where(x => x.HasStereotype(OWL2Namespace.Entsoe)).ToList();
							}

							if (entsoClasses == null && classes != null)
							{
								classesToAdd = new List<Class>();
								classesToAdd.AddRange(classes);
								entsoClasses = new List<Class>();
							}
							else if (entsoClasses != null && classes != null)
							{
								classesToAdd = classes.Except(entsoClasses, new ClassComparer()).ToList();
							}

							if (classesToAdd != null && classesToAdd.Count > 0)
							{
								if (entsoClasses == null)
								{
									entsoClasses = new List<Class>();
								}

								entsoClasses.AddRange(classesToAdd);
								entsoProfile.ProfileMap[type] = entsoClasses.Cast<ProfileElement>().ToList();
								changed = true;
							}


							foreach (Class cls in classes)
							{


								List<Property> propToAdd = null;
								List<Property> entsoProp = null;
								List<Property> prop = null;

								if (entsoProfile.ProfileMap.ContainsKey(ProfileElementTypes.Property))
								{
									entsoProp = entsoProfile.ProfileMap[ProfileElementTypes.Property].Cast<Property>().ToList();
								}

								if (cls.MyProperties != null)
								{
									prop = cls.MyProperties.Cast<Property>().ToList();
								}


								DifferenceBeetwenListOfProperties(ref entsoProp, prop, out propToAdd);

								if (prop != null && propToAdd.Count > 0)
								{
									if (entsoProp == null)
									{
										entsoProp = new List<Property>();
									}

									entsoProp.AddRange(propToAdd);
									entsoProfile.ProfileMap[ProfileElementTypes.Property] = entsoProp.Cast<ProfileElement>().ToList();
									changed = true;
								}

							}

							break;

						case ProfileElementTypes.Property:
							List<Property> propertiesToAdd = null;
							List<Property> entsoProperties = null;
							List<Property> properties = null;

							if (entsoProfile.ProfileMap.ContainsKey(type))
							{
								entsoProperties = entsoProfile.ProfileMap[type].Cast<Property>().ToList();
							}

							if (profileMap.ContainsKey(type))
							{
								properties = profileMap[type].Cast<Property>().Where(x => x.HasStereotype(OWL2Namespace.Entsoe)).ToList();
							}


							DifferenceBeetwenListOfProperties(ref entsoProperties,properties,out propertiesToAdd);

							//if (entsoProperties == null && properties != null)
							//{
							//	propertiesToAdd.AddRange(properties);
							//	entsoProperties = new List<Property>();
							//}
							//else if (entsoProperties != null && properties != null)
							//{
							//	propertiesToAdd = properties.Except(entsoProperties, new PropertyComparer()).ToList();

							//}

							if (propertiesToAdd != null && propertiesToAdd.Count > 0)
							{
								if (entsoProperties == null)
								{
									entsoProperties = new List<Property>();
								}

								entsoProperties.AddRange(propertiesToAdd);
								entsoProfile.ProfileMap[type] = entsoProperties.Cast<ProfileElement>().ToList();
								changed = true;
							}

							//DifferenceBeetwenListOfProperties(ref properties, entsoProp,out propertiesToAdd);
							//if (entsoProperties == null && properties != null)
							//{
							//	propertiesToAdd.AddRange(properties);
							//	entsoProperties = new List<Property>();
							//	//entsoProfile.ProfileMap[type] = properties.Cast<ProfileElement>().ToList();
							//}
							//else if (entsoProperties != null && properties != null)
							//{
							//	foreach (Property p in properties)
							//	{
							//		if (!entsoProperties.Contains(p))
							//			propertiesToAdd.Add(p);
							//	}

							//}
							//else
							//{
							//	continue;
							//}

							////if (properties != null)
							////{
							//entsoProperties.AddRange(propertiesToAdd);
							//entsoProfile.ProfileMap[type] = entsoProperties.Cast<ProfileElement>().ToList();
							//if (propertiesToAdd.Count > 0)
							//	changed = true;

							//}
							break;


					}
				}
				//List<Property> entsoProperties = GetAllProfileElementsOfType(ProfileElementTypes.Property).Cast<Property>().ToList();


			}

			return changed;


		}



		private void DifferenceBeetwenListOfProperties(ref List<Property> firstList, List<Property> secondList, out List<Property> propertiesToAdd) 
		{
			propertiesToAdd = null;


			if (firstList == null && secondList != null)
			{
				propertiesToAdd.AddRange(secondList);
				firstList = new List<Property>();
			}
			else if (firstList!= null && secondList != null)
			{
				propertiesToAdd = secondList.Except(firstList, new PropertyComparer()).ToList();

			}
		}

		//private void DifferenceBeetwenListOfProperties(ref List<Property> firstList, List<Property> secondList, out List<Property> propertiesToAdd)
		//{

		//	//propertiesToAdd = new List<Property>();


		//	//if (firstList == null && secondList != null)
		//	//{
		//	//	propertiesToAdd.AddRange(secondList);
		//	//	firstList = new List<Property>();
		//	//}
		//	//else if (firstList != null && secondList != null)
		//	//{
		//	//	foreach (Property p in secondList)
		//	//	{
		//	//		if (!firstList.Contains(p))
		//	//			firstList.Add(p);
		//	//	}

		//	//}

		//}


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
					List<ProfileElement> list = profileMap[type];
					foreach (ProfileElement elem in list)
					{
						if (uri.Equals(elem.URI))
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

		public void PopulateDomainAsObjectProperties()
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
						if (cls == null)
						{
							p.DomainAsObject = cls;
						}
						else
						{
							p.DomainAsObject = cls;
						}
					}
				}

				else if (classes == null)
				{
					foreach (Property p in properties)
					{
						p.DomainAsObject = null;
					}

				}



			}


		}

		/// <summary>
		/// Remove lements with certain stereotypes
		/// </summary>
		/// <param name="stereotypes"> Steroetypes to be removed </param>
		public void RemoveElementsWithStereotypes(List<string> stereotypes)
		{
			if (profileMap != null && stereotypes != null)
			{
				RemoveClassesWithStereotypes(stereotypes);
				RemovePropertiesWithStereotypes(stereotypes);
			}
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

		public bool IsEntsoe()
		{
			return fileName.Equals(OWL2Namespace.EntsoeOwl);

		}
	}
}