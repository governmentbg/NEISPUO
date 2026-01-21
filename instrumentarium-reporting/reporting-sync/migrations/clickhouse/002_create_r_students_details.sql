-- Migration: Create R_Students_Details table
-- Based on reporting.R_Students_Details view from the MS SQL server
CREATE TABLE IF NOT EXISTS R_Students_Details (
    RegionID Int32,
    RegionName String,
    MunicipalityID Int32,
    MunicipalityName String,
    TownID Int32,
    TownName String,
    LocalAreaID Int32,
    LAreaName String,
    InstitutionID Int32,
    BudgetingSchoolTypeID Int32,
    InstitutionName String,
    InstitutionKind String,
    FirstName String,
    MiddleName String,
    LastName String,
    PublicEduNumber String,
    IdType String,
    PersonalID String,
    BirthDate DateTime64(3, 'UTC'),
    Gender String,
    
    -- Person location fields
    PersonTown String,
    PersonMunicipality String,
    PersonRegion String,
    Nationality String,
    
    -- Education fields
    RomeName String,
    ClassName String,
    EduFormName String,
    ClassType String,
    ProfName String,
    SpecName String,
    IsIndividualCurriculum String,
    IsTravel String,
    IsRepeatClass String,
    IsSOP String,
    IsRP String
) ENGINE = MergeTree()
ORDER BY (RegionID, MunicipalityID, InstitutionID, PersonalID); 