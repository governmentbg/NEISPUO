import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { PieChartTypeReportService } from '@reporting/services/pie-chart-type-report.service';
import { PivotTableTypeReportService } from '@reporting/services/pivot-table-type-report.service';
import { ReportService } from '@reporting/services/report.service';
import { TableTypeReportService } from '@reporting/services/table-type-report.service';
import { SidebarToggleOptionValueEnum } from '@shared/enums/sidebar-toggle-option-value.enum';
import { VisualizationTypeEnum } from '@shared/enums/visualization-type.enum';
import { SidebarToggleOption } from '@shared/models/sidebar-toggle-option.interface';
import { ReportingSidebarService } from '../../../features/reporting/components/reporting-sidebar/reporting-sidebar.service';

@Component({
  selector: 'app-reporting-sidebar-toggle',
  templateUrl: './reporting-sidebar-toggle.component.html',
  styleUrls: ['./reporting-sidebar-toggle.component.scss']
})
export class ReportingSidebarToggleComponent implements OnInit {
  public isComponentVisible: boolean = true;
  public sidebarToggleOptions: SidebarToggleOption[] = [
    {
      label: 'Визуализация',
      value: SidebarToggleOptionValueEnum.visualization,
      iconClass: this.getVisualizationButtonIconClass(this.sidebarService.visualization),
      buttonClass: 'primary'
    },
    { label: 'Настройки', value: SidebarToggleOptionValueEnum.settings, iconClass: 'bi-gear-fill', buttonClass: 'info' },
    { label: 'Обобщение', value: SidebarToggleOptionValueEnum.summary, iconClass: 'bi-stars', buttonClass: 'secondary' },
    {
      label: 'Описание',
      value: SidebarToggleOptionValueEnum.details,
      iconClass: 'bi bi-info-circle-fill',
      buttonClass: 'dark'
    }
  ];

  get isToggleButtonDisabled() {
    return (
      this.tableTypeReportService.isLoading || this.pivotTableTypeReportService.isLoading || this.pieChartService.isLoading
    );
  }

  constructor(
    public sidebarService: ReportingSidebarService,
    public reportService: ReportService,
    private tableTypeReportService: TableTypeReportService,
    private pivotTableTypeReportService: PivotTableTypeReportService,
    private pieChartService: PieChartTypeReportService
  ) {}

  ngOnInit(): void {
    this.sidebarService.change.subscribe((v) => {
      this.sidebarService.isSidebarOpen = v.isSidebarOpen;
      this.sidebarService.visualization = v.visualization;
      this.sidebarService.options = v.options;
    });
  }

  getVisualizationButtonIconClass(visualization: string) {
    if (visualization === VisualizationTypeEnum.table) {
      return 'bi-table';
    } else if (visualization === VisualizationTypeEnum.pivot_table) {
      return 'bi-layout-text-window-reverse';
    } else if (visualization === VisualizationTypeEnum.pie_chart) {
      return 'bi-pie-chart';
    } else {
      return 'bi-table';
    }
  }

  toggleSidebar(option: SidebarToggleOption) {
    this.sidebarService.toggle({ ...option, visualization: this.sidebarService.visualization });
  }
}
