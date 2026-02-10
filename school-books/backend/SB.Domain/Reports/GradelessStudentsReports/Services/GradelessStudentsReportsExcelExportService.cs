namespace SB.Domain;

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SB.Common;

public class GradelessStudentsReportsExcelExportService : IGradelessStudentsReportsExcelExportService
{
    private readonly IGradelessStudentsReportsQueryRepository gradelessStudentsReportsQueryRepository;

    public GradelessStudentsReportsExcelExportService(
        IGradelessStudentsReportsQueryRepository gradelessStudentsReportsQueryRepository)
    {
        this.gradelessStudentsReportsQueryRepository = gradelessStudentsReportsQueryRepository;
    }

    public async Task ExportAsync(
        int schoolYear,
        int instId,
        int gradelessStudentsReportId,
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

        var worksheetPart = workbookPart.AddNewPart<WorksheetPart>("gradelessStudentsReport");

        await this.FillWorksheetPartAsync(
            schoolYear,
            instId,
            gradelessStudentsReportId,
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
                Name = "ученици без оценки"
            });
    }

    public async Task FillWorksheetPartAsync(
        int schoolYear,
        int instId,
        int gradelessStudentsReportId,
        WorksheetPart worksheetPart,
        WorkbookStylesPart workbookStylesPart,
        CancellationToken ct)
    {
        var report =
            await this.gradelessStudentsReportsQueryRepository.GetExcelDataAsync(
                schoolYear,
                instId,
                gradelessStudentsReportId,
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

        // Format number and width of sheet columns
        double primaryColWidth = 13;
        double primaryRowHeight = 15;
        string[] headerRowColumnTitles = new string[] { "Клас", "Ученик", "Предмет" };

        worksheetPart.Worksheet.GetColumns()
            .AppendCustomWidthColumn(1, 1, primaryColWidth)
            .AppendCustomWidthColumn(2, 2, primaryColWidth * 3)
            .AppendCustomWidthColumn(3, 3, primaryColWidth * 3);

        var titleRow =
            worksheetPart.Worksheet
                .AppendRelativeRow(height: primaryRowHeight * 2);

        string titleRowText;
        string gradeTypeText = report.OnlyFinalGrades ? $"{(report.Period != ReportPeriod.WholeYear ? "срочни" : "годишни")}" : "текущи";
        string periodText = report.OnlyFinalGrades ?
            $"{(report.Period != ReportPeriod.WholeYear ? "за " + EnumUtils.GetEnumDescription(report.Period).ToLower() + " " : "")}"
            : "за " + EnumUtils.GetEnumDescription(report.Period).ToLower() + " ";

        if (!string.IsNullOrEmpty(report.ClassBookNames))
        {
            titleRowText = $"Ученици без {gradeTypeText} оценки от {report.ClassBookNames} {periodText}към дата {report.CreateDate: dd.MM.yyyy HH:mm}";
        }
        else
        {
            titleRowText = $"Ученици без {gradeTypeText} оценки {periodText}към дата {report.CreateDate: dd.MM.yyyy HH:mm}";
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
                    text: reportItem.StudentName,
                    styleIndex: primaryTableCellCentered)
                .AppendRelativeInlineStringCell(
                    text: reportItem.CurriculumName,
                    styleIndex: primaryTableCellCentered);
        }

        worksheetPart.Worksheet.Finalize();
    }
}
