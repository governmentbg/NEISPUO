import { Routes } from "@angular/router";
import { AuthGuardAdmin, AuthGuardLogged, FormGuard } from "./auth/auth-guard.service";
import { BodyComponent } from "./layouts/full-control-layout/body/body.component";
import { FullControlLayoutComponent } from "./layouts/full-control-layout/full-control-layout.component";
import { VersionResolver } from './layouts/full-control-layout/version-resolver';
import { WrapperComponent } from "./layouts/full-control-layout/wrapper/wrapper.component";
import { FormCreationComponent } from "./pages/admin/form-creation.component";
import { FakePageComponent } from "./pages/fake-page/fake-page.component";
import { FormWrapperComponent } from "./pages/form-wrapper/form-wrapper.component";
import { HomeComponent } from "./pages/home/home.component";
import { SigninCallbackComponent } from "./pages/login/signin-callback/signin-callback.component";
import { SilentSigninCallbackComponent } from "./pages/login/silent-signin-callback/silent-signin-callback.component";
import { NewStaffMemberComponent } from "./pages/new-staff-member/new-staff-member.component";
import { NewStaffPositionComponent } from './pages/new-staff-position/new-staff-position.component';
import { NewSubjectInstitutionComponent } from "./pages/new-subject-institution/new-subject-institution.component";
import { UnauthorizedComponent } from "./pages/unauthorized/unauthorized.component";
import { InfoTableWrapperComponent } from "./pages/admin/info-table/info-table-wrapper/info-table-wrapper.component";

export const AppRoutes: Routes = [
  {
    path: "signin-callback",
    component: SigninCallbackComponent
  },
  {
    path: "silent-signin-callback",
    component: SilentSigninCallbackComponent
  },
  {
    path: "unauthorized",
    component: UnauthorizedComponent,
    canActivate: [AuthGuardLogged]
  },
  {
    path: "home",
    component: FullControlLayoutComponent,
    canActivate: [AuthGuardLogged],
    children: [
      {
        path: "",
        component: HomeComponent
      }
    ]
  },
  {
    path: "home/settings",
    component: FullControlLayoutComponent,
    canActivate: [AuthGuardAdmin],
    children: [
      {
        path: "",
        component: FormCreationComponent
      }
    ]
  },
  {
    path: "home/infotable",
    component: FullControlLayoutComponent,
    canActivate: [AuthGuardLogged],
    children: [
      {
        path: "",
        component: InfoTableWrapperComponent
      }
    ]
  },
  {
    path: "home/:type",
    component: FullControlLayoutComponent,
    canActivate: [AuthGuardLogged],
    resolve: {
      version: VersionResolver
    },
    children: [
      {
        path: "",
        component: BodyComponent,
        canActivate: [AuthGuardLogged],
        children: [
          {
            path: "data",
            component: FakePageComponent
          },
          {
            path: "list/:menuItem",
            component: WrapperComponent
          },
          {
            path: ":tab/:menuItem/:grandParentForm/:parentForm/:formName",
            component: WrapperComponent
          },
          {
            path: ":tab/:menuItem/:parentForm/:formName",
            component: WrapperComponent
          },
          {
            path: ":tab/:menuItem/:formName",
            component: WrapperComponent
          }
        ]
      }
    ]
  },
  {
    path: "new-staff-member",
    component: FullControlLayoutComponent,
    canActivate: [AuthGuardLogged],
    children: [
      {
        path: "",
        component: NewStaffMemberComponent
      }
    ]
  },
  {
    path: "new-subject-institution",
    component: FullControlLayoutComponent,
    canActivate: [AuthGuardLogged],
    children: [
      {
        path: "",
        component: NewSubjectInstitutionComponent
      }
    ]
  },
  {
    path: "new-staff-position",
    component: FullControlLayoutComponent,
    canActivate: [AuthGuardLogged],
    children: [
      {
        path: "",
        component: NewStaffPositionComponent
      }
    ]
  },
  {
    path: "create/:formName",
    component: FullControlLayoutComponent,
    canActivate: [AuthGuardLogged],
    children: [
      {
        path: "",
        component: FormWrapperComponent,
        canDeactivate: [FormGuard]
      }
    ]
  },
  {
    path: "edit/:formName",
    component: FullControlLayoutComponent,
    canActivate: [AuthGuardLogged],
    children: [
      {
        path: "",
        component: FormWrapperComponent,
        canDeactivate: [FormGuard]
      }
    ]
  },
  {
    path: "preview/:type/:formName",
    component: FullControlLayoutComponent,
    canActivate: [AuthGuardLogged],
    children: [
      {
        path: "",
        component: FormWrapperComponent,
        canDeactivate: [FormGuard]
      }
    ]
  },
  { path: "**", redirectTo: "home" }
];
