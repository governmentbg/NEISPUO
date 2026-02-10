// import { BrowserModule } from "@angular/platform-browser";
// import { LOCALE_ID, NgModule } from "@angular/core";

// import { AppComponent } from "./app.component";
// import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
// import { LayoutModule } from "@angular/cdk/layout";
// import { MatPaginatorIntl } from "@angular/material/paginator";
// import { HttpClientModule, HTTP_INTERCEPTORS } from "@angular/common/http";
// import { RouterModule } from "@angular/router";
// import { AppRoutes } from "./app.routing";
// import { DigitOnlyDirective } from "./shared/digit-only.directive";
// import { getBgPaginatorIntl } from "./shared/paginator-intl";
// import { DomainInterceptorService } from "./services/domain-interceptor.service";
// import { ResponseInterceptorService } from "./services/response-interceptor.service";
// import { SnackbarService } from "./services/snackbar.service";
// import { MenuProfileComponent } from "./layouts/menu-profile/menu-profile.component";
// import { ModalComponent } from "./shared/modal/modal.component";
// import { FullControlLayoutComponent } from "./layouts/full-control-layout/full-control-layout.component";
// import { FullControlHeaderComponent } from "./layouts/full-control-layout/full-control-header/full-control-header.component";
// import { TreeMenuComponent } from "./layouts/full-control-layout/tree-menu/tree-menu.component";
// import { MainTabComponent } from "./layouts/full-control-layout/main-tab/main-tab.component";
// import { SharedModule } from "./shared/shared.module";
// import { TreeDataService } from "./services/tree-data.service";
// import { FormDataService } from "./services/form-data.service";
// import { MessagesService } from "./services/messages.service";
// import { SecondMainTabComponent } from "./layouts/full-control-layout/wrapper/second-main-tab/second-main-tab.component";
// import { BodyComponent } from "./layouts/full-control-layout/body/body.component";
// import { FormWrapperComponent } from "./pages/form-wrapper/form-wrapper.component";
// import { InputComponent } from "./pages/dynamic-components/input/input.component";
// import { SelectComponent } from "./pages/dynamic-components/select/select.component";
// import { DateComponent } from "./pages/dynamic-components/date/date.component";
// import { CheckboxComponent } from "./pages/dynamic-components/checkbox/checkbox.component";
// import { DynamicFieldDirective } from "./pages/form-wrapper/create-edit-form/dynamic-field/dynamic-field.directive";
// import { CreateEditFormComponent } from "./pages/form-wrapper/create-edit-form/create-edit-form.component";
// import { TextareaComponent } from "./pages/dynamic-components/textarea/textarea.component";
// import { TableComponent } from "./pages/table/table.component";
// import { SubsectionComponent } from "./pages/form-wrapper/create-edit-form/subsection/subsection.component";
// import { SearchSelectComponent } from "./pages/dynamic-components/searchselect/searchselect.component";
// import { registerLocaleData } from "@angular/common";
// import bg from "@angular/common/locales/bg";
// import { FileComponent } from "./pages/dynamic-components/file/file.component";
// import { NgSelectModule } from "@ng-select/ng-select";
// import { NgOptionHighlightModule } from "@ng-select/ng-option-highlight";
// import { SilentSigninCallbackComponent } from "./pages/login/silent-signin-callback/silent-signin-callback.component";
// import { SigninCallbackComponent } from "./pages/login/signin-callback/signin-callback.component";
// import { UnauthorizedComponent } from "./pages/unauthorized/unauthorized.component";
// import { TabMenuBodyComponent } from "./layouts/full-control-layout/wrapper/tab-menu-body/tab-menu-body.component";
// import { WrapperComponent } from "./layouts/full-control-layout/wrapper/wrapper.component";
// import { HeadMenuBodyComponent } from "./layouts/full-control-layout/wrapper/head-menu-body/head-menu-body.component";
// import { JwtModule } from "@auth0/angular-jwt";
// import { environment } from "../environments/environment";
// import { HomeComponent } from "./pages/home/home.component";
// import { NewTableRecordComponent } from "./pages/table/new-table-record/new-table-record.component";
// import { DashboardMenuComponent } from "./layouts/dashboard-menu/dashboard-menu.component";
// import { NewStaffMemberComponent } from "./pages/new-staff-member/new-staff-member.component";
// import { MultipleAddComponent } from "./pages/table/multiple-add/multiple-add.component";
// import { UploadComponent } from "./shared/upload/upload.component";
// import { SwitchControlComponent } from "./layouts/full-control-layout/wrapper/switch-control/switch-control.component";
// import { LabelComponent } from "./pages/dynamic-components/label/label.component";
// import { InfoComponent } from "./pages/dynamic-components/info/info.component";
// import { HelpComponent } from "./pages/form-wrapper/create-edit-form/help/help.component";
// import { BreakpointComponent } from "./pages/dynamic-components/breakpoint/breakpoint.component";
// import { NgDompurifyModule } from "@tinkoff/ng-dompurify";
// import { HelperService } from "./services/helpers.service";
// import { FastPreviewComponent } from "./pages/table/fast-preview/fast-preview.component";
// import { MultiselectComponent } from "./pages/dynamic-components/multiselect/multiselect.component";
// import { CenterBarComponent } from "./pages/admin/center-bar/center-bar.component";
// import { CreateEditFormAdminComponent } from "./pages/admin/center-bar/create-edit-form-admin/create-edit-form-admin.component";
// import { CheckboxAdminComponent } from "./pages/admin/center-bar/create-edit-form-admin/dynamic-components/checkbox-admin/checkbox-admin.component";
// import { DateAdminComponent } from "./pages/admin/center-bar/create-edit-form-admin/dynamic-components/date-admin/date-admin.component";
// import { FileAdminComponent } from "./pages/admin/center-bar/create-edit-form-admin/dynamic-components/file-admin/file-admin.component";
// import { InfoAdminComponent } from "./pages/admin/center-bar/create-edit-form-admin/dynamic-components/info-admin/info-admin.component";
// import { InputAdminComponent } from "./pages/admin/center-bar/create-edit-form-admin/dynamic-components/input-admin/input-admin.component";
// import { LabelAdminComponent } from "./pages/admin/center-bar/create-edit-form-admin/dynamic-components/label-admin/label-admin.component";
// import { MultiselectAdminComponent } from "./pages/admin/center-bar/create-edit-form-admin/dynamic-components/multiselect-admin/multiselect-admin.component";
// import { SearchselectAdminComponent } from "./pages/admin/center-bar/create-edit-form-admin/dynamic-components/searchselect-admin/searchselect-admin.component";
// import { SelectAdminComponent } from "./pages/admin/center-bar/create-edit-form-admin/dynamic-components/select-admin/select-admin.component";
// import { TextareaAdminComponent } from "./pages/admin/center-bar/create-edit-form-admin/dynamic-components/textarea-admin/textarea-admin.component";
// import { SubsectionAdminComponent } from "./pages/admin/center-bar/create-edit-form-admin/subsection-admin/subsection-admin.component";
// import { TableAdminComponent } from "./pages/admin/center-bar/create-edit-form-admin/table-admin/table-admin.component";
// import { LeftBarComponent } from "./pages/admin/left-bar/left-bar.component";
// import { RightBarComponent } from "./pages/admin/right-bar/right-bar.component";
// import { DynamicFieldAdminDirective } from "./pages/admin/center-bar/create-edit-form-admin/dynamic-field/dynamic-field.directive";
// import { FormCreationComponent } from "./pages/admin/form-creation.component";
// import { FakePageComponent } from "./pages/fake-page/fake-page.component";
// import { AddressControlComponent } from "./layouts/full-control-layout/wrapper/address-control/address-control.component";
// import { PeriodFiltersComponent } from "./layouts/full-control-layout/wrapper/period-filters/period-filters.component";

// registerLocaleData(bg);

// @NgModule({
//   declarations: [
//     AppComponent,
//     DigitOnlyDirective,
//     ModalComponent,
//     MenuProfileComponent,
//     MainTabComponent,
//     FullControlLayoutComponent,
//     FullControlHeaderComponent,
//     TreeMenuComponent,
//     SecondMainTabComponent,
//     BodyComponent,
//     FormWrapperComponent,
//     InputComponent,
//     SelectComponent,
//     DateComponent,
//     CheckboxComponent,
//     DynamicFieldDirective,
//     DynamicFieldAdminDirective,
//     CreateEditFormComponent,
//     TextareaComponent,
//     TableComponent,
//     SubsectionComponent,
//     SearchSelectComponent,
//     FileComponent,
//     SilentSigninCallbackComponent,
//     SigninCallbackComponent,
//     UnauthorizedComponent,
//     TabMenuBodyComponent,
//     WrapperComponent,
//     HeadMenuBodyComponent,
//     HomeComponent,
//     NewTableRecordComponent,
//     DashboardMenuComponent,
//     NewStaffMemberComponent,
//     MultipleAddComponent,
//     SwitchControlComponent,
//     LabelComponent,
//     InfoComponent,
//     HelpComponent,
//     BreakpointComponent,
//     FastPreviewComponent,
//     MultiselectComponent,
//     AddressControlComponent,
//     CenterBarComponent,
//     CreateEditFormAdminComponent,
//     CheckboxAdminComponent,
//     DateAdminComponent,
//     FileAdminComponent,
//     InfoAdminComponent,
//     InputAdminComponent,
//     LabelAdminComponent,
//     MultiselectAdminComponent,
//     SearchselectAdminComponent,
//     SelectAdminComponent,
//     TextareaAdminComponent,
//     SubsectionAdminComponent,
//     TableAdminComponent,
//     LeftBarComponent,
//     RightBarComponent,
//     FormCreationComponent,
//     PeriodFiltersComponent,
//     FakePageComponent
//   ],
//   imports: [
//     BrowserModule,
//     BrowserAnimationsModule,
//     NgSelectModule,
//     NgOptionHighlightModule,
//     HttpClientModule,
//     LayoutModule,
//     SharedModule,
//     NgDompurifyModule,
//     RouterModule.forRoot(AppRoutes, { paramsInheritanceStrategy: "always" }),
//     JwtModule.forRoot({
//       config: {
//         tokenGetter: () => {
//           const tokenName = `oidc.user:${environment.oidcBaseUrl}:inst_basic_staging`;
//           const jsonToken =
//             JSON.parse(localStorage.getItem(tokenName)) || JSON.parse(sessionStorage.getItem(tokenName));
//           return jsonToken ? jsonToken.id_token : null;
//         }
//         // whitelistedDomains: ['localhost:3000']
//       }
//     })
//   ],
//   providers: [
//     SnackbarService,
//     TreeDataService,
//     FormDataService,
//     HelperService,
//     MessagesService,
//     [
//       { provide: HTTP_INTERCEPTORS, useClass: DomainInterceptorService, multi: true },
//       { provide: HTTP_INTERCEPTORS, useClass: ResponseInterceptorService, multi: true }
//     ],
//     { provide: MatPaginatorIntl, useValue: getBgPaginatorIntl() },
//     { provide: LOCALE_ID, useValue: "bg-BG" }
//   ],
//   bootstrap: [AppComponent],
//   entryComponents: [
//     ModalComponent,
//     InputComponent,
//     SelectComponent,
//     DateComponent,
//     CheckboxComponent,
//     TableComponent,
//     MultipleAddComponent,
//     UploadComponent,
//     LabelComponent,
//     InfoComponent,
//     BreakpointComponent
//   ]
// })
// export class AppModule {}
