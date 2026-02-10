import { Routes } from "@angular/router";
import { SaveToService } from "./auth/auth-guard.service";
import { BodyComponent } from "./pages/body/body.component";
import { LayoutComponent } from "./layouts/layout/layout.component";
import { HomeScreenComponent } from "./pages/home-screen/home-screen.component";
import { FormWrapperComponent } from "./pages/form-wrapper/form-wrapper.component";
import { CheckUpComponent } from "./pages/check-up/check-up.component";

export const AppRoutes: Routes = [
  {
    path: "home",
    component: LayoutComponent,
    children: [
      {
        path: "",
        component: HomeScreenComponent
      }
    ]
  },
  {
    path: "active-institutions",
    component: LayoutComponent,
    children: [
      {
        path: "",
        component: BodyComponent,
        canDeactivate: [SaveToService]
      }
    ]
  },
  {
    path: "inactive-institutions",
    component: LayoutComponent,
    children: [
      {
        path: "",
        component: BodyComponent,
        canDeactivate: [SaveToService]
      }
    ]
  },
  {
    path: "preview/:formName/:id",
    component: LayoutComponent,
    children: [
      {
        path: "",
        component: FormWrapperComponent
      }
    ]
  },
  {
    path: "check-up",
    component: LayoutComponent,
    children: [
      {
        path: "",
        component: CheckUpComponent
      }
    ]
  },
  { path: "**", redirectTo: "home" }
];
