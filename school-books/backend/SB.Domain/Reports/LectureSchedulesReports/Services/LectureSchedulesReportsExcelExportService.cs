namespace SB.Domain;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SB.Common;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class LectureSchedulesReportsExcelExportService : ILectureSchedulesReportsExcelExportService
{
    private readonly ILectureSchedulesReportsQueryRepository lectureSchedulesReportsQueryRepository;

    public LectureSchedulesReportsExcelExportService(ILectureSchedulesReportsQueryRepository lectureSchedulesReportsQueryRepository)
    {
        this.lectureSchedulesReportsQueryRepository = lectureSchedulesReportsQueryRepository;
    }

    public async Task ExportAsync(int schoolYear, int instId, int lectureSchedulesReportId, Stream outputStream, CancellationToken ct)
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

        var worksheetPart = workbookPart.AddNewPart<WorksheetPart>("lectureSchedulesReport");

        await this.FillWorksheetPartAsync(
            schoolYear,
            instId,
            lectureSchedulesReportId,
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
                Name = "лекторски часове",
            });
    }

    public async Task FillWorksheetPartAsync(
        int schoolYear,
        int instId,
        int lectureSchedulesReportId,
        WorksheetPart worksheetPart,
        WorkbookStylesPart workbookStylesPart,
        CancellationToken ct)
    {
        var report =
            await this.lectureSchedulesReportsQueryRepository.GetExcelDataAsync(
                schoolYear,
                instId,
                lectureSchedulesReportId,
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

        var primaryTableCell = workbookStylesPart.Stylesheet.AppendCellFormat(
            verticalAlignment: VerticalAlignmentValues.Center,
            horizontalAlignment: HorizontalAlignmentValues.Left,
            wrapText: true,
            fontId: primaryFont);

        var primaryTableCellCentered = workbookStylesPart.Stylesheet.AppendCellFormat(
            verticalAlignment: VerticalAlignmentValues.Center,
            horizontalAlignment: HorizontalAlignmentValues.Center,
            wrapText: true,
            fontId: primaryFont);

        var dateTableCellCentered = workbookStylesPart.Stylesheet.AppendCellFormat(
            verticalAlignment: VerticalAlignmentValues.Center,
            horizontalAlignment: HorizontalAlignmentValues.Center,
            wrapText: true,
            fontId: primaryFont,
            // special date number format added in the Stylesheet
            numberFormatId: 165);

        var footerTableCell = workbookStylesPart.Stylesheet.AppendCellFormat(
            verticalAlignment: VerticalAlignmentValues.Center,
            horizontalAlignment: HorizontalAlignmentValues.Right,
            wrapText: true,
            fontId: headerFont);

        // Format number and width of sheet columns
        double primaryColWidth = 13;
        double primaryRowHeight = 15;
        string[] headerRowColumnTitles = new string[] { "Учител", "Клас/Паралелка/Група", "Дата", "Предмет", "Взети часове", "Заповед №/Дата" };
        var mergeCells = worksheetPart.Worksheet.GetMergeCells();

        worksheetPart.Worksheet.GetColumns()
            .AppendCustomWidthColumn(1, 1, primaryColWidth * 2)
            .AppendCustomWidthColumn(2, 2, primaryColWidth * 2)
            .AppendCustomWidthColumn(3, 3, primaryColWidth)
            .AppendCustomWidthColumn(4, 4, primaryColWidth * 3)
            .AppendCustomWidthColumn(5, 5, primaryColWidth)
            .AppendCustomWidthColumn(6, 6, primaryColWidth * 2);

        var titleRow =
            worksheetPart.Worksheet
                .AppendRelativeRow(height: primaryRowHeight * 2);

        string teacherFilter = !string.IsNullOrEmpty(report.TeacherName) ? $"на {report.TeacherName} " : "";
        string periodFilter = report.YearAndMonth ?? report.Period.ToLower();

        titleRow
            .AppendRelativeInlineStringCell(
                text: $"Лекторски часове {teacherFilter}{periodFilter} към {report.CreateDate}",
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

        foreach (var teacher in report.Teachers)
        {
            foreach (var hour in teacher.Hours)
            {
                var lectureScheduleHourRow =
                    worksheetPart.Worksheet
                        .AppendRelativeRow(height: primaryRowHeight);

                lectureScheduleHourRow
                    .AppendRelativeInlineStringCell(
                        text: hour.TeacherName,
                        styleIndex: primaryTableCell)
                    .AppendRelativeInlineStringCell(
                        text: hour.ClassBookName,
                        styleIndex: primaryTableCellCentered)
                    .AppendRelativeDateCell(
                        date: hour.Date,
                        styleIndex: dateTableCellCentered)
                    .AppendRelativeInlineStringCell(
                        text: hour.CurriculumName,
                        styleIndex: primaryTableCell)
                    .AppendRelativeNumberCell(
                        number: hour.HoursTaken,
                        styleIndex: primaryTableCellCentered)
                    .AppendRelativeInlineStringCell(
                        text: $"{hour.OrderNumber}/{hour.OrderDate:dd.MM.yyyy}",
                        styleIndex: primaryTableCell);
            }

            var lectureScheduleTotalCountRow =
                    worksheetPart.Worksheet
                        .AppendRelativeRow(height: primaryRowHeight * 1.4);

            var totalCountTextCell = lectureScheduleTotalCountRow
                .AppendRelativeInlineStringCell(
                    text: $"Общ брой взети лекторски часове за периода: {teacher.TotalHoursTaken}",
                    styleIndex: footerTableCell)
                .GetLastCell();

            var columnsCount = worksheetPart.Worksheet.GetColumns().Count();
            //Every part(cell) of merge cell takes border styles from corresponding merged cell
            for (int i = 0; i < columnsCount - 1; i++)
            {
                lectureScheduleTotalCountRow
                    .AppendRelativeInlineStringCell(styleIndex: footerTableCell);
            }

            mergeCells.AppendMergeCell(totalCountTextCell!.CellReference!, cols: columnsCount);
        }

        worksheetPart.Worksheet.Finalize();
    }
}
