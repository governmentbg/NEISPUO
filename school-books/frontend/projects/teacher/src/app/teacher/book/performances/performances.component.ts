import { Component, Inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faFileExcel as fasFileExcel } from '@fortawesome/pro-solid-svg-icons/faFileExcel';
import { faFileSpreadsheet as fasFileSpreadsheet } from '@fortawesome/pro-solid-svg-icons/faFileSpreadsheet';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { PerformancesService, Performances_GetAll } from 'projects/sb-api-client/src/api/performances.service';
import { PerformancesExcelService } from 'projects/sb-api-client/src/api/performancesExcel.service';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { ClassBookInfoType } from 'projects/shared/utils/book';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { CLASS_BOOK_INFO } from '../book/book.component';

@Component({
  selector: 'sb-performances',
  templateUrl: './performances.component.html'
})
export class PerformancesComponent {
  readonly fasPlus = fasPlus;
  readonly fadChevronCircleRight = fadChevronCircleRight;
  downloadUrl?: string;
  downloadForAllBooksUrl?: string;
  fasFileExcel = fasFileExcel;
  fasFileSpreadsheet = fasFileSpreadsheet;

  dataSource: TableDataSource<Performances_GetAll>;
  canCreate = false;
  canExportForAllBooks = false;

  constructor(
    @Inject(CLASS_BOOK_INFO) classBookInfo: Promise<ClassBookInfoType>,
    performancesService: PerformancesService,
    performancesExcelService: PerformancesExcelService,
    route: ActivatedRoute
  ) {
    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');

    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      performancesService.getAll({ schoolYear, instId, classBookId, offset, limit })
    );

    // the promise should be resolved by this stage,
    // so no need for a skeleton screen
    classBookInfo.then((classBookInfo) => {
      this.canCreate = classBookInfo.bookAllowsModifications && classBookInfo.hasCreatePerformanceAccess;
      this.canExportForAllBooks = classBookInfo.hasExportForAllBooksPerformanceAccess;
    });

    performancesExcelService
      .downloadExcelFile({
        schoolYear: schoolYear,
        instId: instId,
        classBookId: classBookId
      })
      .toPromise()
      .then((url) => (this.downloadUrl = url));

    performancesExcelService
      .downloadExcelFileForAllBooks({
        schoolYear: schoolYear,
        instId: instId,
        classBookId: classBookId
      })
      .toPromise()
      .then((url) => (this.downloadForAllBooksUrl = url));
  }
}
