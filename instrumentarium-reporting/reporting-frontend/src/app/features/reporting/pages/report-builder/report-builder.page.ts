import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { Report } from '@core/models/report.model';
import { AppToastMessageService } from '@core/services/app-toast-message.service';
import { ReportingSidebarService } from '@reporting/components/reporting-sidebar/reporting-sidebar.service';
import { PieChartTypeReportService } from '@reporting/services/pie-chart-type-report.service';
import { PivotTableTypeReportService } from '@reporting/services/pivot-table-type-report.service';
import { ReportSummaryService } from '@reporting/services/report-summary.service';
import { ReportService } from '@reporting/services/report.service';
import { SavedReportsService } from '@reporting/services/saved-reports.service';
import { TableTypeReportService } from '@reporting/services/table-type-report.service';
import { VisualizationTypeEnum } from '@shared/enums/visualization-type.enum';
import { FilterMetadata } from 'primeng/api';
import { DomHandler } from 'primeng/dom';
import { Dropdown } from 'primeng/dropdown';
import { ColumnFilter } from 'primeng/table';
import { ReportBuilderService } from './state/report-builder.service';
import { take } from 'rxjs';

@Component({
  selector: 'app-report-builder',
  templateUrl: './report-builder.page.html',
  styleUrls: ['./report-builder.page.scss']
})
export class ReportBuilderPage implements OnInit, OnDestroy {
  private fetchedReportData;

  constructor(
    private route: ActivatedRoute,
    public sidebarService: ReportingSidebarService,
    private router: Router,
    private reportService: ReportService,
    private rbService: ReportBuilderService,
    private tableTypeReportService: TableTypeReportService,
    private pivotTypeReportService: PivotTableTypeReportService,
    private reportSummaryService: ReportSummaryService,
    private savedReportsService: SavedReportsService,
    private pieChartService: PieChartTypeReportService,
    private toastService: AppToastMessageService
  ) {}

  ngOnInit(): void {
    this.overridePrimeNGColumnFilterMethods();
    this.overridePrimeNGFilterDropdownBehaviour();

    this.reportService.loadCubeMeta(this.route.snapshot.paramMap.get('databaseView')).subscribe({
      next: async (v) => {
        this.rbService.updateReport(v);
        const reportId = this.route.snapshot.paramMap.get('reportId');
        const routeArray = this.router.url.split('/');
        const reportSection = routeArray[1];

        if (!reportId) {
          await this.updateInitialData(v);
          this.subscribeForQueryParams(VisualizationTypeEnum.table);
        } else {
          this.savedReportsService
            .getReport(`${reportSection}`, +reportId)
            .pipe(take(1))
            .subscribe({
              next: async (report) => {
                if (!report) {
                  this.router.navigate([`/${reportSection}`], { queryParams: {} });
                  return;
                }
                this.fetchedReportData = JSON.parse(JSON.stringify(report));
                await this.updateSelectedDataFromSavedReport(this.fetchedReportData);
                this.subscribeForQueryParams(this.fetchedReportData.Visualization);
              },
              error: (err) => {
                this.toastService.displayError('Възникна грешка при зареждане на данните. Моля опитайте отново.');
              }
            });
        }
      },
      error: (err) => {
        this.toastService.displayError('Възникна грешка при зареждане на данните. Моля опитайте отново.');
      },
      complete: () => {
        console.info;
      }
    });
  }

  async updateInitialData(report) {
    this.reportService.isCubeData = true;
    this.reportService.selectedDimensions = report.dimensions;
    this.reportService.selectedMeasures = [];
    this.reportService.selectedColumns = report.dimensions;
  }

  async updateSelectedDataFromSavedReport(report: Report) {
    this.reportService.isCubeData = false;
    this.reportService.selectedDimensions = report.Query.dimensions;
    this.reportService.selectedMeasures = report.Query.measures;
    this.reportService.selectedColumns = report.Query.columns;
    this.reportService.lastLazyLoadEvent = {
      ...report.Query,
      dimensions: report.Query.dimensions,
      measures: report.Query.measures
    };
    this.reportService.lastAppliedColumnFilters = report.Query.filters;
    this.reportSummaryService.sumarizedBy = report.Query.sumarizedBy;
    this.reportSummaryService.summaryGroupedBy = report.Query.summaryGroupedBy;
    this.setPivotConfig(report);
    await this.reportService.createQueryObject({ ...report.Query }, report.Query.dimensions, report.Query.measures);
  }

  subscribeForQueryParams(visualization: VisualizationTypeEnum) {
    this.route.queryParamMap.subscribe({
      next: (query) => {
        this.sidebarService.visualization = query.get('visualization') as VisualizationTypeEnum;
        if (!query.get('visualization')) {
          this.sidebarService.visualization = visualization;
          this.router.navigate([], {
            queryParams: {
              visualization: visualization
            }
          });
        }
      },
      error: (err) => {
        this.toastService.displayError();
      }
    });
  }

  setPivotConfig(report: Report) {
    this.route.queryParamMap.subscribe({
      next: (query) => {
        if (query.get('visualization') === VisualizationTypeEnum.pivot_table) {
          this.pivotTypeReportService.tablePivotConfig = report.Query.pivotConfig;
        }
        if (query.get('visualization') === VisualizationTypeEnum.pie_chart) {
          this.pieChartService.pieChartPivotConfig = report.Query.pivotConfig;
        }
      },
      error: (err) => {
        this.toastService.displayError();
      }
    });
  }

  overridePrimeNGColumnFilterMethods() {
    const self = this;
    ColumnFilter.prototype.addConstraint = function () {
      (<FilterMetadata[]>this.dt.filters[this.field]).push({
        value: null,
        matchMode: this.getDefaultMatchMode(),
        operator: this.getDefaultOperator()
      });
    };

    ColumnFilter.prototype.removeConstraint = function (filterMeta: FilterMetadata) {
      self.reportService.lastAppliedColumnFilters[this.field] = JSON.parse(JSON.stringify(this.dt.filters[this.field]));
      this.dt.filters[this.field] = (<FilterMetadata[]>this.dt.filters[this.field]).filter((meta) => meta !== filterMeta);
    };

    ColumnFilter.prototype.clearFilter = function () {
      this.initFieldFilterConstraint();
      this.dt._filter();
      this.hide();
    };

    ColumnFilter.prototype.applyFilter = function () {
      self.reportService.lastAppliedColumnFilters[this.field] = JSON.parse(JSON.stringify(this.dt.filters[this.field]));
      this.dt._filter();
      this.hide();
    };

    ColumnFilter.prototype.toggleMenu = function () {
      this.overlayVisible = !this.overlayVisible;
      const addedFilters = [...this.dt.filters[this.field]];
      const appliedFilters = self.reportService.lastAppliedColumnFilters[this.field]
        ? [...self.reportService.lastAppliedColumnFilters[this.field]]
        : [
            {
              value: null,
              matchMode: this.dt.filters[this.field][0].matchMode,
              operator: this.dt.filters[this.field][0].operator
            }
          ];

      if (
        (self.getFilterDifference(addedFilters, appliedFilters).length || addedFilters?.length < appliedFilters?.length) &&
        !this.overlayVisible
      ) {
        self.reportService.lastAppliedColumnFilters[this.field] = JSON.parse(JSON.stringify(this.dt.filters[this.field]));
        this.dt._filter();
      }
    };

    ColumnFilter.prototype.bindDocumentClickListener = function () {
      if (!this.documentClickListener) {
        const documentTarget: any = this.el ? this.el.nativeElement.ownerDocument : 'document';

        this.documentClickListener = this.renderer.listen(documentTarget, 'click', (event) => {
          if (this.overlayVisible && !this.selfClick && this.isOutsideClicked(event)) {
            const addedFilters = [...this.dt.filters[this.field]];
            const appliedFilters = self.reportService.lastAppliedColumnFilters[this.field]
              ? [...self.reportService.lastAppliedColumnFilters[this.field]]
              : [
                  {
                    value: null,
                    matchMode: this.dt.filters[this.field][0].matchMode,
                    operator: this.dt.filters[this.field][0].operator
                  }
                ];

            if (
              self.getFilterDifference(addedFilters, appliedFilters).length ||
              addedFilters?.length < appliedFilters?.length
            ) {
              self.reportService.lastAppliedColumnFilters[this.field] = JSON.parse(
                JSON.stringify(this.dt.filters[this.field])
              );
              this.dt._filter();
            }
            this.hide();
          }

          this.selfClick = false;
        });
      }
    };
  }
  overridePrimeNGFilterDropdownBehaviour() {
    /* !!!IMPORTANT!!! */
    /* OOB primeng tabel filter display = "menu" have no limitations for height and it exapnds as much as filter conditions added.
       As we want to make the menu scrollable and with max height this causes issues for bigger dropdowns.
       In CSS if we set overflow: auto/scroll ... etc. it is not possible to have child which overlays the parent
       As we want the dropdowns to be able to expand out of the filter menu in primeng this is generally done by adding appendTo = "body"
       In current situation there is no possible way to pass appendTo option for the filter matchmode dropdown.
       This is why current override is needed!
    */
    Dropdown.prototype.appendOverlay = function () {
      /* Check if current dropdown is filter-matchmode-dropdown */
      const isTableMatchModeFilter = this.styleClass === 'p-column-filter-matchmode-dropdown';

      if (this.appendTo || isTableMatchModeFilter) {
        if (this.appendTo === 'body' || isTableMatchModeFilter) document.body.appendChild(this.overlay);
        else DomHandler.appendChild(this.overlay, this.appendTo);
        if (!this.overlay.style.minWidth) {
          this.overlay.style.minWidth = DomHandler.getWidth(this.containerViewChild.nativeElement) + 'px';
        }
      }
    };

    Dropdown.prototype.alignOverlay = function () {
      /* Check if current dropdown is filter-matchmode-dropdown */
      const isTableMatchModeFilter = this.styleClass === 'p-column-filter-matchmode-dropdown';
      if (this.overlay) {
        if (this.appendTo || isTableMatchModeFilter)
          DomHandler.absolutePosition(this.overlay, this.containerViewChild.nativeElement);
        else DomHandler.relativePosition(this.overlay, this.containerViewChild.nativeElement);
      }
    };

    Dropdown.prototype.restoreOverlayAppend = function () {
      /* Check if current dropdown is filter-matchmode-dropdown */
      const isTableMatchModeFilter = this.styleClass === 'p-column-filter-matchmode-dropdown';
      if (this.overlay && (this.appendTo || isTableMatchModeFilter)) {
        this.el.nativeElement.appendChild(this.overlay);
      }
    };
    /* !!!IMPORTANT!!! */
  }

  getFilterDifference(addedFilters: any[], appliedFilters: any[]) {
    return addedFilters.filter((addedFilter) => {
      return !appliedFilters.some((appliedFilter) => {
        return (
          addedFilter.value === appliedFilter.value &&
          addedFilter.matchMode === appliedFilter.matchMode &&
          addedFilter.operator === appliedFilter.operator
        );
      });
    });
  }

  ngOnDestroy(): void {
    this.reportService.cleanUpData();
    this.tableTypeReportService.cleanUpData();
    this.pivotTypeReportService.cleanUpData();
    this.reportSummaryService.cleanUpData();
    this.sidebarService.visualization = null;
    this.sidebarService.isSidebarOpen = false;
  }
}
