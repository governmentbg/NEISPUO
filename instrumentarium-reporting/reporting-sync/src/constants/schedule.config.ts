export const SCHEDULE_CONFIG = {
  // Fast jobs
  syncPhones: '0 0 * * *', // approximate synchronization time: ~1.7s
  syncHostels: '0 0 * * *', // approximate synchronization time: ~2.4s
  syncImmigrantsDobHours: '0 0 * * *', // approximate synchronization time: ~1.5s
  syncOccupationsByQualifications: '0 0 * * *', // approximate synchronization time: ~3.1s
  syncRefugeeWithdrawnRequests: '0 0 * * *', // approximate synchronization time: ~0.2s

  // Medium jobs
  syncInstitutions: '3 0 * * *', // approximate synchronization time: ~6.4s
  syncGraduatedStudents: '3 0 * * *', // approximate synchronization time: ~8.2s
  syncHelpdesk: '3 0 * * *', // approximate synchronization time: ~16.8s
  syncForeignLanguages: '3 0 * * *', // approximate synchronization time: ~29.2s

  syncRefugeesReceivedRejectedAdmissionByRegion: '5 0 * * *', // approximate synchronization time: ~3.1s
  syncRefugeesSearchingOrReceivedAdmission: '5 0 * * *', // approximate synchronization time: ~5.3s
  syncPersonalN: '5 0 * * *', // approximate synchronization time: ~66s

  syncRziStudents: '7 0 * * *', // approximate synchronization time: ~27.3s

  // Heavy
  syncClasses: '10 0 * * *', // approximate synchronization time: ~3.6m
  syncStudentsDetails: '14 0 * * *', // approximate synchronization time: ~3.6m
  syncStudents: '18 0 * * *', // approximate synchronization time: ~11.5m

  syncPgAverageAbsencePerClass: '35 0 * * *', // approximate synchronization time: ~1.2m
  syncRegularAbsencesPerMonth: '35 0 * * *', // approximate synchronization time: ~1.5m
  syncTwentyFivePercentageAbsences: '37 0 * * *', // approximate synchronization time: ~11.4m
  syncPgAverageAbsencePerStudent: '37 0 * * *', // approximate synchronization time: ~12.9m
  syncAverageGradesPerClass: '59 0 * * *', // approximate synchronization time: ~12.6m
  syncRegularAverageAbsencePerClass: '59 0 * * *', // approximate synchronization time: ~17.2m
  syncPgFamilyReasonAbsences: '18 1 * * *', // approximate synchronization time: ~17.6m
  syncAverageGradesPerStudent: '18 1 * * *', // approximate synchronization time: ~27m
  syncRegularAverageAbsencePerStudent: '37 1 * * *', // approximate synchronization time: ~33m
  syncRegularFamilyReasonAbsences: '47 1 * * *', // approximate synchronization time: ~44m
  syncPgAbsencesPerMonth: '20 2 * * *', // approximate synchronization time: ~78m

  syncCurriculumSectionA: '40 3 * * *',
  syncCurriculumSectionBProfessional: '45 3 * * *',
  syncCurriculumSectionBProfiled: '50 3 * * *',
  syncPersonalStaff: '55 3 * * *',

  syncOrganizationsWorkflowsActiveYear: '0 4 * * *',
  syncUsersWorkflowsActiveYear: '0 4 * * *',
  syncClassesWorkflowsActiveYear: '5 4 * * *',
  syncEnrollmentsWorkflowsActiveYear: '10 4 * * *',

  jobDatabaseSync: '0 * * * *',
};
