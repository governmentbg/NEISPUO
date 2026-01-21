namespace SB.Domain;

using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SB.Common;

public class SpbsBookExcelExportService : ISpbsBookExcelExportService
{
    private readonly ISpbsBookRecordsQueryRepository spbsBookRecordsQueryRepository;

    public SpbsBookExcelExportService(ISpbsBookRecordsQueryRepository spbsBookRecordsQueryRepository)
    {
        this.spbsBookRecordsQueryRepository = spbsBookRecordsQueryRepository;
    }

    public async Task ExportAsync(int schoolYear, int instId, int recordSchoolYear, Stream outputStream, CancellationToken ct)
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
            recordSchoolYear,
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
                Name = "книга_за_движението_СПИ_ВУИ",
            });

        document.Save();
    }

    public async Task FillWorksheetPartAsync(
        int schoolYear,
        int instId,
        int recordSchoolYear,
        WorksheetPart worksheetPart,
        WorkbookStylesPart workbookStylesPart,
        CancellationToken ct)
    {
        var spbsBookRecords = await this.spbsBookRecordsQueryRepository.GetAllAsync(instId, recordSchoolYear, null, null, null, 0, int.MaxValue, ct);

        var fontBoldId = workbookStylesPart.Stylesheet.AppendFont(bold: true);

        var vCenter_hCenter_bold_StyleIndex = workbookStylesPart.Stylesheet.AppendCellFormat(
            verticalAlignment: VerticalAlignmentValues.Center,
            horizontalAlignment: HorizontalAlignmentValues.Center,
            wrapText: true,
            fontId: fontBoldId);

        var vTop_hGeneral_StyleIndex = workbookStylesPart.Stylesheet.AppendCellFormat(
            verticalAlignment: VerticalAlignmentValues.Top,
            horizontalAlignment: HorizontalAlignmentValues.General);

        worksheetPart.InitNormalWorksheet();

        var sheetData = worksheetPart.Worksheet.GetSheetData();

        worksheetPart.Worksheet
            .AppendRelativeCustomWidthColumn(11.0)
            .AppendRelativeCustomWidthColumn(44)
            .AppendRelativeCustomWidthColumn(13.0)
            .AppendRelativeCustomWidthColumn(40.0)
            .AppendRelativeCustomWidthColumn(8.0)
            .AppendRelativeCustomWidthColumn(8.0)
            .AppendRelativeCustomWidthColumn(40.0)
            .AppendRelativeCustomWidthColumn(40.0)
            .AppendRelativeCustomWidthColumn(40.0)
            .AppendRelativeCustomWidthColumn(18.0)
            .AppendRelativeCustomWidthColumn(18.0)
            .AppendRelativeCustomWidthColumn(18.0)
            .AppendRelativeCustomWidthColumn(18.0)
            .AppendRelativeCustomWidthColumn(40.0)
            .AppendRelativeCustomWidthColumn(18.0)
            .AppendRelativeCustomWidthColumn(18.0)
            .AppendRelativeCustomWidthColumn(18.0)
            .AppendRelativeCustomWidthColumn(18.0)
            .AppendRelativeCustomWidthColumn(18.0)
            .AppendRelativeCustomWidthColumn(18.0)
            .AppendRelativeCustomWidthColumn(18.0)
            .AppendRelativeCustomWidthColumn(18.0)
            .AppendRelativeCustomWidthColumn(18.0)
            .AppendRelativeCustomWidthColumn(40.0);

        //header rows

        sheetData
            .AppendRelativeRow()
            .AppendRelativeInlineStringCell(text: $"КНИГА ЗА ДВИЖЕНИЕТО НА УЧЕНИЦИТЕ ОТ СПИ/ВУИ", styleIndex: vCenter_hCenter_bold_StyleIndex)
            .AppendRelativeInlineStringCell(offset: 9, text: $"ИНФОРМАЦИЯ ПРИ НАСТАНЯВАНЕ", styleIndex: vCenter_hCenter_bold_StyleIndex)
            .AppendRelativeInlineStringCell(offset: 4, text: $"ИНФОРМАЦИЯ ПРИ ПРЕМЕСТВАНЕ И ПРЕКРАТЯВАНЕ НА НАСТАНЯВАНЕТО", styleIndex: vCenter_hCenter_bold_StyleIndex)
            .AppendRelativeInlineStringCell(offset: 5, text: $"ИНФОРМАЦИЯ ЗА БЯГСТВАТА ОТ СПИ И ОТ ВУИ", styleIndex: vCenter_hCenter_bold_StyleIndex)
            .AppendRelativeInlineStringCell(text: $"", styleIndex: vCenter_hCenter_bold_StyleIndex);

        var headerRow1 = sheetData.AppendRelativeRow();

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "Пореден №",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "Собствено, бащино и фамилно име на ученика",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "ЕГН",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "Месторождение",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "Пол",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "Клас",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "Трите имена, адреси и телефони на родителите/настойниците",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "Адрес и телефони на МКБППМН предложила настаняването",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "Име, адрес и телефони на инспектора от Детска педагогическа стая",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "№ и дата на съдебното решение",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "№ и дата на настанителното писмо от МОН",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "Дата на постъпване",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "№ на удостоверението за преместване",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "Основание",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "№ и дата на протокола от педагогическия съвет",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "№ и дата на писмото за преместване/прекратяване",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "№ и дата на удостоверението за преместване",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "№ и дата на съобщението за преместване",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "Дата и час на регистриране на бягството",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "Дата и час на уведомяването на РПУ",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "№ и дата на писмо до РПУ/НС „Полиция“",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "Дата на връщане",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "Брой дни в бягство",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "Други отсъствия на ученика от СПИ и от ВУИ, дата, основание",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        //result rows

        foreach (var record in spbsBookRecords.Result)
        {
            var fullRecord = await this.spbsBookRecordsQueryRepository.GetAsync(schoolYear, record.SpbsBookRecordId, ct);
            var escapes = await this.spbsBookRecordsQueryRepository.GetEscapeAllAsync(schoolYear, record.SpbsBookRecordId, 0, int.MaxValue, ct);
            var absences = await this.spbsBookRecordsQueryRepository.GetAbsenceAllAsync(schoolYear, record.SpbsBookRecordId, 0, int.MaxValue, ct);

            var recordRow = sheetData.AppendRelativeRow();

            recordRow
                .AppendRelativeNumberCell(number: fullRecord.StudentPersonalInfo.RecordNumber, styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: StringUtils.JoinNames(fullRecord.StudentPersonalInfo.FirstName, fullRecord.StudentPersonalInfo.MiddleName, fullRecord.StudentPersonalInfo.LastName),
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: fullRecord.StudentPersonalInfo.PersonalId,
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: StringUtils.JoinNonEmpty("; ", fullRecord.StudentPersonalInfo.BirthPlaceCountry, fullRecord.StudentPersonalInfo.BirthPlaceTown),
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: fullRecord.StudentPersonalInfo.Gender,
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: fullRecord.StudentPersonalInfo.ClassName,
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: string.Join("\n", fullRecord.Relatives.Select(r => StringUtils.JoinNonEmpty("; ", r.Name, r.Telephone, r.Email, r.PermanentAddress))),
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: StringUtils.JoinNonEmpty("; ", fullRecord.SendingCommission, fullRecord.SendingCommissionAddress, fullRecord.SendingCommissionPhoneNumber),
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: StringUtils.JoinNonEmpty("; ", fullRecord.InspectorNames, fullRecord.InspectorAddress, fullRecord.InspectorPhoneNumber),
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: StringUtils.JoinNonEmpty("/",  fullRecord.Movement.CourtDecisionNumber, fullRecord.Movement.CourtDecisionDate?.ToString("dd.MM.yyyy")),
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: StringUtils.JoinNonEmpty("/", fullRecord.Movement.IncommingLetterNumber, fullRecord.Movement.IncommingLetterDate?.ToString("dd.MM.yyyy")),
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: StringUtils.JoinNonEmpty("/", fullRecord.Movement.IncommingDate?.ToString("dd.MM.yyyy"), fullRecord.Movement.IncommingDocNumber),
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: fullRecord.Movement.IncommingDocNumber ?? string.Empty,
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: fullRecord.Movement.TransferReason ?? string.Empty,
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: StringUtils.JoinNonEmpty("/", fullRecord.Movement.TransferProtocolNumber, fullRecord.Movement.TransferProtocolDate?.ToString("dd.MM.yyyy")),
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: StringUtils.JoinNonEmpty("/", fullRecord.Movement.TransferLetterNumber, fullRecord.Movement.TransferLetterDate?.ToString("dd.MM.yyyy")),
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: StringUtils.JoinNonEmpty("/", fullRecord.Movement.TransferCertificateNumber, fullRecord.Movement.TransferCertificateDate?.ToString("dd.MM.yyyy")),
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: StringUtils.JoinNonEmpty("/", fullRecord.Movement.TransferMessageNumber, fullRecord.Movement.TransferMessageDate?.ToString("dd.MM.yyyy")),
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: string.Join("\n", escapes.Result.Select(e => e.EscapeDateTime.ToString("dd.MM.yyyy HH:mm"))),
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: string.Join("\n", escapes.Result.Select(e => e.PoliceNotificationDateTime.ToString("dd.MM.yyyy HH:mm"))),
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: string.Join("\n", escapes.Result.Select(e => StringUtils.JoinNonEmpty("/", e.PoliceLetterNumber, e.PoliceLetterDate.ToString("dd.MM.yyyy")))),
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: string.Join("\n", escapes.Result.Select(e => e.ReturnDate?.ToString("dd.MM.yyyy") ?? string.Empty)),
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: string.Join("\n", escapes.Result.Select(e => e.EscapeDays)),
                styleIndex: vTop_hGeneral_StyleIndex);

            recordRow.AppendRelativeInlineStringCell(
                text: string.Join("\n", absences.Result.Select(a => StringUtils.JoinNonEmpty(" - ", a.AbsenceDate.ToString("dd.MM.yyyy"), a.AbsenceReason))),
                styleIndex: vTop_hGeneral_StyleIndex);
        }

        worksheetPart.Worksheet
            .AppendMergeCell("A1:I1")
            .AppendMergeCell("J1:M1")
            .AppendMergeCell("N1:R1")
            .AppendMergeCell("S1:W1");

        worksheetPart.Worksheet.Finalize();
    }
}
