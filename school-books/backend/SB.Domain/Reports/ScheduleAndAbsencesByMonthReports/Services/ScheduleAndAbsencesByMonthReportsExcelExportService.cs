namespace SB.Domain;

using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SB.Common;

public class ScheduleAndAbsencesByMonthReportsExcelExportService : IScheduleAndAbsencesByMonthReportsExcelExportService
{
    private readonly IScheduleAndAbsencesByMonthReportsQueryRepository scheduleAndAbsencesByMonthReportsQueryRepository;

    public ScheduleAndAbsencesByMonthReportsExcelExportService(
        IScheduleAndAbsencesByMonthReportsQueryRepository scheduleAndAbsencesByMonthReportsQueryRepository)
    {
        this.scheduleAndAbsencesByMonthReportsQueryRepository = scheduleAndAbsencesByMonthReportsQueryRepository;
    }

    public async Task ExportAsync(
        int schoolYear,
        int instId,
        int scheduleAndAbsencesByMonthReportId,
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

        var worksheetPart = workbookPart.AddNewPart<WorksheetPart>("scheduleAndAbsencesByMonthReport");

        await this.FillWorksheetPartAsync(
            schoolYear,
            instId,
            scheduleAndAbsencesByMonthReportId,
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
                Name = "отсъствия и теми за месец"
            });
    }

    public async Task FillWorksheetPartAsync(
        int schoolYear,
        int instId,
        int scheduleAndAbsencesByMonthReportId,
        WorksheetPart worksheetPart,
        WorkbookStylesPart workbookStylesPart,
        CancellationToken ct)
    {
        var report =
            await this.scheduleAndAbsencesByMonthReportsQueryRepository.GetExcelDataAsync(
                schoolYear,
                instId,
                scheduleAndAbsencesByMonthReportId,
                ct);

        worksheetPart.InitNormalWorksheet();

        var titleFont = workbookStylesPart.Stylesheet.AppendFont(bold: true, size: 14.0);
        var primaryFont = workbookStylesPart.Stylesheet.AppendFont(size: 9.0);
        var primaryBoldFont = workbookStylesPart.Stylesheet.AppendFont(bold: true, size: 9.0);

        var titleTableCellFormat = workbookStylesPart.Stylesheet.AppendCellFormat(
            verticalAlignment: VerticalAlignmentValues.Center,
            horizontalAlignment: HorizontalAlignmentValues.Left,
            wrapText: true,
            fontId: titleFont);

        var primaryTableCellCentered = workbookStylesPart.Stylesheet.AppendCellFormat(
            verticalAlignment: VerticalAlignmentValues.Center,
            horizontalAlignment: HorizontalAlignmentValues.Center,
            wrapText: true,
            fontId: primaryFont);

        var primaryTableCellCenteredBold = workbookStylesPart.Stylesheet.AppendCellFormat(
            verticalAlignment: VerticalAlignmentValues.Center,
            horizontalAlignment: HorizontalAlignmentValues.Center,
            wrapText: true,
            fontId: primaryBoldFont);

        var wrappedBottomAlignedCenteredRotatedStyleIndex = workbookStylesPart.Stylesheet.AppendCellFormat(
            wrapText: true,
            verticalAlignment: VerticalAlignmentValues.Bottom,
            horizontalAlignment: HorizontalAlignmentValues.Center,
            textRotation: 90);

        var sheetData = worksheetPart.Worksheet.GetSheetData();

        // Format number and width of sheet columns
        double primaryColWidth = 6;
        double primaryRowHeight = 35;

        worksheetPart.Worksheet.GetColumns()
            .AppendCustomWidthColumn(1, 1, primaryColWidth)
            .AppendCustomWidthColumn(2, 2, primaryColWidth * 5)
            .AppendCustomWidthColumn(3, 3, primaryColWidth * 2)
            .AppendCustomWidthColumn(4, 4, primaryColWidth * 2)
            .AppendCustomWidthColumn(5, 5, !report.IsDPLR ? primaryColWidth * 2 : primaryColWidth * 5)
            .AppendCustomWidthColumn(6, 6, !report.IsDPLR ? primaryColWidth * 5 : primaryColWidth * 2);

        var titleRow =
            worksheetPart.Worksheet
                .AppendRelativeRow();

        int totalColumnsCount = !report.IsDPLR ? 6 : 5;

        titleRow
            .AppendRelativeInlineStringCell(
                text: $"Отсъствия/теми за {report.ClassBookName} за {report.YearAndMonth} към дата {report.CreateDate: dd.MM.yyyy HH:mm}",
                styleIndex: titleTableCellFormat);
        worksheetPart.Worksheet
            .AppendMergeCell($"A{titleRow.RowIndex}:" +
                $"{OpenXmlExtensions.ColumnIdToColumnIndex(totalColumnsCount - 1)}{titleRow.RowIndex}");

        string[] headerRow2ColumnTitles = !report.IsDPLR ? new string[] { "уваж.", "неуваж.", "закъснели"} : new string[] { "остъствия", "присъствия" };

        foreach (var week in report.Weeks)
        {
            //header rows

            var currentTitleRow =
                worksheetPart.Worksheet
                .AppendRelativeRow();

            currentTitleRow
                .AppendRelativeInlineStringCell(
                    text: week.WeekName,
                    styleIndex: primaryTableCellCenteredBold);
            worksheetPart.Worksheet.AppendRelativeMergeCell(cols: totalColumnsCount);

            var headerRow1 = sheetData.AppendRelativeRow(height: primaryRowHeight);

            headerRow1
                .AppendRelativeInlineStringCell(
                    text: "Ден",
                    styleIndex: primaryTableCellCenteredBold);
            worksheetPart.Worksheet.AppendRelativeMergeCell(rows: 2);

            headerRow1
                .AppendRelativeInlineStringCell(
                    text: "Час/Предмет",
                    styleIndex: primaryTableCellCenteredBold);
            worksheetPart.Worksheet.AppendRelativeMergeCell(rows: 2);

            headerRow1
                .AppendRelativeInlineStringCell(
                    text: $"{(!report.IsDPLR ? "Отсъстващи ученици" : "Отсъстващи/присъстващи ученици")}",
                    styleIndex: primaryTableCellCenteredBold);
            worksheetPart.Worksheet.AppendRelativeMergeCell(cols: headerRow2ColumnTitles.Length);

            var headerRow2 = sheetData.AppendRelativeRow();

            for (int i = 0; i < headerRow2ColumnTitles.Length; i++)
            {
                headerRow2
                .AppendRelativeInlineStringCell(
                    text: headerRow2ColumnTitles[i],
                    offset: i == 0 ? 3 : 1,
                    styleIndex: primaryTableCellCenteredBold);
            }

            headerRow1
                .AppendRelativeInlineStringCell(
                    text: "Тема на урока",
                    offset: !report.IsDPLR ? 3 : 2,
                    styleIndex: primaryTableCellCenteredBold);
            worksheetPart.Worksheet.AppendRelativeMergeCell(rows: 2, relativeRow: headerRow1);

            //result rows

            foreach (var day in week.Days)
            {
                if (day.IsOffDay)
                {
                    var recordRow = sheetData.AppendRelativeRow();

                    recordRow
                        .AppendRelativeInlineStringCell(
                            text: day.DayName + "\n" + day.Date.ToString("dd.MM.yyyy"),
                            styleIndex: wrappedBottomAlignedCenteredRotatedStyleIndex);

                    recordRow
                        .AppendRelativeInlineStringCell(
                            text: "Неучебен ден",
                            styleIndex: primaryTableCellCenteredBold);
                    worksheetPart.Worksheet.AppendRelativeMergeCell(cols: totalColumnsCount - 1);
                }
                else if (day.IsEmptyDay)
                {
                    var recordRow = sheetData.AppendRelativeRow();

                    recordRow
                        .AppendRelativeInlineStringCell(
                            text: day.DayName + "\n" + day.Date.ToString("dd.MM.yyyy"),
                            styleIndex: wrappedBottomAlignedCenteredRotatedStyleIndex);

                    recordRow
                        .AppendRelativeInlineStringCell(
                            text: "Няма въведена програма за деня или е извън периода на справката",
                            styleIndex: primaryTableCellCenteredBold);
                    worksheetPart.Worksheet.AppendRelativeMergeCell(cols: totalColumnsCount - 1);
                }
                else
                {
                    int hourIndex = 0;

                    foreach (var hour in day.Hours)
                    {
                        var recordRow = sheetData.AppendRelativeRow();

                        if (hourIndex == 0)
                        {
                            recordRow
                                .AppendRelativeInlineStringCell(
                                    text: day.DayName + "\n" + day.Date.ToString("dd.MM.yyyy"),
                                    styleIndex: wrappedBottomAlignedCenteredRotatedStyleIndex);

                            var lastHourNumberForDay = day.Hours.Any() ? day.Hours.Max(h => h.HourNumber) : 1;

                            if (lastHourNumberForDay > 1)
                            {
                                worksheetPart.Worksheet.AppendMergeCell($"A{recordRow.RowIndex!}:A{recordRow.RowIndex! + lastHourNumberForDay - 1}");
                            }
                        }

                        int offset = hourIndex == 0 ? 1 : 2;

                        var hourText = $"{hour.HourNumber}. " +
                            (hour.IsEmptyHour == true ? "Свободен час" : $"{hour.CurriculumName} {(!string.IsNullOrEmpty(hour.CurriculumTeacherNames) ? "- " + hour.CurriculumTeacherNames : "")}");

                        recordRow
                            .AppendRelativeInlineStringCell(
                                text: hourText,
                                offset: offset,
                                styleIndex: primaryTableCellCentered);

                        if (report.IsDPLR)
                        {
                            recordRow
                                .AppendRelativeInlineStringCell(
                                    text: hour.DplrAbsenceStudentClassNumbers ?? string.Empty,
                                    styleIndex: primaryTableCellCentered);

                            recordRow
                                .AppendRelativeInlineStringCell(
                                    text: hour.DplrAttendanceStudentClassNumbers ?? string.Empty,
                                    styleIndex: primaryTableCellCentered);
                        }
                        else
                        {
                            recordRow
                                .AppendRelativeInlineStringCell(
                                    text: hour.ExcusedStudentClassNumbers ?? string.Empty,
                                    styleIndex: primaryTableCellCentered);

                            recordRow
                                .AppendRelativeInlineStringCell(
                                    text: hour.UnexcusedStudentClassNumbers ?? string.Empty,
                                    styleIndex: primaryTableCellCentered);

                            recordRow
                                .AppendRelativeInlineStringCell(
                                    text: hour.LateStudentClassNumbers ?? string.Empty,
                                    styleIndex: primaryTableCellCentered);
                        }

                        recordRow
                            .AppendRelativeInlineStringCell(
                                text: hour.Topics ?? string.Empty,
                                styleIndex: primaryTableCellCentered);

                        hourIndex++;
                    }
                }
            }

            if (!string.IsNullOrEmpty(week.AdditionalActivities))
            {
                var activitiesRow =
                    worksheetPart.Worksheet
                    .AppendRelativeRow();
                activitiesRow
                    .AppendRelativeInlineStringCell(
                        text: $"Допълнителни дейности: {week.AdditionalActivities}",
                        styleIndex: titleTableCellFormat);
                worksheetPart.Worksheet.AppendRelativeMergeCell(cols: totalColumnsCount);
            }

            var seperationRow =
                    worksheetPart.Worksheet
                    .AppendRelativeRow();
            seperationRow
                .AppendRelativeInlineStringCell(
                    text: "",
                    styleIndex: titleTableCellFormat);
            worksheetPart.Worksheet.AppendRelativeMergeCell(cols: totalColumnsCount);
        }

        worksheetPart.Worksheet.Finalize();
    }
}
