namespace SB.Domain;

using SB.Common;
using System;

public partial interface IClassBooksQueryRepository
{
    public record GetAllForFinalizationVO
    {
        public GetAllForFinalizationVO(
            int classBookId,
            string className,
            string? basicClassName,
            string fullBookName,
            int? classBookPrintId,
            ClassBookPrintStatus? finalPrintStatus,
            bool? finalPrintIsSigned,
            string blobServiceHmacKey,
            string blobServicePublicWebUrl,
            int? blobId,
            ClassBookStatusChangeType? lastStatusChange,
            DateTime? lastStatusChangeDate,
            string? changedBySysUserFirstName,
            string? changedBySysUserLastName,
            string? signees)
        {
            this.ClassBookId = classBookId;
            this.ClassName = className;
            this.BasicClassName = basicClassName;
            this.FullBookName = fullBookName;
            this.ClassBookPrintId = classBookPrintId;
            this.FinalPrintStatus = finalPrintStatus;
            this.FinalPrintIsSigned = finalPrintIsSigned;
            this.BlobDownloadUrl =
                blobId.HasValue
                    ? BlobUtils.CreateBlobDownloadUrl(
                        blobServiceHmacKey,
                        blobServicePublicWebUrl,
                        blobId!.Value)
                    : null;

            if (lastStatusChange != null)
            {
                string statusStr =
                    lastStatusChange switch
                    {
                        ClassBookStatusChangeType.Finalized => "Приключен",
                        ClassBookStatusChangeType.Signed => $"Подписан с КЕП \"{signees}\"",
                        ClassBookStatusChangeType.FinalizedAndSigned => $"Приключен и подписан с КЕП \"{signees}\"",
                        ClassBookStatusChangeType.Unfinalized => "Премахнато приключване",
                        _ => throw new DomainException("Unknown ClassBookStatusChangeType"),
                    };
                string dateStr = lastStatusChangeDate!.Value.ToString("dd.MM.yyyy HH:mm");
                string name = StringUtils.JoinNames(changedBySysUserFirstName, changedBySysUserLastName);

                this.LastStatusChangeDescription = $"{statusStr} на {dateStr} от {name}";
            }
        }

        public int ClassBookId { get; init; }

        public string ClassName { get; init; }

        public string? BasicClassName { get; init; }

        public string FullBookName { get; init; }

        public int? ClassBookPrintId { get; init; }

        public ClassBookPrintStatus? FinalPrintStatus { get; init; }

        public bool? FinalPrintIsSigned { get; init; }

        public string? BlobDownloadUrl { get; init; }

        public string? LastStatusChangeDescription { get; init; }
    }
}
