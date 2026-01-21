namespace SB.Domain;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.Json;

public class WordTemplateConfig
{
    public static readonly WordTemplateConfig ExamDutyProtocol = new(
        "ExamDutyProtocol",
        "3-82_exam_duty_protocol.docx",
        JsonSerializer.Serialize(
            new
            {
                SchoolYear = "2018/2019",
                InstitutionName = "Алеко Константинов",
                InstitutionTownName = "Пловдив",
                InstitutionMunicipalityName = "Пловдив",
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

    public static readonly WordTemplateConfig ExamResultProtocol = new(
        "ExamResultProtocol",
        "3-80_exam_result_protocol.docx",
        JsonSerializer.Serialize(
            new
            {
                SchoolYear = "2018/2019",
                InstitutionName = "Алеко Константинов",
                InstitutionTownName = "Пловдив",
                InstitutionMunicipalityName = "Пловдив",
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

    public static readonly WordTemplateConfig ExamResultProtocolV2 = new(
        "ExamResultProtocolV2",
        "3-80_exam_result_protocol_v2.docx",
        JsonSerializer.Serialize(
            new
            {
                SchoolYear = "2018/2019",
                InstitutionName = "Алеко Константинов",
                InstitutionTownName = "Пловдив",
                InstitutionMunicipalityName = "Пловдив",
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
                    },
                    new
                    {
                        OrderNum = 2,
                        CommissionerName = "Петър Петров Петров",
                    },
                    new
                    {
                        OrderNum = 3,
                        CommissionerName = "Петър Петров Петров",
                    },
                    new
                    {
                        OrderNum = 4,
                        CommissionerName = "Петър Петров Петров",
                    }
                },
                CommissionMembersDivided = new object[]
                {
                    new
                    {
                        CommissionerNameLeft = "Петър Петров Петров",
                        CommissionerNameRight = "Петър Петров Петров",
                    },
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

    public static readonly WordTemplateConfig ExamResultProtocolV3 = new(
        "ExamResultProtocolV3",
        "3-80_exam_result_protocol_v3.docx",
        JsonSerializer.Serialize(
            new
            {
                SchoolYear = "2018/2019",
                InstitutionName = "Алеко Константинов",
                InstitutionTownName = "Пловдив",
                InstitutionMunicipalityName = "Пловдив",
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
                    },
                    new
                    {
                        OrderNum = 2,
                        CommissionerName = "Петър Петров Петров",
                    },
                    new
                    {
                        OrderNum = 3,
                        CommissionerName = "Петър Петров Петров",
                    },
                    new
                    {
                        OrderNum = 4,
                        CommissionerName = "Петър Петров Петров",
                    },
                    new
                    {
                        OrderNum = 5,
                        CommissionerName = "Петър Петров Петров",
                    },
                    new
                    {
                        OrderNum = 6,
                        CommissionerName = "Петър Петров Петров",
                    }
                },
                CommissionMembersDivided = new object[]
                {
                    new
                    {
                        CommissionerNameLeft = "Петър Петров Петров",
                        CommissionerNameRight = "Петър Петров Петров",
                    },
                    new
                    {
                        CommissionerNameLeft = "Петър Петров Петров",
                        CommissionerNameRight = "Петър Петров Петров",
                    },
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

    public static readonly WordTemplateConfig ExamResultProtocolV4 = new(
        "ExamResultProtocolV4",
        "3-80_exam_result_protocol_v4.docx",
        JsonSerializer.Serialize(
            new
            {
                SchoolYear = "2018/2019",
                InstitutionName = "Алеко Константинов",
                InstitutionTownName = "Пловдив",
                InstitutionMunicipalityName = "Пловдив",
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
                    },
                    new
                    {
                        OrderNum = 2,
                        CommissionerName = "Петър Петров Петров",
                    },
                    new
                    {
                        OrderNum = 3,
                        CommissionerName = "Петър Петров Петров",
                    },
                    new
                    {
                        OrderNum = 4,
                        CommissionerName = "Петър Петров Петров",
                    },
                    new
                    {
                        OrderNum = 5,
                        CommissionerName = "Петър Петров Петров",
                    },
                    new
                    {
                        OrderNum = 6,
                        CommissionerName = "Петър Петров Петров",
                    }
                },
                CommissionMembersDivided = new object[]
                {
                    new
                    {
                        CommissionerNameLeft = "Петър Петров Петров",
                        CommissionerNameRight = "Петър Петров Петров",
                    },
                    new
                    {
                        CommissionerNameLeft = "Петър Петров Петров",
                        CommissionerNameRight = "Петър Петров Петров",
                    },
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

    public static readonly WordTemplateConfig ExamResultProtocolV5 = new(
        "ExamResultProtocolV5",
        "3-80_exam_result_protocol_v5.docx",
        JsonSerializer.Serialize(
            new
            {
                SchoolYear = "2018/2019",
                InstitutionName = "Алеко Константинов",
                InstitutionTownName = "Пловдив",
                InstitutionMunicipalityName = "Пловдив",
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
                    },
                    new
                    {
                        OrderNum = 2,
                        CommissionerName = "Петър Петров Петров",
                    },
                    new
                    {
                        OrderNum = 3,
                        CommissionerName = "Петър Петров Петров",
                    },
                    new
                    {
                        OrderNum = 4,
                        CommissionerName = "Петър Петров Петров",
                    },
                    new
                    {
                        OrderNum = 5,
                        CommissionerName = "Петър Петров Петров",
                    },
                    new
                    {
                        OrderNum = 6,
                        CommissionerName = "Петър Петров Петров",
                    }
                },
                CommissionMembersDivided = new object[]
                {
                    new
                    {
                        CommissionerNameLeft = "Петър Петров Петров",
                        CommissionerNameRight = "Петър Петров Петров",
                    },
                    new
                    {
                        CommissionerNameLeft = "Петър Петров Петров",
                        CommissionerNameRight = "Петър Петров Петров",
                    },
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

    public static readonly WordTemplateConfig ExamResultProtocolV6 = new(
        "ExamResultProtocolV6",
        "3-80_exam_result_protocol_v6.docx",
        JsonSerializer.Serialize(
            new
            {
                SchoolYear = "2018/2019",
                InstitutionName = "Алеко Константинов",
                InstitutionTownName = "Пловдив",
                InstitutionMunicipalityName = "Пловдив",
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
                    },
                    new
                    {
                        OrderNum = 2,
                        CommissionerName = "Петър Петров Петров",
                    },
                    new
                    {
                        OrderNum = 3,
                        CommissionerName = "Петър Петров Петров",
                    },
                    new
                    {
                        OrderNum = 4,
                        CommissionerName = "Петър Петров Петров",
                    },
                    new
                    {
                        OrderNum = 5,
                        CommissionerName = "Петър Петров Петров",
                    },
                    new
                    {
                        OrderNum = 6,
                        CommissionerName = "Петър Петров Петров",
                    }
                },
                CommissionMembersDivided = new object[]
                {
                    new
                    {
                        CommissionerNameLeft = "Петър Петров Петров",
                        CommissionerNameRight = "Петър Петров Петров",
                    },
                    new
                    {
                        CommissionerNameLeft = "Петър Петров Петров",
                        CommissionerNameRight = "Петър Петров Петров",
                    },
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

    public static readonly WordTemplateConfig QualificationExamResultProtocol = new(
        "QualificationExamResultProtocol",
        "3-80_qual_exam_result_protocol.docx",
        JsonSerializer.Serialize(
            new
            {
                SchoolYear = "2018/2019",
                InstitutionName = "Алеко Константинов",
                InstitutionTownName = "Пловдив",
                InstitutionMunicipalityName = "Пловдив",
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

    public static readonly WordTemplateConfig QualificationExamResultProtocolV2 = new(
        "QualificationExamResultProtocolV2",
        "3-80_qual_exam_result_protocol_v2.docx",
        JsonSerializer.Serialize(
            new
            {
                SchoolYear = "2018/2019",
                InstitutionName = "Алеко Константинов",
                InstitutionTownName = "Пловдив",
                InstitutionMunicipalityName = "Пловдив",
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
                    },
                    new
                    {
                        OrderNum = 2,
                        CommissionerName = "Петър Петров Петров",
                    },
                    new
                    {
                        OrderNum = 3,
                        CommissionerName = "Петър Петров Петров",
                    },
                    new
                    {
                        OrderNum = 4,
                        CommissionerName = "Петър Петров Петров",
                    }
                },
                CommissionMembersDivided = new object[]
                {
                    new
                    {
                        CommissionerNameLeft = "Петър Петров Петров",
                        CommissionerNameRight = "Петър Петров Петров",
                    },
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

    public static readonly WordTemplateConfig QualificationExamResultProtocolV3 = new(
        "QualificationExamResultProtocolV3",
        "3-80_qual_exam_result_protocol_v3.docx",
        JsonSerializer.Serialize(
            new
            {
                SchoolYear = "2018/2019",
                InstitutionName = "Алеко Константинов",
                InstitutionTownName = "Пловдив",
                InstitutionMunicipalityName = "Пловдив",
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
                    },
                    new
                    {
                        OrderNum = 2,
                        CommissionerName = "Петър Петров Петров",
                    },
                    new
                    {
                        OrderNum = 3,
                        CommissionerName = "Петър Петров Петров",
                    },
                    new
                    {
                        OrderNum = 4,
                        CommissionerName = "Петър Петров Петров",
                    },
                    new
                    {
                        OrderNum = 5,
                        CommissionerName = "Петър Петров Петров",
                    },
                    new
                    {
                        OrderNum = 6,
                        CommissionerName = "Петър Петров Петров",
                    }
                },
                CommissionMembersDivided = new object[]
                {
                    new
                    {
                        CommissionerNameLeft = "Петър Петров Петров",
                        CommissionerNameRight = "Петър Петров Петров",
                    },
                    new
                    {
                        CommissionerNameLeft = "Петър Петров Петров",
                        CommissionerNameRight = "Петър Петров Петров",
                    },
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

    public static readonly WordTemplateConfig QualificationExamResultProtocolV4 = new(
        "QualificationExamResultProtocolV4",
        "3-80_qual_exam_result_protocol_v4.docx",
        JsonSerializer.Serialize(
            new
            {
                SchoolYear = "2018/2019",
                InstitutionName = "Алеко Константинов",
                InstitutionTownName = "Пловдив",
                InstitutionMunicipalityName = "Пловдив",
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
                    },
                    new
                    {
                        OrderNum = 2,
                        CommissionerName = "Петър Петров Петров",
                    },
                    new
                    {
                        OrderNum = 3,
                        CommissionerName = "Петър Петров Петров",
                    },
                    new
                    {
                        OrderNum = 4,
                        CommissionerName = "Петър Петров Петров",
                    },
                    new
                    {
                        OrderNum = 5,
                        CommissionerName = "Петър Петров Петров",
                    },
                    new
                    {
                        OrderNum = 6,
                        CommissionerName = "Петър Петров Петров",
                    }
                },
                CommissionMembersDivided = new object[]
                {
                    new
                    {
                        CommissionerNameLeft = "Петър Петров Петров",
                        CommissionerNameRight = "Петър Петров Петров",
                    },
                    new
                    {
                        CommissionerNameLeft = "Петър Петров Петров",
                        CommissionerNameRight = "Петър Петров Петров",
                    },
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

    public static readonly WordTemplateConfig QualificationExamResultProtocolV5 = new(
        "QualificationExamResultProtocolV5",
        "3-80_qual_exam_result_protocol_v5.docx",
        JsonSerializer.Serialize(
            new
            {
                SchoolYear = "2018/2019",
                InstitutionName = "Алеко Константинов",
                InstitutionTownName = "Пловдив",
                InstitutionMunicipalityName = "Пловдив",
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
                    },
                    new
                    {
                        OrderNum = 2,
                        CommissionerName = "Петър Петров Петров",
                    },
                    new
                    {
                        OrderNum = 3,
                        CommissionerName = "Петър Петров Петров",
                    },
                    new
                    {
                        OrderNum = 4,
                        CommissionerName = "Петър Петров Петров",
                    },
                    new
                    {
                        OrderNum = 5,
                        CommissionerName = "Петър Петров Петров",
                    },
                    new
                    {
                        OrderNum = 6,
                        CommissionerName = "Петър Петров Петров",
                    }
                },
                CommissionMembersDivided = new object[]
                {
                    new
                    {
                        CommissionerNameLeft = "Петър Петров Петров",
                        CommissionerNameRight = "Петър Петров Петров",
                    },
                    new
                    {
                        CommissionerNameLeft = "Петър Петров Петров",
                        CommissionerNameRight = "Петър Петров Петров",
                    },
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

    public static readonly WordTemplateConfig QualificationExamResultProtocolV6 = new(
        "QualificationExamResultProtocolV6",
        "3-80_qual_exam_result_protocol_v6.docx",
        JsonSerializer.Serialize(
            new
            {
                SchoolYear = "2018/2019",
                InstitutionName = "Алеко Константинов",
                InstitutionTownName = "Пловдив",
                InstitutionMunicipalityName = "Пловдив",
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
                    },
                    new
                    {
                        OrderNum = 2,
                        CommissionerName = "Петър Петров Петров",
                    },
                    new
                    {
                        OrderNum = 3,
                        CommissionerName = "Петър Петров Петров",
                    },
                    new
                    {
                        OrderNum = 4,
                        CommissionerName = "Петър Петров Петров",
                    },
                    new
                    {
                        OrderNum = 5,
                        CommissionerName = "Петър Петров Петров",
                    },
                    new
                    {
                        OrderNum = 6,
                        CommissionerName = "Петър Петров Петров",
                    }
                },
                CommissionMembersDivided = new object[]
                {
                    new
                    {
                        CommissionerNameLeft = "Петър Петров Петров",
                        CommissionerNameRight = "Петър Петров Петров",
                    },
                    new
                    {
                        CommissionerNameLeft = "Петър Петров Петров",
                        CommissionerNameRight = "Петър Петров Петров",
                    },
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

    public static readonly WordTemplateConfig QualificationAcquisitionProtocol = new(
        "QualificationAcquisitionProtocol",
        "3-81v_qual_acquis_protocol.docx",
        JsonSerializer.Serialize(
            new
            {
                SchoolYear = "2018/2019",
                InstitutionName = "Алеко Константинов",
                InstitutionTownName = "Пловдив",
                InstitutionMunicipalityName = "Пловдив",
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
                        TheoryPoints = "12",
                        PracticePoints = "14",
                        AverageDecimalGradeText = "Мн. Добър",
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
                        TheoryPoints = "12",
                        PracticePoints = "14",
                    }
                },
            },
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            }));

    public static readonly WordTemplateConfig QualificationAcquisitionExamGradesProtocol = new(
        "QualificationAcquisitionExamGradesProtocol",
        "3-81v_qual_acquis_exam_grades_protocol.docx",
        JsonSerializer.Serialize(
            new
            {
                SchoolYear = "2018/2019",
                InstitutionName = "Алеко Константинов",
                InstitutionTownName = "Пловдив",
                InstitutionMunicipalityName = "Пловдив",
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

    public static readonly WordTemplateConfig SkillsCheckExamResultProtocol = new(
        "SkillsCheckExamResultProtocol",
        "3-80_skills_check_exam_result_protocol.docx",
        JsonSerializer.Serialize(
            new
            {
                SchoolYear = "2020/2021",
                InstitutionName = "Алеко Константинов",
                InstitutionTownName = "Пловдив",
                InstitutionMunicipalityName = "Пловдив",
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

    public static readonly WordTemplateConfig SkillsCheckExamResultProtocolV2 = new(
        "SkillsCheckExamResultProtocolV2",
        "3-80_skills_check_exam_result_protocol_v2.docx",
        JsonSerializer.Serialize(
            new
            {
                SchoolYear = "2020/2021",
                InstitutionName = "Алеко Константинов",
                InstitutionTownName = "Пловдив",
                InstitutionMunicipalityName = "Пловдив",
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
                    },
                    new
                    {
                        Name = "Петър Петров Петров"
                    },
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

    public static readonly WordTemplateConfig SkillsCheckExamResultProtocolV3 = new(
        "SkillsCheckExamResultProtocolV3",
        "3-80_skills_check_exam_result_protocol_v3.docx",
        JsonSerializer.Serialize(
            new
            {
                SchoolYear = "2020/2021",
                InstitutionName = "Алеко Константинов",
                InstitutionTownName = "Пловдив",
                InstitutionMunicipalityName = "Пловдив",
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
                    },
                    new
                    {
                        Name = "Петър Петров Петров"
                    },
                    new
                    {
                        Name = "Петър Петров Петров"
                    },
                    new
                    {
                        Name = "Петър Петров Петров"
                    },
                    new
                    {
                        Name = "Петър Петров Петров"
                    },
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

    public static readonly WordTemplateConfig SkillsCheckExamResultProtocolV4 = new(
        "SkillsCheckExamResultProtocolV4",
        "3-80_skills_check_exam_result_protocol_v4.docx",
        JsonSerializer.Serialize(
            new
            {
                SchoolYear = "2020/2021",
                InstitutionName = "Алеко Константинов",
                InstitutionTownName = "Пловдив",
                InstitutionMunicipalityName = "Пловдив",
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
                    },
                    new
                    {
                        Name = "Петър Петров Петров"
                    },
                    new
                    {
                        Name = "Петър Петров Петров"
                    },
                    new
                    {
                        Name = "Петър Петров Петров"
                    },
                    new
                    {
                        Name = "Петър Петров Петров"
                    },
                    new
                    {
                        Name = "Петър Петров Петров"
                    },
                    new
                    {
                        Name = "Петър Петров Петров"
                    },
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

    public static readonly WordTemplateConfig SkillsCheckExamResultProtocolV5 = new(
        "SkillsCheckExamResultProtocolV5",
        "3-80_skills_check_exam_result_protocol_v5.docx",
        JsonSerializer.Serialize(
            new
            {
                SchoolYear = "2020/2021",
                InstitutionName = "Алеко Константинов",
                InstitutionTownName = "Пловдив",
                InstitutionMunicipalityName = "Пловдив",
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
                    },
                    new
                    {
                        Name = "Петър Петров Петров"
                    },
                    new
                    {
                        Name = "Петър Петров Петров"
                    },
                    new
                    {
                        Name = "Петър Петров Петров"
                    },
                    new
                    {
                        Name = "Петър Петров Петров"
                    },
                    new
                    {
                        Name = "Петър Петров Петров"
                    },
                    new
                    {
                        Name = "Петър Петров Петров"
                    },
                    new
                    {
                        Name = "Петър Петров Петров"
                    },
                    new
                    {
                        Name = "Петър Петров Петров"
                    },
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

    public static readonly WordTemplateConfig StateExamsAdmProtocol = new(
        "StateExamsAdmProtocol",
        "3-79_state_exams_adm_protocol.docx",
        JsonSerializer.Serialize(
            new
            {
                InstName = "Алеко Константинов",
                InstitutionTownName = "Пловдив",
                InstitutionMunicipalityName = "Пловдив",
                InstitutionRegionName = "Пловдив",
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

    public static readonly WordTemplateConfig StateExamDutyProtocol = new(
        "StateExamDutyProtocol",
        "3-82_state_exam_duty_protocol.docx",
        JsonSerializer.Serialize(
            new
            {
                SchoolYear = "2018/2019",
                InstitutionName = "Алеко Константинов",
                InstitutionTownName = "Пловдив",
                InstitutionMunicipalityName = "Пловдив",
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

    public static readonly WordTemplateConfig SkillsCheckExamDutyProtocol = new(
        "SkillsCheckExamDutyProtocol",
        "3-82_skills_check_exam_duty_protocol.docx",
        JsonSerializer.Serialize(
            new
            {
                SchoolYear = "2020/2021",
                InstitutionName = "Алеко Константинов",
                InstitutionTownName = "Пловдив",
                InstitutionMunicipalityName = "Пловдив",
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

    public static readonly WordTemplateConfig NvoExamDutyProtocol = new(
        "NvoExamDutyProtocol",
        "3-82_NVO_exam_duty_protocol.docx",
        JsonSerializer.Serialize(
            new
            {
                SchoolYear = "2020/2021",
                InstitutionName = "Алеко Константинов",
                InstitutionTownName = "Пловдив",
                InstitutionMunicipalityName = "Пловдив",
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

    public static readonly WordTemplateConfig GradeChangeExamsAdmProtocol = new(
        "GradeChangeExamsAdmProtocol",
        "3-79А_grade_change_exams_adm_protocol.docx",
        JsonSerializer.Serialize(
            new
            {
                InstName = "Алеко Константинов",
                InstitutionTownName = "Пловдив",
                InstitutionMunicipalityName = "Пловдив",
                InstitutionRegionName = "Пловдив",
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

    public static readonly WordTemplateConfig HighSchoolCertificateProtocol = new(
        "HighSchoolCertificateProtocol",
        "3-84_high_school_certificate_protocol.docx",
        JsonSerializer.Serialize(
            new
            {
                InstName = "Алеко Константинов",
                InstitutionTownName = "Пловдив",
                InstitutionMunicipalityName = "Пловдив",
                InstitutionRegionName = "Пловдив",
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

    public static readonly WordTemplateConfig GraduationThesisDefenseProtocol = new(
        "GraduationThesisDefenseProtocol",
        "3-81d_graduation-thesis-defense_protocol.docx",
        JsonSerializer.Serialize(
            new
            {
                SchoolYear = "2020/2021",
                InstitutionName = "Алеко Константинов",
                InstitutionTownName = "Пловдив",
                InstitutionMunicipalityName = "Пловдив",
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

    public static readonly WordTemplateConfig RegBookDocumentForSign = new(
        "RegBookDocumentForSign",
        "reg-book-document-for-sign.docx",
        JsonSerializer.Serialize(
            new
            {
                InstitutionName = "Алеко Константинов",
                InstitutionTownName = "Пловдив",
                InstitutionMunicipalityName = "Пловдив",
                InstitutionRegionName = "Пловдив",
                FullName = "Иван Иванов Иванов",
                RegistrationNumberTotal = "123a",
                RegistrationDate = "05.06.2022",
                BasicDocumentName = "3-30а Дубликат на свидетелство за основно образование",
                BookName = "издадените удостоверения"
            },
            new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            }));

    public static readonly IReadOnlyDictionary<string, WordTemplateConfig> AllTemplates =
        new ReadOnlyDictionary<string, WordTemplateConfig>(
            new Dictionary<string, WordTemplateConfig>(StringComparer.InvariantCultureIgnoreCase)
            {
                { ExamDutyProtocol.TemplateName, ExamDutyProtocol },
                { ExamResultProtocol.TemplateName, ExamResultProtocol },
                { ExamResultProtocolV2.TemplateName, ExamResultProtocolV2 },
                { ExamResultProtocolV3.TemplateName, ExamResultProtocolV3 },
                { ExamResultProtocolV4.TemplateName, ExamResultProtocolV4 },
                { ExamResultProtocolV5.TemplateName, ExamResultProtocolV5 },
                { ExamResultProtocolV6.TemplateName, ExamResultProtocolV6 },
                { QualificationExamResultProtocol.TemplateName, QualificationExamResultProtocol },
                { QualificationExamResultProtocolV2.TemplateName, QualificationExamResultProtocolV2 },
                { QualificationExamResultProtocolV3.TemplateName, QualificationExamResultProtocolV3 },
                { QualificationExamResultProtocolV4.TemplateName, QualificationExamResultProtocolV4 },
                { QualificationExamResultProtocolV5.TemplateName, QualificationExamResultProtocolV5 },
                { QualificationExamResultProtocolV6.TemplateName, QualificationExamResultProtocolV6 },
                { QualificationAcquisitionProtocol.TemplateName, QualificationAcquisitionProtocol },
                { QualificationAcquisitionExamGradesProtocol.TemplateName, QualificationAcquisitionExamGradesProtocol },
                { SkillsCheckExamResultProtocol.TemplateName, SkillsCheckExamResultProtocol },
                { SkillsCheckExamResultProtocolV2.TemplateName, SkillsCheckExamResultProtocolV2 },
                { SkillsCheckExamResultProtocolV3.TemplateName, SkillsCheckExamResultProtocolV3 },
                { SkillsCheckExamResultProtocolV4.TemplateName, SkillsCheckExamResultProtocolV4 },
                { SkillsCheckExamResultProtocolV5.TemplateName, SkillsCheckExamResultProtocolV5 },
                { StateExamDutyProtocol.TemplateName, StateExamDutyProtocol },
                { SkillsCheckExamDutyProtocol.TemplateName, SkillsCheckExamDutyProtocol },
                { NvoExamDutyProtocol.TemplateName, NvoExamDutyProtocol },
                { StateExamsAdmProtocol.TemplateName, StateExamsAdmProtocol },
                { GradeChangeExamsAdmProtocol.TemplateName, GradeChangeExamsAdmProtocol},
                { HighSchoolCertificateProtocol.TemplateName, HighSchoolCertificateProtocol },
                { GraduationThesisDefenseProtocol.TemplateName, GraduationThesisDefenseProtocol },
                { RegBookDocumentForSign.TemplateName, RegBookDocumentForSign }
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
