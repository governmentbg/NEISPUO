namespace SB.Domain;

using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SB.Common;

public class StudentsAtRiskOfDroppingOutReportsExcelExportService : IStudentsAtRiskOfDroppingOutReportsExcelExportService
{
    private readonly IStudentsAtRiskOfDroppingOutReportsQueryRepository studentsAtRiskOfDroppingOutReportsQueryRepository;

    public StudentsAtRiskOfDroppingOutReportsExcelExportService(
        IStudentsAtRiskOfDroppingOutReportsQueryRepository studentsAtRiskOfDroppingOutReportsQueryRepository)
    {
        this.studentsAtRiskOfDroppingOutReportsQueryRepository = studentsAtRiskOfDroppingOutReportsQueryRepository;
    }

    public async Task ExportAsync(
        int schoolYear,
        int instId,
        int studentsAtRiskOfDroppingOutReportId,
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

        var worksheetPart = workbookPart.AddNewPart<WorksheetPart>("studentsAtRiskOfDroppingOutReport");

        await this.FillWorksheetPartAsync(
            schoolYear,
            instId,
            studentsAtRiskOfDroppingOutReportId,
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
                Name = "ученици с риск от отпадане"
            });
    }

    public async Task FillWorksheetPartAsync(
        int schoolYear,
        int instId,
        int studentsAtRiskOfDroppingOutReportId,
        WorksheetPart worksheetPart,
        WorkbookStylesPart workbookStylesPart,
        CancellationToken ct)
    {
        var report =
            await this.studentsAtRiskOfDroppingOutReportsQueryRepository.GetExcelDataAsync(
                schoolYear,
                instId,
                studentsAtRiskOfDroppingOutReportId,
                ct);

        worksheetPart.InitNormalWorksheet();

        var titleFont = workbookStylesPart.Stylesheet.AppendFont(bold: true, size: 14.0);
        var headerFont = workbookStylesPart.Stylesheet.AppendFont(bold: true, size: 12.0);
        var primaryFont = workbookStylesPart.Stylesheet.AppendFont(size: 9.0);

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

        var footerTableCell = workbookStylesPart.Stylesheet.AppendCellFormat(
            verticalAlignment: VerticalAlignmentValues.Center,
            horizontalAlignment: HorizontalAlignmentValues.Right,
            wrapText: true,
            fontId: headerFont);

        // Format number and width of sheet columns
        double primaryColWidth = 13;
        double primaryRowHeight = 15;
        string[] headerRowColumnTitles = new string[] { "ЕГН/ЛНЧ", "Име", "Презиме", "Фамилия", "Паралелка", "Отсъствия - Часове", "Отсъствия - Дни" };
        var mergeCells = worksheetPart.Worksheet.GetMergeCells();

        worksheetPart.Worksheet.GetColumns()
            .AppendCustomWidthColumn(1, 1, primaryColWidth * 2)
            .AppendCustomWidthColumn(2, 2, primaryColWidth * 2)
            .AppendCustomWidthColumn(3, 3, primaryColWidth * 2)
            .AppendCustomWidthColumn(4, 4, primaryColWidth * 2)
            .AppendCustomWidthColumn(5, 5, primaryColWidth * 2)
            .AppendCustomWidthColumn(6, 6, primaryColWidth * 2)
            .AppendCustomWidthColumn(7, 7, primaryColWidth * 2);

        var titleRow =
            worksheetPart.Worksheet
                .AppendRelativeRow(height: primaryRowHeight * 2);

        titleRow
            .AppendRelativeInlineStringCell(
                text: $"Ученици с риск от отпадане до {report.ReportDate: dd.MM.yyyy} към {report.CreateDate: dd.MM.yyyy HH:mm}",
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
            var reportItemRow =
                worksheetPart.Worksheet
                    .AppendRelativeRow(height: primaryRowHeight);

            reportItemRow
                .AppendRelativeInlineStringCell(
                    text: reportItem.PersonalId,
                    styleIndex: primaryTableCellCentered)
                .AppendRelativeInlineStringCell(
                    text: reportItem.FirstName,
                    styleIndex: primaryTableCellCentered)
                .AppendRelativeInlineStringCell(
                    text: reportItem.MiddleName,
                    styleIndex: primaryTableCellCentered)
                .AppendRelativeInlineStringCell(
                    text: reportItem.LastName,
                    styleIndex: primaryTableCellCentered)
                .AppendRelativeInlineStringCell(
                    text: reportItem.ClassBookName,
                    styleIndex: primaryTableCellCentered)
                .AppendRelativeNumberCell(
                    number: reportItem.UnexcusedAbsenceHoursCount,
                    styleIndex: primaryTableCellCentered)
                .AppendRelativeNumberCell(
                    number: reportItem.UnexcusedAbsenceDaysCount,
                    styleIndex: primaryTableCellCentered);
        }

        var totalItemsCount =
            worksheetPart.Worksheet
                .AppendRelativeRow(height: primaryRowHeight * 1.4);

        var totalCountTextCell = totalItemsCount
            .AppendRelativeInlineStringCell(
                text: $"Общ брой ученици с риск от отпадане за периода: {report.ReportItems.Length}",
                styleIndex: footerTableCell)
            .GetLastCell();

        var columnsCount = worksheetPart.Worksheet.GetColumns().Count();
        //Every part(cell) of merge cell takes border styles from corresponding merged cell
        for (int i = 0; i < columnsCount - 1; i++)
        {
            totalItemsCount
                .AppendRelativeInlineStringCell(styleIndex: footerTableCell);
        }

        mergeCells.AppendMergeCell(totalCountTextCell!.CellReference!, cols: columnsCount);

        worksheetPart.Worksheet.Finalize();
    }
}
