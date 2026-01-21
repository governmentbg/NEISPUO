namespace SB.Domain;

using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SB.Common;

class SchedulesExcelService : ISchedulesExcelService
{
    private readonly ISchedulesQueryRepository schedulesQueryRepository;

    public SchedulesExcelService(ISchedulesQueryRepository schedulesQueryRepository)
    {
        this.schedulesQueryRepository = schedulesQueryRepository;
    }

    public async Task GetScheduleUsedHoursTableAsync(
        int schoolYear,
        int instId,
        int classBookId,
        int scheduleId,
        int day,
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

        var worksheetPart = workbookPart.AddNewPart<WorksheetPart>("regBookCertificate");

        await this.FillWorksheetPartAsync(
            schoolYear,
            instId,
            classBookId,
            scheduleId,
            day,
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
                Name = "разписание",
            });

        document.Save();
    }

    public async Task FillWorksheetPartAsync(
        int schoolYear,
        int instId,
        int classBookId,
        int scheduleId,
        int day,
        WorksheetPart worksheetPart,
        WorkbookStylesPart workbookStylesPart,
        CancellationToken ct)
    {
        var usedHoursTable =
            await this.schedulesQueryRepository.GetScheduleUsedHoursTableAsync(
                schoolYear,
                instId,
                classBookId,
                scheduleId,
                day,
                ct);

        var fontBoldId = workbookStylesPart.Stylesheet.AppendFont(bold: true);

        var vCenter_hCenter_bold_StyleIndex = workbookStylesPart.Stylesheet.AppendCellFormat(
            verticalAlignment: VerticalAlignmentValues.Center,
            horizontalAlignment: HorizontalAlignmentValues.Center,
            wrapText: true,
            fontId: fontBoldId);

        var vTop_hGeneral_StyleIndex = workbookStylesPart.Stylesheet.AppendCellFormat(
            verticalAlignment: VerticalAlignmentValues.Top,
            horizontalAlignment: HorizontalAlignmentValues.General);

        var vTop_hGeneral_wrapped_StyleIndex = workbookStylesPart.Stylesheet.AppendCellFormat(
            wrapText: true,
            verticalAlignment: VerticalAlignmentValues.Top,
            horizontalAlignment: HorizontalAlignmentValues.General);

        worksheetPart.InitNormalWorksheet();

        var sheetData = worksheetPart.Worksheet.GetSheetData();

        worksheetPart.Worksheet
            .AppendRelativeCustomWidthColumn(11.0)
            .AppendRelativeCustomWidthColumn(40.0)
            .AppendRelativeCustomWidthColumn(40.0)
            .AppendRelativeCustomWidthColumn(40.0)
            .AppendRelativeCustomWidthColumn(40.0)
            .AppendRelativeCustomWidthColumn(40.0)
            .AppendRelativeCustomWidthColumn(40.0);

        //header rows

        sheetData
            .AppendRelativeRow()
            .AppendRelativeInlineStringCell(text: "Час", styleIndex: vCenter_hCenter_bold_StyleIndex)
            .AppendRelativeInlineStringCell(text: "Предмет", styleIndex: vCenter_hCenter_bold_StyleIndex)
            .AppendRelativeInlineStringCell(text: "Учeнически отсъствия", styleIndex: vCenter_hCenter_bold_StyleIndex)
            .AppendRelativeInlineStringCell(text: "Оценки", styleIndex: vCenter_hCenter_bold_StyleIndex)
            .AppendRelativeInlineStringCell(text: "Теми", styleIndex: vCenter_hCenter_bold_StyleIndex)
            .AppendRelativeInlineStringCell(text: "Учителски отсъствия", styleIndex: vCenter_hCenter_bold_StyleIndex)
            .AppendRelativeInlineStringCell(text: "Лекторски часове", styleIndex: vCenter_hCenter_bold_StyleIndex);

        //result rows

        foreach (var record in usedHoursTable)
        {
            var recordRow = sheetData.AppendRelativeRow();

            recordRow
                .AppendRelativeNumberCell(number: record.HourNumber, styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: record.CurriculumName,
                styleIndex: vTop_hGeneral_wrapped_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: string.Join(", ", record.Absences.Select(d => d.ToString("dd.MM.yyyy"))),
                styleIndex: vTop_hGeneral_wrapped_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: string.Join(", ", record.Grades.Select(d => d.ToString("dd.MM.yyyy"))),
                styleIndex: vTop_hGeneral_wrapped_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: string.Join(", ", record.Topics.Select(d => d.ToString("dd.MM.yyyy"))),
                styleIndex: vTop_hGeneral_wrapped_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: string.Join(",\n", record.TeacherAbsences.Select(ta => $"{ta.StartDate:dd.MM.yyyy} - {ta.EndDate:dd.MM.yyyy} - {ta.TeacherPersonName}")),
                styleIndex: vTop_hGeneral_wrapped_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: string.Join(",\n", record.LectureSchedules.Select(ta => $"{ta.StartDate:dd.MM.yyyy} - {ta.EndDate:dd.MM.yyyy} - {ta.TeacherPersonName}")),
                styleIndex: vTop_hGeneral_wrapped_StyleIndex);
        }

        worksheetPart.Worksheet.Finalize();
    }
}
