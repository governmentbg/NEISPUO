import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { PortalModule } from '@shared/modules/portal/portal.module';
import { SharedModule } from '@shared/shared.module';
import { FilterService } from 'primeng/api';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';
import { CardModule } from 'primeng/card';
import { CheckboxModule } from 'primeng/checkbox';
import { DropdownModule } from 'primeng/dropdown';
import { DynamicDialogModule } from 'primeng/dynamicdialog';
import { FieldsetModule } from 'primeng/fieldset';
import { InputTextModule } from 'primeng/inputtext';
import { RadioButtonModule } from 'primeng/radiobutton';
import { StepsModule } from 'primeng/steps';
import { TableModule } from 'primeng/table';
import { ToolbarModule } from 'primeng/toolbar';
import { InputNumberModule } from 'primeng/inputnumber';
import { MessageModule } from 'primeng/message';
import { MessagesModule } from 'primeng/messages';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { InstitutionCardDetailsComponent } from './components/institution-card-details/institution-card-details.component';
import { InstitutionFlexFieldComponent } from './components/institution-flex-field/institution-flex-field.component';
import { InstitutionHeadmasterDataComponent } from './components/institution-headmaster-data/institution-headmaster-data.component';
import { InstitutionIdentifierDataComponent } from './components/institution-identifier-data/institution-identifier-data.component';
import { InstitutionProcedureDataComponent } from './components/institution-procedure-data/institution-procedure-data.component';
import { InstitutionTypeDataComponent } from './components/institution-type-data/institution-type-data.component';
import { RmtSelectionComponent } from './components/rmt-selection/rmt-selection.component';
import { MunicipalInstitutionRoutingModule } from './municipal-institutions-routing.module';
import { MiBulstatLoaderComponent } from './pages/mi-bulstat-loader/mi-bulstat-loader.component';
import { MiCreateNewComponent } from './pages/mi-create-new/mi-create-new.component';
import { MiCreationStepperComponent } from './pages/mi-creation-stepper/mi-creation-stepper.component';
import { MunicipalInstitutionDetailModalPage } from './pages/municipal-institution-detail-modal/municipal-institution-detail-modal.page';
import { MunicipalInstitutionListPage } from './pages/municipal-institution-list/municipal-institution-list.page';
import { MICommonFormGroupService } from './services/mi-common-form-group.service';
import { MunicipalInstitutionService } from './state/municipal-institutions/municipal-institution.service';
import { MunicipalPublicRegisterPageComponent } from './pages/municipal-public-register-page/municipal-public-register-page.component';
import { RiDocumentComponent } from './components/ri-document/ri-document.component';
import { PremInstitutionDataComponent } from './components/prem-institution-data/prem-institution-data.component';
import { ACLMunicipalityInstitutionService } from './services/acl-mi.service';
import { MiFormComponent } from './components/mi-form/mi-form.component';
import { MiEditExistingComponent } from './pages/mi-edit-existing/mi-edit-existing.component';
import { MiPreviewExistingComponent } from './pages/mi-preview-existing/mi-preview-existing.component';
import { InstitutionDepartmentDataComponent } from './components/institution-department-data/institution-department-data.component';
import { MiDeleteExistingComponent } from './pages/mi-delete-existing/mi-delete-existing.component';
import {DialogModule} from 'primeng/dialog';


@NgModule({
  declarations: [
    MunicipalInstitutionListPage,
    MunicipalInstitutionDetailModalPage,
    InstitutionCardDetailsComponent,
    RmtSelectionComponent,
    InstitutionHeadmasterDataComponent,
    InstitutionTypeDataComponent,
    InstitutionProcedureDataComponent,
    InstitutionIdentifierDataComponent,
    MiBulstatLoaderComponent,
    MiCreateNewComponent,
    MiCreationStepperComponent,
    InstitutionFlexFieldComponent,
    MunicipalPublicRegisterPageComponent,
    PremInstitutionDataComponent,
    RiDocumentComponent,
    MiFormComponent,
    MiEditExistingComponent,
    MiPreviewExistingComponent,
    InstitutionDepartmentDataComponent,
    MiDeleteExistingComponent,
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    MunicipalInstitutionRoutingModule,
    PortalModule,
    TableModule,
    ButtonModule,
    AutoCompleteModule,
    DropdownModule,
    ToolbarModule,
    InputTextModule,
    StepsModule,
    DynamicDialogModule,
    CardModule,
    CheckboxModule,
    FieldsetModule,
    CalendarModule,
    RadioButtonModule,
    MessageModule,
    InputNumberModule,
    InputTextareaModule,
    MessagesModule,
    SharedModule,
    ConfirmDialogModule,
    DialogModule
  ],
  providers: [FilterService, MunicipalInstitutionService, MICommonFormGroupService, ACLMunicipalityInstitutionService],
  exports: [
    InstitutionCardDetailsComponent,
    InstitutionProcedureDataComponent,
    MiFormComponent,
  ],
})
export class MunicipalInstitutionModule {}
