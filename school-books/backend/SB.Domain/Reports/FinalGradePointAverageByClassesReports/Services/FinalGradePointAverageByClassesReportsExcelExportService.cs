namespace SB.Domain;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SB.Common;

public class FinalGradePointAverageByClassesReportsExcelExportService : IFinalGradePointAverageByClassesReportsExcelExportService
{
    private readonly IFinalGradePointAverageByClassesReportsQueryRepository finalGradePointAverageByClassesReportsQueryRepository;

    public FinalGradePointAverageByClassesReportsExcelExportService(
        IFinalGradePointAverageByClassesReportsQueryRepository finalGradePointAverageByClassesReportsQueryRepository)
    {
        this.finalGradePointAverageByClassesReportsQueryRepository = finalGradePointAverageByClassesReportsQueryRepository;
    }

    public async Task ExportAsync(
        int schoolYear,
        int instId,
        int finalGradePointAverageByClassesReportId,
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

        var worksheetPart = workbookPart.AddNewPart<WorksheetPart>("regularGradePointAverageByClassesReport");

        await this.FillWorksheetPartAsync(
            schoolYear,
            instId,
            finalGradePointAverageByClassesReportId,
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
                Name = "среден успех от срочни оценки"
            });
    }

    public async Task FillWorksheetPartAsync(
        int schoolYear,
        int instId,
        int finalGradePointAverageByClassesReportId,
        WorksheetPart worksheetPart,
        WorkbookStylesPart workbookStylesPart,
        CancellationToken ct)
    {
        var report =
            await this.finalGradePointAverageByClassesReportsQueryRepository.GetExcelDataAsync(
                schoolYear,
                instId,
                finalGradePointAverageByClassesReportId,
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
        string[] headerRowColumnTitles = new string[]
        {
            "Клас", "Предмет", "Ученици", "Изпитани ученици", "% изпитани", "Успех", "Оценки", "Слаб 2", "Среден 3", "Добър 4", "Мн. Добър 5", "Отличен 6"
        };

        worksheetPart.Worksheet.GetColumns()
            .AppendCustomWidthColumn(1, 1, primaryColWidth * 2)
            .AppendCustomWidthColumn(2, 2, primaryColWidth * 4)
            .AppendCustomWidthColumn(3, 3, primaryColWidth)
            .AppendCustomWidthColumn(4, 4, primaryColWidth)
            .AppendCustomWidthColumn(5, 5, primaryColWidth)
            .AppendCustomWidthColumn(6, 6, primaryColWidth)
            .AppendCustomWidthColumn(7, 7, primaryColWidth)
            .AppendCustomWidthColumn(8, 8, primaryColWidth)
            .AppendCustomWidthColumn(9, 9, primaryColWidth)
            .AppendCustomWidthColumn(10, 10, primaryColWidth)
            .AppendCustomWidthColumn(11, 11, primaryColWidth)
            .AppendCustomWidthColumn(12, 12, primaryColWidth);

        var titleRow =
            worksheetPart.Worksheet
                .AppendRelativeRow(height: primaryRowHeight * 2);

        string titleRowText;
        if (!string.IsNullOrEmpty(report.ClassBookNames))
        {
            titleRowText = $"Среден успех от срочни/годишни оценки по класове от {report.ClassBookNames} за {report.Period.ToLower()} към дата {report.CreateDate: dd.MM.yyyy HH:mm}";
        }
        else
        {
            titleRowText = $"Среден успех от срочни/годишни оценки по класове за {report.Period.ToLower()} към дата {report.CreateDate: dd.MM.yyyy HH:mm}";
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
                .AppendRelativeInlineStringCell(
                    text: reportItem.CurriculumInfo,
                    styleIndex: styleIndex)
                .AppendRelativeNumberCell(
                    number: reportItem.StudentsCount,
                    styleIndex: styleIndex)
                .AppendRelativeInlineStringCell(
                    text: reportItem.StudentsWithGradesCount == 0M ? "-" : reportItem.StudentsWithGradesCount.ToString(),
                    styleIndex: styleIndex)
                .AppendRelativeInlineStringCell(
                    text: reportItem.StudentsWithGradesPercentage == 0M ? "-" : reportItem.StudentsWithGradesPercentage.ToString("N2"),
                    styleIndex: styleIndex)
                .AppendRelativeInlineStringCell(
                    text: reportItem.GradePointAverage == 0M ? "-" : reportItem.GradePointAverage.ToString("N2"),
                    styleIndex: styleIndex)
                .AppendRelativeNumberCell(
                    number: reportItem.TotalGradesCount,
                    styleIndex: styleIndex)
                .AppendRelativeNumberCell(
                    number: reportItem.PoorGradesCount,
                    styleIndex: styleIndex)
                .AppendRelativeNumberCell(
                    number: reportItem.FairGradesCount,
                    styleIndex: styleIndex)
                .AppendRelativeNumberCell(
                    number: reportItem.GoodGradesCount,
                    styleIndex: styleIndex)
                .AppendRelativeNumberCell(
                    number: reportItem.VeryGoodGradesCount,
                    styleIndex: styleIndex)
                .AppendRelativeNumberCell(
                    number: reportItem.ExcellentGradesCount,
                    styleIndex: styleIndex);
        }

        worksheetPart.Worksheet.Finalize();
    }
}
