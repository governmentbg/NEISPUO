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
import { MergeMIsToDeleteComponent } from './pages/merge-mis-to-delete/merge-mis-to-delete.component';
import { MergeProceduresRoutingModule } from './merge-procedure-routing.module';
import { MergeMiPreviewDeleteComponent } from './components/merge-mi-preview-delete/merge-mi-preview-delete.component';
import { MergeProcedureStepperMenuPage } from './components/merge-procedure-stepper-menu/merge-procedure-stepper-menu.page';
import { MergeMIToCreateComponent } from './pages/merge-mi-to-create/merge-mi-to-create.component';
import { MIPreviewOnlyComponent } from './components/mi-preview-only/mi-preview-only.component';
import { MergeConfirmationComponent } from './pages/merge-confirmation/merge-confirmation.component';
import { MergeMiPreviewRiDocumentComponent } from './components/merge-mi-preview-ri-document/merge-mi-preview-ri-document.component';
import { MergeRiDocumentComponent } from './pages/merge-ri-document/merge-ri-document.component';
import { ConfirmDialogModule } from 'primeng/confirmdialog';

@NgModule({
  declarations: [
    MergeMIsToDeleteComponent,
    MergeMiPreviewDeleteComponent,
    MIPreviewOnlyComponent,
    MergeProcedureStepperMenuPage,
    MergeMIToCreateComponent,
    MergeConfirmationComponent,
    MergeMiPreviewRiDocumentComponent,
    MergeRiDocumentComponent,
  ],
  imports: [
    BrowserModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    CommonModule,
    MergeProceduresRoutingModule,
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
    ConfirmDialogModule,
  ],
})
export class MergeProcedureModule { }
