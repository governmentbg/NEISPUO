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
import { DetachProceduresRoutingModule } from './detach-procedure-routing.module';
import { DetachProcedureStepperMenuPage } from './components/detach-procedure-stepper-menu/detach-procedure-stepper-menu.page';
import { DetachMIModalPreviewComponent } from './components/detach-mi-modal-preview/detach-mi-modal-preview.component';
import { DetachMIsToCreateComponent } from './pages/detach-mis-to-create/detach-mis-to-create.component';
import { MIPreviewOnlyComponent } from './components/mi-preview-only/mi-preview-only.component';
import { DetachConfirmationComponent } from './pages/detach-confirmation/detach-confirmation.component';
import { DetachMIToUpdateComponent } from './pages/detach-mi-to-update/detach-mi-to-update.component';
import { DetachMiPreviewUpdateComponent } from './components/detach-mi-preview-update/detach-mi-preview-update.component';
import { DetachRiDocumentComponent } from './pages/detach-ri-document/detach-ri-document.component';
import { DetachMiPreviewRiDocumentComponent } from './components/detach-mi-preview-ri-document/detach-mi-preview-ri-document.component';
import { ConfirmDialogModule } from 'primeng/confirmdialog';

@NgModule({
  declarations: [
    MIPreviewOnlyComponent,
    DetachProcedureStepperMenuPage,
    DetachMIModalPreviewComponent,
    DetachMIsToCreateComponent,
    DetachConfirmationComponent,
    DetachMIToUpdateComponent,
    DetachMiPreviewUpdateComponent,
    DetachRiDocumentComponent,
    DetachMiPreviewRiDocumentComponent,
  ],
  imports: [
    BrowserModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    CommonModule,
    DetachProceduresRoutingModule,
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
export class DetachProcedureModule { }
