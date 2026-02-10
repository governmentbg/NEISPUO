import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AutoCompleteModule } from 'primeng/autocomplete';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { CheckboxModule } from 'primeng/checkbox';
import { DropdownModule } from 'primeng/dropdown';
import { DynamicDialogModule } from 'primeng/dynamicdialog';
import { InputTextModule } from 'primeng/inputtext';
import { StepsModule } from 'primeng/steps';
import { TableModule } from 'primeng/table';
import { ToolbarModule } from 'primeng/toolbar';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputSwitchModule } from 'primeng/inputswitch';
import { SelectButtonModule } from 'primeng/selectbutton';
import { TooltipModule } from 'primeng/tooltip';
import { MessagesModule } from 'primeng/messages';
import { MessageModule } from 'primeng/message';
import { FieldsetModule } from 'primeng/fieldset';
import { FlexFieldDetailPage } from './pages/flex-field-detail/flex-field-detail.page';
import { SharedModule } from '../../shared/shared.module';
import { FlexFieldListPage } from './pages/flex-field-list/flex-field-list.page';
import { FlexFieldsRoutingModule } from './flex-fields-routing.module';

@NgModule({
  declarations: [FlexFieldListPage, FlexFieldDetailPage],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
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
    SharedModule,
    FlexFieldsRoutingModule,
    InputNumberModule,
    InputSwitchModule,
    SelectButtonModule,
    TooltipModule,
    MessagesModule,
    MessageModule,
    FieldsetModule,
  ],
})
export class FlexFieldsModule {}
