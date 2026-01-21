using Kontrax.RegiX.Core.TestStandard.Models.Xsd;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Linq;

namespace Kontrax.RegiX.Core.TestStandard.Services
{
    public static class XmlBuilder
    {
        private const string nsAttrName = "xmlns";

        public static string BuildXml(DataNode rootNode)
        {
            // Namespace-ът на главния елемент се задава като default namespace за документа,
            // т.е. при сериализация автоматично се появява атрибут xmlns="<ns>" на главния елемент.
            SplitQName(rootNode.QName, out string ns, out string localName);
            XmlDocument doc = new XmlDocument();
            XmlElement rootElement = doc.CreateElement(localName, ns);

            doc.AppendChild(rootElement);
            FillXmlElement(doc, rootElement, rootNode);
            return BeautifyXml(doc.OuterXml);
        }

        public static string BeautifyXml(string xml)
        {
            if (!string.IsNullOrWhiteSpace(xml))
            {
                try
                {
                    return XDocument.Parse(xml).ToString();
                }
                catch
                {
                    // Подаденият текст вероятно не е XML, затова се връща в оригинал.
                }
            }
            return xml;
        }

        private static void FillXmlElement(XmlDocument doc, XmlElement element, DataNode dataNode)
        {
            string value = dataNode.Value;
            if (value != null)
            {
                element.AppendChild(doc.CreateTextNode(value));
            }

            IReadOnlyList<DataNode> children = dataNode.Children;
            if (children != null)
            {
                foreach (DataNode childNode in children)
                {
                    XmlElement childElement = CreateXmlElement(doc, childNode);

                    // Празните елементи не се добавят.
                    // TODO: Да се съобрази с изискването на .xsd-то.
                    if (!string.IsNullOrEmpty(childElement.InnerXml))
                    {
                        element.AppendChild(childElement);
                    }
                }
            }
        }

        private static XmlElement CreateXmlElement(XmlDocument doc, DataNode dataNode)
        {
            DeclareNamespace(doc.DocumentElement, dataNode.QName, out string prefix, out string ns, out string localName);
            XmlElement element = doc.CreateElement(prefix, localName, ns);
            FillXmlElement(doc, element, dataNode);
            return element;
        }

        private static void DeclareNamespace(XmlElement element, string qName, out string prefix, out string ns, out string localName)
        {
            prefix = null;
            SplitQName(qName, out ns, out localName);

            if (ns == null || ns == element.NamespaceURI)
            {
                return;  // Празен префикс.
            }

            foreach (XmlAttribute attr in element.Attributes)
            {
                string attrName = attr.Name;
                string attrValue = attr.Value;
                if (attrName.StartsWith($"{nsAttrName}:") && attrValue == ns)
                {
                    prefix = attrName.Substring(6);
                    break;
                }
            }

            // Всички други namespaces (без този на главния елемент) се декларират с префикси n1, n2 и т.н.
            if (prefix == null)
            {
                prefix = $"n{element.Attributes.Count + 1}";
                element.SetAttribute($"{nsAttrName}:{prefix}", ns);
            }
        }

        private static void SplitQName(string qName, out string ns, out string localName)
        {
            if (!string.IsNullOrEmpty(qName))
            {
                int lastColon = qName.LastIndexOf(':');
                ns = lastColon >= 0 ? qName.Substring(0, lastColon) : null;
                localName = lastColon >= 0 ? qName.Substring(lastColon + 1) : qName;
            }
            else
            {
                ns = null;
                localName = "NoName";
            }
        }
    }
}
