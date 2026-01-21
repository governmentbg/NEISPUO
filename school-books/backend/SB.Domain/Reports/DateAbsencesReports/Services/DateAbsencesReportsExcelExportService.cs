namespace SB.Domain;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SB.Common;

public class DateAbsencesReportsExcelExportService : IDateAbsencesReportsExcelExportService
{
    private readonly IDateAbsencesReportsQueryRepository dateAbsencesReportsQueryRepository;

    public DateAbsencesReportsExcelExportService(
        IDateAbsencesReportsQueryRepository dateAbsencesReportsQueryRepository)
    {
        this.dateAbsencesReportsQueryRepository = dateAbsencesReportsQueryRepository;
    }

    public async Task ExportAsync(
        int schoolYear,
        int instId,
        int dateAbsencesReportId,
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

        var worksheetPart = workbookPart.AddNewPart<WorksheetPart>("dateAbsencesReport");

        await this.FillWorksheetPartAsync(
            schoolYear,
            instId,
            dateAbsencesReportId,
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
                Name = "oтсъстващи за деня"
            });
    }

    public async Task FillWorksheetPartAsync(
        int schoolYear,
        int instId,
        int dateAbsencesReportId,
        WorksheetPart worksheetPart,
        WorkbookStylesPart workbookStylesPart,
        CancellationToken ct)
    {
        var report =
            await this.dateAbsencesReportsQueryRepository.GetExcelDataAsync(
                schoolYear,
                instId,
                dateAbsencesReportId,
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

        var sheetData = worksheetPart.Worksheet.GetSheetData();

        var itemsByShifts = report.Items.SelectMany(i => i.Hours).ToLookup(i => new { i.ShiftId, ShiftName = i.ShiftName ?? string.Empty });

        var shiftHoursTotalCount = report.Items.FirstOrDefault()?.Hours.Length ?? 0;

        var titleRow =
            worksheetPart.Worksheet
                .AppendRelativeRow();

        titleRow
            .AppendRelativeInlineStringCell(
                text: $"Отсъстващи за {report.ReportDate:dd.MM.yyyy} към дата {report.CreateDate: dd.MM.yyyy HH:mm}",
                styleIndex: titleTableCellFormat);
        worksheetPart.Worksheet
            .AppendMergeCell($"A{titleRow.RowIndex}:" +
                $"{OpenXmlExtensions.ColumnIdToColumnIndex(shiftHoursTotalCount)}{titleRow.RowIndex}");

        //header rows

        var headerRow1 = sheetData.AppendRelativeRow();

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "Клас",
                styleIndex: primaryTableCellCenteredBold);
        worksheetPart.Worksheet.AppendRelativeMergeCell(rows: report.IsUnited ? 2 : 3);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: $"{(report.IsUnited ? "Час" : "Смяна")}",
                styleIndex: primaryTableCellCenteredBold);
        worksheetPart.Worksheet.AppendRelativeMergeCell(cols: shiftHoursTotalCount);

        var headerRow2 = sheetData.AppendRelativeRow();

        var absencesAggregatedCount = new Dictionary<(int? shiftId, int hour), int>();

        if (!report.IsUnited)
        {
            var mergedCellOffset = 0;
            var headerRow3 = sheetData.AppendRelativeRow();

            foreach (var shift in itemsByShifts.Select((value, index) => new { index, value }))
            {
                var hours = shift.value.Select(s => s.HourNumber).Distinct().ToArray();

                if (shift.index == 0)
                {
                    mergedCellOffset = 2;
                }

                headerRow2
                    .AppendRelativeInlineStringCell(
                        text: shift.value.Key.ShiftName!,
                        offset: mergedCellOffset,
                        styleIndex: primaryTableCellCenteredBold);

                mergedCellOffset = hours.Length;

                if (hours.Length > 1)
                {
                    worksheetPart.Worksheet.AppendRelativeMergeCell(cols: hours.Length, relativeRow: headerRow2);
                }

                for (int i = 0; i < hours.Length; i++)
                {
                    absencesAggregatedCount.Add((shift.value.Key.ShiftId, hours[i]), 0);

                    headerRow3
                        .AppendRelativeInlineStringCell(
                            text: hours[i].ToString(),
                            offset: shift.index == 0 && i == 0 ? 2 : 1,
                            styleIndex: primaryTableCellCenteredBold);
                }
            }
        }
        else
        {
            foreach (var shift in itemsByShifts)
            {
                var hours = shift.Select(s => s.HourNumber).Distinct().ToArray();

                for (int i = 0; i < hours.Length; i++)
                {
                    absencesAggregatedCount.Add((shift.Key.ShiftId, hours[i]), 0);

                    headerRow2
                        .AppendRelativeInlineStringCell(
                            text: hours[i].ToString(),
                            offset: i == 0 ? 2 : 1,
                            styleIndex: primaryTableCellCenteredBold);
                }                
            }
        }

        //result rows

        foreach (var item in report.Items)
        {
            var recordRow = sheetData.AppendRelativeRow();

            recordRow.AppendRelativeInlineStringCell(
                text: item.ClassBookName ?? string.Empty,
                styleIndex: primaryTableCellCenteredBold);

            if (item.IsOffDay)
            {
                recordRow
                    .AppendRelativeInlineStringCell(
                        text: "Този ден е неучебен за класа и избраната дата",
                        styleIndex: primaryTableCellCenteredBold);
                        worksheetPart.Worksheet.AppendRelativeMergeCell(cols: shiftHoursTotalCount);
            }
            else if (!item.HasScheduleDate)
            {
                recordRow
                    .AppendRelativeInlineStringCell(
                        text: "Този клас няма въведено разписание за избраната дата",
                        styleIndex: primaryTableCellCenteredBold);
                worksheetPart.Worksheet.AppendRelativeMergeCell(cols: shiftHoursTotalCount);
            }
            else
            {
                foreach (var hour in item.Hours)
                {
                    absencesAggregatedCount[(hour.ShiftId, hour.HourNumber)] += hour.AbsenceStudentCount;

                    recordRow
                        .AppendRelativeInlineStringCell(
                            text: hour.AbsenceStudentNumbers ?? string.Empty,
                            styleIndex: primaryTableCellCentered);
                }
            }
        }

        // total row

        var totalRow = sheetData.AppendRelativeRow();

        totalRow
            .AppendRelativeInlineStringCell(
                text: "Общо",
                styleIndex: primaryTableCellCenteredBold);

        foreach (var shift in itemsByShifts)
        {
            var hours = shift.Select(s => s.HourNumber).Distinct();

            foreach (var hour in hours)
            {
                totalRow
                .AppendRelativeInlineStringCell(
                    text: absencesAggregatedCount[(shift.Key.ShiftId, hour)].ToString(),
                    styleIndex: primaryTableCellCenteredBold);
            }
        }

        worksheetPart.Worksheet.Finalize();
    }
}
