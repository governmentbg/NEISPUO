using Kontrax.Utility.Xsd.Models;
using System;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Schema;

namespace Kontrax.Utility.Xsd
{
    public static class XsdParser
    {
        public static Models.Xsd ParseXsd(string filePath)
        {
            Models.Xsd xsd = new Models.Xsd();

            using (XmlReader reader = XmlReader.Create(filePath))
            {
                var schema = XmlSchema.Read(reader, null);

                XmlSchemaSet set = new XmlSchemaSet
                {
                    // the following resolves error "Type 'XXX' is not declared." in .Net Standard 
                    // https://github.com/dotnet/runtime/issues/21946
                    // That's happening because the inner schemas have not been imported. 
                    // In .NET Core, resolving URIs inside a schema is not allowed by default and that's by design. 
                    // You will need to add a XmlUrlResolver to your SchemaSet
                    XmlResolver = new XmlUrlResolver()
                };
                set.Add(schema);

                set.Compile();

                // Вариант 1: Глобалните типове се добавят предварително, независимо дали се използват.
                // Това подобрява поддръжката на фиктивни елементи с maxOccurs="0". Типът на тези елементи не се прочита
                // от .net parser-а, но името на типа (ако не е inline тип) се прочита. Така, типът може да се намери
                // по името му в колекциятга от всички типове, защото глобалните са добавени предварително.
                foreach (XmlSchemaType type in set.GlobalTypes.Values)
                {
                    CreateType(type, xsd, false);
                }

                foreach (XmlSchemaElement element in set.GlobalElements.Values)
                {
                    xsd.AddRoot(CreateElement(element, xsd));
                }

                // Вариант 2: Дава възможност да не се добавят неизползваните типове.
                // Ако е закоментиран Вариант 1, до момента са създадени всички типове, които се използват пряко или косвено
                // от root елементите. Обикновено не е нужно да се създават и неизползваните типове, но за пълнота:
                //AddUnusedTypes(set, xsd);

                // Нещо от интернет на тема MaxLength
                //var results = GetElementMaxLength(set, "Setup_Type");
                //foreach (var item in results)
                //{
                //    builder.AppendFormat("{0} | {1}", item.Key, item.Value);
                //}
            }
            return xsd;
        }

        private static void AddUnusedTypes(XmlSchemaSet set, Models.Xsd xsd)
        {
            foreach (XmlSchemaType type in set.GlobalTypes.Values)
            {
                xsd.Types.TryGetValue(FormatQName(type.QualifiedName), out XsdType xsdType);
                if (xsdType == null)
                {
                    CreateType(type, xsd, false);
                }
            }
        }

        private static XsdType CreateType(XmlSchemaType type, Models.Xsd xsd, bool isInline)
        {
            XsdType xsdType = new XsdType(
                type.Name,
                FormatQName(type.QualifiedName),
                FormatAnnotation(type),
                type.TypeCode != XmlTypeCode.None ? type.TypeCode.ToString() : null);

            // Именуваните типове се добавят към колекцията преди да се създадат елементите им.
            // Така се избягва безкрайната рекурсия при вложени елементи от същия тип.
            if (!isInline && !xsd.Types.ContainsKey(xsdType.QName))
            {
                xsd.AddType(xsdType);
            }

            if (type is XmlSchemaComplexType complexType)
            {
                // Не е ясна разликата между ContentTypeParticle и Particle. За фиктивни елементи с maxOccurs="0",
                // Particle връща правилния тип, например sequence, докато ContentTypeParticle връща private тип EmptyParticle,
                // от който не може да извлече нищо. За елементи с complexContent, Particle е празно, докато ContentTypeParticle
                // връща правилния тип, например sequence. Затова се пробват и двете с приоритет на Particle.
                if (complexType.Particle is XmlSchemaSequence sequence1)
                {
                    xsdType.Sequence = CreateGroup(sequence1, xsd);
                }
                else if (complexType.Particle is XmlSchemaChoice choice1)
                {
                    xsdType.Choice = CreateGroup(choice1, xsd);
                }
                else if (complexType.ContentTypeParticle is XmlSchemaSequence sequence2)
                {
                    xsdType.Sequence = CreateGroup(sequence2, xsd);
                }
                else if (complexType.ContentTypeParticle is XmlSchemaChoice choice2)
                {
                    xsdType.Choice = CreateGroup(choice2, xsd);
                }
                else if (type.TypeCode != XmlTypeCode.None)
                {
                    // Типът е съставен само на теория, защото съдържанието е от прост тип.
                }
                else if (complexType.Particle == null)
                {
                    // Типът е празен(счупен) - няма sequence, choice, нищо.
                }
                else
                {
                    xsdType.Sequence = new XsdGroup(false, new XsdMultiplicity(1, 1), null);
                    xsdType.Sequence.Add(CreateDummyElement("TODO: Неочаквано съдържание на съставен тип", complexType.Particle.GetType()));
                }
            }
            else if (type is XmlSchemaSimpleType simpleType)
            {
                if (simpleType.Content is XmlSchemaSimpleTypeRestriction restriction)
                {
                    foreach (XmlSchemaObject facet in restriction.Facets)
                    {
                        if (facet is XmlSchemaEnumerationFacet enumValue)
                        {
                            xsdType.AddEnumValue(enumValue.Value);
                        }
                        // Съществуват още следните видове ограничения:
                        // XmlSchemaMaxExclusiveFacet
                        // XmlSchemaMaxInclusiveFacet
                        // XmlSchemaMinExclusiveFacet
                        // XmlSchemaMinInclusiveFacet
                        // XmlSchemaPatternFacet
                        // XmlSchemaWhiteSpaceFacet
                        // ...като тези наследяват абстрактния XmlSchemaNumericFacet:
                        // XmlSchemaFractionDigitsFacet
                        // XmlSchemaLengthFacet
                        // XmlSchemaMaxLengthFacet
                        // XmlSchemaMinLengthFacet
                        // XmlSchemaTotalDigitsFacet
                    }
                }
            }
            return xsdType;
        }

        private static XsdElement CreateDummyElement(string name, Type t)
        {
            return new XsdElement(name, null, new XsdMultiplicity(0, 0), new XsdType(t.Name, t.FullName, null, null), null);
        }

        private static XsdElement CreateElement(XmlSchemaElement element, Models.Xsd xsd)
        {
            XmlSchemaType type = element.ElementSchemaType;
            XmlQualifiedName typeQName = element.SchemaTypeName;

            XsdType xsdType = null;
            if (!typeQName.IsEmpty)
            {
                // Използване или добавяне на именуван тип.
                if (!xsd.Types.TryGetValue(FormatQName(typeQName), out xsdType))
                {
                    // За елементи с maxOccurs="0" ElementSchemaType не е попълнено, но типът може да се извлече по името му.
                    // Типът е стандартен, защото ако беше дефиниран в схемата, (при Вариант 1) щеше да бъде намерен от TryGetValue по-горе.
                    if (type == null)
                    {
                        type = XmlSchemaType.GetBuiltInSimpleType(typeQName) ?? (XmlSchemaType)XmlSchemaType.GetBuiltInComplexType(typeQName);
                    }

                    if (type != null)
                    {
                        xsdType = CreateType(type, xsd, false);
                    }
                }
            }
            else
            {
                // Създаване на inline тип.
                // Забележка: За елементи с maxOccurs="0" inline типът не се прочита от .net parser-а.
                if (type != null)
                {
                    xsdType = CreateType(type, xsd, true);
                }
            }

            return new XsdElement(
                element.Name,
                FormatQName(element.QualifiedName),
                CreateMultiplicity(element),
                xsdType,
                FormatAnnotation(element));
        }

        private static XsdAny CreateAny(XmlSchemaAny any)
        {
            return new XsdAny(
                any.Namespace,
                any.ProcessContents.ToString(),
                CreateMultiplicity(any),
                FormatAnnotation(any));
        }

        private static XsdGroup CreateGroup(XmlSchemaGroupBase group, Models.Xsd xsd)
        {
            bool isChoice = group is XmlSchemaChoice;
            XsdGroup xsdGroup = new XsdGroup(isChoice, CreateMultiplicity(group), FormatAnnotation(group));

            foreach (XmlSchemaObject child in group.Items)
            {
                if (child is XmlSchemaElement element)
                {
                    xsdGroup.Add(CreateElement(element, xsd));
                }
                else if (child is XmlSchemaAny any)
                {
                    xsdGroup.Add(CreateAny(any));
                }
                else if (child is XmlSchemaGroupBase childGroup)
                {
                    xsdGroup.Add(CreateGroup(childGroup, xsd));
                }
                else
                {
                    string groupType = group.GetType().Name.Replace("XmlSchema", string.Empty);
                    xsdGroup.Add(CreateDummyElement($"TODO: Неочакван обект в {groupType}.Items", child.GetType()));
                }
            }
            return xsdGroup;
        }

        private static XsdMultiplicity CreateMultiplicity(XmlSchemaParticle particle)
        {
            return new XsdMultiplicity(
                (int)particle.MinOccurs,
                particle.MaxOccurs < decimal.MaxValue ? (int)particle.MaxOccurs : (int?)null);
        }

        private static string FormatAnnotation(XmlSchemaAnnotated node)
        {
            List<string> lines = new List<string>();
            XmlSchemaAnnotation annotation = node.Annotation;
            if (annotation != null)
            {
                foreach (var annItem in annotation.Items)
                {
                    if (annItem is XmlSchemaDocumentation doc)
                    {
                        foreach (XmlNode docNode in doc.Markup)
                        {
                            lines.Add(docNode.OuterXml);
                        }
                    }
                    else
                    {
                        lines.Add("TODO: Неочакван обект в Annotation.Items от тип " + annItem.GetType().Name);
                    }
                }
            }
            return lines.Count > 0 ? string.Join(Environment.NewLine, lines) : null;
        }

        private static string FormatQName(XmlQualifiedName qName)
        {
            return qName.IsEmpty ? null : qName.ToString();
        }

        #region MaxLength

        public static Dictionary<string, int> GetElementMaxLength(XmlSchemaSet set, String xsdElementName)
        {
            if (xsdElementName == null) throw new ArgumentException();
            // if your XSD has a target namespace, you need to replace null with the namespace name
            var qname = new XmlQualifiedName(xsdElementName, null);

            // find the type you want in the XmlSchemaSet    
            var parentType = set.GlobalTypes[qname];

            // call GetAllMaxLength with the parentType as parameter
            var results = GetAllMaxLength(set, parentType);

            return results;
        }

        private static Dictionary<string, int> GetAllMaxLength(XmlSchemaSet set, XmlSchemaObject obj)
        {
            Dictionary<string, int> dict = new Dictionary<string, int>();

            // do some type checking on the XmlSchemaObject
            if (obj is XmlSchemaSimpleType)
            {
                // if it is a simple type, then call GetMaxLength to get the MaxLength restriction
                var st = obj as XmlSchemaSimpleType;
                dict[st.QualifiedName.Name] = GetMaxLength(st);
            }
            else if (obj is XmlSchemaComplexType)
            {

                // if obj is a complexType, cast the particle type to a sequence
                //  and iterate the sequence
                //  warning - this will fail if it is not a sequence, so you might need
                //  to make some adjustments if you have something other than a xs:sequence
                var ct = obj as XmlSchemaComplexType;
                var seq = ct.ContentTypeParticle as XmlSchemaSequence;

                foreach (var item in seq.Items)
                {
                    // item will be an XmlSchemaObject, so just call this same method
                    //  with item as the parameter to parse it out
                    var rng = GetAllMaxLength(set, item);

                    // add the results to the dictionary
                    foreach (var kvp in rng)
                    {
                        dict[kvp.Key] = kvp.Value;
                    }
                }
            }
            else if (obj is XmlSchemaElement)
            {
                // if obj is an XmlSchemaElement, the you need to find the type
                //  based on the SchemaTypeName property.  This is why your 
                //  XmlSchemaSet needs to have class-level scope
                var ele = obj as XmlSchemaElement;
                var type = set.GlobalTypes[ele.SchemaTypeName];

                // once you have the type, call this method again and get the dictionary result
                var rng = GetAllMaxLength(set, type);

                // put the results in this dictionary.  The difference here is the dictionary
                //  key is put in the format you specified
                foreach (var kvp in rng)
                {
                    dict[string.Format("{0}/{1}", ele.QualifiedName.Name, kvp.Key)] = kvp.Value;
                }
            }

            return dict;
        }

        private static int GetMaxLength(XmlSchemaSimpleType xsdSimpleType)
        {
            // get the content of the simple type
            var restriction = xsdSimpleType.Content as XmlSchemaSimpleTypeRestriction;

            // if it is null, then there are no restrictions and return -1 as a marker value
            if (restriction == null) return -1;

            int result = -1;

            // iterate the facets in the restrictions, look for a MaxLengthFacet and parse the value
            foreach (XmlSchemaObject facet in restriction.Facets)
            {
                if (facet is XmlSchemaMaxLengthFacet)
                {
                    result = int.Parse(((XmlSchemaFacet)facet).Value);
                    break;
                }
            }

            return result;
        }

        #endregion
    }
}
