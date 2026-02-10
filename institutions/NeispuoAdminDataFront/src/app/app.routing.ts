import { Routes } from "@angular/router";
import { AuthGuardLogged, FormGuard } from "./auth/auth-guard.service";
import { BodyComponent } from "./layouts/full-control-layout/body/body.component";
import { FullControlLayoutComponent } from "./layouts/full-control-layout/full-control-layout.component";
import { FormCreationComponent } from "./pages/admin/form-creation.component";
import { FormWrapperComponent } from "./pages/form-wrapper/form-wrapper.component";
import { SigninCallbackComponent } from "./pages/login/signin-callback/signin-callback.component";
import { SilentSigninCallbackComponent } from "./pages/login/silent-signin-callback/silent-signin-callback.component";
import { UnauthorizedComponent } from "./pages/unauthorized/unauthorized.component";
import { AppService } from "./app.service";
import { WrapperComponent } from "./layouts/full-control-layout/body/wrapper/wrapper.component";

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
    path: "settings",
    component: FullControlLayoutComponent,
    children: [
      {
        path: "",
        component: FormCreationComponent
      }
    ]
  },
  {
    path: "home",
    component: FullControlLayoutComponent,
    canActivate: [AuthGuardLogged],
    resolve: {
      init: AppService
    },
    children: [
      {
        path: "",
        component: BodyComponent,
        canActivate: [AuthGuardLogged],
        children: [
          {
            path: ":menuItem/:grandParentForm/:parentForm/:formName",
            component: WrapperComponent
          },
          {
            path: ":menuItem/:parentForm/:formName",
            component: WrapperComponent
          },
          {
            path: ":menuItem/:formName",
            component: WrapperComponent
          }
        ]
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
    path: "preview/:formName",
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
