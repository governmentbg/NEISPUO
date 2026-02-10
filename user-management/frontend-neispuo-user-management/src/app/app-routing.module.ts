import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { HomeRedirectGuard } from '@core/guard/home-redirect.guard';
import { LeadTeacherGuard } from '@core/guard/lead-teacher.guard';
import { RoleEnum } from '@shared/enums/roles.enum';
import { AuthGuard } from './core/guard/auth.guard';
import { NoAuthGuard } from './core/guard/no-auth.guard';
import { RoleGuard } from './core/guard/role.guard';
import { AzureClassesPageComponent } from './layout/content-layout/components/pages/azure-classes-page/azure-classes-page.component';
import { AzureEnrollmentsPageComponent } from './layout/content-layout/components/pages/azure-enrollments-page/azure-enrollments-page.component';
import { AzureOrganizationsPageComponent } from './layout/content-layout/components/pages/azure-organizations-page/azure-organizations-page.component';
import { AzureUsersPageComponent } from './layout/content-layout/components/pages/azure-users-page/azure-users-page.component';
import { BudgetInstitutionPageComponent } from './layout/content-layout/components/pages/budget-institution-page/budget-institution-page.component';
import { ErrorsPageComponent } from './layout/content-layout/components/pages/errors-page/errors-page.component';
import { HomePageComponent } from './layout/content-layout/components/pages/home-page/home-page.component';
import { InstitutionPageComponent } from './layout/content-layout/components/pages/institution-page/institution-page.component';
import { JobsPageComponent } from './layout/content-layout/components/pages/jobs-page/jobs-page.component';
import { LinkedUsersPageComponent } from './layout/content-layout/components/pages/linked-users-page/linked-users-page.component';
import { LoginAuditPageComponent } from './layout/content-layout/components/pages/login-audit-page/login-audit-page.component';
import { MonPageComponent } from './layout/content-layout/components/pages/mon-page/mon-page.component';
import { MunicipalityPageComponent } from './layout/content-layout/components/pages/municipality-page/municipality-page.component';
import { NoRightsPageComponent } from './layout/content-layout/components/pages/no-rights-page/no-rights-page.component';
import { OtherAzureUsersPageComponent } from './layout/content-layout/components/pages/other-azure-users-page/other-azure-users-page.component';
import { ParentPageComponent } from './layout/content-layout/components/pages/parent-page/parent-page.component';
import { RoleAuditPageComponent } from './layout/content-layout/components/pages/role-audit-page/role-audit-page.component';
import { RuoPageComponent } from './layout/content-layout/components/pages/ruo-page/ruo-page.component';
import { SchoolBooksAccessPageComponent } from './layout/content-layout/components/pages/school-books-access/school-books-access-page/school-books-access-page.component';
import { StudentUsersPageComponent } from './layout/content-layout/components/pages/student-users-page/student-users-page.component';
import { SyncHistoryComponent } from './layout/content-layout/components/pages/sync-history/sync-history.component';
import { TeacherClassPageComponent } from './layout/content-layout/components/pages/teacher-class-page/teacher-class-page.component';
import { TeacherPageComponent } from './layout/content-layout/components/pages/teacher-page/teacher-page.component';
import { UpdateInstitutionPageComponent } from './layout/content-layout/components/pages/update-institution-page/update-institution-page.component';
import { UpdateRolesPageComponent } from './layout/content-layout/components/pages/update-roles-page/update-roles-page.component';
import { ContentLayoutComponent } from './layout/content-layout/content-layout.component';
import { LoginPageComponent } from './layout/login-layout/components/pages/login-page/login-page.component';
import { SignInCallbackPageComponent } from './layout/login-layout/components/pages/signin-callback-page/signin-callback-page.component';
import { SilentSigninCallbackPageComponent } from './layout/login-layout/components/pages/silent-signin-callback-page/silent-signin-callback-page.component';
import { LoginLayoutComponent } from './layout/login-layout/login-layout.component';
import { CONSTANTS } from './shared/constants';

const routes: Routes = [
    {
        path: CONSTANTS.ROUTER_PATH_LOGIN_LAYOUT,
        pathMatch: 'full',
        redirectTo: `${CONSTANTS.ROUTER_PATH_LOGIN_LAYOUT}/${CONSTANTS.ROUTER_PATH_LOGIN}`,
    },
    {
        path: CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT,
        pathMatch: 'full',
        redirectTo: `${CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT}/${CONSTANTS.ROUTER_PATH_HOME}`,
    },
    {
        path: CONSTANTS.ROUTER_PATH_LOGIN_LAYOUT,
        component: LoginLayoutComponent,
        children: [
            {
                path: CONSTANTS.ROUTER_PATH_LOGIN,
                component: LoginPageComponent,
                data: { title: 'Вход' },
                canActivate: [NoAuthGuard],
            },
            {
                path: CONSTANTS.ROUTER_PATH_SIGN_IN_CALLBACK,
                component: SignInCallbackPageComponent,
                data: { title: 'Вход' },
            },
            {
                path: CONSTANTS.ROUTER_PATH_SILENT_SIGN_IN_CALLBACK,
                component: SilentSigninCallbackPageComponent,
                data: { title: 'Вход' },
            },
        ],
    },
    {
        path: CONSTANTS.ROUTER_PATH_CONTENT_LAYOUT,
        component: ContentLayoutComponent,
        canActivate: [AuthGuard],
        children: [
            {
                path: CONSTANTS.ROUTER_PATH_HOME,
                component: HomePageComponent,
                data: {
                    title: `${CONSTANTS.SIDEMENU_TAB_TITLE_HOME}`,
                    roles: [RoleEnum.MON_ADMIN, RoleEnum.CIOO, RoleEnum.RUO, RoleEnum.INSTITUTION, RoleEnum.TEACHER],
                },
                canActivate: [RoleGuard, HomeRedirectGuard],
            },
            {
                path: CONSTANTS.ROUTER_PATH_TEACHER_CLASSES,
                component: TeacherClassPageComponent,
                data: {
                    title: `${CONSTANTS.SIDEMENU_TAB_TITLE_TEACHER_CLASSES}`,
                },
                canActivate: [LeadTeacherGuard],
            },
            {
                path: CONSTANTS.ROUTER_PATH_STUDENTS,
                component: StudentUsersPageComponent,
                data: {
                    title: `${CONSTANTS.SIDEMENU_TAB_TITLE_STUDENTS}`,
                    roles: [
                        RoleEnum.MON_ADMIN,
                        RoleEnum.CIOO,
                        RoleEnum.CONSORTIUM_HELPDESK,
                        RoleEnum.RUO,
                        RoleEnum.INSTITUTION,
                    ],
                },
                canActivate: [RoleGuard],
            },
            {
                path: `${CONSTANTS.ROUTER_PATH_STUDENTS}/:personID/${CONSTANTS.ROUTER_PATH_SYNC_HISTORY}`,
                component: SyncHistoryComponent,
                data: {
                    roles: [RoleEnum.MON_ADMIN],
                },
                canActivate: [RoleGuard],
            },
            {
                path: `${CONSTANTS.ROUTER_PATH_STUDENTS}/:personID/${CONSTANTS.ROUTER_PATH_LINKED_USERS}`,
                component: LinkedUsersPageComponent,
                data: {
                    roles: [RoleEnum.MON_ADMIN],
                },
                canActivate: [RoleGuard],
            },
            {
                path: CONSTANTS.ROUTER_PATH_PARENTS,
                component: ParentPageComponent,
                data: {
                    title: `${CONSTANTS.SIDEMENU_TAB_TITLE_PARENTS}`,
                    roles: [
                        RoleEnum.MON_ADMIN,
                        RoleEnum.CIOO,
                        RoleEnum.CONSORTIUM_HELPDESK,
                        RoleEnum.RUO,
                        RoleEnum.INSTITUTION,
                    ],
                },
                canActivate: [RoleGuard],
            },
            {
                path: `${CONSTANTS.ROUTER_PATH_PARENTS}/:personID/${CONSTANTS.ROUTER_PATH_SYNC_HISTORY}`,
                component: SyncHistoryComponent,
                data: {
                    roles: [RoleEnum.MON_ADMIN],
                },
                canActivate: [RoleGuard],
            },
            {
                path: `${CONSTANTS.ROUTER_PATH_PARENTS}/:personID/${CONSTANTS.ROUTER_PATH_LINKED_USERS}`,
                component: LinkedUsersPageComponent,
                data: {
                    roles: [RoleEnum.MON_ADMIN],
                },
                canActivate: [RoleGuard],
            },
            {
                path: CONSTANTS.ROUTER_PATH_TEACHERS,
                component: TeacherPageComponent,
                data: {
                    title: `${CONSTANTS.SIDEMENU_TAB_TITLE_TEACHERS}`,
                    roles: [
                        RoleEnum.MON_ADMIN,
                        RoleEnum.CIOO,
                        RoleEnum.CONSORTIUM_HELPDESK,
                        RoleEnum.RUO,
                        RoleEnum.INSTITUTION,
                    ],
                },
                canActivate: [RoleGuard],
            },
            {
                path: `${CONSTANTS.ROUTER_PATH_TEACHERS}/:personID/${CONSTANTS.ROUTER_PATH_SYNC_HISTORY}`,
                component: SyncHistoryComponent,
                data: {
                    roles: [RoleEnum.MON_ADMIN],
                },
                canActivate: [RoleGuard],
            },
            {
                path: CONSTANTS.ROUTER_PATH_INSTITUTIONS,
                component: InstitutionPageComponent,
                data: {
                    title: `${CONSTANTS.SIDEMENU_TAB_TITLE_INSTITUTIONS}`,
                    roles: [
                        RoleEnum.MON_ADMIN,
                        RoleEnum.CIOO,
                        RoleEnum.CONSORTIUM_HELPDESK,
                        RoleEnum.RUO,
                        RoleEnum.INSTITUTION,
                    ],
                },
                canActivate: [RoleGuard],
            },
            {
                path: `${CONSTANTS.ROUTER_PATH_AUDIT}/${CONSTANTS.ROUTER_PATH_ROLE_AUDIT}`,
                component: RoleAuditPageComponent,
                data: {
                    title: `${CONSTANTS.SIDEMENU_TAB_TITLE_ROLE_AUDIT}`,
                    roles: [RoleEnum.MON_ADMIN, RoleEnum.CIOO, RoleEnum.CONSORTIUM_HELPDESK, RoleEnum.INSTITUTION],
                },
                canActivate: [RoleGuard],
            },
            {
                path: `${CONSTANTS.ROUTER_PATH_AUDIT}/${CONSTANTS.ROUTER_PATH_LOGIN_AUDIT}`,
                component: LoginAuditPageComponent,
                data: {
                    title: `${CONSTANTS.SIDEMENU_TAB_TITLE_LOGIN_AUDIT}`,
                    roles: [RoleEnum.MON_ADMIN, RoleEnum.CIOO, RoleEnum.CONSORTIUM_HELPDESK],
                },
                canActivate: [RoleGuard],
            },
            {
                path: `${CONSTANTS.ROUTER_PATH_INSTITUTIONS}/${CONSTANTS.ROUTER_PATH_INSTITUTIONS_UPDATE}/:institutionID`,
                component: UpdateInstitutionPageComponent,
                data: { title: `${CONSTANTS.SIDEMENU_TAB_TITLE_INSTITUTIONS_UPDATE}` },
            },
            {
                path: `${CONSTANTS.ROUTER_PATH_INSTITUTIONS}/:institutionID/${CONSTANTS.ROUTER_PATH_SYNC_HISTORY}`,
                component: SyncHistoryComponent,
                data: {
                    roles: [RoleEnum.MON_ADMIN],
                },
                canActivate: [RoleGuard],
            },
            {
                path: `${CONSTANTS.ROUTER_PATH_TEACHERS}/${CONSTANTS.ROUTER_PATH_ROLES_UPDATE}`,
                component: UpdateRolesPageComponent,
                data: { title: `${CONSTANTS.SIDEMENU_TAB_TITLE_ROLES_UPDATE}` },
            },
            {
                path: `${CONSTANTS.ROUTER_PATH_TEACHERS}/:institutionId/:personId/${CONSTANTS.ROUTER_PATH_SCHOOL_BOOKS_ACCESS_UPDATE}`,
                component: SchoolBooksAccessPageComponent,
                data: { title: `${CONSTANTS.SIDEMENU_TAB_SCHOOL_BOOKS_ACCESS_UPDATE}` },
            },
            {
                path: CONSTANTS.ROUTER_PATH_MON,
                component: MonPageComponent,
                data: {
                    title: `${CONSTANTS.SIDEMENU_TAB_TITLE_MON}`,
                    roles: [RoleEnum.MON_ADMIN, RoleEnum.CONSORTIUM_HELPDESK],
                },
                canActivate: [RoleGuard],
            },
            {
                path: CONSTANTS.ROUTER_PATH_RUO,
                component: RuoPageComponent,
                data: {
                    title: `${CONSTANTS.SIDEMENU_TAB_TITLE_RUO}`,
                    roles: [RoleEnum.MON_ADMIN, RoleEnum.CIOO, RoleEnum.CONSORTIUM_HELPDESK, RoleEnum.RUO],
                },
                canActivate: [RoleGuard],
            },
            {
                path: CONSTANTS.ROUTER_PATH_MUNICIPALITIES,
                component: MunicipalityPageComponent,
                data: {
                    title: `${CONSTANTS.SIDEMENU_TAB_TITLE_MUNICIPALITIES}`,
                    roles: [RoleEnum.MON_ADMIN, RoleEnum.CIOO, RoleEnum.CONSORTIUM_HELPDESK, RoleEnum.RUO],
                },
                canActivate: [RoleGuard],
            },
            {
                path: CONSTANTS.ROUTER_PATH_BUDGET_INSTITUTIONS,
                component: BudgetInstitutionPageComponent,
                data: {
                    title: `${CONSTANTS.SIDEMENU_TAB_TITLE_BUDGET_INSTITUTIONS}`,
                    roles: [RoleEnum.MON_ADMIN, RoleEnum.CIOO, RoleEnum.CONSORTIUM_HELPDESK],
                },
                canActivate: [RoleGuard],
            },
            {
                path: CONSTANTS.ROUTER_PATH_OTHER_USERS,
                component: OtherAzureUsersPageComponent,
                data: {
                    title: `${CONSTANTS.SIDEMENU_TAB_TITLE_OTHER_USERS}`,
                    roles: [RoleEnum.MON_ADMIN, RoleEnum.CIOO, RoleEnum.CONSORTIUM_HELPDESK],
                },
                canActivate: [RoleGuard],
            },
            {
                path: CONSTANTS.ROUTER_PATH_CLASSES,
                component: AzureClassesPageComponent,
                data: {
                    title: `${CONSTANTS.SIDEMENU_TAB_TITLE_CLASSES}`,
                    roles: [RoleEnum.MON_ADMIN, RoleEnum.CIOO, RoleEnum.CONSORTIUM_HELPDESK],
                },
                canActivate: [RoleGuard],
            },
            {
                path: CONSTANTS.ROUTER_PATH_ENROLLMENTS,
                component: AzureEnrollmentsPageComponent,
                data: {
                    title: `${CONSTANTS.SIDEMENU_TAB_TITLE_ENROLLMENTS}`,
                    roles: [RoleEnum.MON_ADMIN, RoleEnum.CIOO, RoleEnum.CONSORTIUM_HELPDESK],
                },
                canActivate: [RoleGuard],
            },
            {
                path: CONSTANTS.ROUTER_PATH_ORGANIZATIONS,
                component: AzureOrganizationsPageComponent,
                data: {
                    title: `${CONSTANTS.SIDEMENU_TAB_TITLE_ORGANIZATIONS}`,
                    roles: [RoleEnum.MON_ADMIN, RoleEnum.CIOO, RoleEnum.CONSORTIUM_HELPDESK],
                },
                canActivate: [RoleGuard],
            },
            {
                path: CONSTANTS.ROUTER_PATH_USERS,
                component: AzureUsersPageComponent,
                data: {
                    title: `${CONSTANTS.SIDEMENU_TAB_TITLE_USERS}`,
                    roles: [RoleEnum.MON_ADMIN, RoleEnum.CIOO, RoleEnum.CONSORTIUM_HELPDESK],
                },
                canActivate: [RoleGuard],
            },
            {
                path: CONSTANTS.ROUTER_PATH_ERRORS,
                component: ErrorsPageComponent,
                data: {
                    title: `${CONSTANTS.SIDEMENU_TAB_TITLE_ERRORS}`,
                    roles: [RoleEnum.MON_ADMIN],
                },
                canActivate: [RoleGuard],
            },
            /*
            {
                path: CONSTANTS.ROUTER_PATH_CREATE_USER,
                component: CreateUserPageComponent,
                data: {
                    title: `${CONSTANTS.SIDEMENU_TAB_TITLE_CREATE_USER}`,
                    roles: [
                        RoleEnum.MON_ADMIN,
                        RoleEnum.CIOO,
                        RoleEnum.RUO,
                        RoleEnum.RUO_EXPERT,
                        RoleEnum.CONSORTIUM_HELPDESK,
                    ],
                },
                canActivate: [RoleGuard],
            },
            Commenting feature out due to reworked lists for teachers and students. since we combined the create and update page. if no problem occur in the future after 09.12.22 please delete this commented code. Ivelin.
            */
            {
                path: CONSTANTS.ROUTER_PATH_JOBS,
                component: JobsPageComponent,
                data: {
                    title: `${CONSTANTS.SIDEMENU_TAB_TITLE_JOBS}`,
                    roles: [RoleEnum.MON_ADMIN],
                },
                canActivate: [RoleGuard],
            },
            {
                path: CONSTANTS.ROUTER_PATH_UNAUTHORIZED,
                component: NoRightsPageComponent,
                data: { title: `${CONSTANTS.SIDEMENU_TAB_TITLE_UNAUTHORIZED}` },
                canActivate: [],
            },
        ],
    },

    {
        path: '**',
        redirectTo: `${CONSTANTS.ROUTER_PATH_LOGIN_LAYOUT}/${CONSTANTS.ROUTER_PATH_LOGIN}`,
    },
];

@NgModule({
    imports: [RouterModule.forRoot(routes)],
    exports: [RouterModule],
})
export class AppRoutingModule {}
