using Diplomas.Public.DataAccess;
using Diplomas.Public.Models;
using Diplomas.Public.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using Diplomas.Public.Services.Extensions;
using Diplomas.Public.Models.Configuration;
using Microsoft.Extensions.Options;

namespace Diplomas.Public.Services.Implementations
{
    public class DiplomaService : BaseService, IDiplomaService
    {
        private readonly BlobServiceConfig _blobServiceConfig;

        public DiplomaService(DiplomasContext context, ILogger<DiplomaService> logger, IOptions<BlobServiceConfig> blobServiceConfig) : base(context, logger)
        {
            _blobServiceConfig = blobServiceConfig.Value;
        }

        public async Task<List<DiplomaViewModel>> SearchAsync(DiplomaSearchModel searchModel)
        {
            if (String.IsNullOrWhiteSpace(searchModel.DocNumber)) throw new ArgumentException("Не е подаден номер, по който да се търси");

            _logger.LogInformation($"Search for diploma {JsonConvert.SerializeObject(searchModel)}");
            bool searchByFactoryNumber = true;
            var factoryNumber = searchModel.DocNumber.
                    Trim().
                    Replace(" ", "").
                    Split(new string[] { ",", ";", ":" }, StringSplitOptions.RemoveEmptyEntries);
            searchByFactoryNumber = (factoryNumber != null && factoryNumber.Length == 2);
            List<DiplomaViewModel> diplomaViewModels = new List<DiplomaViewModel>();

            var diplomas = await (
                from i in _context.Diplomas
                where i.PersonalId == searchModel.IdNumber
                    && i.IsPublic
                    && (searchByFactoryNumber ?
                        (i.Series.Replace(" ", "") == factoryNumber[0].Trim() && i.FactoryNumber.Trim() == factoryNumber[1].Trim()) :
                            (i.RegistrationNumberTotal.Replace(" ", "") + "-" + i.RegistrationNumberYear.Replace(" ", "") == searchModel.DocNumber.Trim()
                                || i.RegistrationNumberTotal.Replace(" ", "") == searchModel.DocNumber.Trim()
                                || i.RegistrationNumberYear.Replace(" ", "") == searchModel.DocNumber.Trim())
                        )
                select new
                {
                    BasicDocumentProperties = i.BasicDocument.ContentsJson,
                    DocumentProperties = i.ContentsJson,
                    RuoRegId =  i.RuoRegId,
                    Diploma = new DiplomaViewModel()
                    {
                        Id = i.Id,
                        Series = i.Series,
                        FactoryNumber = i.FactoryNumber,
                        InstitutionName = i.InstitutionName,
                        SchoolYear = i.SchoolYear,
                        SchoolYearName = i.SchoolYear + "/" + (i.SchoolYear + 1),
                        YearGraduated = i.YearGraduated,
                        IsCancelled = i.IsCancelled,
                        CancellationDate = i.CancellationDate != null ? i.CancellationDate.Value.ToString("dd.MM.yyyy") : "",
                        RegistrationNumberTotal = i.RegistrationNumberTotal,
                        RegistrationNumberYear = i.RegistrationNumberYear,
                        RegistrationNumberDate = i.RegistrationDate != null ? i.RegistrationDate.Value.ToString("dd.MM.yyyy") : "",
                        ContentsJson = i.Contents,
                        SchemaJson = i.BasicDocument.Contents,
                        Gpa = i.Gpa,
                        GpaText = i.Gpatext,
                        Person = new PersonViewModel()
                        {
                            FullName = i.FirstName + " " + i.MiddleName + " " + i.LastName,
                            FullNameLatin = i.FirstNameLatin + " " + i.MiddleNameLatin + " " + i.LastNameLatin,
                            BirthPlace = $"гр./с {i.BirthPlaceTown}, общ. {i.BirthPlaceMunicipality}, обл. {i.BirthPlaceRegion}",
                            Nationality = i.Nationality
                        },
                        BasicDocument = new BasicDocumentViewModel()
                        {
                            Name = i.BasicDocument.Name,
                            Subjects = i.DiplomaSubjects.Select(s => new DocumentSubjectViewModel()
                            {
                                Position = s.Position,
                                Id = s.Id,
                                Grade = s.Grade,
                                GradeText = s.GradeText,
                                Horarium = s.Horarium,
                                Subject = (s.SubjectName != null || s.SubjectName != "") ? s.SubjectName : (s.SubjectId != null ? s.Subject.NameFr : ""),
                                Points = s.Nvopoints,
                                FLLevel = s.FlLevel,
                                BasicDocumentPartId = s.BasicDocumentPartId
                            }).OrderBy(s => s.Position).ToList(),
                            Parts = i.BasicDocument.BasicDocumentParts.Select(x => new BasicDocumentPartViewModel()
                            {
                                Id = x.Id,
                                Name = x.Name,
                                Position = x.Position,
                                IsHorariumHidden = x.IsHorariumHidden,
                                SubjectTypesList = x.SubjectTypesList,
                                ExternalEvaluationTypesList = x.ExternalEvaluationTypesList
                            }).ToList()
                        },
                        Documents = (from d in i.DiplomaDocuments
                                     select new DiplomaDocumentModel()
                                     {
                                         Id = d.Id,
                                         BlobId = d.BlobId,
                                         Description = d.Description,
                                         Url = DocumentExtensions.CalcHmac(d.BlobId, _blobServiceConfig)
                                     }).ToList()
                    }
                }).ToListAsync();


            if (diplomas != null)
            {
                foreach (var diploma in diplomas)
                {
                    if (diploma.RuoRegId != null)
                    {
                        // Издаден от РУО
                        var region = await _context.Regions.FirstOrDefaultAsync(i => i.RegionId == diploma.RuoRegId);
                        diploma.Diploma.InstitutionName = $"РУО - {region.Name}";
                    }
                    DiplomaViewModel diplomaViewModel = null;
                    diplomaViewModel = diploma.Diploma;
                    foreach (var part in diplomaViewModel.BasicDocument.Parts)
                    {
                        part.SubjectTypes = (part.SubjectTypesList ?? "")
                            .Split("|", StringSplitOptions.RemoveEmptyEntries)
                            .Select(int.Parse)
                            .ToList();
                        part.ExternalEvaluationTypes = (part.ExternalEvaluationTypesList ?? "")
                            .Split("|", StringSplitOptions.RemoveEmptyEntries)
                            .Select(int.Parse)
                            .ToList();
                        part.Subjects = diplomaViewModel.BasicDocument.Subjects.Where(i => i.BasicDocumentPartId == part.Id).ToList();
                    }
                    diplomaViewModel.Contents = (
                        from c in diploma.BasicDocumentProperties
                        join v in diploma.DocumentProperties on c.Key equals v.Key into props
                        from p in props.DefaultIfEmpty()
                        select new PropertyDescriptionValue()
                        {
                            Key = c.Key,
                            Position = c.Position,
                            Value = p?.Value,
                            Description = c.Description,
                            Type = c.Type
                        }).ToList();

                    diplomaViewModels.Add(diplomaViewModel);
                }
            }

            return diplomaViewModels;
        }
    }
}
