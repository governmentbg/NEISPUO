using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace Domain
{
    public class WordTemplateConfig
    {
        public static readonly WordTemplateConfig PersonalFile = new WordTemplateConfig(
            "PersonalFile",
            "Personal_File.docx",
            JsonSerializer.Serialize(
                new
                {
                    SchoolName = "Алеко Константинов"
                }
            ));

        public static readonly WordTemplateConfig Lod = new WordTemplateConfig(
            "Lod",
            "ЛОД.docx",
            JsonSerializer.Serialize(
                new
                {
                    SchoolName = "Алеко Константинов"
                }
            ));

        public static readonly WordTemplateConfig ApplicationFile = new WordTemplateConfig(
            "ApplicationFile",
            "Application_Diplomas.docx",
            JsonSerializer.Serialize(
                new
                {
                    InstitutionName = "Алеко Константинов"
                }
            ));

        public static readonly WordTemplateConfig ApplicationDuplicatesFile = new WordTemplateConfig(
            "ApplicationDuplicatesFile",
            "Application_Duplicates.docx",
            JsonSerializer.Serialize(
                new
                {
                    InstitutionName = "Алеко Константинов"
                }
            ));

        public static readonly WordTemplateConfig ExamDutyProtocol = new WordTemplateConfig(
            "ExamDutyProtocol",
            "3-82_exam_duty_protocol.docx",
            JsonSerializer.Serialize(
                new
                {
                    SchoolYear = "2018/2019",
                    InstitutionName = "Алеко Константинов",
                    InstitutionTownName = "Пловдив",
                    InstitutionMunicipalityName = "Пловдив",
                    InstitutionLocalAreaName = "Пловдив",
                    InstitutionRegionName = "Пловдив",
                    SessionType = "майска",
                    SubjectName = "Математика ЗП",
                    EduFormName = "дневна",
                    ExamType = "писмен изпит",
                    ExamTypeUppercase = "ПИСМЕН ИЗПИТ",
                    ExamSubType = "поправителен",
                    OrderNumber = "123а",
                    OrderDate = "05.06.2019",
                    Date = "05.06.2019",
                    GroupNum = "2a",
                    ClassNames = "8a, 8b",
                    Supervisors = new object[]
                    {
                        new
                        {
                            SupervisorName = "Петър Петров Петров",
                        }
                    },
                    Students = new object[]
                    {
                        new
                        {
                            StudentName = "Радка Видева Пенчева",
                            ClassName = "8А"
                        }
                    },
                },
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                }));

        public static readonly WordTemplateConfig ExamResultProtocol = new WordTemplateConfig(
            "ExamResultProtocol",
            "3-80_exam_result_protocol.docx",
            JsonSerializer.Serialize(
                new
                {
                    SchoolYear = "2018/2019",
                    InstitutionName = "Алеко Константинов",
                    InstitutionTownName = "Пловдив",
                    InstitutionMunicipalityName = "Пловдив",
                    InstitutionLocalAreaName = "Пловдив",
                    InstitutionRegionName = "Пловдив",
                    SessionType = "майска",
                    SubjectName = "Математика ЗП",
                    EduFormName = "дневна",
                    ProtocolType = "писмен",
                    ExamType = "поправителен",
                    ProtocolNumber = "123а",
                    ProtocolDate = "05.06.2019",
                    Date = "05.06.2019",
                    GroupNum = "2a",
                    ClassNames = "8a, 8b",
                    CommissionNominationOrderNum = "АА-26",
                    CommissionNominationOrderDate = "08.06.2019",
                    ChairmanName = "Петър Петров Петров",
                    CommissionMembers = new object[]
                    {
                        new
                        {
                            OrderNum = 1,
                            CommissionerName = "Петър Петров Петров",
                        }
                    },
                    CommissionMembersDivided = new object[]
                    {
                        new
                        {
                            CommissionerNameLeft = "Петър Петров Петров",
                            CommissionerNameRight = "Петър Петров Петров",
                        }
                    },
                    Students = new object[]
                    {
                        new
                        {
                            StudentName = "Радка Видева Пенчева",
                            ClassName = "8А"
                        }
                    },
                },
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                }));

        public static readonly WordTemplateConfig QualificationExamResultProtocol = new WordTemplateConfig(
            "QualificationExamResultProtocol",
            "3-80_qual_exam_result_protocol.docx",
            JsonSerializer.Serialize(
                new
                {
                    SchoolYear = "2018/2019",
                    InstitutionName = "Алеко Константинов",
                    InstitutionTownName = "Пловдив",
                    InstitutionMunicipalityName = "Пловдив",
                    InstitutionLocalAreaName = "Пловдив",
                    InstitutionRegionName = "Пловдив",
                    SessionType = "майска",
                    Profession = "лаборант",
                    Speciality = "молекулярна биология",
                    QualificationDegree = "първа",
                    EduFormName = "дневна",
                    ProtocolType = "писмен",
                    ExamType = "държавен изпит за придобиване на степен на ПК - част по теория на професията",
                    ProtocolNumber = "123а",
                    ProtocolDate = "05.06.2019",
                    Date = "05.06.2019",
                    GroupNum = "2a",
                    ClassNames = "8a, 8b",
                    CommissionNominationOrderNum = "АА-26",
                    CommissionNominationOrderDate = "08.06.2019",
                    ChairmanName = "Петър Петров Петров",
                    CommissionMembers = new object[]
                    {
                        new
                        {
                            OrderNum = 1,
                            CommissionerName = "Петър Петров Петров",
                        }
                    },
                    CommissionMembersDivided = new object[]
                    {
                        new
                        {
                            CommissionerNameLeft = "Петър Петров Петров",
                            CommissionerNameRight = "Петър Петров Петров",
                        }
                    },
                    Students = new object[]
                    {
                        new
                        {
                            StudentName = "Радка Видева Пенчева",
                            ClassName = "8А"
                        }
                    },
                },
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                }));

        public static readonly WordTemplateConfig QualificationAcquisitionProtocol = new WordTemplateConfig(
            "QualificationAcquisitionProtocol",
            "3-80_qual_acquis_protocol.docx",
            JsonSerializer.Serialize(
                new
                {
                    SchoolYear = "2018/2019",
                    InstitutionName = "Алеко Константинов",
                    InstitutionTownName = "Пловдив",
                    InstitutionMunicipalityName = "Пловдив",
                    InstitutionLocalAreaName = "Пловдив",
                    InstitutionRegionName = "Пловдив",
                    Profession = "лаборант",
                    Speciality = "молекулярна биология",
                    QualificationDegree = "първа",
                    ProtocolNumber = "123а",
                    ProtocolDate = "05.06.2019",
                    Date = "05.06.2019",
                    CommissionNominationOrderNum = "АА-26",
                    CommissionNominationOrderDate = "08.06.2019",
                    DirectorName = "Петър Петров Петров",
                    DirectorNameInParentheses = "Петър Петров",
                    ChairmanName = "Петър Петров Петров",
                    CommissionMembers = new object[]
                    {
                        new
                        {
                            OrderNum = 1,
                            CommissionerName = "Петър Петров Петров",
                        }
                    },
                    CommissionMembersDivided = new object[]
                    {
                        new
                        {
                            CommissionerNameLeft = "Петър Петров Петров",
                            CommissionerNameRight = "Петър Петров Петров",
                        }
                    },
                    PassedExamStudents = new object[]
                    {
                        new
                        {
                            ClassName = "XIIa",
                            StudentName = "Петър Петров Петров",
                            ExamsPassed = "ДА",
                            TheoryDecimalGradeText = "Много Добър",
                            TheoryDecimalGrade = "5.24",
                            PracticeDecimalGradeText = "Много Добър",
                            PracticeDecimalGrade = "5.49",
                            AverageDecimalGradeText = "Много Добър",
                            AverageDecimalGrade = "5.37",
                        }
                    },
                    FailedExamStudents = new object[]
                    {
                        new
                        {
                            ClassName = "XIIa",
                            StudentName = "Петър Петров Петров",
                            ExamsPassed = "ДА",
                            TheoryDecimalGradeText = "Много Добър",
                            TheoryDecimalGrade = "5.24",
                            PracticeDecimalGradeText = "Много Добър",
                            PracticeDecimalGrade = "5.49",
                        }
                    },
                },
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                }));

        public static readonly WordTemplateConfig QualificationAcquisitionExamGradesProtocol = new WordTemplateConfig(
            "QualificationAcquisitionExamGradesProtocol",
            "3-80_qual_acquis_exam_grades_protocol.docx",
            JsonSerializer.Serialize(
                new
                {
                    SchoolYear = "2018/2019",
                    InstitutionName = "Алеко Константинов",
                    InstitutionTownName = "Пловдив",
                    InstitutionMunicipalityName = "Пловдив",
                    InstitutionLocalAreaName = "Пловдив",
                    InstitutionRegionName = "Пловдив",
                    ProtocolType = "ЗА ОЦЕНКИТЕ ОТ ИЗПИТ ЗА ПРИДОБИВАНЕ НА ПРОФЕСИОНАЛНА КВАЛИФИКАЦИЯ ПО ЧАСТ ОТ ПРОФЕСИЯ",
                    PassedExamResultTableText = "държавен изпит за придобиване на степен на професионална квалификация",
                    FailedExamResultTableText = "държавен/държавни изпит/и за придобиване на професионална квалификация",
                    Profession = "лаборант",
                    Speciality = "молекулярна биология",
                    QualificationDegree = "първа",
                    ProtocolNumber = "123а",
                    ProtocolDate = "05.06.2019",
                    Date = "05.06.2019",
                    CommissionNominationOrderNum = "АА-26",
                    CommissionNominationOrderDate = "08.06.2019",
                    DirectorName = "Петър Петров Петров",
                    DirectorNameInParentheses = "Петър Петров",
                    ChairmanName = "Петър Петров Петров",
                    CommissionMembers = new object[]
                    {
                        new
                        {
                            OrderNum = 1,
                            CommissionerName = "Петър Петров Петров",
                        }
                    },
                    CommissionMembersDivided = new object[]
                    {
                        new
                        {
                            CommissionerNameLeft = "Петър Петров Петров",
                            CommissionerNameRight = "Петър Петров Петров",
                        }
                    },
                    PassedExamStudents = new object[]
                    {
                        new
                        {
                            ClassName = "XIIa",
                            StudentName = "Петър Петров Петров",
                            ExamsPassed = "ДА",
                            TheoryPoints = "68",
                            PracticePoints = "77",
                        }
                    },
                    FailedExamStudents = new object[]
                    {
                        new
                        {
                            ClassName = "XIIa",
                            StudentName = "Петър Петров Петров",
                            ExamsPassed = "ДА",
                            TheoryPoints = "78",
                            PracticePoints = "17",
                        }
                    },
                },
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                }));

        public static readonly WordTemplateConfig SkillsCheckExamResultProtocol = new WordTemplateConfig(
            "SkillsCheckExamResultProtocol",
            "3-80_skills_check_exam_result_protocol.docx",
            JsonSerializer.Serialize(
                new
                {
                    SchoolYear = "2020/2021",
                    InstitutionName = "Алеко Константинов",
                    InstitutionTownName = "Пловдив",
                    InstitutionMunicipalityName = "Пловдив",
                    InstitutionLocalAreaName = "Пловдив",
                    InstitutionRegionName = "Пловдив",
                    SubjectName = "Математика ЗП",
                    Date = "05.06.2019",
                    FirstEvaluatorName = "Петър Петров Петров",
                    SecondEvaluatorName = "Петър Петров Петров",
                    Students = new object[]
                    {
                        new
                        {
                            OrderNum = 1
                        }
                    },
                    Evaluators = new object[]
                    {
                        new
                        {
                            Name = "Петър Петров Петров"
                        }
                    }
                },
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                }));

        public static readonly WordTemplateConfig StateExamsAdmProtocol = new WordTemplateConfig(
            "StateExamsAdmProtocol",
            "3-79_state_exams_adm_protocol.docx",
            JsonSerializer.Serialize(
                new
                {
                    InstName = "Алеко Константинов",
                    InstAddress = "град София, община Столична, район Овча Купел, област София-град",
                    SchoolYear = "2018/2019",
                    ExamSession = "майска",
                    ProtocolNum = "АА-26",
                    ProtocolDate = "05.06.2019",
                    CommissionMeetingDate = "08.06.2019",
                    CommissionNominationOrderNum = "АА-26",
                    CommissionNominationOrderDate = "08.06.2019",
                    DirectorName = "Петър Петров Петров",
                    DirectorNameInParentheses = "Петър Петров",
                    ChairmanName = "Петър Петров Петров",
                    CommissionMembers = new object[]
                    {
                        new
                        {
                            OrderNum = 1,
                            CommissionerName = "Петър Петров Петров",
                        }
                    },
                    CommissionMembersDivided = new object[]
                    {
                        new
                        {
                            CommissionerNameLeft = "Петър Петров Петров",
                            CommissionerNameRight = "Петър Петров Петров",
                        }
                    },
                    Students = new object[]
                    {
                        new
                        {
                            ClassName = "XIIa",
                            StudentName = "Петър Петров Петров",
                            HasFirstMandatorySubject = "ДА",
                            SecondMandatorySubject = "Английски език",
                            AdditionalStateExams = "История, Химия",
                            QualificationStateExams = "Математика, Биология",
                        }
                    }
                },
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                }));

        public static readonly WordTemplateConfig StateExamDutyProtocol = new WordTemplateConfig(
            "StateExamDutyProtocol",
            "3-82_state_exam_duty_protocol.docx",
            JsonSerializer.Serialize(
                new
                {
                    SchoolYear = "2018/2019",
                    InstitutionName = "Алеко Константинов",
                    InstitutionTownName = "Пловдив",
                    InstitutionMunicipalityName = "Пловдив",
                    InstitutionLocalAreaName = "Пловдив",
                    InstitutionRegionName = "Пловдив",
                    SessionType = "майска",
                    SubjectName = "Математика ЗП",
                    EduFormName = "дневна",
                    OrderNumber = "123а",
                    OrderDate = "05.06.2019",
                    Date = "05.06.2019",
                    ModulesCount = 3,
                    RoomNumber = "2a",
                    Supervisors = new object[]
                    {
                        new
                        {
                            SupervisorName = "Петър Петров Петров",
                        }
                    }
                },
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                }));

        public static readonly WordTemplateConfig SkillsCheckExamDutyProtocol = new WordTemplateConfig(
            "SkillsCheckExamDutyProtocol",
            "3-82_skills_check_exam_duty_protocol.docx",
            JsonSerializer.Serialize(
                new
                {
                    SchoolYear = "2020/2021",
                    InstitutionName = "Алеко Константинов",
                    InstitutionTownName = "Пловдив",
                    InstitutionMunicipalityName = "Пловдив",
                    InstitutionLocalAreaName = "Пловдив",
                    InstitutionRegionName = "Пловдив",
                    SubjectName = "Математика ЗП",
                    Date = "05.06.2019",
                    DirectorName = "Петър Петров Петров",
                    Students = new object[]
                    {
                        new
                        {
                            OrderNum = 1
                        }
                    },
                    Supervisors = new object[]
                    {
                        new
                        {
                            SupervisorName = "Петър Петров Петров",
                        }
                    }
                },
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                }));

        public static readonly WordTemplateConfig NvoExamDutyProtocol = new WordTemplateConfig(
            "NvoExamDutyProtocol",
            "3-82_NVO_exam_duty_protocol.docx",
            JsonSerializer.Serialize(
                new
                {
                    SchoolYear = "2020/2021",
                    InstitutionName = "Алеко Константинов",
                    InstitutionTownName = "Пловдив",
                    InstitutionMunicipalityName = "Пловдив",
                    InstitutionLocalAreaName = "Пловдив",
                    InstitutionRegionName = "Пловдив",
                    BasicClassName = "8",
                    SubjectName = "Математика ЗП",
                    Date = "05.06.2019",
                    RoomNumber = "302A",
                    DirectorName = "Петър Петров Петров",
                    Students = new object[]
                    {
                        new
                        {
                            StudentName = "Радка Видева Пенчева",
                            ClassName = "8А"
                        }
                    },
                    Supervisors = new object[]
                    {
                        new
                        {
                            SupervisorName = "Петър Петров Петров",
                        }
                    }
                },
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                }));

        public static readonly WordTemplateConfig GradeChangeExamsAdmProtocol = new WordTemplateConfig(
            "GradeChangeExamsAdmProtocol",
            "3-79А_grade_change_exams_adm_protocol.docx",
            JsonSerializer.Serialize(
                new
                {
                    InstName = "Алеко Константинов",
                    InstAddress = "град София, община Столична, район Овча Купел, област София-град",
                    SchoolYear = "2018/2019",
                    ExamSession = "майска",
                    ProtocolNum = "АА-26",
                    ProtocolDate = "05.06.2019",
                    CommissionMeetingDate = "08.06.2019",
                    CommissionNominationOrderNumber = "АА-26",
                    CommissionNominationOrderDate = "08.06.2019",
                    DirectorName = "Петър Петров Петров",
                    DirectorNameInParentheses = "Петър Петров",
                    ChairmanName = "Петър Петров Петров",
                    CommissionMembers = new object[]
                    {
                        new
                        {
                            CommissionerName = "Петър Петров Петров",
                        }
                    },
                    CommissionMembersDivided = new object[]
                    {
                        new
                        {
                            CommissionerNameLeft = "Петър Петров Петров",
                            CommissionerNameRight = "Петър Петров Петров",
                        }
                    },
                    Students = new object[]
                    {
                        new
                        {
                            ClassName = "XIIa",
                            StudentName = "Петър Петров Петров",
                            Subjects = "История, Химия",
                        }
                    }
                },
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                }));

        public static readonly WordTemplateConfig HighSchoolCertificateProtocol = new WordTemplateConfig(
            "HighSchoolCertificateProtocol",
            "3-84_high_school_certificate_protocol.docx",
            JsonSerializer.Serialize(
                new
                {
                    InstName = "Алеко Константинов",
                    InstAddress = "град София, община Столична, район Овча Купел, област София-град",
                    SchoolYear = "2018/2019",
                    ExamSession = "майска",
                    Stage = "втори гимназиален",
                    StageUppercase = "ВТОРИ ГИМНАЗИАЛЕН",
                    ProtocolNum = "АА-26",
                    ProtocolDate = "05.06.2019",
                    CommissionMeetingDate = "08.06.2019",
                    CommissionNominationOrderNum = "АА-26",
                    CommissionNominationOrderDate = "08.06.2019",
                    DirectorName = "Киро Пенев Петров",
                    DirectorNameInParentheses = "Петър Петров",
                    ChairmanName = "Петър Петров Петров",
                    CommissionMembers = new object[]
                    {
                        new
                        {
                            OrderNum = 1,
                            CommissionerName = "Петър Петров Петров",
                        }
                    },
                    CommissionMembersDivided = new object[]
                    {
                        new
                        {
                            CommissionerNameLeft = "Петър Петров Петров",
                            CommissionerNameRight = "Иван Петров Петров",
                        }
                    },
                    Students = new object[]
                    {
                        new
                        {
                            ClassName = "XIIa",
                            StudentName = "Петър Петров Петров"
                        }
                    }
                },
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                }));

        public static readonly WordTemplateConfig GraduationThesisDefenseProtocol = new WordTemplateConfig(
            "GraduationThesisDefenseProtocol",
            "3-81d_graduation-thesis-defense_protocol.docx",
            JsonSerializer.Serialize(
                new
                {
                    SchoolYear = "2020/2021",
                    InstitutionName = "Алеко Константинов",
                    InstitutionTownName = "Пловдив",
                    InstitutionMunicipalityName = "Пловдив",
                    InstitutionLocalAreaName = "Пловдив",
                    InstitutionRegionName = "Пловдив",
                    ProtocolNumber = "АА-26",
                    ProtocolDate = "05.06.2019",
                    ExamSession = "майска",
                    EduFormName = "дневна",
                    CommissionMeetingDate = "08.06.2019",
                    DirectorOrderNumber = "АА-26",
                    DirectorOrderDate = "08.06.2019",
                    DirectorName = "Киро Пенев Петров",
                    DirectorNameInParentheses = "Петър Петров",
                    ChairmanName = "Петър Петров Петров",
                    CommissionMembers = new object[]
                    {
                        new
                        {
                            OrderNum = 1,
                            CommissionerName = "Петър Петров Петров",
                        }
                    },
                    CommissionMembersDivided = new object[]
                    {
                        new
                        {
                            CommissionerNameLeft = "Петър Петров Петров",
                            CommissionerNameRight = "Иван Петров Петров",
                        }
                    },
                    Section1Students = new object[]
                    {
                        new
                        {
                            OrderNum = 1
                        }
                    },
                    Section2Students = new object[]
                    {
                        new
                        {
                            OrderNum = 1
                        }
                    },
                    Section3Students = new object[]
                    {
                        new
                        {
                            OrderNum = 1
                        }
                    },
                    Section4Students = new object[]
                    {
                        new
                        {
                            OrderNum = 1
                        }
                    }
                },
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                }));

        public static readonly WordTemplateConfig Institution_ZUD_Request = new WordTemplateConfig(
            "Institution_ZUD_Request",
            "Заявка за ЗУД_1.docx",
            JsonSerializer.Serialize(
                new
                {
                    SchoolYearName = "2020/2021",
                    InstitutionDirectorName = "Киро Пенев Петров",
                    InstitutionName = "Алеко Константинов",
                    InstitutionPhoneNumber = "00359111111",
                    InstitutionEmail = "email@email.bg",
                    InstitutionAddress = "София",
                    ИnstitutionBulstat = "5555555",
                }
            ));

        public static readonly WordTemplateConfig All_ZUD_Request = new WordTemplateConfig(
            "All_ZUD_Request",
            "Заявка за ЗУД_Всички.docx",
            JsonSerializer.Serialize(
                new
                {
                    SchoolYearName = "2020/2021",
                    InstitutionDirectorName = "Киро Пенев Петров",
                    InstitutionName = "Алеко Константинов",
                    InstitutionPhoneNumber = "00359111111",
                    InstitutionEmail = "email@email.bg",
                    InstitutionAddress = "София",
                    ИnstitutionBulstat = "5555555",
                }
            ));

        public static readonly WordTemplateConfig MON_ZUD_Request = new WordTemplateConfig(
            "MON_ZUD_Request",
            "Заявка за ЗУД_Всички.docx",
            JsonSerializer.Serialize(
                new
                {
                    SchoolYearName = "2020/2021",
                    InstitutionDirectorName = "Киро Пенев Петров",
                    InstitutionName = "Алеко Константинов",
                    InstitutionPhoneNumber = "00359111111",
                    InstitutionEmail = "email@email.bg",
                    InstitutionAddress = "София",
                    ИnstitutionBulstat = "5555555",
                }
            ));

        public static readonly WordTemplateConfig ROU_ZUD_Request = new WordTemplateConfig(
            "ROU_ZUD_Request",
            "Заявка за ЗУД_РУО.docx",
            JsonSerializer.Serialize(
                new
                {
                    SchoolYearName = "2020/2021",
                    InstitutionDirectorName = "Киро Пенев Петров",
                    InstitutionName = "Алеко Константинов",
                    InstitutionPhoneNumber = "00359111111",
                    InstitutionEmail = "email@email.bg",
                    InstitutionAddress = "София",
                    ИnstitutionBulstat = "5555555",
                }
            ));

        public static readonly WordTemplateConfig Protocol_ZUD_Request = new WordTemplateConfig(
            "Protocol_ZUD_Request",
            "ППП за ЗУД.docx",
            JsonSerializer.Serialize(
                new
                {
                    Today = $"{DateTime.Now: dd.MM.yyyy} г.",
                    InstitutionDirectorName = "Киро Пенев Петров",
                    InstitutionName = "Алеко Константинов",
                    RequestedInstitutionName = "Алеко Константинов",
                    RequestedInstitutionTown = "София",
                    RequestedInstitutionMunicipality = "Столична",
                    RequestedInstitutionRegion = "Софийска",
                    RequestedInstitutionDirectorName = "Киро Пенев Петров",
                }
            ));

        public static readonly WordTemplateConfig Report_ZUD = new WordTemplateConfig(
            "Report_ZUD",
            "Протокол за ЗУД.docx",
            JsonSerializer.Serialize(
                new
                {
                    Today = $"{DateTime.Now: dd.MM.yyyy} г.",
                    InstitutionDirectorName = "Киро Пенев Петров",
                    InstitutionName = "Алеко Константинов",
                    RequestedInstitutionName = "Алеко Константинов",
                    RequestedInstitutionTown = "София",
                    RequestedInstitutionMunicipality = "Столична",
                    RequestedInstitutionRegion = "Софийска",
                    RequestedInstitutionDirectorName = "Киро Пенев Петров",
                }
            ));

        public static readonly WordTemplateConfig Destruction_Protocol_ZUD = new WordTemplateConfig(
           "Destruction_Protocol_ZUD",
           "Протокол за унищожаване на ЗУД.docx",
           JsonSerializer.Serialize(
               new
               {
                   Today = $"{DateTime.Now: dd.MM.yyyy} г.",
                   InstitutionDirectorName = "Киро Пенев Петров",
                   InstitutionName = "Алеко Константинов",
                   RequestedInstitutionName = "Алеко Константинов",
                   RequestedInstitutionTown = "София",
                   RequestedInstitutionMunicipality = "Столична",
                   RequestedInstitutionRegion = "Софийска",
                   RequestedInstitutionDirectorName = "Киро Пенев Петров",
               }
           ));

        public static readonly IReadOnlyDictionary<string, WordTemplateConfig> AllTemplates =
            new ReadOnlyDictionary<string, WordTemplateConfig>(
                new Dictionary<string, WordTemplateConfig>(StringComparer.InvariantCultureIgnoreCase)
                {
                    { ApplicationDuplicatesFile.TemplateName, ApplicationDuplicatesFile },
                    { ApplicationFile.TemplateName, ApplicationFile },
                    { PersonalFile.TemplateName, PersonalFile },
                    { Lod.TemplateName, Lod },
                    { ExamDutyProtocol.TemplateName, ExamDutyProtocol },
                    { ExamResultProtocol.TemplateName, ExamResultProtocol },
                    { QualificationExamResultProtocol.TemplateName, QualificationExamResultProtocol },
                    { QualificationAcquisitionProtocol.TemplateName, QualificationAcquisitionProtocol },
                    { QualificationAcquisitionExamGradesProtocol.TemplateName, QualificationAcquisitionExamGradesProtocol },
                    { SkillsCheckExamResultProtocol.TemplateName, SkillsCheckExamResultProtocol },
                    { StateExamDutyProtocol.TemplateName, StateExamDutyProtocol },
                    { SkillsCheckExamDutyProtocol.TemplateName, SkillsCheckExamDutyProtocol },
                    { NvoExamDutyProtocol.TemplateName, NvoExamDutyProtocol },
                    { StateExamsAdmProtocol.TemplateName, StateExamsAdmProtocol },
                    { GradeChangeExamsAdmProtocol.TemplateName, GradeChangeExamsAdmProtocol},
                    { HighSchoolCertificateProtocol.TemplateName, HighSchoolCertificateProtocol },
                    { GraduationThesisDefenseProtocol.TemplateName, GraduationThesisDefenseProtocol },
                    { Institution_ZUD_Request.TemplateName, Institution_ZUD_Request },
                    { All_ZUD_Request.TemplateName, All_ZUD_Request },
                    { MON_ZUD_Request.TemplateName, MON_ZUD_Request },
                    { ROU_ZUD_Request.TemplateName, ROU_ZUD_Request },
                    { Protocol_ZUD_Request.TemplateName, Protocol_ZUD_Request },
                    { Report_ZUD.TemplateName, Report_ZUD },
                    { Destruction_Protocol_ZUD.TemplateName, Destruction_Protocol_ZUD }
                });

        private WordTemplateConfig(
            string templateName,
            string templateFileName,
            string jsonSampleModel)
        {
            this.TemplateName = templateName;
            this.TemplateFileName = templateFileName;
            this.JsonSampleModel = jsonSampleModel;
        }

        public string TemplateName { get; private set; }

        public string TemplateFileName { get; private set; }

        public string JsonSampleModel { get; private set; }

        public static WordTemplateConfig Get(string name)
        {
            return AllTemplates[name];
        }
    }
}
