namespace SB.Domain;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SB.Common;

public class AbsencesByClassesReportsExcelExportService : IAbsencesByClassesReportsExcelExportService
{
    private readonly IAbsencesByClassesReportsQueryRepository absencesByClassesReportsQueryRepository;

    public AbsencesByClassesReportsExcelExportService(
        IAbsencesByClassesReportsQueryRepository absencesByClassesReportsQueryRepository)
    {
        this.absencesByClassesReportsQueryRepository = absencesByClassesReportsQueryRepository;
    }

    public async Task ExportAsync(
        int schoolYear,
        int instId,
        int absencesByClassesReportId,
        Stream outputStream,
        CancellationToken ct)
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

        var worksheetPart = workbookPart.AddNewPart<WorksheetPart>("absencesByClassesReport");

        await this.FillWorksheetPartAsync(
            schoolYear,
            instId,
            absencesByClassesReportId,
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
                Name = "отсъствия по класове"
            });
    }

    public async Task FillWorksheetPartAsync(
        int schoolYear,
        int instId,
        int absencesByClassesReportId,
        WorksheetPart worksheetPart,
        WorkbookStylesPart workbookStylesPart,
        CancellationToken ct)
    {
        var report =
            await this.absencesByClassesReportsQueryRepository.GetExcelDataAsync(
                schoolYear,
                instId,
                absencesByClassesReportId,
                ct);

        worksheetPart.InitNormalWorksheet();

        var titleFont = workbookStylesPart.Stylesheet.AppendFont(bold: true, size: 14.0);
        var headerFont = workbookStylesPart.Stylesheet.AppendFont(bold: true, size: 12.0);
        var primaryFont = workbookStylesPart.Stylesheet.AppendFont(size: 9.0);
        var primaryBoldFont = workbookStylesPart.Stylesheet.AppendFont(bold: true, size: 9.0);

        var titleTableCellFormat = workbookStylesPart.Stylesheet.AppendCellFormat(
            verticalAlignment: VerticalAlignmentValues.Center,
            horizontalAlignment: HorizontalAlignmentValues.Left,
            wrapText: true,
            fontId: titleFont);

        var headerTableCell = workbookStylesPart.Stylesheet.AppendCellFormat(
            verticalAlignment: VerticalAlignmentValues.Center,
            horizontalAlignment: HorizontalAlignmentValues.Center,
            wrapText: true,
            fontId: headerFont);

        var primaryTableCellCentered = workbookStylesPart.Stylesheet.AppendCellFormat(
            verticalAlignment: VerticalAlignmentValues.Center,
            horizontalAlignment: HorizontalAlignmentValues.Center,
            wrapText: true,
            fontId: primaryFont);

        var primaryTotalTableCellCentered = workbookStylesPart.Stylesheet.AppendCellFormat(
            verticalAlignment: VerticalAlignmentValues.Center,
            horizontalAlignment: HorizontalAlignmentValues.Center,
            wrapText: true,
            fontId: primaryBoldFont);

        // Format number and width of sheet columns
        double primaryColWidth = 13;
        double primaryRowHeight = 17;
        string[] headerRowColumnTitles = new string[] { "Клас", "Ученици", "Уважителни", "Ув. (средно на ученик)", "Неуважителни", "Неув. (средно на ученик)" };

        worksheetPart.Worksheet.GetColumns()
            .AppendCustomWidthColumn(1, 1, primaryColWidth)
            .AppendCustomWidthColumn(2, 2, primaryColWidth)
            .AppendCustomWidthColumn(3, 3, primaryColWidth)
            .AppendCustomWidthColumn(4, 4, primaryColWidth * 2)
            .AppendCustomWidthColumn(5, 5, primaryColWidth)
            .AppendCustomWidthColumn(6, 6, primaryColWidth * 2);

        var titleRow =
            worksheetPart.Worksheet
                .AppendRelativeRow(height: primaryRowHeight * 2);

        string titleRowText;
        if (!string.IsNullOrEmpty(report.ClassBookNames))
        {
            titleRowText = $"Отсъствия по класове от {report.ClassBookNames} за {report.Period.ToLower()} към дата {report.CreateDate: dd.MM.yyyy HH:mm}";
        }
        else
        {
            titleRowText = $"Отсъствия по класове за {report.Period.ToLower()} към дата {report.CreateDate: dd.MM.yyyy HH:mm}";
        }

        titleRow
            .AppendRelativeInlineStringCell(
                text: titleRowText,
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

        foreach (var reportItem in report.Items)
        {
            var reportItemRow =
                worksheetPart.Worksheet
                    .AppendRelativeRow(height: primaryRowHeight);

            var styleIndex = reportItem.IsTotal ? primaryTotalTableCellCentered : primaryTableCellCentered;

            reportItemRow
                .AppendRelativeInlineStringCell(
                    text: reportItem.ClassBookName,
                    styleIndex: styleIndex)
                .AppendRelativeNumberCell(
                    number: reportItem.StudentsCount,
                    styleIndex: styleIndex)
                .AppendRelativeNumberCell(
                    number: reportItem.ExcusedAbsencesCount,
                    styleIndex: styleIndex)
                .AppendRelativeNumberCell(
                    number: reportItem.ExcusedAbsencesCountAverage,
                    styleIndex: styleIndex)
                .AppendRelativeNumberCell(
                    number: reportItem.UnexcusedAbsencesCount,
                    styleIndex: styleIndex)
                .AppendRelativeNumberCell(
                    number: reportItem.UnexcusedAbsencesCountAverage,
                    styleIndex: styleIndex);
        }

        worksheetPart.Worksheet.Finalize();
    }
}
