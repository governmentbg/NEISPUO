namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SB.Common;

internal record CreateScheduleAndAbsencesByTermAllClassesReportCommandHandler(
    IUnitOfWork UnitOfWork,
    IServiceScopeFactory serviceScopeFactory,
    IScopedAggregateRepository<ScheduleAndAbsencesByTermAllClassesReport> ScheduleAndAbsencesByTermAllClassesReportsAggregateRepository,
    IClassBooksQueryRepository ClassBooksQueryRepository,
    BlobServiceClient BlobServiceClient)
    : IRequestHandler<CreateScheduleAndAbsencesByTermAllClassesReportCommand, int>
{
    public async Task<int> Handle(CreateScheduleAndAbsencesByTermAllClassesReportCommand command, CancellationToken ct)
    {
        DateTime createDate = DateTime.Now;

        var classBooks = await this.ClassBooksQueryRepository.GetAllAsync(command.SchoolYear!.Value, command.InstId!.Value, ct);

        var memoryStream = new MemoryStream();

        using (ZipArchive archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, leaveOpen: true))
        {
            var tasks = new List<Task>();
            foreach (var classBook in classBooks)
            {
                tasks.Add(this.GetData(command, classBook, archive, ct));
                Task.WaitAll(tasks.ToArray(), ct);
            }
        }

        memoryStream.Seek(0, SeekOrigin.Begin);

        var uploadedBlob = await this.BlobServiceClient.UploadBlobAsync(
            memoryStream,
            "report.zip",
            ct);

        var blobId = uploadedBlob.BlobId;

        ScheduleAndAbsencesByTermAllClassesReport scheduleAndAbsencesByTermAllClassesReport = new(
            command.SchoolYear!.Value,
            command.InstId!.Value,
            command.Term!.Value,
            blobId,
            createDate,
            command.SysUserId!.Value);

        await this.ScheduleAndAbsencesByTermAllClassesReportsAggregateRepository.AddAsync(scheduleAndAbsencesByTermAllClassesReport, ct);

        await this.UnitOfWork.SaveAsync(ct);

        return scheduleAndAbsencesByTermAllClassesReport.ScheduleAndAbsencesByTermAllClassesReportId;
    }

    private async Task GetData(
        CreateScheduleAndAbsencesByTermAllClassesReportCommand command,
        IClassBooksQueryRepository.GetAllVO classBook,
        ZipArchive archive,
        CancellationToken ct)
    {
        using IServiceScope scope = this.serviceScopeFactory.CreateScope();
        var scheduleAndAbsencesByTermReportsQueryRepository = scope.ServiceProvider.GetRequiredService<IScheduleAndAbsencesByTermReportsQueryRepository>();
        var scheduleAndAbsencesByTermAllClassesReportsExcelExportService = scope.ServiceProvider.GetRequiredService<IScheduleAndAbsencesByTermAllClassesReportsExcelExportService>();

        using (var excelStream = new MemoryStream())
        {
            var weeks = await scheduleAndAbsencesByTermReportsQueryRepository.GetWeeksForAddAsync(
                command.SchoolYear!.Value,
                command.InstId!.Value,
                command.Term!.Value,
                classBook.ClassBookId,
                ct);

            scheduleAndAbsencesByTermAllClassesReportsExcelExportService.ExportAsync(
                EnumUtils.GetEnumDescription(command.Term!.Value),
                classBook.FullBookName,
                classBook.BookType == ClassBookType.Book_DPLR,
                weeks,
                excelStream);

            excelStream.Seek(0, SeekOrigin.Begin);

            ZipArchiveEntry excelEntry = archive.CreateEntry($"{classBook.FullBookName}.xlsx");
            using (var entryStream = excelEntry.Open())
            {
                excelStream.CopyTo(entryStream);
            }
        }
    }
}
