using Kontrax.RegiX.Core.TestStandard.Models;
using Kontrax.RegiX.Core.TestStandard.Models.RegiXReport;
using Kontrax.RegiX.Core.TestStandard.Models.Xsd;
using Kontrax.Utility.Xsd;
using Kontrax.Utility.Xsd.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Kontrax.RegiX.Core.TestStandard.Services
{
    public class XsdService
    {
        public static XsdModel ParseXsd(string xsdPath)
        {
            Xsd xsd = XsdParser.ParseXsd(xsdPath);

            MultiNodeModel multiNode = null;
            string warning = null;
            if (xsd.Roots.Count > 0)
            {
                XsdElement firstRoot = xsd.Roots[0];
                if (xsd.Roots.Count > 1)
                {
                    warning = $"В схемата има {xsd.Roots.Count} root елемента. Ще бъде използван само първият.";
                }
                multiNode = CreateMultiNodeModel(firstRoot);
            }
            else
            {
                warning = "В схемата няма нито един root елемент.";
            }

            return new XsdModel
            {
                Path = xsdPath,
                Root = multiNode,
                RootWarning = warning,
                Description = xsd.ToString()
            };
        }

        private static MultiNodeModel CreateMultiNodeModel(XsdObject o)
        {
            if (o is XsdElement element)
            {
                MultiNodeModel model = new MultiNodeModel
                {
                    QName = element.QName,
                    Name = element.Name,
                    Title = element.Description ?? (element.Name != null ? $"[{element.Name}]" : null),
                };
                CreateEmptyInstances(model, element.Multiplicity);

                XsdType type = element.Type;
                if (type != null)
                {
                    string simpleTypeCode = type.SimpleTypeCode;
                    if (simpleTypeCode != null)
                    {
                        model.TypeName = simpleTypeCode;
                    }
                    else
                    {
                        model.TypeName = type.Name;
                        model.TypeTitle = type.Description;

                        if (model.Instances != null)
                        {
                            foreach (DataNodeModel instance in model.Instances)
                            {
                                instance.Children = CreateChildModels(type);
                            }
                        }
                    }

                    model.EnumValues = type.EnumValues.ToArray();
                }
                return model;
            }

            // За sequence и choice елементите се създава изкуствена група от контроли.
            if (o is XsdGroup group)
            {
                int itemCount = group.Items.Count;
                MultiNodeModel model = new MultiNodeModel
                {
                    // На теория sequence и choice поддържат анотация/документация, но на практика тя не се прочита от .xsd файла и group.Description е null.
                    Title = group.Description ?? (group.IsChoice
                        ? $"Въведете само едно от {itemCount}-те"
                        : $"Въведете всички {itemCount} елемента"),
                };
                CreateEmptyInstances(model, group.Multiplicity);

                foreach (DataNodeModel instance in model.Instances)
                {
                    instance.Children = group.Items.Select(i => CreateMultiNodeModel(i)).ToArray();
                }
                return model;
            }

            throw new Exception($"Не се поддържа {o.GetType().Name}.");
        }

        private static void CreateEmptyInstances(MultiNodeModel model, XsdMultiplicity multiplicity)
        {
            int min = multiplicity.Min;
            int? max = multiplicity.Max;
            model.Min = min;
            model.Max = max;
            model.MultiplicitySymbol = multiplicity.ToString();

            if (!max.HasValue || max > 0)
            {
                // TODO: Засега са една-две стойности (или един-два набора children).
                // Да се поддържат min и max по-пълноценно.
                List<DataNodeModel> instances = new List<DataNodeModel>();
                for (int i = 0; i < min; i++)
                {
                    instances.Add(new DataNodeModel { IsRequired = true });
                }
                if (!max.HasValue || max > min)
                {
                    instances.Add(new DataNodeModel());
                }
                model.Instances = instances.ToArray();
            }
            //else Срещат се dummy елементи с minOccurs==maxOccurs==0. Те нямат инстанции.
        }

        private static MultiNodeModel[] CreateChildModels(XsdType type)
        {
            if (type.Sequence != null)
            {
                //return type.Sequence.Items.Select(i => CreateMultiNodeModel(i)).ToArray();
                return type.Sequence.Multiplicity.Max == 1
                    ? type.Sequence.Items.Select(i => CreateMultiNodeModel(i)).ToArray()
                    : new MultiNodeModel[] { CreateMultiNodeModel(type.Sequence) };
            }
            else if (type.Choice != null)
            {
                return new MultiNodeModel[] { CreateMultiNodeModel(type.Choice) };
            }
            else
            {
                return new MultiNodeModel[0];
            }
        }

        private static RequestEditModel CreateEmptyRequest(Dependency dependency, IdNameModel administration, UserPermissionsModel currentUser)
        {
            RequestEditModel model = CreateEmptyRequest(dependency.RegiXReport);
            model.Dependency.LegalBasis = dependency.LegalBasis;
            model.Dependency.Permission = currentUser.RegiXReportIsAllowed(dependency.RegiXReportId, administration);
            return model;
        }

        public static string GetRequestXML(RegiXReportModel report, RegiXReportKey[] testKeyValues)
        {
            string xml = "";
            RequestEditModel model = CreateEmptyRequest(report);
            MultiNodeModel root = model.Xsd?.Root;
            if (root != null)
            {
                xml = BuildXml(root);

                // set parameter value
                SetKeyElements(root, testKeyValues);

            }

            return xml;
        }




        private static string BuildXml(MultiNodeModel root)
        {
            DataNode[] rootNodes = CreateDataNodes(root);
            // Root елементът винаги има точно една стойност или точно един набор от поделементи.
            return rootNodes.Length > 0 ? XmlBuilder.BuildXml(rootNodes[0]) : null;
        }

        private static DataNode[] CreateDataNodes(MultiNodeModel model)
        {
            DataNodeModel[] instances = model.Instances;
            if (instances == null)
            {
                return new DataNode[0];
            }

            // Ако това е групиращ елемент (sequence или choice), той няма стойности, а поделементите му се изсипват в по-горното ниво.
            if (model.QName == null)
            {
                if (instances.Any(i => i.Value != null))
                {
                    throw new Exception("Групиращ елемент (sequence или choice) има стойности, а трябва да има само поделементи.");
                }
                return (
                    from i in instances
                    where i.SupportsChildren
                    from c in i.Children
                    select CreateDataNodes(c)
                ).SelectMany(dn => dn).ToArray();
            }

            return (
                from i in instances
                where !string.IsNullOrEmpty(i.Value) || i.SupportsChildren
                select new DataNode(
                    model.QName,
                    i.Value,
                    i.SupportsChildren ? i.Children.Select(c => CreateDataNodes(c)).SelectMany(dn => dn).ToArray() : null)
            ).ToArray();
        }

        private static RequestEditModel CreateEmptyRequest(RegiXReportModel regiXReport)
        {
            RequestEditModel model = new RequestEditModel
            {
                Dependency = new AllowedDependencyViewModel
                {
                    RegiXReport = new RegiXReportBaseModel
                    {
                        Id = regiXReport.Id,
                        RegisterName = regiXReport.RegisterName,
                        ReportName = regiXReport.ReportName,
                        IsDeleted = regiXReport.IsDeleted
                    }
                },
                //Help = regiXReport.Help,
                ValidationErrors = CallService.GetRegiXReportRequestConfigErrors(regiXReport)
            };

            string xsdPath = null;
            try
            {
                xsdPath = AdapterFileReader.DemandXsdPath(regiXReport.AdapterSubdirectory, regiXReport.RequestXsd, false);
                model.Xsd = ParseXsd(xsdPath);
            }
            catch (Exception ex)
            {
                model.Xsd = new XsdModel { Path = xsdPath };
                model.ValidationErrors.Add(ex.Message);
            }

            MultiNodeModel root = model.Xsd.Root;
            if (root != null)
            {
                SetKeyElements(root, regiXReport.RegiXReportKeys);
                root.Request = model;
            }
            return model;
        }

        private static void SetKeyElements(MultiNodeModel root, IEnumerable<RegiXReportKey> regiXReportKeys)
        {
            Dictionary<string, KeyElementViewModel> keyElements = (
                from k in regiXReportKeys
                group k by k.ElementName into g
                select new KeyElementViewModel
                {
                    LocalName = g.Key,
                    KeyTypes =
                        from k in g
                        orderby k.KeyType.Name
                        select new KeyTypeModel
                        {
                            Code = k.TypeCode,
                            Name = k.KeyType.Name
                        }
                }
            ).ToDictionary(k => k.LocalName);

            root.SetKeyElements(keyElements);
        }


    }
}
