namespace SB.Domain;

using System;

public partial interface IClassBooksAdminQueryRepository
{
    public record GetPrintsVO
    {
        public GetPrintsVO(
            DateTime createDate,
            string createBySysUserFirstName,
            string createBySysUserLastName,
            ClassBookPrintStatus status,
            string blobServiceHmacKey,
            string blobServicePublicWebUrl,
            int? blobId)
        {
            this.CreateDate = createDate;
            this.CreateBySysUserFirstName = createBySysUserFirstName;
            this.CreateBySysUserLastName = createBySysUserLastName;
            this.Status = status;
            this.BlobDownloadUrl =
                blobId.HasValue
                    ? BlobUtils.CreateBlobDownloadUrl(
                        blobServiceHmacKey,
                        blobServicePublicWebUrl,
                        blobId!.Value)
                    : null;
        }

        public DateTime CreateDate { get; init; }

        public string CreateBySysUserFirstName { get; init; }

        public string CreateBySysUserLastName { get; init; }

        public ClassBookPrintStatus Status { get; init; }

        public string? BlobDownloadUrl { get; init; }
    }
}
