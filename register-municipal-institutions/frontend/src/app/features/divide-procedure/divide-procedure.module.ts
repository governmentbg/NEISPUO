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
import { DivideMIToDeleteComponent } from './pages/divide-mi-to-delete/divide-mi-to-delete.component';
import { DivideProceduresRoutingModule } from './divide-procedure-routing.module';
import { DivideMiPreviewDeleteComponent } from './components/divide-mi-preview-delete/divide-mi-preview-delete.component';
import { DivideProcedureStepperMenuPage } from './components/divide-procedure-stepper-menu/divide-procedure-stepper-menu.page';
import { DivideMIModalPreviewComponent } from './components/divide-mi-modal-preview/divide-mi-modal-preview.component';
import { DivideMIsToCreateComponent } from './pages/divide-mis-to-create/divide-mis-to-create.component';
import { MIPreviewOnlyComponent } from './components/mi-preview-only/mi-preview-only.component';
import { DivideConfirmationComponent } from './pages/divide-confirmation/divide-confirmation.component';
import { DivideMiPreviewRiDocumentComponent } from './components/divide-mi-preview-ri-document/divide-mi-preview-ri-document.component';
import { DivideRiDocumentComponent } from './pages/divide-ri-document/divide-ri-document.component';
import { ConfirmDialogModule } from 'primeng/confirmdialog';

@NgModule({
  declarations: [
    DivideMIToDeleteComponent,
    MIPreviewOnlyComponent,
    DivideMiPreviewDeleteComponent,
    DivideProcedureStepperMenuPage,
    DivideMIModalPreviewComponent,
    DivideMIsToCreateComponent,
    DivideConfirmationComponent,
    DivideMiPreviewRiDocumentComponent,
    DivideRiDocumentComponent,
  ],
  imports: [
    BrowserModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    CommonModule,
    DivideProceduresRoutingModule,
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
    ConfirmDialogModule
  ]
})
export class DivideProcedureModule { }
