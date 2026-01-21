namespace SB.Domain;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SB.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using static SB.Domain.ITopicPlanItemsExcelReaderWriter;

public class TopicPlanItemsExcelReaderWriter : ITopicPlanItemsExcelReaderWriter
{
    public string[] TryRead(Stream inputStream, out TopicPlanItemDO[] items)
    {
        List<string> errors = new();

        string[][] rows;
        try
        {
            rows = ExcelHelper.ReadExcel(inputStream, 9).ToArray();
        }
        catch (Exception ex) when (ex is FileFormatException || ex is OpenXmlPackageException)
        {
            items = Array.Empty<TopicPlanItemDO>();
            return new string[] { "Импортираният файл не може да бъде разчетен като Excel Workbook (*.xlsx)!" };
        }

        static bool isEmptyRow(string[] row) =>
            row.All(s => string.IsNullOrWhiteSpace(s));

        if (!rows.Any() ||
            !"№".Equals(rows[0][0]?.Trim(), StringComparison.OrdinalIgnoreCase) ||
            !"Тема".Equals(rows[0][1]?.Trim(), StringComparison.OrdinalIgnoreCase) ||
            !"Забележки".Equals(rows[0][2]?.Trim(), StringComparison.OrdinalIgnoreCase))
        {
            items = Array.Empty<TopicPlanItemDO>();
            return new string[] { "Импортираният файл не е с необходимия шаблон!" };
        }

        List<TopicPlanItemDO> itemsList = new();
        for (int rowIndex = 1; rowIndex < rows.Length; rowIndex++)
        {
            var row = rows[rowIndex];

            if (isEmptyRow(row) && rows.Skip(rowIndex + 1).All(r => isEmptyRow(r)))
            {
                // if all unprocessed rows are empty
                break;
            }

            if (!int.TryParse(row[0], out var number) || number > 1000)
            {
                errors.Add($"Открит е некоректен/липсващ номер на тема на ред {rowIndex + 1} (максималният е 1000)!");
                break;
            }

            string title = row[1];
            string note = row[2];

            if (string.IsNullOrWhiteSpace(title))
            {
                errors.Add($"Задължителното поле 'Тема' на ред {rowIndex + 1} не е попълнено!");
                continue;
            }

            itemsList.Add(
                new TopicPlanItemDO(
                    number,
                    title,
                    note));
        }

        items = itemsList.ToArray();
        return errors.ToArray();
    }

    public void Write(TopicPlanItemDO[] items, Stream outputStream)
    {
        using var document = SpreadsheetDocument.Create(outputStream, SpreadsheetDocumentType.Workbook, autoSave: true);

        WorkbookPart workbookPart = document.AddNewPart<WorkbookPart>(
            OpenXmlExtensions.WorkbookPartContentType,
            "workbook");

        WorkbookStylesPart workbookStylesPart = workbookPart.AddNewPart<WorkbookStylesPart>("styles");
        workbookStylesPart.Stylesheet = new Stylesheet(
            new NumberingFormats(
                new NumberingFormat()
                {
                    NumberFormatId = 165,
                    FormatCode = @"[$]dd\.mm\.yy;@",
                }
            ),
            new Fonts(
                new Font() // Default style
            ),
            new Fills(
                new Fill() // Default fill
            ),
            new Borders(
                new Border() // Default border
            ),
            new CellFormats(
                new CellFormat() // Default cell format
            ));

        workbookPart.Workbook = new Workbook(new Sheets());

        var worksheetPart = workbookPart.AddNewPart<WorksheetPart>("topicPlanItems");

        this.FillWorksheetPart(
            items,
            worksheetPart,
            workbookStylesPart);

        var sheets = workbookPart.Workbook.GetFirstChild<Sheets>()!;
        var lastSheet = sheets.GetLastChild<Sheet>();

        sheets.AppendChild(
            new Sheet()
            {
                Id = workbookPart.GetIdOfPart(worksheetPart),
                SheetId = (lastSheet?.SheetId?.Value ?? 0) + 1,
                Name = "теми",
            });
    }

    private void FillWorksheetPart(
        TopicPlanItemDO[] items,
        WorksheetPart worksheetPart,
        WorkbookStylesPart workbookStylesPart)
    {
        double regNum1ColWidth = 9.0;
        double topicColWidth = 44;

        var fontBoldId = workbookStylesPart.Stylesheet.AppendFont(bold: true);

        var vCenter_hCenter_bold_StyleIndex = workbookStylesPart.Stylesheet.AppendCellFormat(
            verticalAlignment: VerticalAlignmentValues.Center,
            horizontalAlignment: HorizontalAlignmentValues.Center,
            wrapText: true,
            fontId: fontBoldId);

        var vTop_hGeneral_StyleIndex = workbookStylesPart.Stylesheet.AppendCellFormat(
            verticalAlignment: VerticalAlignmentValues.Top,
            horizontalAlignment: HorizontalAlignmentValues.General,
            wrapText: true);

        worksheetPart.InitNormalWorksheet();

        var sheetData = worksheetPart.Worksheet.GetSheetData();

        worksheetPart.Worksheet
            .AppendRelativeCustomWidthColumn(regNum1ColWidth)
            .AppendRelativeCustomWidthColumn(topicColWidth, span: 2);

        //header rows

        var headerRow1 = sheetData.AppendRelativeRow();

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "№",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "Тема",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "Забележки",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        //result rows

        foreach (var item in items)
        {
            var recordRow = sheetData.AppendRelativeRow();

            recordRow.AppendRelativeNumberCell(
                number: item.Number,
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: item.Title,
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: item.Note ?? string.Empty,
                styleIndex: vTop_hGeneral_StyleIndex);
        }

        worksheetPart.Worksheet.Finalize();
    }
}
