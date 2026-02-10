import { getInstitutionIds } from '../databases/mssql/repositories/sync.repository';
import { MSSQLOperatorsEnum } from '../enums/mssql-operators.enum';
import { SyncConfig } from '../interfaces';
import {
  generateSchoolYearRanges,
  transformDate,
} from '../utils';

export const SYNC_CONFIGS: Record<string, SyncConfig> = {
  institutions: {
    source: 'inst_basic.R_institutions',
    target: 'R_Institutions',
    columnMappings: [],
  },
  students: {
    source: 'reporting.R_Students',
    target: 'R_Students',
    columnMappings: [],
  },
  studentsDetails: {
    source: 'reporting.R_Students_Details',
    target: 'R_Students_Details',
    columnMappings: [
      {
        columnName: 'BirthDate',
        transformFunction: transformDate,
      },
    ],
  },
  graduatedStudents: {
    source: 'reporting.R_Graduated_Students',
    target: 'R_Graduated_Students',
    columnMappings: [],
  },
  phones: {
    source: 'inst_basic.Phones',
    target: 'Phones',
    columnMappings: [],
  },
  classes: {
    source: 'inst_basic.R_Classes',
    target: 'R_Classes',
    columnMappings: [],
  },
  foreignLanguages: {
    source: 'inst_basic.R_foreign_languages',
    target: 'R_foreign_languages',
    columnMappings: [],
  },
  hostels: {
    source: 'inst_basic.R_hostels',
    target: 'R_hostels',
    columnMappings: [],
  },
  immigrantsDobHours: {
    source: 'inst_basic.RImmigrantsDOBHours',
    target: 'RImmigrantsDOBHours',
    columnMappings: [],
  },
  occupationsByQualifications: {
    source: 'reporting.R_Occupations_By_Qualifications',
    target: 'R_Occupations_By_Qualifications',
    columnMappings: [],
  },
  personalN: {
    source: 'inst_basic.R_Personal_N',
    target: 'R_Personal_N',
    columnMappings: [
      {
        columnName: 'BirthDate',
        transformFunction: transformDate,
      },
    ],
  },
  rziStudents: {
    source: 'reporting.R_RZI_Students',
    target: 'R_RZI_Students',
    columnMappings: [],
  },
  refugeeWithdrawnRequests: {
    source: 'refugee.RefugeeWithdrawnRequests',
    target: 'RefugeeWithdrawnRequests',
    columnMappings: [],
  },
  refugeesReceivedRejectedAdmissionByRegion: {
    source: 'refugee.RefugeesReceivedRejectedAdmissionByRegion',
    target: 'RefugeesReceivedRejectedAdmissionByRegion',
    columnMappings: [],
  },
  refugeesSearchingOrReceivedAdmission: {
    source: 'refugee.RefugeesSearchingOrReceivedAdmission',
    target: 'RefugeesSearchingOrReceivedAdmission',
    columnMappings: [],
  },
  helpdesk: {
    source: 'reporting.R_Helpdesk',
    target: 'R_Helpdesk',
    columnMappings: [
      {
        columnName: 'IssueResolveDate',
        transformFunction: transformDate,
      },
      {
        columnName: 'IssueCreateDate',
        transformFunction: transformDate,
      },
      {
        columnName: 'IssueModifyDate',
        transformFunction: transformDate,
      },
    ],
  },
  twentyFivePercentageAbsences: {
    source: 'reporting.R_Twenty_Five_Percentage_Absences',
    target: 'R_Twenty_Five_Percentage_Absences',
    columnMappings: [
      {
        columnName: 'BirthDate',
        transformFunction: transformDate,
      },
    ],
    chunkConfigs: [
      {
        columnName: 'SchoolYear',
        operator: MSSQLOperatorsEnum.EQUALS,
        values: generateSchoolYearRanges,
      },
      {
        columnName: 'InstitutionID',
        operator: MSSQLOperatorsEnum.EQUALS,
        values: getInstitutionIds,
      },
    ],
  },
  pgAbsencesPerMonth: {
    source: 'reporting.R_PG_Absences_Per_Month',
    target: 'R_PG_Absences_Per_Month',
    columnMappings: [
      {
        columnName: 'Day',
        transformFunction: transformDate,
      },
    ],
    chunkConfigs: [
      {
        columnName: 'SchoolYear',
        operator: MSSQLOperatorsEnum.EQUALS,
        values: generateSchoolYearRanges,
      },
      {
        columnName: 'InstitutionID',
        operator: MSSQLOperatorsEnum.EQUALS,
        values: getInstitutionIds,
      },
    ],
  },
  regularAbsencesPerMonth: {
    source: 'reporting.R_Regular_Absences_Per_Month',
    target: 'R_Regular_Absences_Per_Month',
    columnMappings: [],
    chunkConfigs: [
      {
        columnName: 'SchoolYear',
        operator: MSSQLOperatorsEnum.EQUALS,
        values: generateSchoolYearRanges,
      },
      {
        columnName: 'InstitutionID',
        operator: MSSQLOperatorsEnum.EQUALS,
        values: getInstitutionIds,
      },
    ],
  },
  regularFamilyReasonAbsences: {
    source: 'reporting.R_Regular_Family_Reason_Absences',
    target: 'R_Regular_Family_Reason_Absences',
    columnMappings: [
      {
        columnName: 'Day',
        transformFunction: transformDate,
      },
    ],
    chunkConfigs: [
      {
        columnName: 'SchoolYear',
        operator: MSSQLOperatorsEnum.EQUALS,
        values: generateSchoolYearRanges,
      },
      {
        columnName: 'InstitutionID',
        operator: MSSQLOperatorsEnum.EQUALS,
        values: getInstitutionIds,
      },
    ],
  },
  pgFamilyReasonAbsences: {
    source: 'reporting.R_PG_Family_Reason_Absences',
    target: 'R_PG_Family_Reason_Absences',
    columnMappings: [
      {
        columnName: 'Day',
        transformFunction: transformDate,
      },
    ],
    chunkConfigs: [
      {
        columnName: 'SchoolYear',
        operator: MSSQLOperatorsEnum.EQUALS,
        values: generateSchoolYearRanges,
      },
      {
        columnName: 'InstitutionID',
        operator: MSSQLOperatorsEnum.EQUALS,
        values: getInstitutionIds,
      },
    ],
  },
  averageGradesPerStudent: {
    source: 'reporting.R_Average_Grades_Per_Student',
    target: 'R_Average_Grades_Per_Student',
    columnMappings: [],
    chunkConfigs: [
      {
        columnName: 'SchoolYear',
        operator: MSSQLOperatorsEnum.EQUALS,
        values: generateSchoolYearRanges,
      },
      {
        columnName: 'InstitutionID',
        operator: MSSQLOperatorsEnum.EQUALS,
        values: getInstitutionIds,
      },
    ],
  },
  averageGradesPerClass: {
    source: 'reporting.R_Average_Grades_Per_Class',
    target: 'R_Average_Grades_Per_Class',
    columnMappings: [],
    chunkConfigs: [
      {
        columnName: 'SchoolYear',
        operator: MSSQLOperatorsEnum.EQUALS,
        values: generateSchoolYearRanges,
      },
      {
        columnName: 'InstitutionID',
        operator: MSSQLOperatorsEnum.EQUALS,
        values: getInstitutionIds,
      },
    ],
  },
  pgAverageAbsencePerStudent: {
    source: 'reporting.R_PG_Average_Absence_Per_Student',
    target: 'R_PG_Average_Absence_Per_Student',
    columnMappings: [],
    chunkConfigs: [
      {
        columnName: 'SchoolYear',
        operator: MSSQLOperatorsEnum.EQUALS,
        values: generateSchoolYearRanges,
      },
      {
        columnName: 'InstitutionID',
        operator: MSSQLOperatorsEnum.EQUALS,
        values: getInstitutionIds,
      },
    ],
  },
  regularAverageAbsencePerStudent: {
    source: 'reporting.R_Regular_Average_Absence_Per_Student',
    target: 'R_Regular_Average_Absence_Per_Student',
    columnMappings: [],
    chunkConfigs: [
      {
        columnName: 'SchoolYear',
        operator: MSSQLOperatorsEnum.EQUALS,
        values: generateSchoolYearRanges,
      },
      {
        columnName: 'InstitutionID',
        operator: MSSQLOperatorsEnum.EQUALS,
        values: getInstitutionIds,
      },
    ],
  },
  pgAverageAbsencePerClass: {
    source: 'reporting.R_PG_Average_Absence_Per_Class',
    target: 'R_PG_Average_Absence_Per_Class',
    columnMappings: [],
    chunkConfigs: [
      {
        columnName: 'SchoolYear',
        operator: MSSQLOperatorsEnum.EQUALS,
        values: generateSchoolYearRanges,
      },
      {
        columnName: 'InstitutionID',
        operator: MSSQLOperatorsEnum.EQUALS,
        values: getInstitutionIds,
      },
    ],
  },
  regularAverageAbsencePerClass: {
    source: 'reporting.R_Regular_Average_Absence_Per_Class',
    target: 'R_Regular_Average_Absence_Per_Class',
    columnMappings: [],
    chunkConfigs: [
      {
        columnName: 'SchoolYear',
        operator: MSSQLOperatorsEnum.EQUALS,
        values: generateSchoolYearRanges,
      },
      {
        columnName: 'InstitutionID',
        operator: MSSQLOperatorsEnum.EQUALS,
        values: getInstitutionIds,
      },
    ],
  },
  curriculumSectionA: {
    source: 'reporting.R_Curriculum_Section_A',
    target: 'R_Curriculum_Section_A',
    columnMappings: [],
  },
  curriculumSectionBProfessional: {
    source: 'reporting.R_Curriculum_Section_B_Professional',
    target: 'R_Curriculum_Section_B_Professional',
    columnMappings: [],
  },
  curriculumSectionBProfiled: {
    source: 'reporting.R_Curriculum_Section_B_Profiled',
    target: 'R_Curriculum_Section_B_Profiled',
    columnMappings: [],
  },
  personalStaff: {
    source: 'reporting.R_Personal_Staff',
    target: 'R_Personal_Staff',
    columnMappings: [
      {
        columnName: 'Дата на раждане',
        transformFunction: transformDate,
      },
    ],
  },
  organizationsWorkflowsActiveYear: {
    source: 'azure_temp.AzureOrganizationsView',
    target: 'R_Organizations_Workflows_Active_Year',
    columnMappings: [
      {
        columnName: 'createdOn',
        transformFunction: transformDate,
      },
      {
        columnName: 'updatedOn',
        transformFunction: transformDate,
      },
    ],
  },
  usersWorkflowsActiveYear: {
    source: 'azure_temp.AzureUsersView',
    target: 'R_Users_Workflows_Active_Year',
    columnMappings: [
      {
        columnName: 'birthDate',
        transformFunction: transformDate,
      },
      {
        columnName: 'createdOn',
        transformFunction: transformDate,
      },
      {
        columnName: 'updatedOn',
        transformFunction: transformDate,
      },
    ],
  },
  classesWorkflowsActiveYear: {
    source: 'azure_temp.AzureClassesView',
    target: 'R_Classes_Workflows_Active_Year',
    columnMappings: [
      {
        columnName: 'termStartDate',
        transformFunction: transformDate,
      },
      {
        columnName: 'termEndDate',
        transformFunction: transformDate,
      },
      {
        columnName: 'createdOn',
        transformFunction: transformDate,
      },
      {
        columnName: 'updatedOn',
        transformFunction: transformDate,
      },
    ],
  },
  enrollmentsWorkflowsActiveYear: {
    source: 'azure_temp.AzureEnrollmentsView',
    target: 'R_Enrollments_Workflows_Active_Year',
    columnMappings: [
      {
        columnName: 'createdOn',
        transformFunction: transformDate,
      },
      {
        columnName: 'updatedOn',
        transformFunction: transformDate,
      },
    ],
  },
} as const;
