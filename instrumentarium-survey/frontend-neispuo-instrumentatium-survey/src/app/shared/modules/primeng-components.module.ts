import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { TableModule } from 'primeng/table';
import { CheckboxModule } from 'primeng/checkbox';
import { StepsModule } from 'primeng/steps';
import { ToastModule } from 'primeng/toast';
import { RadioButtonModule } from 'primeng/radiobutton';
import { InputSwitchModule } from 'primeng/inputswitch';
import { ChartModule } from 'primeng/chart';
import { DialogModule } from 'primeng/dialog';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { CalendarModule } from 'primeng/calendar';
import { MultiSelectModule } from 'primeng/multiselect';
import { DropdownModule } from 'primeng/dropdown';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { PaginatorModule } from 'primeng/paginator';
import { ProgressBarModule } from 'primeng/progressbar';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { TooltipModule } from 'primeng/tooltip';
import { DialogService } from 'primeng/dynamicdialog';
import { MessagesModule } from 'primeng/messages';
import { MessageModule } from 'primeng/message';

const primeNGModule = [
  TableModule,
  CheckboxModule,
  StepsModule,
  ToastModule,
  RadioButtonModule,
  InputSwitchModule,
  ChartModule,
  DialogModule,
  ButtonModule,
  CardModule,
  CalendarModule,
  MultiSelectModule,
  DropdownModule,
  PaginatorModule,
  InputTextModule,
  InputNumberModule,
  ProgressBarModule,
  ProgressSpinnerModule,
  TooltipModule,
  MessageModule,
  MessagesModule
];

@NgModule({
  declarations: [],
  imports: [CommonModule],
  providers: [DialogService],
  exports: [...primeNGModule]
})
export class PrimeNGComponentsModule {}
