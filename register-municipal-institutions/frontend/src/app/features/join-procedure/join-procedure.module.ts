import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { RadioButtonModule } from 'primeng/radiobutton';
import { FieldsetModule } from 'primeng/fieldset';
import { InputTextModule } from 'primeng/inputtext';
import { StepsModule } from 'primeng/steps';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { SharedModule } from '@shared/shared.module';
import { ToolbarModule } from 'primeng/toolbar';
import { DropdownModule } from 'primeng/dropdown';
import { MunicipalInstitutionModule } from '@municipal-institutions/municipal-institutions.module';
import { CardModule } from 'primeng/card';
import { TableModule } from 'primeng/table';
import { MultiSelectModule } from 'primeng/multiselect';
import { DividerModule } from 'primeng/divider';
import { JoinMIsToDeleteComponent } from './pages/join-mis-to-delete/join-mis-to-delete.component';
import { JoinProceduresRoutingModule } from './join-procedure-routing.module';
import { JoinMiPreviewDeleteComponent } from './components/join-mi-preview-delete/join-mi-preview-delete.component';
import { JoinProcedureStepperMenuPage } from './components/join-procedure-stepper-menu/join-procedure-stepper-menu.page';
import { JoinMIToUpdateComponent } from './pages/join-mi-to-update/join-mi-to-update.component';
import { MIPreviewOnlyComponent } from './components/mi-preview-only/mi-preview-only.component';
import { JoinConfirmationComponent } from './pages/join-confirmation/join-confirmation.component';
import { JoinMiPreviewRiDocumentComponent } from './components/join-mi-preview-ri-document/join-mi-preview-ri-document.component';
import { JoinRiDocumentComponent } from './pages/join-ri-document/join-ri-document.component';

@NgModule({
  declarations: [
    JoinMIsToDeleteComponent,
    JoinMiPreviewDeleteComponent,
    MIPreviewOnlyComponent,
    JoinProcedureStepperMenuPage,
    JoinMIToUpdateComponent,
    JoinConfirmationComponent,
    JoinMiPreviewRiDocumentComponent,
    JoinRiDocumentComponent,
  ],
  imports: [
    BrowserModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    CommonModule,
    JoinProceduresRoutingModule,
    ButtonModule,
    RadioButtonModule,
    FieldsetModule,
    InputTextModule,
    StepsModule,
    ToolbarModule,
    DropdownModule,
    ToolbarModule,
    CardModule,
    TableModule,
    MultiSelectModule,
    MunicipalInstitutionModule,
    DividerModule,
    SharedModule,
  ],
})
export class JoinProcedureModule { }
