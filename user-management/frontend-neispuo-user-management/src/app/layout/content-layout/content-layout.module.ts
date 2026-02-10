import { NgModule } from '@angular/core';
import { HeadingModule } from 'src/app/shared/components/heading/heading.module';
import { SharedModule } from 'src/app/shared/shared.module';
import { FooterModule } from './components/footer/footer.module';
import { HeaderModule } from './components/header/header.module';
import { AzureClassesPageModule } from './components/pages/azure-classes-page/azure-classes-page.module';
import { AzureEnrollmentsPageModule } from './components/pages/azure-enrollments-page/azure-enrollments-page.module';
import { AzureOrganizationsPageModule } from './components/pages/azure-organizations-page/azure-organizations-page.module';
import { AzureUsersPageModule } from './components/pages/azure-users-page/azure-users-page.module';
import { BudgetInstitutionPageModule } from './components/pages/budget-institution-page/budget-institution-page.module';
import { HomePageModule } from './components/pages/home-page/home-page.module';
import { InstitutionPageModule } from './components/pages/institution-page/institution-page.module';
import { JobsPageModule } from './components/pages/jobs-page/jobs-page.module';
import { MonPageModule } from './components/pages/mon-page/mon-page.module';
import { MunicipalityPageModule } from './components/pages/municipality-page/municipality-page.module';
import { NoRightsPageModule } from './components/pages/no-rights-page/no-rights-page.module';
import { RuoPageModule } from './components/pages/ruo-page/ruo-page.module';
import { StudentUsersPageModule } from './components/pages/student-users-page/student-users-page.module';
import { TeacherPageModule } from './components/pages/teacher-page/teacher-page.module';
import { UpdateInstitutionPageModule } from './components/pages/update-institution-page/update-institution-page.module';
import { UpdateRolesPageModule } from './components/pages/update-roles-page/update-roles-page.module';
import { SideMenuModule } from './components/side-menu/side-menu.module';
import { ContentLayoutComponent } from './content-layout.component';
import { RoleAuditPageModule } from './components/pages/role-audit-page/role-audit-page.module';
import { TeacherClassPageModule } from './components/pages/teacher-class-page/teacher-class-page.module';
import { LoginAuditPageModule } from './components/pages/login-audit-page/login-audit-page.module';
import { OtherAzureUsersPageModule } from './components/pages/other-azure-users-page/other-azure-users-page.module';
import { ErrorsPageModule } from './components/pages/errors-page/errors-page.module';
import { CreateUserPageModule } from './components/pages/create-user-page/create-user-page.module';
import { ParentPageModule } from './components/pages/parent-page/parent-page.module';
import { SchoolBooksAccessPageModule } from './components/pages/school-books-access/school-books-access-page.module';
import { SyncHistoryModule } from './components/pages/sync-history/sync-history.module';
import { LinkedUsersPageModule } from './components/pages/linked-users-page/linked-users-page.module';

@NgModule({
    declarations: [ContentLayoutComponent],
    imports: [
        AzureClassesPageModule,
        AzureEnrollmentsPageModule,
        AzureOrganizationsPageModule,
        AzureUsersPageModule,
        BudgetInstitutionPageModule,
        CreateUserPageModule,
        ErrorsPageModule,
        FooterModule,
        HeaderModule,
        HeadingModule,
        HomePageModule,
        InstitutionPageModule,
        JobsPageModule,
        LoginAuditPageModule,
        MonPageModule,
        MunicipalityPageModule,
        NoRightsPageModule,
        OtherAzureUsersPageModule,
        ParentPageModule,
        RoleAuditPageModule,
        RuoPageModule,
        SharedModule,
        SideMenuModule,
        StudentUsersPageModule,
        TeacherClassPageModule,
        TeacherPageModule,
        SchoolBooksAccessPageModule,
        UpdateInstitutionPageModule,
        UpdateRolesPageModule,
        SyncHistoryModule,
        LinkedUsersPageModule,
    ],
    exports: [ContentLayoutComponent],
})
export class ContentLayoutModule {}
