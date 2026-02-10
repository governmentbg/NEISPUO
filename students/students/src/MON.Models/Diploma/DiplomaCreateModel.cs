
namespace MON.Models.Diploma
{
    using MON.Models.Diploma.Import;
    using MON.Models.Dropdown;
    using MON.Shared;
    using MON.Shared.Enums;
    using MON.Shared.Extensions;
    using MON.Shared.Extensions.Converters;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class DiplomaCreateModel
    {
        private readonly Dictionary<int, int> SpecialNeedGradeCodeMapper = new Dictionary<int, int>
        {
            { 1, 22 }, // Среща затруднени
            { 2, 21 }, // Справя се
            { 3, 20 }, // Постига изискванията
        };

        private readonly Dictionary<int, int> OtherGradeCodeMapper = new Dictionary<int, int>
        {
            { 1, 30 }, // Зачита се
            { 2, 31 }, // Не се зачита
            { 3, 32 }, // Освободен
            { 4, 33 }, // Интензивно
            { 5, 34 }, // Без оценка
        };

        public int? TemplateId { get; set; }

        public int? PersonId { get; set; }

        public string Contents { get; set; }

        public bool IsDiplomaFormPrinted { get; set; }

        public int BasicDocumentId { get; set; }

        public int? InstitutionId { get; set; }

        public string CommissionOrderNumber { get; set; }

        public DateTime? CommissionOrderData { get; set; }

        public List<BasicDocumentTemplatePartModel> Parts { get; set; }

        public List<DiplomaAdditionalDocumentModel> AdditionalDocuments { get; set; }

        public List<CommissionMemberModel> CommissionMembers { get; set; }

        public BasicDocumentModel BasicDocument { get; set; }

        public BasicDocumentTemplateModel BasicDocumentTemplate { get; set; }

        public List<DiplomaMessage> Messages { get; set; } = new List<DiplomaMessage>();

        private DiplomaModel DynamicContentToDiplomaModel(string contents)
        {
            if (contents.IsNullOrWhiteSpace())
            {
                throw new ArgumentNullException(nameof(contents), "DynamicContentToDiplomaModel");
            }

            return JsonConvert.DeserializeObject<DiplomaModel>(contents, new DecimalJsonConverter());
        }

        public DiplomaImportModel ToImportModel(IEnumerable<GradeDropdownViewModel> gradeNomenclature)
        {
            DiplomaModel dynamicContentModel = DynamicContentToDiplomaModel(Contents);
            if (dynamicContentModel == null)
            {
                throw new ArgumentNullException(nameof(dynamicContentModel));
            }

            DiplomaImportModel model = new DiplomaImportModel
            {
                RawData = JsonConvert.SerializeObject(this),
                ParseModel = new DiplomaImportParseModel
                {
                    Person = new DiplomaPerson
                    {
                        PersonalIdType = dynamicContentModel.PersonalIdType,
                        PersonalId = dynamicContentModel.PersonalId,
                        FirstName = dynamicContentModel.FirstName,
                        FirstNameLatin = dynamicContentModel.FirstNameLatin,
                        MiddleName = dynamicContentModel.MiddleName,
                        MiddleNameLatin = dynamicContentModel.MiddleNameLatin,
                        LastName = dynamicContentModel.LastName,
                        LastNameLatin = dynamicContentModel.LastNameLatin,
                        BirthPlaceTown = dynamicContentModel.BirthPlaceTown,
                        BirthPlaceMunicipality = dynamicContentModel.BirthPlaceMunicipality,
                        BirthPlaceRegion = dynamicContentModel.BirthPlaceRegion,
                        Nationality = dynamicContentModel.Nationality?.Code,
                        NationalityName = dynamicContentModel.Nationality?.Text
                    },
                    Institution = dynamicContentModel.Institution != null
                        ? new DiplomaInstitution
                        {
                            InstitutionCode = dynamicContentModel.Institution.Value.ToString(),
                            InstitutionName = dynamicContentModel.Institution.ClearName
                        }
                        : null,
                    Document = new DiplomaDocument
                    {
                        BasicDocumentType = BasicDocument?.Id ?? throw new ArgumentNullException(nameof(BasicDocument)),
                        BasicDocumentTypeName = BasicDocument?.Name,
                        BasicDocumentIsRuoDoc = BasicDocument?.IsRuoDoc,
                        Ministry = dynamicContentModel.Ministry != null
                            ? dynamicContentModel.Ministry.Value
                            : default,
                        MinistrySpecified = dynamicContentModel.Ministry != null,
                        MinistryName = dynamicContentModel.Ministry?.Name,
                        Series = dynamicContentModel.Series,
                        FactoryNumber = dynamicContentModel.FactoryNumber,
                        RegNumber1 = dynamicContentModel.RegistrationNumberTotal,
                        RegNumber2 = dynamicContentModel.RegistrationNumberYear,
                        RegDate = dynamicContentModel.RegistrationDate ?? throw new ArgumentNullException(nameof(dynamicContentModel.RegistrationDate)),
                        ProtocolNumber = dynamicContentModel.ProtocolNumber,
                        ProtocolDate = dynamicContentModel.ProtocolDate,
                        // На част от местата в дипломите е director, на други е principal
                        Principal = String.IsNullOrEmpty(dynamicContentModel.Director) ? dynamicContentModel.Principal : dynamicContentModel.Director,
                        Deputy = dynamicContentModel.Deputy,
                        LeadTeacher = dynamicContentModel.LeadTeacher
                    },
                    Education = new DiplomaEducation
                    {
                        SchoolYear = dynamicContentModel.SchoolYear,
                        YearGraduated = dynamicContentModel.YearGraduated ?? dynamicContentModel.SchoolYear,
                        Gpa = dynamicContentModel.Gpa,
                        GpaText = dynamicContentModel.Gpatext,
                        StateExamQualificationGradeText = dynamicContentModel.StateExamQualificationGradeText,
                        ProfessionPart = dynamicContentModel.ProfessionPart,
                        Qualification = dynamicContentModel.Qualification,
                        FLGELevel = String.IsNullOrEmpty(dynamicContentModel.FLLevel) ? dynamicContentModel.FLGELevel : dynamicContentModel.FLLevel,
                        Session = dynamicContentModel.Session,
                        Description = dynamicContentModel.Description
                    }
                }
            };

            if (dynamicContentModel.EduForm != null)
            {
                model.ParseModel.Education.EducationFormSpecified = true;
                model.ParseModel.Education.EducationForm = dynamicContentModel.EduForm.Value;
                model.ParseModel.Education.EducationFormName = dynamicContentModel.EduForm.Text;
            }
            else
            {
                model.ParseModel.Education.EducationFormSpecified = false;
            }

            if (dynamicContentModel.StateExamQualificationGrade.HasValue)
            {
                model.ParseModel.Education.StateExamQualificationGradeSpecified = true;
                model.ParseModel.Education.StateExamQualificationGrade = dynamicContentModel.StateExamQualificationGrade.Value;
            }
            else
            {
                model.ParseModel.Education.StateExamQualificationGradeSpecified = false;
            }

            if (dynamicContentModel.EduType != null)
            {
                model.ParseModel.Education.EducationTypeSpecified = true;
                model.ParseModel.Education.EducationType = (short)dynamicContentModel.EduType.Value;
            }
            else
            {
                model.ParseModel.Education.EducationTypeSpecified = false;
            }

            if (dynamicContentModel.Profile != null)
            {
                model.ParseModel.Education.ProfileSpecified = true;
                model.ParseModel.Education.Profile = dynamicContentModel.Profile.Value;
                model.ParseModel.Education.ProfileName = dynamicContentModel.Profile.Text;

            }
            else
            {
                if (dynamicContentModel.ClassType != null)
                {
                    model.ParseModel.Education.ProfileSpecified = true;
                    model.ParseModel.Education.Profile = dynamicContentModel.ClassType.Value;
                }
                else
                {
                    model.ParseModel.Education.ProfileSpecified = false;
                }
            }

            if (model.ParseModel.Education.ProfileName == null || (!String.IsNullOrWhiteSpace(dynamicContentModel.ClassTypeName)
                && model.ParseModel.Education.ProfileName.CompareTo(dynamicContentModel.ClassTypeName) != 0))
            {
                if (!String.IsNullOrWhiteSpace(dynamicContentModel.ClassTypeName))
                {
                    model.ParseModel.Education.ProfileName = dynamicContentModel.ClassTypeName;
                }
            }

            if (dynamicContentModel.SPPOOProfession != null)
            {
                model.ParseModel.Education.ProfessionSpecified = true;
                model.ParseModel.Education.Profession = dynamicContentModel.SPPOOProfession.Value;
                model.ParseModel.Education.ProfessionName = dynamicContentModel.SPPOOProfession.Name;
            }
            else
            {
                model.ParseModel.Education.ProfessionSpecified = false;
            }

            if (!String.IsNullOrWhiteSpace(dynamicContentModel.SPPOOProfessionName)
                && (model.ParseModel.Education.ProfessionName == null || model.ParseModel.Education.ProfessionName.CompareTo(dynamicContentModel.SPPOOProfessionName) != 0))
            {
                model.ParseModel.Education.ProfessionName = dynamicContentModel.SPPOOProfessionName;
            }


            if (dynamicContentModel.SPPOOSpeciality != null)
            {
                model.ParseModel.Education.SpecialitySpecified = true;
                model.ParseModel.Education.Speciality = dynamicContentModel.SPPOOSpeciality.Value;
                model.ParseModel.Education.SpecialityName = dynamicContentModel.SPPOOSpeciality.Name;
            }
            else
            {
                model.ParseModel.Education.SpecialitySpecified = false;
            }

            if (!String.IsNullOrWhiteSpace(dynamicContentModel.SPPOOSpecialityName)
                && (model.ParseModel.Education.SpecialityName == null || model.ParseModel.Education.SpecialityName.CompareTo(dynamicContentModel.SPPOOSpecialityName) != 0))
            {
                model.ParseModel.Education.SpecialityName = dynamicContentModel.SPPOOSpecialityName;
            }

            if (dynamicContentModel.VetLevel.HasValue)
            {
                model.ParseModel.Education.VetLevelSpecified = true;
                model.ParseModel.Education.VetLevel = dynamicContentModel.VetLevel.Value;
            }
            else
            {
                model.ParseModel.Education.VetLevelSpecified = false;
            }

            if (dynamicContentModel.EduDuration.HasValue)
            {
                model.ParseModel.Education.DurationSpecified = true;
                model.ParseModel.Education.Duration = dynamicContentModel.EduDuration.Value;
            }
            else
            {
                model.ParseModel.Education.DurationSpecified = false;
            }

            if (dynamicContentModel.ITLevel.HasValue)
            {
                model.ParseModel.Education.ITLevelSpecified = true;
                model.ParseModel.Education.ITLevel = dynamicContentModel.ITLevel.Value;
            }
            else
            {
                model.ParseModel.Education.ITLevelSpecified = false;
            }

            if (dynamicContentModel.BasicClass.HasValue)
            {
                model.ParseModel.Education.BasicClassSpecified = true;
                model.ParseModel.Education.BasicClass = dynamicContentModel.BasicClass.Value;
            }
            else
            {
                model.ParseModel.Education.BasicClassSpecified = false;
            }

            if (dynamicContentModel.NKR.HasValue)
            {
                model.ParseModel.Education.NKRSpecified = true;
                model.ParseModel.Education.NKR = dynamicContentModel.NKR.Value;
            }
            else
            {
                model.ParseModel.Education.NKRSpecified = false;
            }

            if (dynamicContentModel.EKR.HasValue)
            {
                model.ParseModel.Education.EKRSpecified = true;
                model.ParseModel.Education.EKR = dynamicContentModel.EKR.Value;
            }
            else
            {
                model.ParseModel.Education.EKRSpecified = false;
            }

            if (!AdditionalDocuments.IsNullOrEmpty())
            {
                model.ParseModel.AdditionalDocuments = AdditionalDocuments
                    .Select(x => new DiplomaAdditionalDocument
                    {
                        BasicDocumentType = x.BasicDocumentId,
                        BasicDocumentTypeSpecified = x.BasicDocumentId.HasValue,
                        MainDiploma = x.MainDiplomaId,
                        MainDiplomaSpecified = x.MainDiplomaId.HasValue,
                        Institution = new DiplomaAdditionalDocumentInstitution
                        {
                            InstitutionCode = x.InstitutionId.HasValue ? x.InstitutionId.ToString() : null,
                            InstitutionName = x.InstitutionName,
                            InstitutionAddress = x.InstitutionAddress,
                            Town = x.Town,
                            Municipality = x.Municipality,
                            Region = x.Region,
                            LocalArea = x.LocalArea
                        },
                        Series = x.Series,
                        FactoryNumber = x.FactoryNumber,
                        RegNumber1 = x.RegistrationNumber,
                        RegNumber2 = x.RegistrationNumberYear,
                        RegDate = x.RegistrationDate ?? default,
                    })
                    .ToArray();
            }

            if (CommissionOrderNumber != null || CommissionOrderData != null || !CommissionMembers.IsNullOrEmpty())
            {
                // Ако имаме нещо попълнено, то е задължително да направим секцията Commission
                model.ParseModel.Commission = new DiplomaCommission();
                model.ParseModel.Commission.OrderNo = CommissionOrderNumber;

                if (CommissionOrderData.HasValue)
                {
                    model.ParseModel.Commission.OrderDateSpecified = true;
                    model.ParseModel.Commission.OrderDate = CommissionOrderData.Value;
                }
                else
                {
                    model.ParseModel.Commission.OrderDateSpecified = false;
                }

                if (!CommissionMembers.IsNullOrEmpty())
                {
                    model.ParseModel.Commission.Member = CommissionMembers
                            .Select(x => new DiplomaCommissionMember
                            {
                                Position = x.Position,
                                Name = x.FullName,
                                NameLatin = x.FullNameLatin
                            })
                            .ToArray();
                }
            }

            if (!Parts.IsNullOrEmpty())
            {
                List<SubjectType> list = new List<SubjectType>();
                foreach (var part in Parts.Where(x => !x.Subjects.IsNullOrEmpty()))
                {
                    foreach (BasicDocumentTemplateSubjectModel subjectModel in part.Subjects)
                    {
                        SubjectType subject = ToSubject(subjectModel, part.Id, part.BasicClassId, gradeNomenclature);

                        if (!subjectModel.Modules.IsNullOrEmpty())
                        {
                            subject.Modules = subjectModel.Modules.Select(m => ToSubject(m, part.Id, part.BasicClassId, gradeNomenclature)).ToArray();
                        }

                        list.Add(subject);
                    }
                }

                model.ParseModel.Subjects = list.ToArray();
            }

            return model;
        }

        private SubjectType ToSubject(BasicDocumentTemplateSubjectModel subjectModel, int? basicDocumentPartId, int? basicClassId, IEnumerable<GradeDropdownViewModel> gradeNomenclature)
        {
            if (subjectModel == null)
            {
                throw new ArgumentNullException(nameof(subjectModel));
            }

            SubjectType subject = new SubjectType
            {
                PositionSpecified = true,
                Position = subjectModel.Position,
                SubjectName = subjectModel.SubjectName,
                SubjectType1 = subjectModel.SubjectTypeId,
                FlLevel = subjectModel.FlLevel,
                ECTS = subjectModel.Ects // видим за 3-44, 3-44.1, 3-44.2, 3-44.3
            };

            if (subjectModel.SubjectId == default)
            {
                // Ако SubjectId == 0 слага null. В противен случай гърми FK-то. 
                subject.SubjectIdSpecified = false;
                subject.SubjectId = null;
            }
            else
            {
                subject.SubjectIdSpecified = true;
                subject.SubjectId = subjectModel.SubjectId;
            }

            if (basicDocumentPartId.HasValue)
            {
                subject.BasicDocumentPartSpecified = true;
                subject.BasicDocumentPart = basicDocumentPartId.Value;
            }
            else
            {
                subject.BasicDocumentPartSpecified = false;
            }

            if (basicClassId.HasValue)
            {
                subject.BasicClassSpecified = true;
                subject.BasicClass = basicClassId.Value;
            }
            else if (subjectModel.BasicClassId.HasValue)
            {
                subject.BasicClassSpecified = true;
                subject.BasicClass = subjectModel.BasicClassId.Value;
            }
            else
            {
                subject.BasicClassSpecified = false;
            }

            if (subjectModel.Horarium.HasValue)
            {
                subject.HorariumSpecified = true;
                subject.Horarium = subjectModel.Horarium.Value;
            }
            else
            {
                subject.HorariumSpecified = false;
            }

            if (subjectModel.NvoPoints.HasValue)
            {
                subject.PointsSpecified = true;
                subject.Points = subjectModel.NvoPoints.Value;
            }
            else
            {
                subject.PointsSpecified = false;
            }

            switch ((GradeCategoryEnum)(subjectModel.GradeCategory ?? 1))
            {
                case GradeCategoryEnum.Normal:
                    if (subjectModel.Grade.HasValue)
                    {
                        subject.Item = new GradeType
                        {
                            Grade = subjectModel.Grade.Value,
                            GradeText = GradeUtils.GetDecimalGradeText(subjectModel.Grade.Value)
                        };
                    }
                    break;
                case GradeCategoryEnum.SpecialNeeds:
                    int specialNeedGradeId = subjectModel.SpecialNeedsGrade ?? 20;
                    if (specialNeedGradeId < 20 && SpecialNeedGradeCodeMapper.ContainsKey(specialNeedGradeId))
                    {
                        specialNeedGradeId = SpecialNeedGradeCodeMapper[specialNeedGradeId];
                    }

                    subject.Item = new SpecialNeedsGradeType
                    {
                        Grade = (short)specialNeedGradeId,
                        GradeText = gradeNomenclature.FirstOrDefault(x => x.Value == specialNeedGradeId)?.Text

                    };
                    break;
                case GradeCategoryEnum.Other:
                    int otherGradeId = subjectModel.OtherGrade ?? 30;
                    if (otherGradeId < 30 && OtherGradeCodeMapper.ContainsKey(otherGradeId))
                    {
                        otherGradeId = OtherGradeCodeMapper[otherGradeId];
                    }

                    subject.Item = new OtherGradeType
                    {
                        Grade = (short)otherGradeId,
                        GradeText = gradeNomenclature.FirstOrDefault(x => x.Value == otherGradeId)?.Text
                    };
                    break;
                case GradeCategoryEnum.Qualitative:
                    if (subjectModel.QualitativeGrade.HasValue)
                    {
                        subject.Item = new QualitativeGradeType
                        {
                            Grade = (short)subjectModel.QualitativeGrade.Value,
                            GradeText = gradeNomenclature.FirstOrDefault(x => x.Value == subjectModel.QualitativeGrade.Value)?.Text
                        };
                    }
                    break;
                case GradeCategoryEnum.SubSection:
                    subject.Item = new NoGradeType
                    {
                        Information = ""
                    };
                    break;
                default:
                    break;
            }


            if (subjectModel.ShowFlSubject && subjectModel.FlSubjectId.HasValue)
            {
                subject.FlSubjectIdSpecified = true;
                subject.FlSubjectId = subjectModel.FlSubjectId.Value;
                subject.FlSubjectName = subjectModel.FlSubjectName;
            }
            else
            {
                subject.FlSubjectIdSpecified = false;
            }

            if (subjectModel.ShowFlSubject && subjectModel.FlHorarium.HasValue)
            {
                subject.FlHorariumSpecified = true;
                subject.FlHorarium = subjectModel.FlHorarium.Value;
            }
            else
            {
                subject.FlHorariumSpecified = false;
            }

            return subject;
        }
    }

    public class DiplomaMessage
    {
        public string Message { get; set; }
    }
}