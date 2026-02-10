namespace SB.Domain;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using SB.Common;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class RegBookQualificationExcelExportService : IRegBookQualificationExcelExportService
{
    private readonly IRegBookQualificationQueryRepository regBookQualificationQueryRepository;

    public RegBookQualificationExcelExportService(IRegBookQualificationQueryRepository regBookQualificationQueryRepository)
    {
        this.regBookQualificationQueryRepository = regBookQualificationQueryRepository;
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

        var worksheetPart = workbookPart.AddNewPart<WorksheetPart>("regBookQualifications");

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
                Name = "документи",
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
        var regBookQualifications = await this.regBookQualificationQueryRepository.GetExcelDataAsync(schoolYear, instId, basicDocumentId, ct);

        double regNumColWidth = 13.5;
        double regDateColWidth = 10.0;
        double nameColWidth = 30.0;
        double personalIdColWidth = 13.0;
        double documentTypeColWidth = 60.0;
        double eduFormColWidth = 15.0;
        double classSpecialityNameColWidth = 40.0;
        double gpaColWidth = 17.0;
        double isCancelledColWidth = 10.0;
        double docSeriesAndNumberColWidth = 12.0;
        double duplicateRegDateColWidth = 13.0;
        double duplicateDocSeriesAndNumberColWidth = 18.0;
        double signatureColWidth = 12.0;

        double headerRowsTotalHeight = 60.0;

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
            numberFormatId: 165);

        worksheetPart.InitNormalWorksheet();

        var sheetData = worksheetPart.Worksheet.GetSheetData();

        worksheetPart.Worksheet
            .AppendRelativeCustomWidthColumn(regNumColWidth, span: 2)
            .AppendRelativeCustomWidthColumn(regDateColWidth)
            .AppendRelativeCustomWidthColumn(nameColWidth)
            .AppendRelativeCustomWidthColumn(personalIdColWidth)
            .AppendRelativeCustomWidthColumn(documentTypeColWidth)
            .AppendRelativeCustomWidthColumn(eduFormColWidth)
            .AppendRelativeCustomWidthColumn(classSpecialityNameColWidth)
            .AppendRelativeCustomWidthColumn(gpaColWidth)
            .AppendRelativeCustomWidthColumn(docSeriesAndNumberColWidth, span: 2)
            .AppendRelativeCustomWidthColumn(isCancelledColWidth)
            .AppendRelativeCustomWidthColumn(duplicateRegDateColWidth)
            .AppendRelativeCustomWidthColumn(duplicateDocSeriesAndNumberColWidth)
            .AppendRelativeCustomWidthColumn(signatureColWidth);

        //header rows

        var headerRow1 = sheetData.AppendRelativeRow(height: headerRowsTotalHeight / 2);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "Рег. № по реда за издаване на документа",
                styleIndex: vCenter_hCenter_bold_StyleIndex);
        worksheetPart.Worksheet.AppendRelativeMergeCell(rows: 2);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "Рег. № по ред на годината",
                styleIndex: vCenter_hCenter_bold_StyleIndex);
        worksheetPart.Worksheet.AppendRelativeMergeCell(rows: 2);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "Дата",
                styleIndex: vCenter_hCenter_bold_StyleIndex);
        worksheetPart.Worksheet.AppendRelativeMergeCell(rows: 2);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "Собствено, бащино и фамилно име на ученика",
                styleIndex: vCenter_hCenter_bold_StyleIndex);
        worksheetPart.Worksheet.AppendRelativeMergeCell(rows: 2);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "ЕГН/ЛНЧ",
                styleIndex: vCenter_hCenter_bold_StyleIndex);
        worksheetPart.Worksheet.AppendRelativeMergeCell(rows: 2);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "Вид на документа",
                styleIndex: vCenter_hCenter_bold_StyleIndex);
        worksheetPart.Worksheet.AppendRelativeMergeCell(rows: 2);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "Форма на обучение",
                styleIndex: vCenter_hCenter_bold_StyleIndex);
        worksheetPart.Worksheet.AppendRelativeMergeCell(rows: 2);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "Общообразователна подготовка / профил, професия, специалност",
                styleIndex: vCenter_hCenter_bold_StyleIndex);
        worksheetPart.Worksheet.AppendRelativeMergeCell(rows: 2);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "Общ успех",
                styleIndex: vCenter_hCenter_bold_StyleIndex);
        worksheetPart.Worksheet.AppendRelativeMergeCell(rows: 2);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "Серия на документа",
                styleIndex: vCenter_hCenter_bold_StyleIndex);
        worksheetPart.Worksheet.AppendRelativeMergeCell(rows: 2);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "№ на документа",
                styleIndex: vCenter_hCenter_bold_StyleIndex);
        worksheetPart.Worksheet.AppendRelativeMergeCell(rows: 2);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "Анулиран",
                styleIndex: vCenter_hCenter_bold_StyleIndex);
        worksheetPart.Worksheet.AppendRelativeMergeCell(rows: 2);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "Данни за издаден дубликат",
                styleIndex: vCenter_hCenter_bold_StyleIndex);
        worksheetPart.Worksheet
            .AppendRelativeMergeCell(rows: 1, cols: 2);

        headerRow1
            .AppendRelativeInlineStringCell(
                text: "Подпис на получателя",
                offset: 2,
                styleIndex: vCenter_hCenter_bold_StyleIndex);
        worksheetPart.Worksheet.AppendRelativeMergeCell(rows: 2);

        var headerRow2 = sheetData.AppendRelativeRow(height: headerRowsTotalHeight / 2);

        headerRow2
            .AppendRelativeInlineStringCell(
                text: "Дата на издаване",
                offset: 13,
                styleIndex: vCenter_hCenter_bold_StyleIndex);
        headerRow2
            .AppendRelativeInlineStringCell(
                text: "Серия и № на дубликата",
                styleIndex: vCenter_hCenter_bold_StyleIndex);

        //result rows

        foreach (var record in regBookQualifications)
        {
            var rowspan = Math.Max(record.Duplicates.Length, 1);
            Row[] recordRows = new Row[rowspan];

            for (int i = 0; i < recordRows.Length; i++)
            {
                recordRows[i] = worksheetPart.Worksheet.AppendRelativeRow();
            }


            string numberCellRef = recordRows[0].AppendRelativeInlineStringCell(
                text: record.RegistrationNumberTotal,
                styleIndex: vTop_hGeneral_StyleIndex).GetLastCell()!.CellReference!;
            if (rowspan > 1)
            {
                worksheetPart.Worksheet.AppendMergeCell(numberCellRef, rows: rowspan);
            }

            string regNumberCellRef = recordRows[0].AppendRelativeInlineStringCell(
                text: record.RegistrationNumberYear ?? string.Empty,
                styleIndex: vTop_hGeneral_StyleIndex).GetLastCell()!.CellReference!;
            if (rowspan > 1)
            {
                worksheetPart.Worksheet.AppendMergeCell(regNumberCellRef, rows: rowspan);
            }

            string regDateCellRef = recordRows[0]
                .AppendRelativeDateCell(date: record.RegistrationDate, styleIndex: vTop_date_StyleIndex).GetLastCell()!.CellReference!;
            if (rowspan > 1)
            {
                worksheetPart.Worksheet.AppendMergeCell(regDateCellRef, rows: rowspan);
            }

            string nameCellRef = recordRows[0].AppendRelativeInlineStringCell(
                text: StringUtils.JoinNames(record.FirstName, record.MiddleName, record.LastName),
                styleIndex: vTop_hGeneral_StyleIndex).GetLastCell()!.CellReference!;
            if (rowspan > 1)
            {
                worksheetPart.Worksheet.AppendMergeCell(nameCellRef, rows: rowspan);
            }

            string personalIdCellRef = recordRows[0].AppendRelativeInlineStringCell(
                text: record.PersonalId,
                styleIndex: vTop_hGeneral_StyleIndex).GetLastCell()!.CellReference!;
            if (rowspan > 1)
            {
                worksheetPart.Worksheet.AppendMergeCell(personalIdCellRef, rows: rowspan);
            }

            string documentTypeCellRef = recordRows[0].AppendRelativeInlineStringCell(
                text: record.BasicDocumentName,
                styleIndex: vTop_hGeneral_StyleIndex).GetLastCell()!.CellReference!;
            if (rowspan > 1)
            {
                worksheetPart.Worksheet.AppendMergeCell(documentTypeCellRef, rows: rowspan);
            }

            string eduFormNameCellRef = recordRows[0].AppendRelativeInlineStringCell(
                text: record.EduFormName ?? string.Empty,
                styleIndex: vTop_hGeneral_StyleIndex).GetLastCell()!.CellReference!;
            if (rowspan > 1)
            {
                worksheetPart.Worksheet.AppendMergeCell(eduFormNameCellRef, rows: rowspan);
            }

            string classSpecialityNameCellRef = recordRows[0].AppendRelativeInlineStringCell(
                text: string.Join(
                    ", ",
                    new []
                    {
                        record.ClassTypeName,
                        record.SPPOOProfessionName,
                        record.SPPOOSpecialityName
                    }.Where(s => !string.IsNullOrEmpty(s))),
                styleIndex: vTop_hGeneral_StyleIndex).GetLastCell()!.CellReference!;
            if (rowspan > 1)
            {
                worksheetPart.Worksheet.AppendMergeCell(classSpecialityNameCellRef, rows: rowspan);
            }

            string gpaCellRef = recordRows[0].AppendRelativeInlineStringCell(
                text: record.Gpa,
                styleIndex: vTop_hGeneral_StyleIndex).GetLastCell()!.CellReference!;
            if (rowspan > 1)
            {
                worksheetPart.Worksheet.AppendMergeCell(gpaCellRef, rows: rowspan);
            }

            string docSeriesCellRef = recordRows[0].AppendRelativeInlineStringCell(
                text: record.Series ?? string.Empty,
                styleIndex: vTop_hGeneral_StyleIndex).GetLastCell()!.CellReference!;
            if (rowspan > 1)
            {
                worksheetPart.Worksheet.AppendMergeCell(docSeriesCellRef, rows: rowspan);
            }

            string docFactoryNumberCellRef = recordRows[0].AppendRelativeInlineStringCell(
                text: record.FactoryNumber ?? string.Empty,
                styleIndex: vTop_hGeneral_StyleIndex).GetLastCell()!.CellReference!;
            if (rowspan > 1)
            {
                worksheetPart.Worksheet.AppendMergeCell(docFactoryNumberCellRef, rows: rowspan);
            }

            string isCancelledCellRef = recordRows[0].AppendRelativeInlineStringCell(
                text: record.IsCancelled ? "Да" : "Не",
                styleIndex: vTop_hGeneral_StyleIndex).GetLastCell()!.CellReference!;
            if (rowspan > 1)
            {
                worksheetPart.Worksheet.AppendMergeCell(isCancelledCellRef, rows: rowspan);
            }

            for (int i = 0; i < record.Duplicates.Length; i++)
            {
                recordRows[i].AppendRelativeDateCell(
                    offset: i == 0 ? 1 : 10,
                    date: record.Duplicates[i].RegistrationDate,
                    styleIndex: vTop_date_StyleIndex);
                recordRows[i].AppendRelativeInlineStringCell(
                    text: record.Duplicates[i].DocumentSeriesAndNumber,
                    styleIndex: vTop_hGeneral_StyleIndex);
            }
        }

        worksheetPart.Worksheet.Finalize();
    }
}
