namespace SB.Domain;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SB.Common;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class RegBookQualificationDuplicateExcelExportService : IRegBookQualificationDuplicateExcelExportService
{
    private readonly IRegBookQualificationDuplicateQueryRepository regBookQualificationDuplicateQueryRepository;

    public RegBookQualificationDuplicateExcelExportService(IRegBookQualificationDuplicateQueryRepository regBookQualificationDuplicateQueryRepository)
    {
        this.regBookQualificationDuplicateQueryRepository = regBookQualificationDuplicateQueryRepository;
    }

    public async Task ExportAsync(int schoolYear, int instId, int basicDocumentId, Stream outputStream, CancellationToken ct)
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

        var worksheetPart = workbookPart.AddNewPart<WorksheetPart>("regBookQualificationDuplicates");

        await this.FillWorksheetPartAsync(
            schoolYear,
            instId,
            basicDocumentId,
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
                Name = "дубликати на документи",
            });
    }

    public async Task FillWorksheetPartAsync(
        int schoolYear,
        int instId,
        int basicDocumentId,
        WorksheetPart worksheetPart,
        WorkbookStylesPart workbookStylesPart,
        CancellationToken ct)
    {
        var regBookQualificationDuplicates = await this.regBookQualificationDuplicateQueryRepository.GetExcelDataAsync(schoolYear, instId, basicDocumentId, ct);

        double regNumColWidth = 27.0;
        double regDateColWidth = 20.0;
        double nameColWidth = 44.0;
        double personalIdColWidth = 13.0;
        double duplicateTypeColWidth = 90.0;
        double docSeriesAndNumberColWidth = 20.0;
        double classSpecialityNameColWidth = 40.0;
        double eduFormColWidth = 15.0;
        double isCancelledColWidth = 10.0;
        double signatureColWidth = 12.0;

        double headerRowTotalHeight = 35.0;

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

        worksheetPart.InitNormalWorksheet();

        var sheetData = worksheetPart.Worksheet.GetSheetData();

        worksheetPart.Worksheet
            .AppendRelativeCustomWidthColumn(regNumColWidth, span: 2)
            .AppendRelativeCustomWidthColumn(regDateColWidth)
            .AppendRelativeCustomWidthColumn(nameColWidth)
            .AppendRelativeCustomWidthColumn(personalIdColWidth)
            .AppendRelativeCustomWidthColumn(duplicateTypeColWidth)
            .AppendRelativeCustomWidthColumn(docSeriesAndNumberColWidth, span: 2)
            .AppendRelativeCustomWidthColumn(classSpecialityNameColWidth)
            .AppendRelativeCustomWidthColumn(eduFormColWidth)
            .AppendRelativeCustomWidthColumn(docSeriesAndNumberColWidth, span: 2)
            .AppendRelativeCustomWidthColumn(regNumColWidth, span: 2)
            .AppendRelativeCustomWidthColumn(regDateColWidth)
            .AppendRelativeCustomWidthColumn(isCancelledColWidth)
            .AppendRelativeCustomWidthColumn(signatureColWidth);

        //header rows

        var headerRow = sheetData.AppendRelativeRow(height: headerRowTotalHeight);

        headerRow
            .AppendRelativeInlineStringCell(
                text: "Рег. № по реда на издаване на дубликата",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow
            .AppendRelativeInlineStringCell(
                text: "Рег. № по ред за годината на издаване на дубликата",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow
            .AppendRelativeInlineStringCell(
                text: "Дата на регистриране на дубликата",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow
            .AppendRelativeInlineStringCell(
                text: "Трите имена на притежателя на документа",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow
            .AppendRelativeInlineStringCell(
                text: "ЕГН/ЛНЧ",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow
            .AppendRelativeInlineStringCell(
                text: "Вид на издадения дубликат",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow
            .AppendRelativeInlineStringCell(
                text: "Серия на документа",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow
            .AppendRelativeInlineStringCell(
                text: "№ на бланката на дубликата",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow
            .AppendRelativeInlineStringCell(
                text: "Общообразователна подготовка / профил, професия, специалност",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow
            .AppendRelativeInlineStringCell(
                text: "Форма на обучение",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow
            .AppendRelativeInlineStringCell(
                text: "Серия на оригинала на документа",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow
            .AppendRelativeInlineStringCell(
                text: "Фабричен № на оригинала на документа",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow
            .AppendRelativeInlineStringCell(
                text: "Рег. № на оригиналния издаден документ",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow
            .AppendRelativeInlineStringCell(
                text: "Рег. № на оригиналния издаден документ за годината",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow
            .AppendRelativeInlineStringCell(
                text: "Дата на издаване на оригинала",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow
            .AppendRelativeInlineStringCell(
                text: "Анулиран",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow
            .AppendRelativeInlineStringCell(
                text: "Подпис на получателя",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        //result rows

        foreach (var record in regBookQualificationDuplicates)
        {
            var recordRow = sheetData.AppendRelativeRow();

            recordRow.AppendRelativeInlineStringCell(
                text: record.RegistrationNumberTotal,
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: record.RegistrationNumberYear ?? string.Empty,
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow
                .AppendRelativeDateCell(date: record.RegistrationDate, styleIndex: vTop_date_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: StringUtils.JoinNames(record.FirstName, record.MiddleName, record.LastName),
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: record.PersonalId,
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: record.BasicDocumentName,
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: record.Series ?? string.Empty,
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: record.FactoryNumber ?? string.Empty,
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: string.Join(
                    ", ",
                    new []
                    {
                        record.ClassTypeName,
                        record.SPPOOProfessionName,
                        record.SPPOOSpecialityName
                    }.Where(s => !string.IsNullOrEmpty(s))),
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: record.EduFormName ?? string.Empty,
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: record.OrigSeries ?? string.Empty,
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: record.OrigFactoryNumber ?? string.Empty,
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: record.OrigRegistrationNumber ?? string.Empty,
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: record.OrigRegistrationNumberYear ?? string.Empty,
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeDateCell(
                date: record.OrigRegistrationDate,
                styleIndex: vTop_date_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: record.IsCancelled ? "Да" : "Не",
                styleIndex: vTop_hGeneral_StyleIndex);
        }

        worksheetPart.Worksheet.Finalize();
    }
}
