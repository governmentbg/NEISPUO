import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReportingRoutingModule } from './reporting-routing.module';
import { ReportingLayoutPage } from './reporting-layout.page';
import { CoreModule } from '../../core/core.module';
import { SharedModule } from '../../shared/shared.module';
import { VisualizationComponent } from './components/visualization/visualization.component';
import { SettingsComponent } from './components/settings/settings.component';
import { SummaryComponent } from './components/summary/summary.component';
import { ReportListPage } from './pages/report-list/report-list.page';
import { ReportBuilderPage } from './pages/report-builder/report-builder.page';
import { TableModule } from 'primeng/table';
import { ReportingSidebarComponent } from './components/reporting-sidebar/reporting-sidebar.component';
import { TableTypeReportSettingsComponent } from './components/table-type-report-settings/table-type-report-settings.component';
import { TableTypeReportComponent } from './components/table-type-report/table-type-report.component';
import { ReportBuilderContentComponent } from './components/report-builder-content/report-builder-content.component';
import { MultiSelectModule } from 'primeng/multiselect';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { TooltipModule } from 'primeng/tooltip';
import { DynamicDialogModule } from 'primeng/dynamicdialog';
import { SaveReportFormComponent } from './components/save-report-form/save-report-form.component';
import { SaveReportButtonComponent } from './components/save-report-button/save-report-button.component';
import { DataViewModule } from 'primeng/dataview';
import { ReportCardComponent } from './pages/report-list/report-card/report-card.component';
import { ButtonModule } from 'primeng/button';
import { SavedReportsPage } from './pages/saved-reports/saved-reports.page';
import { TabMenuModule } from 'primeng/tabmenu';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { SavedReportCardComponent } from './pages/saved-reports/saved-report-card/saved-report-card.component';
import { PivotTableTypeReportSettingsComponent } from './components/pivot-table-type-report-settings/pivot-table-type-report-settings.component';
import { DropdownModule } from 'primeng/dropdown';
import { PivotTableTypeReportComponent } from './components/pivot-table-type-report/pivot-table-type-report.component';
import { DragDropModule } from '@angular/cdk/drag-drop';
import { AccordionModule } from 'primeng/accordion';
import { InputTextModule } from 'primeng/inputtext';
import { FieldsetModule } from 'primeng/fieldset';
import { FilterComponent } from './components/pivot-table-type-report-settings/filter/filter.component';
import { SplitButtonModule } from 'primeng/splitbutton';
import { InputNumberModule } from 'primeng/inputnumber';
import { CalendarModule } from 'primeng/calendar';
import { CardModule } from 'primeng/card';
import { MessagesModule } from 'primeng/messages';
import { MessageModule } from 'primeng/message';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ToastModule } from 'primeng/toast';
import { DialogService } from 'primeng/dynamicdialog';
import { DrillDownTableComponent } from './components/drill-down-table/drill-down-table.component';
import { DrillDownService } from './services/drill-down.service';
import { ReportBuilderStore } from './pages/report-builder/state/report-builder.store';
import { ReportBuilderQuery } from './pages/report-builder/state/report-builder.query';
import { ReportBuilderService } from './pages/report-builder/state/report-builder.service';
import { ReportExcelExportService } from './services/report-excel-export.service';
import { ReportService } from './services/report.service';
import { TableTypeReportService } from './services/table-type-report.service';
import { PivotTableTypeReportService } from './services/pivot-table-type-report.service';
import { ReportSummaryService } from './services/report-summary.service';
import { SharedReportsPage } from './pages/shared-reports/shared-reports.page';
import { ExportExcelButtonComponent } from './components/export-excel-button/export-excel-button.component';
import { CopyReportFormComponent } from './components/copy-report-form/copy-report-form.component';
import { ReportBuilderContentHeaderComponent } from './components/report-builder-content-header/report-builder-content-header.component';
import { PieChartTypeReportComponent } from './components/pie-chart-type-report/pie-chart-type-report.component';
import { PieChartTypeReportSettingsComponent } from './components/pie-chart-type-report-settings/pie-chart-type-report-settings.component';
import { PieChartTypeReportService } from './services/pie-chart-type-report.service';
import { NgChartsModule } from 'ng2-charts';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DetailsComponent } from './components/details/details.component';
import { NewModuleInfoComponent } from './components/new-module-info/new-module-info.component';

@NgModule({
  declarations: [
    ReportListPage,
    ReportBuilderPage,
    ReportingLayoutPage,
    VisualizationComponent,
    SettingsComponent,
    SummaryComponent,
    ReportingSidebarComponent,
    TableTypeReportSettingsComponent,
    TableTypeReportComponent,
    ReportBuilderContentComponent,
    ReportCardComponent,
    SavedReportCardComponent,
    SharedReportsPage,
    SavedReportsPage,
    SavedReportsPage,
    SaveReportFormComponent,
    SaveReportButtonComponent,
    PivotTableTypeReportComponent,
    PivotTableTypeReportSettingsComponent,
    FilterComponent,
    DrillDownTableComponent,
    ExportExcelButtonComponent,
    CopyReportFormComponent,
    ReportBuilderContentHeaderComponent,
    PieChartTypeReportComponent,
    PieChartTypeReportSettingsComponent,
    DetailsComponent,
    NewModuleInfoComponent
  ],
  imports: [
    ToastModule,
    InputTextModule,
    InputTextareaModule,
    CommonModule,
    ButtonModule,
    DataViewModule,
    ReportingRoutingModule,
    FormsModule,
    ReactiveFormsModule,
    CoreModule,
    CardModule,
    SharedModule,
    TableModule,
    MessagesModule,
    MessageModule,
    MultiSelectModule,
    TooltipModule,
    DataViewModule,
    DynamicDialogModule,
    TabMenuModule,
    DropdownModule,
    DragDropModule,
    AccordionModule,
    InputTextModule,
    FieldsetModule,
    SplitButtonModule,
    InputNumberModule,
    CalendarModule,
    DynamicDialogModule,
    NgChartsModule,
    ProgressSpinnerModule,
    ConfirmDialogModule
  ],
  providers: [
    DialogService,
    MessageService,
    ReportBuilderStore,
    ReportBuilderQuery,
    ReportBuilderService,
    ReportExcelExportService,
    ReportService,
    TableTypeReportService,
    PivotTableTypeReportService,
    PieChartTypeReportService,
    ReportSummaryService,
    DrillDownService,
    ConfirmationService
  ]
})
export class ReportingModule {}
