namespace SB.Domain;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SB.Common;

public class FinalGradePointAverageByStudentsReportsExcelExportService : IFinalGradePointAverageByStudentsReportsExcelExportService
{
    private readonly IFinalGradePointAverageByStudentsReportsQueryRepository finalGradePointAverageByStudentsReportsQueryRepository;

    public FinalGradePointAverageByStudentsReportsExcelExportService(
        IFinalGradePointAverageByStudentsReportsQueryRepository finalGradePointAverageByStudentsReportsQueryRepository)
    {
        this.finalGradePointAverageByStudentsReportsQueryRepository = finalGradePointAverageByStudentsReportsQueryRepository;
    }

    public async Task ExportAsync(
        int schoolYear,
        int instId,
        int finalGradePointAverageByStudentsReportId,
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

        var worksheetPart = workbookPart.AddNewPart<WorksheetPart>("finalGradePointAverageByStudentsReport");

        await this.FillWorksheetPartAsync(
            schoolYear,
            instId,
            finalGradePointAverageByStudentsReportId,
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
                Name = "оценки по ученици"
            });
    }

    public async Task FillWorksheetPartAsync(
        int schoolYear,
        int instId,
        int finalGradePointAverageByStudentsReportId,
        WorksheetPart worksheetPart,
        WorkbookStylesPart workbookStylesPart,
        CancellationToken ct)
    {
        var report =
            await this.finalGradePointAverageByStudentsReportsQueryRepository.GetExcelDataAsync(
                schoolYear,
                instId,
                finalGradePointAverageByStudentsReportId,
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

        var primaryNumberTableCellCentered = workbookStylesPart.Stylesheet.AppendCellFormat(
            verticalAlignment: VerticalAlignmentValues.Center,
            horizontalAlignment: HorizontalAlignmentValues.Center,
            wrapText: true,
            numberFormatId: 2,
            fontId: primaryFont);

        // Format number and width of sheet columns
        double primaryColWidth = 13;
        double primaryRowHeight = 17;
        string[] headerRowColumnTitles = new string[]
        {
            "Паралелка", "Ученик", "Среден успех"
        };

        worksheetPart.Worksheet.GetColumns()
            .AppendCustomWidthColumn(1, 1, primaryColWidth * 2)
            .AppendCustomWidthColumn(2, 2, primaryColWidth * 4)
            .AppendCustomWidthColumn(3, 3, primaryColWidth * 2);

        var titleRow =
            worksheetPart.Worksheet
                .AppendRelativeRow(height: primaryRowHeight * 2);

        string titleRowText;
        var periodType = report.Period == ReportPeriod.WholeYear.GetEnumDescription() ? "годишни" : "срочни";
        var minimalGradeValue = report.MinimalGradePointAverage != null ? $"с успех по-голям или равен на {report.MinimalGradePointAverage:N2}" : "";
        if (!string.IsNullOrEmpty(report.ClassBookNames))
        {
            titleRowText = $"Среден успех от {periodType} оценки по ученици {minimalGradeValue} от {report.ClassBookNames} за {report.Period.ToLower()} към дата {report.CreateDate: dd.MM.yyyy HH:mm}";
        }
        else
        {
            titleRowText = $"Среден успех от {periodType} оценки по ученици {minimalGradeValue} за {report.Period.ToLower()} към дата {report.CreateDate: dd.MM.yyyy HH:mm}";
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

            reportItemRow
                .AppendRelativeInlineStringCell(
                    text: reportItem.ClassBookName,
                    styleIndex: primaryTableCellCentered)
                .AppendRelativeInlineStringCell(
                    text: reportItem.StudentNames + (reportItem.IsTransferred ? " (ОТПИСАН)" : null),
                    styleIndex: primaryTableCellCentered)
                .AppendRelativeNumberCell(
                    number: reportItem.FinalGradePointAverage,
                    styleIndex: primaryNumberTableCellCentered);
        }

        worksheetPart.Worksheet.Finalize();
    }
}
