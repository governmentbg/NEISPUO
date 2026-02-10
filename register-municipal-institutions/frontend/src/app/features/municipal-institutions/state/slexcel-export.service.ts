import { Injectable } from '@angular/core';
import { MessageService } from 'primeng';
import { tap, catchError } from 'rxjs/operators';
import { of } from 'rxjs';
import { SubmissionStateEnum } from '@application-process-backend/domain/submission/enums/submission-state.enum';
import { ExcelExportService } from '../../../../shared/services/excel-export.service';
import { ApiService } from '../../../../core/services/api.service';
import { Submission } from '../../../../core/models/submission.model';
import {
  SubmissionListColumnService,
  SubmissionListColumn,
} from './submission-list-column.service';

interface ExportColumn {
  dottedPropName: string;
  propTranslation: string;
}

@Injectable({
  providedIn: 'root',
})
export class SLExcelExportService {
  tableVisibleColumns: SubmissionListColumn[];

  constructor(
    private excelExportService: ExcelExportService,
    private apiService: ApiService,
    private messageService: MessageService,
    private columnService: SubmissionListColumnService,
  ) {
    this.columnService.visibleColumns.subscribe((vc) => (this.tableVisibleColumns = vc));
  }

  /**
   * export columns are the same as submission list view (columnService), plus wishes
   */
  private getColumnsToExport(submissions: Submission[]) {
    const columnsToExport: ExportColumn[] = this.tableVisibleColumns.map((c) => ({
      dottedPropName: c.field,
      propTranslation: c.header,
    }));

    const maxWishes = submissions.reduce(
      (acc, val) => (this.statesWithRankedWish.includes(val.state)
        ? 1
        : acc > val.wishes?.length
          ? acc
          : val.wishes?.length),
      0,
    ) || 0;

    const wishColumns: ExportColumn[] = [];
    for (let i = 0; i < maxWishes; i += 1) {
      const displayOrder = i + 1;
      const wishProperties: ExportColumn[] = [
        {
          dottedPropName: `wishes.${i}.course.name`,
          propTranslation: `[Желание ${displayOrder}] Курс`,
        },
        {
          dottedPropName: `wishes.${i}.course.university.name`,
          propTranslation: `[Желание ${displayOrder}] Университет`,
        },
        {
          dottedPropName: `wishes.${i}.gradeSubject1.name`,
          propTranslation: `[Желание ${displayOrder}] Предмет 1`,
        },
        {
          dottedPropName: `wishes.${i}.convertedGrade1`,
          propTranslation: `[Желание ${displayOrder}] 6-бална оценка 1`,
        },
        {
          dottedPropName: `wishes.${i}.gradeSubject2.name`,
          propTranslation: `[Желание ${displayOrder}] Предмет 2`,
        },
        {
          dottedPropName: `wishes.${i}.convertedGrade2`,
          propTranslation: `[Желание ${displayOrder}] 6-бална оценка 2`,
        },
        {
          dottedPropName: `wishes.${i}.gradeSubject3.name`,
          propTranslation: `[Желание ${displayOrder}] Предмет 3`,
        },
        {
          dottedPropName: `wishes.${i}.convertedGrade3`,
          propTranslation: `[Желание ${displayOrder}] 6-бална оценка 3`,
        },
        {
          dottedPropName: `wishes.${i}.totalScore`,
          propTranslation: `[Желание ${displayOrder}] Общ бал`,
        },
      ];
      wishColumns.push(...wishProperties);
    }

    return [...columnsToExport, ...wishColumns];
  }

  private createExportRows(
    submissions: Submission[],
    columnsToExport: ExportColumn[],
    getDataMethod: (value: any, dotNotatedProperty: string) => string,
  ) {
    const exportRows: any[] = [];
    for (const submission of submissions) {
      if (this.statesWithRankedWish.includes(submission.state)) {
        submission.wishes = submission.wishes.filter((w) => w.isRanked);
      }

      if (submission.wishes?.length) {
        submission.wishes.sort((w1, w2) => (w1.order > w2.order ? 1 : -1));
      }

      const exportObject = {};
      for (const exportColumn of columnsToExport) {
        const translatedHeader = exportColumn.propTranslation;
        const fieldData = getDataMethod(submission, exportColumn.dottedPropName);
        exportObject[translatedHeader] = fieldData;
      }
      exportRows.push(exportObject);
    }
    return exportRows;
  }

  createExcel(queryParams: any, getDataMethod: (value: any, dotNotatedProperty: string) => string) {
    return this.apiService.get('v1/submission', queryParams).pipe(
      tap((success: { data: any }) => {
        const columnsToExport = this.getColumnsToExport(success.data);
        const rowsToExport = this.createExportRows(success.data, columnsToExport, getDataMethod);
        this.excelExportService.exportExcel(rowsToExport, 'export');
      }),
      catchError((error) => {
        this.messageService.clear();
        this.messageService.add({
          summary: error.message,
          severity: 'error',
          key: 'globalAppToast',
        });
        return of(false);
      }),
    );
  }

  private get statesWithRankedWish() {
    return [
      SubmissionStateEnum.AWAITING_STUDENT_CONFIRMATION_REGULAR,
      SubmissionStateEnum.AWAITING_STUDENT_CONFIRMATION_RP,
      SubmissionStateEnum.AWAITING_UNIVERSITY_CONFIRMATION,
      SubmissionStateEnum.UNIVERSITY_CONFIRMED,
      SubmissionStateEnum.REJECTED_UNIVERSITY,
    ];
  }
}
