import { Routes } from "@angular/router";
import { AuthGuardAdmin, AuthGuardLogged, FormGuard } from "./auth/auth-guard.service";
import { BodyComponent } from "./layouts/full-control-layout/body/body.component";
import { FullControlLayoutComponent } from "./layouts/full-control-layout/full-control-layout.component";
import { FormCreationComponent } from "./pages/admin/form-creation.component";
import { CheckUpComponent } from "./pages/check-up/check-up.component";
import { FormWrapperComponent } from "./pages/form-wrapper/form-wrapper.component";
import { HomeScreenComponent } from "./pages/home-screen/home-screen.component";
import { SigninCallbackComponent } from "./pages/login/signin-callback/signin-callback.component";
import { SilentSigninCallbackComponent } from "./pages/login/silent-signin-callback/silent-signin-callback.component";
import { NewInstitutionComponent } from "./pages/new-institution/new-institution.component";
import { UnauthorizedComponent } from "./pages/unauthorized/unauthorized.component";

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
        component: HomeScreenComponent
      }
    ]
  },
  {
    path: "active-institutions/:type",
    component: FullControlLayoutComponent,
    canActivate: [AuthGuardLogged],
    children: [
      {
        path: "",
        component: BodyComponent,
        children: [
          {
            path: ":formName",
            component: FormWrapperComponent
          }
        ]
      }
    ]
  },
  {
    path: "inactive-institutions/:type",
    component: FullControlLayoutComponent,
    canActivate: [AuthGuardLogged],
    children: [
      {
        path: "",
        component: BodyComponent,
        children: [
          {
            path: ":formName",
            component: FormWrapperComponent
          }
        ]
      }
    ]
  },
  {
    path: "check-up",
    component: FullControlLayoutComponent,
    canActivate: [AuthGuardLogged],
    children: [
      {
        path: "",
        component: CheckUpComponent
      }
    ]
  },
  {
    path: "settings",
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
    path: "new-institution",
    component: FullControlLayoutComponent,
    canActivate: [AuthGuardLogged],
    children: [
      {
        path: "",
        component: NewInstitutionComponent
      }
    ]
  },
  {
    path: "create-procedure/:formName",
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
    path: "edit-procedure/:formName",
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
