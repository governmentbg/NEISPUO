import { BrowserModule } from "@angular/platform-browser";
import { LOCALE_ID, NgModule } from "@angular/core";

import { AppComponent } from "./app.component";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { LayoutModule } from "@angular/cdk/layout";
import { MatPaginatorIntl } from "@angular/material/paginator";
import { HttpClientModule, HTTP_INTERCEPTORS } from "@angular/common/http";
import { RouterModule } from "@angular/router";
import { AppRoutes } from "./app.routing";
import { JwtModule } from "@auth0/angular-jwt";
import { DigitOnlyDirective } from "./shared/digit-only.directive";
import { getBgPaginatorIntl } from "./shared/paginator-intl";
import { DomainInterceptorService } from "./services/domain-interceptor.service";
import { ResponseInterceptorService } from "./services/response-interceptor.service";
import { SnackbarService } from "./services/snackbar.service";
import { MenuProfileComponent } from "./layouts/menu-profile/menu-profile.component";
import { ModalComponent } from "./shared/modal/modal.component";
import { SharedModule } from "./shared/shared.module";
import { FormDataService } from "./services/form-data.service";
import { MessagesService } from "./services/messages.service";
import { BodyComponent } from "./pages/body/body.component";
import { HomeScreenComponent } from "./pages/home-screen/home-screen.component";
import { FormWrapperComponent } from "./pages/form-wrapper/form-wrapper.component";
import { InputComponent } from "./pages/dynamic-components/input/input.component";
import { SelectComponent } from "./pages/dynamic-components/select/select.component";
import { DateComponent } from "./pages/dynamic-components/date/date.component";
import { RadiobuttonComponent } from "./pages/dynamic-components/radiobutton/radiobutton.component";
import { CheckboxComponent } from "./pages/dynamic-components/checkbox/checkbox.component";
import { DynamicFieldDirective } from "./pages/form-wrapper/create-edit-form/dynamic-field/dynamic-field.directive";
import { CreateEditFormComponent } from "./pages/form-wrapper/create-edit-form/create-edit-form.component";
import { FormTypeDisplayPipe } from "./pipes/form-type.pipe";
import { TextareaComponent } from "./pages/dynamic-components/textarea/textarea.component";
import { TableComponent } from "./pages/table/table.component";
import { SubsectionComponent } from "./pages/form-wrapper/create-edit-form/subsection/subsection.component";
import { SearchSelectComponent } from "./pages/dynamic-components/searchselect/searchselect.component";
import { NgxMatSelectSearchModule } from "ngx-mat-select-search";
import { registerLocaleData } from "@angular/common";
import bg from "@angular/common/locales/bg";
import { ControlComponentComponent } from "./pages/form-wrapper/control-component/control-component.component";
import { HistoryControlComponent } from "./pages/form-wrapper/history-control/history-control.component";
import { LayoutComponent } from "./layouts/layout/layout.component";
import { HeaderComponent } from "./layouts/layout/header/header.component";
import { MainTabComponent } from "./layouts/layout/main-tab/main-tab.component";
import { ActiveDisplayPipe } from "./pipes/active.pipe";
import { KeepFiltersService } from "./services/keep-filters.service";
import { SearchmultiselectComponent } from "./pages/dynamic-components/searchmultiselect/searchmultiselect.component";
import { FileComponent } from "./pages/dynamic-components/file/file.component";
import { CheckUpComponent } from "./pages/check-up/check-up.component";
import { ButtonComponent } from "./pages/dynamic-components/button/button.component";
import { BreakpointComponent } from "./pages/dynamic-components/breakpoint/breakpoint.component";
registerLocaleData(bg);

@NgModule({
  declarations: [
    AppComponent,
    DigitOnlyDirective,
    ModalComponent,
    MenuProfileComponent,
    MainTabComponent,
    BodyComponent,
    HomeScreenComponent,
    FormWrapperComponent,
    InputComponent,
    ButtonComponent,
    SelectComponent,
    DateComponent,
    RadiobuttonComponent,
    CheckboxComponent,
    DynamicFieldDirective,
    CreateEditFormComponent,
    FormTypeDisplayPipe,
    ActiveDisplayPipe,
    TextareaComponent,
    TableComponent,
    SubsectionComponent,
    SearchSelectComponent,
    ControlComponentComponent,
    HistoryControlComponent,
    LayoutComponent,
    HeaderComponent,
    SearchmultiselectComponent,
    FileComponent,
    CheckUpComponent,
    BreakpointComponent
  ],
  imports: [
    BrowserModule,
    BrowserAnimationsModule,
    HttpClientModule,
    LayoutModule,
    SharedModule,
    NgxMatSelectSearchModule,
    RouterModule.forRoot(AppRoutes, { paramsInheritanceStrategy: "always" }),
    JwtModule.forRoot({
      config: {
        tokenGetter: () => {
          return localStorage.getItem("token") || sessionStorage.getItem("token");
        }
        // whitelistedDomains: ['localhost:3000']
      }
    })
    // ServiceWorkerModule.register("ngsw-worker.js", { enabled: environment.production })
  ],
  providers: [
    SnackbarService,
    FormDataService,
    MessagesService,
    KeepFiltersService,
    [
      { provide: HTTP_INTERCEPTORS, useClass: DomainInterceptorService, multi: true },
      { provide: HTTP_INTERCEPTORS, useClass: ResponseInterceptorService, multi: true }
    ],
    { provide: MatPaginatorIntl, useValue: getBgPaginatorIntl() },
    { provide: LOCALE_ID, useValue: "bg-BG" }
  ],
  bootstrap: [AppComponent],
  entryComponents: [
    ModalComponent,
    InputComponent,
    ButtonComponent,
    SelectComponent,
    DateComponent,
    RadiobuttonComponent,
    CheckboxComponent,
    BreakpointComponent
  ]
})
export class AppModule {}
