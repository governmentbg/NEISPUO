import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { AccordionModule } from 'primeng/accordion';
import { AvatarModule } from 'primeng/avatar';
import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';
import { CheckboxModule } from 'primeng/checkbox';
import { ChipsModule } from 'primeng/chips';
import { DialogModule } from 'primeng/dialog';
import { DropdownModule } from 'primeng/dropdown';
import { DynamicDialogModule } from 'primeng/dynamicdialog';
import { InputSwitchModule } from 'primeng/inputswitch';
import { MenuModule } from 'primeng/menu';
import { MenubarModule } from 'primeng/menubar';
import { MultiSelectModule } from 'primeng/multiselect';
import { PanelMenuModule } from 'primeng/panelmenu';
import { TableModule } from 'primeng/table';
import { ToggleButtonModule } from 'primeng/togglebutton';
import { ToolbarModule } from 'primeng/toolbar';
import { ConfirmationService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { OverlayPanelModule } from 'primeng/overlaypanel';
import { ConfirmPopupModule } from 'primeng/confirmpopup';
import { TabViewModule } from 'primeng/tabview';
import { RadioButtonModule } from 'primeng/radiobutton';

const PrimeNgModules = [
    ButtonModule,
    AvatarModule,
    AccordionModule,
    CalendarModule,
    ChipsModule,
    CheckboxModule,
    DialogModule,
    DropdownModule,
    DynamicDialogModule,
    MenubarModule,
    MenuModule,
    MultiSelectModule,
    PanelMenuModule,
    TableModule,
    ToggleButtonModule,
    InputSwitchModule,
    ToastModule,
    ToolbarModule,
    OverlayPanelModule,
    ConfirmPopupModule,
    TabViewModule,
    RadioButtonModule,
];

@NgModule({
    declarations: [],
    imports: [CommonModule],
    exports: [...PrimeNgModules],
    providers: [ConfirmationService],
})
export class PrimeNGComponentsModule {}
