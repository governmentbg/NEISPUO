namespace SB.Domain;

#nullable disable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

public static partial class ExcelHelper
{
    [GeneratedRegex("^([A-Z]{1,3})(\\d{1,7})$", RegexOptions.Singleline)]
    private static partial Regex CellRegex();

    private const NumberStyles OpenXmlNumberStyle = NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign | NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite | NumberStyles.AllowExponent;
    private static readonly CultureInfo OpenXmlParseCulture = CultureInfo.InvariantCulture;

    public const int CellLengthLimit = 32767;

    public static IEnumerable<string[]> ReadExcel(Stream excelStream, int numberOfColumns)
    {
        using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(excelStream, false))
        {
            var workbookPart = spreadsheetDocument.WorkbookPart;
            var workbook = workbookPart.Workbook;

            SharedStringItem[] sharedStrings = Array.Empty<SharedStringItem>();

            if (workbookPart.SharedStringTablePart != null)
            {
                sharedStrings = workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ToArray();
            }

            // search only visible sheets
            var sheet = workbook.Descendants<Sheet>().First();

            var worksheetPart = (WorksheetPart)workbookPart.GetPartById(sheet.Id);

            using (OpenXmlReader reader = OpenXmlReader.Create(worksheetPart))
            {
                int lastRowIndex = 0;
                while (reader.Read())
                {
                    if (reader.ElementType != typeof(Row))
                    {
                        continue;
                    }

                    lastRowIndex++;

                    var rowIndex = reader.Attributes.Where(a => a.LocalName == "r").Select(a => a.Value).FirstOrDefault();
                    if (!string.IsNullOrEmpty(rowIndex))
                    {
                        int ri = int.Parse(rowIndex);
                        while (lastRowIndex < ri)
                        {
                            yield return new string[numberOfColumns];
                            lastRowIndex++;
                        }
                    }

                    string[] row = new string[numberOfColumns];

                    if (reader.ReadFirstChild())
                    {
                        do
                        {
                            if (reader.ElementType != typeof(Cell))
                            {
                                continue;
                            }

                            Cell c = (Cell)reader.LoadCurrentElement()!;

                            int cellColumnId = ColumnIndexToColumnId(ParseCellReference(c.CellReference).columnIndex);

                            if (cellColumnId >= numberOfColumns)
                            {
                                continue;
                            }

                            row[cellColumnId] = GetCellValue(c, sharedStrings);
                        }
                        while (reader.ReadNextSibling());
                    }

                    yield return row;
                }

                yield break;
            }
        }
    }

    private static string GetCellValue(Cell cell, SharedStringItem[] sharedStrings)
    {
        string cellValue = string.Empty;
        if (cell.DataType != null && cell.DataType == CellValues.SharedString)
        {
            if (cell.CellValue != null && !string.IsNullOrWhiteSpace(cell.CellValue.Text))
            {
                SharedStringItem ssi = sharedStrings[int.Parse(cell.CellValue.Text, OpenXmlNumberStyle, OpenXmlParseCulture)];

                StringBuilder sb = new StringBuilder();
                foreach (var e in ssi.Elements())
                {
                    if (e is Text t)
                    {
                        sb.Append(t.Text);
                    }
                    else if(e is Run r)
                    {
                        // a Rich Text Run can have only one Text child element
                        sb.Append(r.Text.Text);
                    }
                    else
                    {
                        // skipping Phonetic Properties and Phonetic Runs
                    }
                }
                cellValue = sb.ToString();
            }
        }
        else if (cell.DataType != null && cell.DataType == CellValues.InlineString)
        {
            if (cell.InlineString != null && cell.InlineString.Text != null)
            {
                cellValue = cell.InlineString.Text.Text;
            }
        }
        else if (cell.DataType != null && (cell.DataType == CellValues.Date || cell.DataType == CellValues.Number))
        {
            if (cell.CellValue != null && !string.IsNullOrWhiteSpace(cell.CellValue.Text))
            {
                cellValue = double.Parse(cell.CellValue.Text, OpenXmlNumberStyle, OpenXmlParseCulture).ToString(CultureInfo.InvariantCulture);
            }
        }
        else
        {
            if (cell.CellValue != null)
            {
                cellValue = cell.CellValue.InnerText;
            }
        }

        return cellValue;
    }

    private static int ColumnIndexToColumnId(string col)
    {
        if (col.Length > 3)
        {
            throw new Exception("Column out of range ");
        }

        int colIndex = 0;
        int[] coef = { 1, 26, 26 * 26 };
        for (int i = 0; i < col.Length; i++)
        {
            colIndex += coef[col.Length - i - 1] * ((int)col[i] - (int)'A' + 1);
        }

        return colIndex - 1;
    }

    public static (int rowIndex, string columnIndex) ParseCellReference(string cellReference)
    {
        var m = CellRegex().Match(cellReference);
        if (!m.Success)
        {
            throw new Exception($"Invalid cellReference '{cellReference}'.");
        }
        return (rowIndex: int.Parse(m.Groups[2].Value), columnIndex: m.Groups[1].Value);
    }
}
