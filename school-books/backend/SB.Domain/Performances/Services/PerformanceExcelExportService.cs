namespace SB.Domain;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SB.Common;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

public class PerformanceExcelExportService : IPerformanceExcelExportService
{
    private readonly IPerformancesQueryRepository performancesQueryRepository;

    public PerformanceExcelExportService(IPerformancesQueryRepository performancesQueryRepository)
    {
        this.performancesQueryRepository = performancesQueryRepository;
    }

    public async Task ExportAsync(int schoolYear, int instId, int classBookId, bool isForAllBooks, Stream outputStream, CancellationToken ct)
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

        var worksheetPart = workbookPart.AddNewPart<WorksheetPart>("performances");

        await this.FillWorksheetPartAsync(
            schoolYear,
            instId,
            classBookId,
            isForAllBooks,
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
                Name = "изяви",
            });
    }

    public async Task FillWorksheetPartAsync(
        int schoolYear,
        int instId,
        int classBookId,
        bool isForAllBooks,
        WorksheetPart worksheetPart,
        WorkbookStylesPart workbookStylesPart,
        CancellationToken ct)
    {
        var performances = isForAllBooks ?
            (await this.performancesQueryRepository.GetAllExcelDataAsync(schoolYear, instId, ct)) :
            (await this.performancesQueryRepository.GetExcelDataAsync(schoolYear, classBookId, ct));

        double dateColWidth = 14.0;
        double nameColWidth = 20;
        double description = 90.0;
        double typeColWidth = 40.0;

        var font10Id = workbookStylesPart.Stylesheet.AppendFont(size: 10.0);

        var fontBoldId = workbookStylesPart.Stylesheet.AppendFont(bold: true);

        var vCenter_hCenter_bold_StyleIndex = workbookStylesPart.Stylesheet.AppendCellFormat(
            verticalAlignment: VerticalAlignmentValues.Center,
            horizontalAlignment: HorizontalAlignmentValues.Center,
            wrapText: true,
            fontId: fontBoldId);

        var vTop_hGeneral_StyleIndex = workbookStylesPart.Stylesheet.AppendCellFormat(
            verticalAlignment: VerticalAlignmentValues.Top,
            horizontalAlignment: HorizontalAlignmentValues.General);

        var vTop_date_StyleIndex = workbookStylesPart.Stylesheet.AppendCellFormat(
            verticalAlignment: VerticalAlignmentValues.Top,
            // special date number format added in the Stylesheet
            numberFormatId: 165,
            fontId: font10Id);

        var vTop_hWrapped_StyleIndex = workbookStylesPart.Stylesheet.AppendCellFormat(
            wrapText: true,
            verticalAlignment: VerticalAlignmentValues.Top,
            horizontalAlignment: HorizontalAlignmentValues.General);

        worksheetPart.InitNormalWorksheet();

        var sheetData = worksheetPart.Worksheet.GetSheetData();

        worksheetPart.Worksheet
            .AppendRelativeCustomWidthColumn(nameColWidth, span: isForAllBooks ? 2 : 1)
            .AppendRelativeCustomWidthColumn(description)
            .AppendRelativeCustomWidthColumn(typeColWidth)
            .AppendRelativeCustomWidthColumn(dateColWidth, span: 2)
            .AppendRelativeCustomWidthColumn(nameColWidth)
            .AppendRelativeCustomWidthColumn(description);

        //header rows

        var headerRow1 = sheetData.AppendRelativeRow();

        if (isForAllBooks)
        {
            headerRow1
            .AppendRelativeInlineStringCell(
                text: "Име на дневника",
                styleIndex: vCenter_hCenter_bold_StyleIndex);
            worksheetPart.Worksheet.AppendRelativeMergeCell(rows: 2);
        }

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "Наименование",
                styleIndex: vCenter_hCenter_bold_StyleIndex);
        worksheetPart.Worksheet.AppendRelativeMergeCell(rows: 2);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "Описание",
                styleIndex: vCenter_hCenter_bold_StyleIndex);
        worksheetPart.Worksheet.AppendRelativeMergeCell(rows: 2);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "Вид на изявата",
                styleIndex: vCenter_hCenter_bold_StyleIndex);
        worksheetPart.Worksheet.AppendRelativeMergeCell(rows: 2);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "Период на провеждане",
                styleIndex: vCenter_hCenter_bold_StyleIndex);
        worksheetPart.Worksheet
            .AppendRelativeMergeCell(rows: 1, cols: 2);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "Място на провеждане",
                offset: 2,
                styleIndex: vCenter_hCenter_bold_StyleIndex);
        worksheetPart.Worksheet.AppendRelativeMergeCell(rows: 2);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "Получени награди от учениците",
                styleIndex: vCenter_hCenter_bold_StyleIndex);
        worksheetPart.Worksheet.AppendRelativeMergeCell(rows: 2);

        var headerRow2 = sheetData.AppendRelativeRow();

        headerRow2
            .AppendRelativeInlineStringCell(
                text: "От",
                offset: isForAllBooks ? 5 : 4,
                styleIndex: vCenter_hCenter_bold_StyleIndex);
        headerRow2
            .AppendRelativeInlineStringCell(
                text: "До",
                styleIndex: vCenter_hCenter_bold_StyleIndex);
        

        //result rows

        foreach (var record in performances)
        {
            var recordRow = sheetData.AppendRelativeRow();

            if (isForAllBooks)
            {
                recordRow.AppendRelativeInlineStringCell(
                text: record.ClassBookName ?? string.Empty,
                styleIndex: vTop_hGeneral_StyleIndex);
            }

            recordRow.AppendRelativeInlineStringCell(
                text: record.Name,
                styleIndex: vTop_hWrapped_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: record.Description,
                styleIndex: vTop_hWrapped_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: record.PerformanceTypeName,
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow
                .AppendRelativeDateCell(date: record.StartDate, styleIndex: vTop_date_StyleIndex);

            recordRow
                .AppendRelativeDateCell(date: record.EndDate, styleIndex: vTop_date_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: record.Location,
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: record.StudentAwards ?? string.Empty,
                styleIndex: vTop_hWrapped_StyleIndex);
        }

        worksheetPart.Worksheet.Finalize();
    }
}
