using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MON.Models;
using MON.Models.Configuration;
using MON.Models.Enums;
using MON.Services.Interfaces;
using MON.Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MON.Services.Implementations
{
    public class DocumentService : BaseService<DocumentService>, IDocumentService
    {
        private readonly BlobServiceConfig _blobServiceConfig;

        public DocumentService(DbServiceDependencies<DocumentService> dependencies,
            IOptions<BlobServiceConfig> blobServiceConfig)
            : base(dependencies)
        {
            _blobServiceConfig = blobServiceConfig.Value;
        }

        public async Task<DocumentViewModel> GetDocumentByIdAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(id), "Id must be possitive number!");
            }

            DocumentViewModel document = await (from i in _context.Documents
                                  where i.Id == id
                                  select i.ToViewModel(_blobServiceConfig))
                            .FirstOrDefaultAsync();

            return document;
        }

        public async Task DeleteResourceSupportDocumentByIdAsync(int documentId)
        {
            if (documentId <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(documentId), "DocumentId must be possitive number!");
            }

            var rssDocument = await _context.ResourceSupportDocuments.Include(x => x.Document).FirstOrDefaultAsync(x => x.Id == documentId);

            _context.ResourceSupportDocuments.Remove(rssDocument);
            _context.Documents.Remove(rssDocument.Document);

            await SaveAsync();
        }
        public Task<List<DocumentViewModel>> TestFileManager()
        {
            return _context.Documents
                .AsNoTracking()
                .Where(x => x.BlobId.HasValue)
                .Take(10)
                .Select(x => x.ToViewModel(_blobServiceConfig))
                .ToListAsync();
        }

        public async Task<bool> CheckForExistingAdmissionDocumentAsync(int personId, int institutionId)
        {
            return await _context.AdmissionDocuments
                .AsNoTracking()
                .AnyAsync(d => d.PersonId == personId && d.Status == (int)DocumentStatus.Draft && d.InstitutionId != institutionId);
        }

        public async Task<bool> CheckForAdmissionDocumentInTheSameInstitutionAsync(int personId, int institutionId)
        {
            return await _context.EducationalStates
                .AsNoTracking()
                .AnyAsync(e => e.PersonId == personId && e.InstitutionId == institutionId && e.PositionId == (int)PositionType.Student);
        }
    }
}
