import { Component, OnInit } from '@angular/core';
import { ReportService } from '@reporting/services/report.service';
import { SidebarToggleOptionValueEnum } from '@shared/enums/sidebar-toggle-option-value.enum';
import { VisualizationTypeEnum } from '@shared/enums/visualization-type.enum';
import { VisualizationOption } from '@shared/models/visualization-option.interface';
import { ReportingSidebarService } from '../reporting-sidebar/reporting-sidebar.service';

@Component({
  selector: 'app-visualization',
  templateUrl: './visualization.component.html',
  styleUrls: ['./visualization.component.scss']
})
export class VisualizationComponent implements OnInit {
  public visualizationOptions: VisualizationOption[] = [
    { label: 'Таблица', value: VisualizationTypeEnum.table, iconClass: 'bi bi-table' },
    { label: 'Пивот таблица', value: VisualizationTypeEnum.pivot_table, iconClass: 'bi bi-layout-text-window-reverse' },
    { label: 'Пай чарт', value: VisualizationTypeEnum.pie_chart, iconClass: 'bi bi-pie-chart' }
  ];
  constructor(public sidebarService: ReportingSidebarService, public reportService: ReportService) {}

  ngOnInit(): void {
    this.sidebarService.change.subscribe((v) => {
      this.sidebarService.isSidebarOpen = v.isSidebarOpen;
      this.sidebarService.visualization = v.visualization;
      this.sidebarService.options = v.options;
    });
  }

  toggleSidebar(visualizationOption: VisualizationOption) {
    this.sidebarService.toggle({
      value: SidebarToggleOptionValueEnum.settings,
      visualization: visualizationOption.value
    });
  }
}
