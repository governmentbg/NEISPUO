import { CdkDragDrop, CdkDropList, moveItemInArray, transferArrayItem } from '@angular/cdk/drag-drop';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { AppToastMessageService } from '@core/services/app-toast-message.service';
import { FormArray, FormBuilder, FormGroup } from '@angular/forms';
import { ReportBuilderQuery } from '@reporting/pages/report-builder/state/report-builder.query';
import { PieChartTypeReportService } from '@reporting/services/pie-chart-type-report.service';
import { ReportSummaryService } from '@reporting/services/report-summary.service';
import { ReportService } from '@reporting/services/report.service';
import { CONSTANTS } from '@shared/constants';
import { combineLatest, take, Subscription } from 'rxjs';

@Component({
  selector: 'app-pie-chart-type-report-settings',
  templateUrl: './pie-chart-type-report-settings.component.html',
  styleUrls: ['./pie-chart-type-report-settings.component.scss']
})
export class PieChartTypeReportSettingsComponent implements OnInit, OnDestroy {
  public availableDimensions$ = this.rbQuery.availableDimensions$;
  public availableMeasures$ = this.rbQuery.availableMeasures$;
  @ViewChild('xList') xList: CdkDropList;
  @ViewChild('yList') yList: CdkDropList;
  public sortField: string;
  public sortOrder: string;
  public queryObjectOrder: string[][];
  public filterMembers = [];
  public lazyLoadEventFilters = {};
  public availableColumns$ = combineLatest([this.availableDimensions$, this.availableMeasures$]);
  public columns: any[];
  public x: string[];
  public y: string[];
  selectedFilterColumn = {};
  public form: FormGroup;
  private subscription: Subscription;

  constructor(
    public reportSummaryService: ReportSummaryService,
    private rbQuery: ReportBuilderQuery,
    public reportService: ReportService,
    public pieChartService: PieChartTypeReportService,
    private toastService: AppToastMessageService,
    private fb: FormBuilder
  ) {
    this.form = fb.group({
      filters: fb.array([])
    });
  }

  ngOnInit(): void {
    this.availableColumns$.pipe(take(1)).subscribe({
      next: ([dimensions, measures]) => {
        this.columns = [...dimensions, ...measures].filter((c) => c.type !== 'time');
      }, // Remove column type `time`, because Cube.js doesn't support time dimension filters
      error: (err) => {
        this.toastService.displayError();
      }
    });

    this.subscription = this.pieChartService.pieChartPivotConfig$.subscribe({
      next: (config) => {
        this.x = config['x'];
        this.y = config['y'];
      },
      error: (err) => {
        this.toastService.displayError();
      }
    });

    if (!!this.reportService.lastLazyLoadEvent?.filters) {
      for (const [key, value] of Object.entries(this.reportService.lastLazyLoadEvent.filters)) {
        if (!this.reportService.lastLazyLoadEvent.filters[key][0].value) continue;

        this.filterMembers.push({
          shortTitle: this.findColumnDetails(key)?.shortTitle,
          type: this.findColumnDetails(key)?.type,
          member: key,
          values: value
        });
      }
    }

    this.queryObjectOrder = this.reportService.queryObject.order;
    if (this.queryObjectOrder) {
      this.sortField = this.queryObjectOrder[0][0];
      this.sortOrder = this.queryObjectOrder[0][1];
    }

    if (this.filterMembers.length) {
      for (const member of this.filterMembers) {
        this.addAppliedFilterMembers(member);
      }
    }
  }

  findColumnDetails(key: string) {
    return this.columns.find((c: any) => c.name === key);
  }

  async drop(event: CdkDragDrop<string[]>) {
    if (event.previousContainer === event.container) {
      moveItemInArray(event.container.data, event.previousIndex, event.currentIndex);
    } else {
      transferArrayItem(event.previousContainer.data, event.container.data, event.previousIndex, event.currentIndex);
    }
  }

  async sort(order: string, key: string) {
    if (this.sortField === key && this.sortOrder === order) {
      return;
    }
    this.sortField = key;
    this.sortOrder = order;

    this.reportService.lastLazyLoadEvent = {
      ...this.reportService.lastLazyLoadEvent,
      rows: CONSTANTS.CUBEJS_MAX_RESULTS,
      first: 0,
      sortField: this.sortField,
      sortOrder: this.sortOrder === 'asc' ? 1 : -1
    };

    await this.pieChartService.loadPie(this.reportService.lastLazyLoadEvent, this.pieChartService.pieChartPivotConfig);
  }

  private addAppliedFilterMembers(filterMember: any) {
    const fg = this.fb.group({
      filterMemberName: [filterMember.member],
      filterMemberType: [filterMember.type],
      filterMemberValues: this.fb.array([])
    });
    (<FormArray>this.form.get('filters')).push(fg);

    const memberIndex = (<FormArray>this.form.get('filters')).length - 1;

    for (const value of filterMember.values) {
      this.addFilterMemberValue(memberIndex, value);
    }
  }

  addFilterMemberValue(memberIndex: number, data?: any) {
    const fg = this.fb.group({
      matchMode: [data ? data.matchMode : ''],
      value: [data ? data.value : null]
    });

    (<FormArray>(
      (<FormGroup>(<FormArray>this.form.controls['filters']).controls[memberIndex]).controls['filterMemberValues']
    )).push(fg);
  }

  async removeFilterMember(index: number) {
    delete this.lazyLoadEventFilters[(<FormArray>this.form.get('filters')).value[index].filterMemberName];
    (<FormArray>this.form.get('filters')).removeAt(index);
    await this.applyFilters((<FormArray>this.form.get('filters')).value);
  }

  async removeFilterMemberValue(memberIndex: number, index: number) {
    (<FormArray>(
      (<FormGroup>(<FormArray>this.form.controls['filters']).controls[memberIndex]).controls['filterMemberValues']
    )).removeAt(index);

    this.lazyLoadEventFilters[
      (<FormGroup>(<FormArray>this.form.controls['filters']).controls[memberIndex]).value.filterMemberName
    ] = (<FormArray>(
      (<FormGroup>(<FormArray>this.form.controls['filters']).controls[memberIndex]).controls['filterMemberValues']
    )).value;

    if (
      !(<FormArray>(
        (<FormGroup>(<FormArray>this.form.controls['filters']).controls[memberIndex]).controls['filterMemberValues']
      )).value.length
    ) {
      await this.removeFilterMember(memberIndex);
      return;
    }

    await this.applyFilters((<FormArray>this.form.get('filters')).value);
  }

  addNewFilterMember(event) {
    if (!event.value || <FormArray>this.form.get('filters').value.find((f) => f.filterMemberName === event.value.name)) {
      this.selectedFilterColumn = {};
      return;
    }

    const fg = this.fb.group({
      filterMemberName: [event.value.name],
      filterMemberType: [event.value.type],
      filterMemberValues: this.fb.array([])
    });

    (<FormArray>this.form.get('filters')).push(fg);
    const memberIndex = (<FormArray>this.form.get('filters')).length - 1;
    this.addFilterMemberValue(memberIndex);
    this.selectedFilterColumn = {};
  }

  async applyFilters(formValue: any) {
    for (const filter of formValue) {
      this.lazyLoadEventFilters[filter.filterMemberName] = filter.filterMemberValues;
    }

    this.reportService.lastLazyLoadEvent = {
      ...this.reportService.lastLazyLoadEvent,
      rows: CONSTANTS.CUBEJS_MAX_RESULTS,
      first: 0,
      filters: this.lazyLoadEventFilters
    };

    await this.pieChartService.loadPie(this.reportService.lastLazyLoadEvent, this.pieChartService.pieChartPivotConfig);
  }

  async updatePivot() {
    this.pieChartService.loadOnInit = false;
    await this.pieChartService.updatePivotConfig(this.xList.data, this.yList.data);
    await this.pieChartService.loadPie(
      { ...this.reportService.lastLazyLoadEvent, rows: CONSTANTS.CUBEJS_MAX_RESULTS, first: 0 },
      this.pieChartService.pieChartPivotConfig
    );
  }

  ngOnDestroy(): void {
    if(this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
