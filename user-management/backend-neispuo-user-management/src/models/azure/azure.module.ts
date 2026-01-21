import { Module } from '@nestjs/common';
import { AzureClassesModule } from './azure-classes/azure-classes.module';
import { AzureEnrollmentsModule } from './azure-enrollments/azure-enrollments.module';
import { AzureOrganizationsModule } from './azure-organizations/azure-organizations.module';
import { AzureParentModule } from './azure-parent/azure-parent.module';
import { AzureStudentModule } from './azure-student/azure-student.module';
import { AzureTeacherModule } from './azure-teacher/azure-teacher.module';
import { AzureUsersModule } from './azure-users/azure-users.module';

@Module({
    imports: [
        AzureClassesModule,
        AzureOrganizationsModule,
        AzureUsersModule,
        AzureStudentModule,
        AzureTeacherModule,
        AzureEnrollmentsModule,
        AzureParentModule,
    ],
    exports: [
        AzureClassesModule,
        AzureOrganizationsModule,
        AzureUsersModule,
        AzureStudentModule,
        AzureTeacherModule,
        AzureEnrollmentsModule,
        AzureParentModule,
    ],
})
export class AzureModule {}
