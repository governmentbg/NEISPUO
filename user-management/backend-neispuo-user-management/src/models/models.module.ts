import { Module } from '@nestjs/common';
import { AccountantModule } from './accountant/accountant.module';
import { AzureArchiveModule } from './azure/azure-archive/azure-archive.module';
import { AzureClassesModule } from './azure/azure-classes/azure-classes.module';
import { AzureEnrollmentsModule } from './azure/azure-enrollments/azure-enrollments.module';
import { AzureOrganizationsModule } from './azure/azure-organizations/azure-organizations.module';
import { AzureParentModule } from './azure/azure-parent/azure-parent.module';
import { AzureStudentModule } from './azure/azure-student/azure-student.module';
import { AzureTeacherModule } from './azure/azure-teacher/azure-teacher.module';
import { AzureUsersModule } from './azure/azure-users/azure-users.module';
import { BearerTokenModule } from './bearer-token/bearer-token.module';
import { BudgetingInstitutionModule } from './budgeting-institution/budgeting-institution.module';
import { ClassModule } from './class/class.module';
import { DailySyncAzureUsersModule } from './cli-scripts/sync-azure-users/daily-sync-azure-users.module';
import { EducationalStateModule } from './educational-state/educational-state.module';
import { EmailerModule } from './emailer/emailer.module';
import { EntitiesInGenerationModule } from './entities-in-generation/entities-in-generation.module';
import { ErrorNotificationModule } from './error-notification/error-notification.module';
import { GraphApiModule } from './graph-api/graph-api.module';
import { InstitutionModule } from './institution/institution.module';
import { JobsModule } from './jobs/jobs.module';
import { LeadTeacherModule } from './lead-teacher/lead-teacher.module';
import { LoggerModule } from './logging/logging.module';
import { LogsModule } from './logs/logs.module';
import { MonModule } from './mon/mon.module';
import { MunicipalityModule } from './municipality/municipality.module';
import { OtherAzureUsersModule } from './other-azure-users/other-azure-users.module';
import { ParentAccessModule } from './parent-access/parent-access.module';
import { ParentModule } from './parent/parent.module';
import { PersonModule } from './person/person.module';
import { RedisModule } from './redis/redis.module';
import { RequestContextModule } from './request-context/request-context.module';
import { RoleManagementModule } from './role-management/role-management.module';
import { RuoModule } from './ruo/ruo.module';
import { SchoolBookAccessModule } from './school-book-access/school-book-access.module';
import { SchoolBookCodeModule } from './school-book-code/school-book-code.module';
import { SchoolBooksModule } from './school-books/school-books.module';
import { StudentModule } from './student/student.module';
import { SystemUserModule } from './system-user/system-user.module';
import { TeacherModule } from './teacher/teacher.module';
import { TelelinkModule } from './telelink/telelink.module';
import { UserModule } from './user/user.module';
import { VariablesModule } from './variables/variables.module';
import { VersionModule } from './version/version.module';

const modelModules = [
    AccountantModule,
    AzureClassesModule,
    AzureEnrollmentsModule,
    AzureOrganizationsModule,
    AzureParentModule,
    AzureStudentModule,
    AzureTeacherModule,
    AzureUsersModule,
    BearerTokenModule,
    BudgetingInstitutionModule,
    ClassModule,
    DailySyncAzureUsersModule,
    EducationalStateModule,
    EmailerModule,
    EntitiesInGenerationModule,
    ErrorNotificationModule,
    GraphApiModule,
    InstitutionModule,
    JobsModule,
    LeadTeacherModule,
    LoggerModule,
    LogsModule,
    MonModule,
    MunicipalityModule,
    OtherAzureUsersModule,
    ParentModule,
    ParentAccessModule,
    PersonModule,
    RedisModule,
    RequestContextModule,
    RoleManagementModule,
    RuoModule,
    SchoolBookCodeModule,
    SchoolBookAccessModule,
    SchoolBooksModule,
    StudentModule,
    SystemUserModule,
    TeacherModule,
    TelelinkModule,
    UserModule,
    VariablesModule,
    VersionModule,
    AzureArchiveModule,
];
@Module({
    imports: [...modelModules],
    exports: [...modelModules],
})
export class ModelsModule {}
