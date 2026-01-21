using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Domain
{
    public static class WordTemplateTransformer
    {
        // filters control characters but allows only properly-formed surrogate sequences
        private static Regex _invalidXMLChars = new Regex(
            @"(?<![\uD800-\uDBFF])[\uDC00-\uDFFF]|[\uD800-\uDBFF](?![\uDC00-\uDFFF])|[\x00-\x08\x0B\x0C\x0E-\x1F\x7F-\x9F\uFEFF\uFFFE\uFFFF]",
            RegexOptions.Compiled);

        /// <summary>
        /// removes any unusual unicode characters that can't be encoded into XML
        /// </summary>
        public static string RemoveInvalidXMLChars(string text)
        {
            if (string.IsNullOrEmpty(text)) return "";
            return _invalidXMLChars.Replace(text, "");
        }

        public static void Transform(Stream template, string jsonContext)
        {
            using WordprocessingDocument doc = WordprocessingDocument.Open(template, true);
            MainDocumentPart main = doc.MainDocumentPart ?? throw new Exception("Document has no MainDocumentPart");
            var footer = main.FooterParts;

            if (jsonContext.Length == 0)
            {
                return;
            }

            using JsonDocument jDoc = JsonDocument.Parse(jsonContext);
            JsonElement root = jDoc.RootElement;

            foreach (var sdtElement in main.Document.Descendants<SdtElement>().ToList())
            {
                var tag = sdtElement.GetFirstChild<SdtProperties>()?.GetFirstChild<Tag>()?.Val;
                if (tag == "__delete__")
                {
                    sdtElement.Remove();
                }
            }

            foreach (var sdtElement in ClosestDescendants<SdtElement>(main.Document).ToList())
            {
                try
                {
                    TransformElement(sdtElement, root);
                }
                catch { }
            }

            foreach (var sdtElement in main.Document.Descendants<SdtElement>().Reverse())
            {
                RemoveContentControl(sdtElement);
            }

            foreach (var footerPart in footer.ToList())
            {
                if (footerPart != null)
                {
                    foreach (var sdtElement in ClosestDescendants<SdtElement>(footerPart.Footer).ToList())
                    {
                        TransformElement(sdtElement, root);
                    }

                    foreach (var sdtElement in footerPart.Footer.Descendants<SdtElement>().Reverse())
                    {
                        RemoveContentControl(sdtElement);
                    }
                }
            }
        }

        private static IEnumerable<T> ClosestDescendants<T>(OpenXmlElement root) where T : OpenXmlElement
        {
            foreach (var child in root.ChildElements)
            {
                if (child is T childEl)
                {
                    yield return childEl;
                }
                else
                {
                    foreach (var descendant in ClosestDescendants<T>(child))
                    {
                        yield return descendant;
                    }
                }
            }
        }

        private static void TransformElement(SdtElement sdtElement, JsonElement root)
        {
            var tag = sdtElement.GetFirstChild<SdtProperties>()?.GetFirstChild<Tag>();
            if (tag?.Val?.InnerText == null || root.ValueKind == JsonValueKind.Undefined)
            {
                return;
            }

            bool isExist = root.TryGetProperty(tag.Val.InnerText, out JsonElement jsonEl);
            if (!isExist)
            {
                return;
            }

            if (!sdtElement.Descendants<SdtElement>().Any())
            {
                if (sdtElement is SdtBlock sdtElBlock)
                {
                    SdtBlock sdtBlock = sdtElBlock;

                    Paragraph p = sdtBlock.SdtContentBlock!.GetFirstChild<Paragraph>()!;

                    Run r = p.GetFirstChild<Run>()!;

                    SetRunText(r, jsonEl);

                    RemoveAllChildrenButFirst<Run>(p);
                    RemoveAllChildrenButFirst<Paragraph>(sdtBlock.SdtContentBlock);
                }
                else if (sdtElement is SdtRun sdtEl)
                {
                    SdtRun sdtRun = sdtEl;
                    Run r = sdtRun.SdtContentRun!.GetFirstChild<Run>()!;

                    bool preserveContent = false;
                    var checkbox = sdtRun.SdtProperties!.GetFirstChild<DocumentFormat.OpenXml.Office2010.Word.SdtContentCheckBox>();
                    if (checkbox != null)
                    {
                        preserveContent = true;
                    }
                    SetRunText(r, jsonEl, preserveContent);

                    RemoveAllChildrenButFirst<Run>(sdtRun.SdtContentRun);
                }
                else if (sdtElement is SdtCell sdtElCell)
                {
                    SdtCell sdtCell = sdtElCell;
                    TableCell cell = sdtCell.SdtContentCell!.GetFirstChild<TableCell>()!;
                    Paragraph p = cell.GetFirstChild<Paragraph>()!;
                    Run r = p.GetFirstChild<Run>()!;
                    SetRunText(r, jsonEl);

                    RemoveAllChildrenButFirst<Run>(p);
                    RemoveAllChildrenButFirst<Paragraph>(cell);
                }
            }
            else
            {
                if (jsonEl.ValueKind == JsonValueKind.Array)
                {
                    OpenXmlElement insertAfter = sdtElement;
                    foreach (var arrayItem in jsonEl.EnumerateArray())
                    {
                        var clone = sdtElement.CloneNode(true);
                        sdtElement.Parent!.InsertAfter(clone, insertAfter);
                        insertAfter = clone;

                        foreach (SdtElement childSdtElement in ClosestDescendants<SdtElement>(clone).ToList())
                        {
                            TransformElement(childSdtElement, arrayItem);
                        }
                    }

                    if (jsonEl.ValueKind == JsonValueKind.Undefined)
                    {
                        sdtElement.Parent!.Append(new Paragraph());
                    }

                    sdtElement.Remove();
                }
                else
                {
                    if (jsonEl.ValueKind == JsonValueKind.Undefined)
                    {
                        sdtElement.Parent!.Append(new Paragraph());
                        sdtElement.Remove();
                    }
                    else
                    {
                        foreach (SdtElement childSdtElement in ClosestDescendants<SdtElement>(sdtElement).ToList())
                        {
                            TransformElement(childSdtElement, jsonEl);
                        }
                    }
                    foreach (SdtElement childSdtElement in ClosestDescendants<SdtElement>(sdtElement).ToList())
                    {
                        TransformElement(childSdtElement, jsonEl);
                    }
                }
            }
        }

        private static void RemoveContentControl(SdtElement sdtElement)
        {
            OpenXmlElement sdtContent;
            if (sdtElement.GetFirstChild <SdtContentRun>() != null)
            {
                sdtContent = sdtElement.GetFirstChild<SdtContentRun>()!;
            }
            else if (sdtElement.GetFirstChild<SdtContentBlock>() != null)
            {
                sdtContent = sdtElement.GetFirstChild<SdtContentBlock>()!;
            }
            else if (sdtElement.GetFirstChild<SdtContentRow>() != null)
            {
                sdtContent = sdtElement.GetFirstChild<SdtContentRow>()!;
            }
            else if (sdtElement.GetFirstChild<SdtContentCell>() != null)
            {
                sdtContent = sdtElement.GetFirstChild<SdtContentCell>()!;
            }
            else
            {
                return;
            }

            var checkBox = sdtElement.SdtProperties!.GetFirstChild<DocumentFormat.OpenXml.Office2010.Word.SdtContentCheckBox>();
            if (checkBox == null)
            {
                OpenXmlElement parent = sdtElement.Parent!;
                OpenXmlElementList childElements = sdtContent.ChildElements;

                foreach (OpenXmlElement childElement in childElements)
                {
                    parent.InsertBefore((OpenXmlElement)childElement.Clone(), sdtElement);
                }

                sdtElement.Remove();
            }
            else
            {
                sdtElement.SdtProperties.RemoveAllChildren();
                sdtElement.SdtProperties.InsertAt((DocumentFormat.OpenXml.Office2010.Word.SdtContentCheckBox)checkBox.Clone(), 0);
            }
        }

        private static void SetRunText(Run r, JsonElement jsonContext, bool preserveContent = false)
        {
            Text runText = r.GetFirstChild<Text>();
            if (runText == null)
            {
                runText = new Text();
                r.AppendChild(runText);
            }
            runText.SetAttribute(new OpenXmlAttribute("space", XNamespace.Xml.NamespaceName, "preserve"));

            if (jsonContext.ValueKind == JsonValueKind.Undefined)
            {
                runText.Text = string.Empty;
            }

            if (preserveContent && jsonContext.GetRawText().Length == 0)
            {
                return;
            }
            else if (DateTime.TryParse(jsonContext.GetRawText(), out var dateTime))
            {
                runText.Text = dateTime.ToString("dd.MM.yyyy");
            }
            else if (jsonContext.ValueKind == JsonValueKind.False || jsonContext.ValueKind == JsonValueKind.True)
            {
                runText.Text = jsonContext.GetBoolean() ? "☒" : runText.Text;
            }
            else
            {
#pragma warning disable CS8601 // Possible null reference assignment.
                runText.Text = RemoveInvalidXMLChars(jsonContext.GetString());
#pragma warning restore CS8601 // Possible null reference assignment.
            }
        }

        private static void RemoveAllChildrenButFirst<T>(OpenXmlCompositeElement content) where T : OpenXmlElement
        {
            foreach (var e in content.Descendants<T>().Skip(1).Reverse())
            {
                e.Remove();
            }
        }
    }
}
