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
import { ProcedureStepperMenuPage } from './pages/procedure-stepper/procedure-stepper-menu.page';
import { ProcedureStepperComponent } from './components/procedure-stepper/procedure-stepper.component';
import { ProceduresRoutingModule } from './procedures-routing.module';
import { ProcedureChoicePage } from './pages/procedure-choice/procedure-choice.page';
import { SingleMiChoiceComponent } from './pages/single-mi-choice/single-mi-choice.component';

@NgModule({
  declarations: [
    ProcedureStepperMenuPage,
    ProcedureStepperComponent,
    SingleMiChoiceComponent,
    ProcedureChoicePage,
  ],
  imports: [
    BrowserModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    CommonModule,
    ProceduresRoutingModule,
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

  exports: [SingleMiChoiceComponent],
})
export class ProceduresModule { }
