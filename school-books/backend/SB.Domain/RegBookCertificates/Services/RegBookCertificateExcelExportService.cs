namespace SB.Domain;

using System.IO;
using System.Threading;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SB.Common;

class RegBookCertificateExcelExportService : IRegBookCertificateExcelExportService
{
    IRegBookCertificateQueryRepository regBookCertificateQueryRepository;
    public RegBookCertificateExcelExportService(IRegBookCertificateQueryRepository regBookCertificateQueryRepository)
    {
        this.regBookCertificateQueryRepository = regBookCertificateQueryRepository;
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

        var worksheetPart = workbookPart.AddNewPart<WorksheetPart>("regBookCertificate");

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
                Name = "удостоверения",
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
        var regBookRecords = await this.regBookCertificateQueryRepository.GetExcelDataAsync(schoolYear, instId, basicDocumentId, ct);

        double regNumColWidth = 9.0;
        double dateColWidth = 14.0;
        double nameColWidth = 44;
        double personalIdColWidth = 13.0;
        double docTypeColWidth = 80.0;
        double eduFormColWidth = 15.0;
        double docSeriesAndNumberColWidth = 20.0;
        double duplicateRegNumAndDateColWidth = 30.0;
        double signatureColWidth = 12.0;

        double headerRowTotalHeight = 30.0;

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
            .AppendRelativeCustomWidthColumn(dateColWidth)
            .AppendRelativeCustomWidthColumn(nameColWidth)
            .AppendRelativeCustomWidthColumn(personalIdColWidth)
            .AppendRelativeCustomWidthColumn(docTypeColWidth)
            .AppendRelativeCustomWidthColumn(eduFormColWidth)
            .AppendRelativeCustomWidthColumn(docSeriesAndNumberColWidth, span: 2)
            .AppendRelativeCustomWidthColumn(duplicateRegNumAndDateColWidth)
            .AppendRelativeCustomWidthColumn(signatureColWidth);

        //header rows

        var headerRow = sheetData.AppendRelativeRow(height: headerRowTotalHeight);

        headerRow
            .AppendRelativeInlineStringCell(
                text: "Пореден рег. №",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow
            .AppendRelativeInlineStringCell(
                text: "Рег. № за годината",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow
            .AppendRelativeInlineStringCell(
                text: "Дата на издаване",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow
            .AppendRelativeInlineStringCell(
                text: "Собствено, бащино и фамилно име",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow
            .AppendRelativeInlineStringCell(
                text: "ЕГН/ЛНЧ",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow
            .AppendRelativeInlineStringCell(
                text: "Вид документ",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow
            .AppendRelativeInlineStringCell(
                text: "Форма на обучение",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow
            .AppendRelativeInlineStringCell(
                text: "Серия",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow
            .AppendRelativeInlineStringCell(
                text: "Фабричен номер",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow
            .AppendRelativeInlineStringCell(
                text: "Рег. № и дата на издаване на дубликатите",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow
            .AppendRelativeInlineStringCell(
                text: "Подпис на получателя",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        //result rows

        foreach (var record in regBookRecords)
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
                text: record.EduFormName ?? string.Empty,
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: record.Series ?? string.Empty,
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: record.FactoryNumber ?? string.Empty,
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: string.Join(", ", record.DuplicatesNumberTotalAndRegDate),
                styleIndex: vTop_hGeneral_StyleIndex);
        }

        worksheetPart.Worksheet.Finalize();
    }
}
