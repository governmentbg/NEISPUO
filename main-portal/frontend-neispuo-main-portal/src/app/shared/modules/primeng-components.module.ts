import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ToastModule } from '@shared/components/toast/toast.module';
import { ConfirmationService, MessageService } from 'primeng/api';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';
import { CheckboxModule } from 'primeng/checkbox';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DropdownModule } from 'primeng/dropdown';
import { DialogService, DynamicDialogModule } from 'primeng/dynamicdialog';
import { FileUploadModule } from 'primeng/fileupload';
import { InputTextModule } from 'primeng/inputtext';
import { MultiSelectModule } from 'primeng/multiselect';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { TableModule } from 'primeng/table';
import { ToolbarModule } from 'primeng/toolbar';
import { TooltipModule } from 'primeng/tooltip';

@NgModule({
  declarations: [],
  imports: [CommonModule, ProgressSpinnerModule],
  providers: [ConfirmationService, DialogService, MessageService],
  exports: [
    TableModule,
    CalendarModule,
    InputTextModule,
    ToolbarModule,
    FileUploadModule,
    DropdownModule,
    ConfirmDialogModule,
    ToastModule,
    TooltipModule,
    MultiSelectModule,
    AutoCompleteModule,
    CheckboxModule,
    ButtonModule,
    ProgressSpinnerModule,
    DynamicDialogModule
  ]
})
export class PrimeNGComponentsModule {}
