namespace SB.Domain;

using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SB.Common;

public class MissingTopicsReportsExcelExportService : IMissingTopicsReportsExcelExportService
{
    private readonly IMissingTopicsReportsQueryRepository missingTopicsReportsQueryRepository;

    public MissingTopicsReportsExcelExportService(IMissingTopicsReportsQueryRepository missingTopicsReportsQueryRepository)
    {
        this.missingTopicsReportsQueryRepository = missingTopicsReportsQueryRepository;
    }

    public async Task ExportAsync(int schoolYear, int instId, int missingTopicsReportId, Stream outputStream, CancellationToken ct)
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

        var worksheetPart = workbookPart.AddNewPart<WorksheetPart>("missingTopicsReport");

        await this.FillWorksheetPartAsync(
            schoolYear,
            instId,
            missingTopicsReportId,
            worksheetPart,
            workbookStylesPart,
            ct);

        var sheets = workbookPart.Workbook.GetFirstChild<Sheets>()!;
        var lastSheet = sheets.GetLastChild<Sheet>();

        sheets.AppendChild(
            new Sheet()
            {
                Id = workbookPart.GetIdOfPart(worksheetPart),
                SheetId = (lastSheet?.SheetId?.Value ?? 0) + 1,
                Name = "невписани теми",
            });
    }

    public async Task FillWorksheetPartAsync(
        int schoolYear,
        int instId,
        int missingTopicsReportId,
        WorksheetPart worksheetPart,
        WorkbookStylesPart workbookStylesPart,
        CancellationToken ct)
    {
        var report =
            await this.missingTopicsReportsQueryRepository.GetExcelDataAsync(
                schoolYear,
                instId,
                missingTopicsReportId,
                ct);

        worksheetPart.InitNormalWorksheet();

        var tableCellBorder = workbookStylesPart.Stylesheet.AppendBorder(borderStyle: BorderStyleValues.Thin);

        var titleFont = workbookStylesPart.Stylesheet.AppendFont(bold: true, size: 14.0);
        var headerFont = workbookStylesPart.Stylesheet.AppendFont(bold: true, size: 12.0);
        var primaryFont = workbookStylesPart.Stylesheet.AppendFont(size: 9.0);

        var titleTableCellFormat = workbookStylesPart.Stylesheet.AppendCellFormat(
            verticalAlignment: VerticalAlignmentValues.Center,
            horizontalAlignment: HorizontalAlignmentValues.Left,
            wrapText: true,
            fontId: titleFont,
            borderId: tableCellBorder);

        var headerTableCell = workbookStylesPart.Stylesheet.AppendCellFormat(
            verticalAlignment: VerticalAlignmentValues.Center,
            horizontalAlignment: HorizontalAlignmentValues.Center,
            wrapText: true,
            fontId: headerFont,
            borderId: tableCellBorder);

        var primaryTableCell = workbookStylesPart.Stylesheet.AppendCellFormat(
            verticalAlignment: VerticalAlignmentValues.Center,
            horizontalAlignment: HorizontalAlignmentValues.Left,
            wrapText: true,
            fontId: primaryFont,
            borderId: tableCellBorder);

        var primaryTableCellCentered = workbookStylesPart.Stylesheet.AppendCellFormat(
            verticalAlignment: VerticalAlignmentValues.Center,
            horizontalAlignment: HorizontalAlignmentValues.Center,
            wrapText: true,
            fontId: primaryFont,
            borderId: tableCellBorder);

        var dateTableCellCentered = workbookStylesPart.Stylesheet.AppendCellFormat(
            verticalAlignment: VerticalAlignmentValues.Center,
            horizontalAlignment: HorizontalAlignmentValues.Center,
            wrapText: true,
            fontId: primaryFont,
            borderId: tableCellBorder,
            // special date number format added in the Stylesheet
            numberFormatId: 165);

        var footerTableCell = workbookStylesPart.Stylesheet.AppendCellFormat(
            verticalAlignment: VerticalAlignmentValues.Center,
            horizontalAlignment: HorizontalAlignmentValues.Right,
            wrapText: true,
            fontId: headerFont,
            borderId: tableCellBorder);

        // Format number and width of sheet columns
        double primaryColWidth = 13;
        double primaryRowHeight = 15;
        string[] headerRowColumnTitles = new string[] { "Дата", "Клас/Паралелка/Група", "Предмет", "Учители" };
        var mergeCells = worksheetPart.Worksheet.GetMergeCells();

        worksheetPart.Worksheet.GetColumns()
            .AppendCustomWidthColumn(1, 1, primaryColWidth)
            .AppendCustomWidthColumn(2, 2, primaryColWidth * 2)
            .AppendCustomWidthColumn(3, 3, primaryColWidth * 4)
            .AppendCustomWidthColumn(4, 4, primaryColWidth * 3);

        var titleRow =
            worksheetPart.Worksheet
                .AppendRelativeRow(height: primaryRowHeight * 2);

        string teacherFilter = !string.IsNullOrEmpty(report.TeacherName) ? $"на {report.TeacherName} " : "";
        string periodFilter = report.YearAndMonth ?? report.Period.ToLower();

        titleRow
            .AppendRelativeInlineStringCell(
                text: $"Невписани теми {teacherFilter}{periodFilter} към {report.CreateDate}",
                styleIndex: titleTableCellFormat);
        worksheetPart.Worksheet
            .AppendMergeCell($"A{titleRow.RowIndex}:" +
                $"{OpenXmlExtensions.ColumnIdToColumnIndex(headerRowColumnTitles.Length - 1)}{titleRow.RowIndex}");

        var headerRow =
            worksheetPart.Worksheet
                .AppendRelativeRow(height: primaryRowHeight * 2);

        foreach (var columnTitle in headerRowColumnTitles)
        {
            headerRow
                .AppendRelativeInlineStringCell(
                    text: $"{columnTitle}",
                    styleIndex: headerTableCell);
        }

        foreach (var reportItem in report.ReportItems)
        {
            var missingTopicRow =
                worksheetPart.Worksheet
                    .AppendRelativeRow(height: primaryRowHeight);

            missingTopicRow
                .AppendRelativeDateCell(
                    date: reportItem.Date,
                    styleIndex: dateTableCellCentered)
                .AppendRelativeInlineStringCell(
                    text: reportItem.ClassBookName,
                    styleIndex: primaryTableCellCentered)
                .AppendRelativeInlineStringCell(
                    text: reportItem.CurriculumName,
                    styleIndex: primaryTableCell)
                .AppendRelativeInlineStringCell(
                    text: string.Join(", ", reportItem.TeachersNames),
                    styleIndex: primaryTableCellCentered);
        }

        var missingTopicsTotalCountRow =
            worksheetPart.Worksheet
                .AppendRelativeRow(height: primaryRowHeight * 1.4);

        var totalCountTextCell = missingTopicsTotalCountRow
            .AppendRelativeInlineStringCell(
                text: $"Общ брой невписани теми за периода: {report.ReportItems.Length}",
                styleIndex: footerTableCell)
            .GetLastCell();

        var columnsCount = worksheetPart.Worksheet.GetColumns().Count();
        //Every part(cell) of merge cell takes border styles from corresponding merged cell
        for (int i = 0; i < columnsCount - 1; i++)
        {
            missingTopicsTotalCountRow
                .AppendRelativeInlineStringCell(styleIndex: footerTableCell);
        }

        mergeCells.AppendMergeCell(totalCountTextCell!.CellReference!, cols: columnsCount);

        worksheetPart.Worksheet.Finalize();
    }
}
