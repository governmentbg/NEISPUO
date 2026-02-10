
using Microsoft.EntityFrameworkCore;
using MON.DataAccess;
using MON.Models;
using MON.Models.Dynamic;
using MON.Models.Grid;
using MON.Services.Interfaces;
using MON.Services.Security.Permissions;
using MON.Shared;
using MON.Shared.Extensions;
using MON.Shared.Interfaces;
using MON.Shared.ErrorHandling;
using MON.Shared.Extensions.Dynamic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Net;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;
using System.Collections.Generic;
using DocumentFormat.OpenXml.Office2013.Excel;

namespace MON.Services.Implementations
{
    public class BasicDocumentService : BaseService<BasicDocumentService>, IBasicDocumentService
    {
        private readonly IEmailService _emailService;

        public BasicDocumentService(DbServiceDependencies<BasicDocumentService> dependencies, IEmailService emailService)
            : base(dependencies)
        {
            _emailService = emailService;

        }

        #region Private members
        private void UpdateBasicDocumentPart(BasicDocumentPartUpdateModel source, BasicDocumentPart target)
        {
            if (source == null || target == null) return;

            target.Position = source.Position;
            target.Name = source.Name;
            target.Description = source.Description;
            target.IsHorariumHidden = source.IsHorariumHidden ?? false;
            target.Code = source.Code;
            target.BasicClassId = source.BasicClassId;
            target.TotalLines = source.TotalLines ?? default;
            target.PrintedLines = source.PrintedLines ?? default;
            target.SubjectTypesList = source.SubjectTypes != null && source.SubjectTypes.Any()
                ? string.Join("|", source.SubjectTypes)
                : null;
            target.ExternalEvaluationTypesList = source.ExternalEvaluationTypes != null && source.ExternalEvaluationTypes.Any()
                ? string.Join("|", source.ExternalEvaluationTypes)
                : null;

            // Предмети в модела на секцията с валидно Id (Id != null и Id > 0).
            HashSet<int> existingSubjectsIds = source.BasicDocumentSubjects != null
                ? source.BasicDocumentSubjects.Where(x => x.Id.HasValue).Select(x => x.Id.Value).ToHashSet()
                : new HashSet<int>();

            // Предмети от секция за триене. Налични са в базата, но не са в модела на секцията.
            IEnumerable<BasicDocumentSubject> subjectsToDelete = target.BasicDocumentSubjects.Where(x => !existingSubjectsIds.Contains(x.Id));
            if (subjectsToDelete.Any())
            {
                _context.BasicDocumentSubjects.RemoveRange(subjectsToDelete);
            }

            if (source.BasicDocumentSubjects != null)
            {
                // Предмети към секциятата за добавяне. Са в модела, но не са в базата.
                IEnumerable<BasicDocumentSubjectModel> subjectsToAdd = source.BasicDocumentSubjects.Where(x => !x.Id.HasValue).OrderBy(x => x.Position);
                if (subjectsToAdd.Any())
                {
                    _context.BasicDocumentSubjects.AddRange(subjectsToAdd.Select(x => new BasicDocumentSubject
                    {
                        BasicDocumentPartId = target.Id,
                        SubjectId = x.SubjectId,
                        SubjectTypeId = x.SubjectTypeId,
                        Position = x.Position,
                        SubjectCanChange = x.SubjectCanChange
                    }));
                }
            }

            if (existingSubjectsIds.Any())
            {
                // В модела на текущата секция има предмети за за редакция. Такива с Id, които са налични в базата.
                foreach (BasicDocumentSubject subjetTarget in target.BasicDocumentSubjects.Where(x => existingSubjectsIds.Contains(x.Id)))
                {
                    BasicDocumentSubjectModel subjectSource = source.BasicDocumentSubjects.SingleOrDefault(x => x.Id.HasValue && x.Id.Value == subjetTarget.Id);
                    UpdateBasicDocumentSubject(subjectSource, subjetTarget);
                }
            }
        }

        private void UpdateBasicDocumentSubject(BasicDocumentSubjectModel source, BasicDocumentSubject target)
        {
            if (source == null || target == null) return;

            target.SubjectId = source.SubjectId;
            target.Position = source.Position;
            target.SubjectTypeId = source.SubjectTypeId;
            target.SubjectCanChange = source.SubjectCanChange;
        }
        #endregion

        public async Task<BasicDocumentModel> GetByIdAsync(int id)
        {
            var basicDocument = await (
                from doc in _context.BasicDocuments
                where doc.Id == id
                select new BasicDocumentModel()
                {
                    Id = doc.Id,
                    Name = doc.Name,
                    Description = doc.Description,
                    IsValidation = doc.IsValidation,
                    CodeClassName = doc.CodeClassName,
                    IsDuplicate = doc.IsDuplicate
                }).FirstOrDefaultAsync();

            return basicDocument;

        }

        /// <summary>
        /// Изтриване на номер от поредицата
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteBasicDocumentSequenceAsync(int id)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForBasicDocumentSequenceManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 404);
            }

            var basicDocumentSequence = await _context.BasicDocumentSequences.FirstOrDefaultAsync(i => i.Id == id)
                ?? throw new ApiException($"{Messages.EmptyEntityError} - basicDocumentSequence", 401);

            var basicDocumentGenerator = basicDocumentSequence.InstitutionId.HasValue
                ? await _context.BasicDocumentGenerators.FirstOrDefaultAsync(i => i.InstitutionId == _userInfo.InstitutionID && i.BasicDocumentId == basicDocumentSequence.BasicDocumentId && i.SchoolYear == basicDocumentSequence.SchoolYear)
                : await _context.BasicDocumentGenerators.FirstOrDefaultAsync(i => i.RegionId == _userInfo.RegionID && i.BasicDocumentId == basicDocumentSequence.BasicDocumentId && i.SchoolYear == basicDocumentSequence.SchoolYear);


            if (basicDocumentGenerator == null)
            {
                throw new ApiException($"{Messages.EmptyEntityError} - basicDocumentGenerator", 401);
            }

            if (basicDocumentSequence.InstitutionId.HasValue)
            {
                if (basicDocumentSequence.InstitutionId.Value != _userInfo.InstitutionID)
                {
                    throw new ApiException($"Опитвате се да изтриете номер, който не е създаден за Вашата институция ({_userInfo.InstitutionID})", 401);
                }
            }

            if (basicDocumentSequence.RegionId.HasValue)
            {
                if (basicDocumentSequence.RegionId.Value != _userInfo.RegionID)
                {
                    throw new ApiException($"Опитвате се да изтриете номер, който не е създаден за Вашата организация ({_userInfo.RegionID})", 401);
                }
            }

            bool existsLaterSequence =  basicDocumentSequence.InstitutionId.HasValue
                ? _context.BasicDocumentSequences.Any(i => i.InstitutionId == basicDocumentSequence.InstitutionId && i.BasicDocumentId == basicDocumentSequence.BasicDocumentId &&
                    (i.SchoolYear > basicDocumentSequence.SchoolYear || (i.SchoolYear == basicDocumentSequence.SchoolYear && i.RegNumberTotal > basicDocumentSequence.RegNumberTotal)))
                : _context.BasicDocumentSequences.Any(i => i.RegionId == basicDocumentSequence.RegionId && i.BasicDocumentId == basicDocumentSequence.BasicDocumentId &&
                    (i.SchoolYear > basicDocumentSequence.SchoolYear || (i.SchoolYear == basicDocumentSequence.SchoolYear && i.RegNumberTotal > basicDocumentSequence.RegNumberTotal)));

            if (existsLaterSequence)
            {
                throw new ApiException("Съществува номер за този вид документ, който е по-късен от този, който искате да изтриете");
            }

            _context.BasicDocumentSequences.Remove(basicDocumentSequence);
            if (basicDocumentGenerator.RegNumberYear > 1)
            {
                basicDocumentGenerator.RegNumberTotal--;
                basicDocumentGenerator.RegNumberYear--;
            }
            else
            {
                // RegNumberYear е бил 1, тоест това е бил първият номер за годината
                // Няма такива записи, трябва да изтрием генератора за тази година
                _context.BasicDocumentGenerators.Remove(basicDocumentGenerator);
            }

            await SaveAsync();
        }

        /// <summary>
        /// Списък с генерирани номера за институция
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public async Task<IPagedList<BasicDocumentSequenceViewModel>> GetBasicDocumentSequencesAsync(BasicDocumentSequenceListInput input, CancellationToken cancellationToken)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForBasicDocumentSequenceRead))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            if (input == null)
                throw new ArgumentNullException(nameof(input), Messages.EmptyModelError);

            bool filterByInstitution = _userInfo.IsInRole(Shared.Enums.UserRoleEnum.School);

            IQueryable<VBasicDocumentSequenceList> query = _context.VBasicDocumentSequenceLists
                .AsNoTracking();

            if (filterByInstitution)
            {
                query = query.Where(x => x.InstitutionId == _userInfo.InstitutionID);
            }

            if (_userInfo.IsRuo || _userInfo.IsRuoExpert)
            {
                query = (input.RegNumType ?? 0) switch
                {
                    2 => query.Where(x => !x.InstitutionId.HasValue && x.RegionId == _userInfo.RegionID),
                    _ => query.Where(x => x.InstitutionId.HasValue && x.RegionId == _userInfo.RegionID),
                };
            } else
            {
                query = (input.RegNumType ?? 0) switch
                {
                    2 => query.Where(x => !x.InstitutionId.HasValue && x.RegionId.HasValue),
                    _ => query.Where(x => x.InstitutionId.HasValue),
                };
            }

            if (input.Year.HasValue)
            {
                query = query.Where(x => x.SchoolYear == input.Year.Value);
            }

            if (input.InstitutionId.HasValue)
            {
                query = query.Where(x => x.InstitutionId == input.InstitutionId.Value);
            }

            if (!input.BasicDocuments.IsNullOrEmpty())
            {
                HashSet<int> basicDocumentsFilter = input.BasicDocuments
                    .Split("|", StringSplitOptions.RemoveEmptyEntries)
                    .ToHashSet<int>();

                if (basicDocumentsFilter.Count > 0)
                {
                    query = query.Where(x => basicDocumentsFilter.Contains(x.BasicDocumentId));
                }
            }

            if (!input.InstitutionIdFilter.IsNullOrWhiteSpace())
            {
                query = query.Where("InstitutionId", typeof(int), input.InstitutionIdFilterOp, input.InstitutionIdFilter);
            }

            if (!input.FullNameFilter.IsNullOrWhiteSpace())
            {
                query = query.Where("FullName", typeof(string), input.FullNameFilterOp, input.FullNameFilter);
            }

            IQueryable<BasicDocumentSequenceViewModel> listQuery = query.Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.BasicDocumentName.Contains(input.Filter)
                   || predicate.InstitutionId.ToString().Contains(input.Filter)
                   || predicate.FullName.Contains(input.Filter))
                    .Select(x => new BasicDocumentSequenceViewModel()
                    {
                        Id = x.Id,
                        RegDate = x.RegDate,
                        RegNumberTotal = x.RegNumberTotal,
                        RegNumberYear = x.RegNumberYear,
                        SchoolYear = x.SchoolYear,
                        DiplomaId = x.DiplomaId,
                        PersonId = x.PersonId,
                        InstitutionId = x.InstitutionId,
                        InstitutionName = x.InstitutionName,
                        FullName = x.FullName,
                        BasicDocumentId = x.BasicDocumentId,
                        BasicDocumentName = x.BasicDocumentName,
                        RegionId = x.RegionId,
                        IsUsed = x.DiplomaId != null
                    })
                    .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "RegDate desc" : input.SortBy);


            int totalCount = await listQuery.CountAsync(cancellationToken);
            List<BasicDocumentSequenceViewModel> items = await listQuery.PagedBy(input.PageIndex, input.PageSize).ToListAsync(cancellationToken);

            return items.ToPagedList(totalCount);
        }


        public async Task<BasicDocumentTemplateUpdateModel> GetBasicDocumentTemplate(int? id)
        {
            var entity = await _context.BasicDocuments
                .Where(x => x.Id == id)
                .Select(x => new
                {
                    x.Id,
                    x.Name,
                    x.Contents,
                    BasicDocumentParts = x.BasicDocumentParts.Select(p => new
                    {
                        p.Id,
                        p.Name,
                        p.Description,
                        p.Position,
                        p.IsHorariumHidden,
                        p.BasicClassId,
                        p.SubjectTypesList,
                        p.ExternalEvaluationTypesList,
                        p.Code,
                        p.PrintedLines,
                        p.TotalLines,
                        BasicDocumentSubjects = p.BasicDocumentSubjects.Select(s => new
                        {
                            s.Id,
                            s.Position,
                            s.SubjectCanChange,
                            s.SubjectId,
                            s.SubjectTypeId
                        }).ToList()
                    }).ToList()
                })
                .SingleOrDefaultAsync();


            if (entity == null)
            {
                throw new ArgumentNullException(nameof(BasicDocument));
            }

            BasicDocumentTemplateUpdateModel model = new BasicDocumentTemplateUpdateModel
            {
                Id = entity.Id,
                BasicDocumentName = entity.Name,
                BasicDocumentParts = entity.BasicDocumentParts
                    .OrderBy(x => x.Position)
                    .Select(x => new BasicDocumentPartUpdateModel
                    {
                        Id = x.Id,
                        Name = x.Name,
                        Description = x.Description,
                        Position = x.Position,
                        IsHorariumHidden = x.IsHorariumHidden,
                        BasicClassId = x.BasicClassId,
                        Code = x.Code,
                        PrintedLines = x.PrintedLines,
                        TotalLines = x.TotalLines,
                        SubjectTypes = (x.SubjectTypesList ?? "")
                            .Split("|", StringSplitOptions.RemoveEmptyEntries)
                            .Select(int.Parse)
                            .ToList(),
                        ExternalEvaluationTypes = (x.ExternalEvaluationTypesList ?? "")
                            .Split("|", StringSplitOptions.RemoveEmptyEntries)
                            .Select(int.Parse)
                            .ToList(),
                        BasicDocumentSubjects = x.BasicDocumentSubjects.Select(s => new BasicDocumentSubjectModel
                        {
                            Id = s.Id,
                            Uid = Guid.NewGuid().ToString(),
                            Position = s.Position,
                            SubjectCanChange = s.SubjectCanChange,
                            SubjectId = s.SubjectId,
                            SubjectTypeId = s.SubjectTypeId
                        }).ToList()
                    })
                    .ToList(),
                Schema = entity.Contents.IsNullOrWhiteSpace()
                    ? new List<DynamicEntitySection>()
                    : JsonConvert.DeserializeObject<List<DynamicEntitySection>>(entity.Contents)
            };

            return model;
        }

        public async Task<string> GetSchema(int id)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDiplomaTemplatesRead)
                && !await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDiplomaTemplatesManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            return await _context.BasicDocuments
                .Where(x => x.Id == id)
                .Select(x => x.Contents)
                .SingleOrDefaultAsync();
        }

        public async Task<string> GetSchemaByTemplateId(int templateId)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDiplomaTemplatesRead)
                && !await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForDiplomaTemplatesManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            return await _context.Templates
                .Where(x => x.Id == templateId)
                .Select(x => x.BasicDocument.Contents)
                .SingleOrDefaultAsync();
        }

        public async Task<IPagedList<BasicDocumentModel>> List(DiplomaTypesListInput input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input), Messages.EmptyModelError);
            }

            IQueryable<BasicDocument> listQuery = _context.BasicDocuments
                .AsNoTracking()
                .Where(x => x.IsValid);

            if (input.HasSchema.HasValue)
            {
                if (input.HasSchema.Value)
                {
                    listQuery = listQuery.Where(x => x.Contents != null);
                }
                else
                {
                    listQuery = listQuery.Where(x => x.Contents == null);
                }
            }

            if (input.HasBarcode.HasValue)
            {
                listQuery = listQuery.Where(x => x.HasBarcode == input.HasBarcode.Value);
            }

            if (input.IsValidation.HasValue)
            {
                listQuery = listQuery.Where(x => x.IsValidation == input.IsValidation.Value);
            }

            if (input.IsAppendix.HasValue)
            {
                listQuery = listQuery.Where(x => x.IsAppendix == input.IsAppendix.Value);
            }

            if (input.IsDuplicate.HasValue)
            {
                listQuery = listQuery.Where(x => x.IsDuplicate == input.IsDuplicate.Value);
            }

            if (input.IsIncludedInRegister.HasValue)
            {
                listQuery = listQuery.Where(x => x.IsIncludedInRegister == input.IsIncludedInRegister.Value);
            }

            if (input.IsRuoDoc.HasValue)
            {
                listQuery = listQuery.Where(x => x.IsRuoDoc == input.IsRuoDoc.Value);
            }

            IQueryable<BasicDocumentModel> query = listQuery
                .Where(!input.Filter.IsNullOrWhiteSpace(),
                   predicate => predicate.Name.Contains(input.Filter)
                   || predicate.Description.Contains(input.Filter))
                .Select(x => new BasicDocumentModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    HasSchema = x.Contents != null,
                    HasBarcode = x.HasBarcode,
                    IsAppendix = x.IsAppendix,
                    IsDuplicate = x.IsDuplicate,
                    IsValidation = x.IsValidation,
                    IsIncludedInRegister = x.IsIncludedInRegister,
                    IsRuoDoc = x.IsRuoDoc,
                })
                .OrderBy(string.IsNullOrEmpty(input.SortBy) ? "Name asc" : input.SortBy);

            int totalCount = await query.CountAsync();
            IList<BasicDocumentModel> items = await query.PagedBy(input.PageIndex, input.PageSize).ToListAsync();

            return items.ToPagedList(totalCount);
        }

        public async Task SaveBasicDocumentTemplate(BasicDocumentTemplateUpdateModel model)
        {
            if (model == null) throw new ArgumentException(nameof(model), nameof(BasicDocumentTemplateUpdateModel));

            BasicDocument entity = await _context.BasicDocuments
                .Include(x => x.BasicDocumentParts).ThenInclude(x => x.BasicDocumentSubjects)
                .SingleOrDefaultAsync(x => x.Id == model.Id);

            if (entity == null) throw new ArgumentException(nameof(entity), nameof(BasicDocument));

            // Името на свойствата се сериализират в CamelCase
            var serializerSettings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            entity.Contents = model.Schema != null
                ? JsonConvert.SerializeObject(model.Schema, serializerSettings)
                : null;


            HashSet<int> existedIds = model.BasicDocumentParts != null
                ? model.BasicDocumentParts.Where(x => x.Id.HasValue).Select(x => x.Id.Value).ToHashSet()
                : new HashSet<int>();

            var toDelete = entity.BasicDocumentParts.Where(x => !existedIds.Contains(x.Id));
            if (toDelete.Any())
            {
                // Секции за изтриване
                _context.BasicDocumentParts.RemoveRange(toDelete);
            }

            if (model.BasicDocumentParts != null)
            {
                // Секции за добавяне
                var toAdd = model.BasicDocumentParts.Where(x => !x.Id.HasValue);
                if (toAdd.Any())
                {
                    _context.BasicDocumentParts.AddRange(toAdd.Select(x => new BasicDocumentPart
                    {
                        BasicDocumentId = entity.Id,
                        Position = x.Position,
                        Name = x.Name,
                        Description = x.Description,
                        IsHorariumHidden = x.IsHorariumHidden ?? false,
                        Code = x.Code,
                        BasicClassId = x.BasicClassId,
                        PrintedLines = x.PrintedLines ?? default,
                        TotalLines = x.TotalLines ?? default,
                        SubjectTypesList = x.SubjectTypes != null && x.SubjectTypes.Any()
                            ? string.Join("|", x.SubjectTypes)
                            : null,
                        ExternalEvaluationTypesList = x.ExternalEvaluationTypes != null && x.ExternalEvaluationTypes.Any()
                            ? string.Join("|", x.ExternalEvaluationTypes)
                            : null,
                        BasicDocumentSubjects = x.BasicDocumentSubjects != null && x.BasicDocumentSubjects.Any()
                            ? x.BasicDocumentSubjects.Select(s => new BasicDocumentSubject
                            {
                                SubjectId = s.SubjectId,
                                SubjectTypeId = s.SubjectTypeId,
                                Position = s.Position,
                                SubjectCanChange = s.SubjectCanChange
                            }).ToList()
                            : null
                    }));

                }
            }

            if (existedIds.Any())
            {
                // Секции за редакция
                foreach (var target in entity.BasicDocumentParts.Where(x => existedIds.Contains(x.Id)))
                {
                    BasicDocumentPartUpdateModel source = model.BasicDocumentParts.SingleOrDefault(x => x.Id.HasValue && x.Id.Value == target.Id);
                    UpdateBasicDocumentPart(source, target);
                }
            }

            await SaveAsync();
        }

        public async Task IncludeInRegister(int id)
        {
            BasicDocument basicDocument = await _context.BasicDocuments
                .SingleOrDefaultAsync(x => x.Id == id);

            if (basicDocument == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            basicDocument.IsIncludedInRegister = true;
            await SaveAsync();
        }

        public async Task ExcludeFromRegister(int id)
        {
            BasicDocument basicDocument = await _context.BasicDocuments
                .SingleOrDefaultAsync(x => x.Id == id);

            if (basicDocument == null)
            {
                throw new ApiException(Messages.EmptyEntityError);
            }

            basicDocument.IsIncludedInRegister = false;
            await SaveAsync();
        }

        public async Task<List<DropdownViewModel>> GetPrintFormDropdownOptions(string searchStr, int? basicDocumentId)
        {
            IQueryable<BasicDocumentPrintForm> query = _context.BasicDocumentPrintForms.AsQueryable();

            if (basicDocumentId.HasValue)
            {
                query = query.Where(x => x.BasicDocumentId == basicDocumentId.Value);
            }

            if (searchStr.IsNullOrWhiteSpace())
            {
                searchStr = null;
            }
            else
            {
                searchStr = searchStr.Trim();
            }

            query = query.Where(x => searchStr == null || x.Edition.ToString().Contains(searchStr));

            var items = await query.Select(x => new DropdownViewModel
            {
                Value = x.Id,
                Text = x.Edition.ToString()
            }).ToListAsync();

            return items;
        }

        public async Task<BasicDocumentSequenceViewModel> GetNextBasicDocumentSequence(int basicDocumentId, DateTime? desiredRegDate = null)
        {
            if (!await _authorizationService.AuthorizeUser(DefaultPermissions.PermissionNameForBasicDocumentSequenceManage))
            {
                throw new ApiException(Messages.UnauthorizedMessageError, 401);
            }

            int? detailedSchoolTypeId = (await _context.Institutions.FirstOrDefaultAsync(x => x.InstitutionId == _userInfo.InstitutionID))?.DetailedSchoolTypeId;
            short schoolYear = (desiredRegDate ?? DateTime.MaxValue).GetSchoolYearForSchoolTypeBySelectedRegDate(detailedSchoolTypeId);

            DateTime? minRegDate = (await _context.BasicDocumentRegDates.FirstOrDefaultAsync(d => d.BasicDocumentId == basicDocumentId && d.SchoolYear == schoolYear))?.MinRegDate;
            if ((minRegDate ?? DateTime.MinValue) > (desiredRegDate ?? DateTime.MaxValue) )
            {
                throw new ApiException(string.Format(Messages.InvalidBasicDocumentRegDate,$"{minRegDate:dd.MM.yyyy} г."), (int)HttpStatusCode.BadRequest);
            }

            var basicDocument = await _context.BasicDocuments.FirstOrDefaultAsync(i => i.Id == basicDocumentId);
            if (basicDocument == null)
            {
                throw new ArgumentNullException(nameof(BasicDocument));
            }

            var regDate = new OutputParameter<DateTime?>();
            var regNumberTotal = new OutputParameter<int?>();
            var regNumberYear = new OutputParameter<int?>();

            bool isRuo = _userInfo.IsRuo || _userInfo.IsRuoExpert;
            await _context.GetProcedures().GetNextBasicDocumentSequenceAsync(!isRuo ? _userInfo.InstitutionID : null,
                isRuo ? _userInfo.RegionID : null, basicDocumentId, desiredRegDate, regDate, regNumberTotal, regNumberYear);

            if (_userInfo.InstitutionID.HasValue)
            {
                return new BasicDocumentSequenceViewModel()
                {
                    RegDate = regDate.Value.Value,
                    RegNumberTotal = regNumberTotal.Value.Value,
                    RegNumberYear = regNumberYear.Value.Value,
                    InstitutionId = _userInfo.InstitutionID,
                    BasicDocumentId = basicDocumentId,
                    BasicDocumentName = basicDocument.Name
                };
            }
            else if(_userInfo.RegionID.HasValue)
            {
                return new BasicDocumentSequenceViewModel()
                {
                    RegDate = regDate.Value.Value,
                    RegNumberTotal = regNumberTotal.Value.Value,
                    RegNumberYear = regNumberYear.Value.Value,
                    InstitutionId = _userInfo.InstitutionID,
                    BasicDocumentId = basicDocumentId,
                    BasicDocumentName = basicDocument.Name
                };
            } else
            {
                return null;
            }
        }

    }
}
