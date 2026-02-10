import { Module } from '@nestjs/common';
import { ScheduleModule } from '@nestjs/schedule';
import { ArchiveClassesPreviousYearJob } from 'src/jobs/archive-classes-previous-year.job';
import { ArchiveClassesJob } from 'src/jobs/archive-classes.job';
import { ArchiveEnrollmentsPreviousYearJob } from 'src/jobs/archive-enrollments-previous-year..job';
import { ArchiveEnrollmentsJob } from 'src/jobs/archive-enrollments.job';
import { ArchiveNotStartedWorkflowsJob } from 'src/jobs/archive-not-started-workflows.job';
import { ArchiveOrganizationsPreviousYearJob } from 'src/jobs/archive-organizations-previous-year..job';
import { ArchiveOrganizationsJob } from 'src/jobs/archive-organizations.job';
import { ArchiveUsersPreviousYearJob } from 'src/jobs/archive-users-previous-year..job';
import { ArchiveUsersJob } from 'src/jobs/archive-users.job';
import { CheckAzureClassesJob } from 'src/jobs/check-azure-classes.job';
import { CheckAzureEnrollmentsToClassJob } from 'src/jobs/check-azure-enrollments-to-class.job';
import { CheckAzureEnrollmentsToSchoolJob } from 'src/jobs/check-azure-enrollments-to-school.job';
import { CheckAzureOrganizationsJob } from 'src/jobs/check-azure-organizations.job';
import { CheckAzureUsersJob } from 'src/jobs/check-azure-users.job';
import { CustomEmailNotificationJob } from 'src/jobs/custom-email-notification.job';
import { DailyRunCliScriptSyncAzureClassEnrollmentsJob } from 'src/jobs/daily-run-cli-script-sync-azure-class-enrollements.job';
import { DailyRunCliScriptSyncAzureClassesJob } from 'src/jobs/daily-run-cli-script-sync-azure-classes.job';
import { DailyRunCliScriptSyncAzureOrganizaionsJob } from 'src/jobs/daily-run-cli-script-sync-azure-organizations.job';
import { DailyRunCliScriptSyncAzureSchoolEnrollmentsJob } from 'src/jobs/daily-run-cli-script-sync-azure-school-enrollements.job';
import { DailyRunCliScriptSyncAzureUsersJob } from 'src/jobs/daily-run-cli-script-sync-azure-users.job';
import { DeleteOldEntitiesInGenerationJob } from 'src/jobs/delete-old-entities-in-generation.job';
import { ErrorEmailNotificationJob } from 'src/jobs/error-email-notification.job';
import { RevertAzureClassesJob } from 'src/jobs/revert-azure-classes.job';
import { RevertAzureEnrollmentsJob } from 'src/jobs/revert-azure-enrollments.job';
import { RevertAzureOrganizationsJob } from 'src/jobs/revert-azure-organizations.job';
import { RevertAzureUsersJob } from 'src/jobs/revert-azure-users.job';
import { SendAzureClassesCreateJob } from 'src/jobs/send-azure-classes-create';
import { SendAzureClassesDeleteJob } from 'src/jobs/send-azure-classes-delete';
import { SendAzureClassesUpdateJob } from 'src/jobs/send-azure-classes-update';
import { SendAzureEnrollmentsStudentToClassCreateJob } from 'src/jobs/send-azure-enrollments-student-to-class-create.job';
import { SendAzureEnrollmentsStudentToClassDeleteJob } from 'src/jobs/send-azure-enrollments-student-to-class-delete.job';
import { SendAzureEnrollmentsStudentToSchoolCreateJob } from 'src/jobs/send-azure-enrollments-student-to-school-create.job';
import { SendAzureEnrollmentsStudentToSchoolDeleteJob } from 'src/jobs/send-azure-enrollments-student-to-school-delete.job';
import { SendAzureEnrollmentsTeacherToClassCreateJob } from 'src/jobs/send-azure-enrollments-teacher-to-class-create.job';
import { SendAzureEnrollmentsTeacherToClassDeleteJob } from 'src/jobs/send-azure-enrollments-teacher-to-class-delete.job';
import { SendAzureEnrollmentsTeacherToSchoolCreateJob } from 'src/jobs/send-azure-enrollments-teacher-to-school-create.job';
import { SendAzureEnrollmentsTeacherToSchoolDeleteJob } from 'src/jobs/send-azure-enrollments-teacher-to-school-delete.job';
import { SendAzureOrganizationsCreateJob } from 'src/jobs/send-azure-organizations-create.job';
import { SendAzureOrganizationsDeleteJob } from 'src/jobs/send-azure-organizations-delete.job';
import { SendAzureOrganizationsUpdateJob } from 'src/jobs/send-azure-organizations-update.job';
import { SendAzureUsersCreateJob } from 'src/jobs/send-azure-users-create.job';
import { SendAzureUsersDeleteJob } from 'src/jobs/send-azure-users-delete.job';
import { SendAzureUsersUpdateJob } from 'src/jobs/send-azure-users-update.job';
import { SyncClassJob } from 'src/jobs/sync-class.job';
import { SyncInstitutionJob } from 'src/jobs/sync-institution.job';
import { SyncUserJob } from 'src/jobs/sync-user.job';
import { JobsRepository } from './jobs.repository';
import { JobsController } from './routing/jobs.controller';
import { JobsService } from './routing/jobs.service';

@Module({
    controllers: [JobsController],
    imports: [ScheduleModule.forRoot()],
    providers: [
        JobsService,
        JobsRepository,
        ArchiveClassesJob,
        ArchiveClassesPreviousYearJob,
        ArchiveEnrollmentsJob,
        ArchiveEnrollmentsPreviousYearJob,
        ArchiveOrganizationsJob,
        ArchiveOrganizationsPreviousYearJob,
        ArchiveNotStartedWorkflowsJob,
        ArchiveUsersJob,
        ArchiveUsersPreviousYearJob,
        CheckAzureClassesJob,
        CheckAzureEnrollmentsToClassJob,
        CheckAzureEnrollmentsToSchoolJob,
        CheckAzureOrganizationsJob,
        CheckAzureUsersJob,
        DailyRunCliScriptSyncAzureClassesJob,
        DailyRunCliScriptSyncAzureClassEnrollmentsJob,
        DailyRunCliScriptSyncAzureOrganizaionsJob,
        DailyRunCliScriptSyncAzureSchoolEnrollmentsJob,
        DailyRunCliScriptSyncAzureUsersJob,
        // DeleteGraduatedStudentsJob,
        DeleteOldEntitiesInGenerationJob,
        SendAzureEnrollmentsStudentToClassCreateJob,
        SendAzureEnrollmentsStudentToClassDeleteJob,
        SendAzureEnrollmentsStudentToSchoolCreateJob,
        SendAzureEnrollmentsStudentToSchoolDeleteJob,
        SendAzureEnrollmentsTeacherToClassCreateJob,
        SendAzureEnrollmentsTeacherToClassDeleteJob,
        SendAzureEnrollmentsTeacherToSchoolCreateJob,
        SendAzureEnrollmentsTeacherToSchoolDeleteJob,
        SendAzureUsersCreateJob,
        SendAzureUsersUpdateJob,
        SendAzureUsersDeleteJob,
        SendAzureClassesCreateJob,
        SendAzureClassesUpdateJob,
        SendAzureClassesDeleteJob,
        SendAzureOrganizationsCreateJob,
        SendAzureOrganizationsUpdateJob,
        SendAzureOrganizationsDeleteJob,
        SyncClassJob,
        SyncUserJob,
        SyncInstitutionJob,
        RevertAzureClassesJob,
        RevertAzureEnrollmentsJob,
        RevertAzureUsersJob,
        RevertAzureOrganizationsJob,
        ErrorEmailNotificationJob,
        CustomEmailNotificationJob,
    ],
    exports: [JobsService],
})
export class JobsModule {}
