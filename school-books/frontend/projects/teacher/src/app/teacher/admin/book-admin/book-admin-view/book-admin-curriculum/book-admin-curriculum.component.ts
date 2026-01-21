import { Component, Inject } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import {
  ClassBooksAdminService,
  ClassBooksAdmin_GetCurriculum
} from 'projects/sb-api-client/src/api/classBooksAdmin.service';
import { ClassBookType } from 'projects/sb-api-client/src/model/classBookType';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { openTypedDialog } from 'projects/shared/utils/dialog';
import { notEmpty, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { ClassBookAdminInfoType, CLASS_BOOK_ADMIN_INFO } from '../book-admin-view.component';
import { BookAdminCurriculumItemDialogComponent } from './book-admin-curriculum-item-dialog.component';

@Component({
  selector: 'sb-book-admin-curriculum',
  templateUrl: './book-admin-curriculum.component.html'
})
export class BookAdminCurriculumComponent {
  readonly fasPlus = fasPlus;
  readonly fadChevronCircleRight = fadChevronCircleRight;
  displayedColumns: string[] = [];

  dataSource: TableDataSource<ClassBooksAdmin_GetCurriculum>;

  constructor(
    @Inject(CLASS_BOOK_ADMIN_INFO) classBookAdminInfo: Promise<ClassBookAdminInfoType>,
    classBooksAdminService: ClassBooksAdminService,
    public route: ActivatedRoute,
    public router: Router,
    private dialog: MatDialog
  ) {
    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');

    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      classBooksAdminService.getCurriculum({ schoolYear, instId, classBookId, offset, limit })
    );

    // the promise should be resolved by this stage,
    // so no need for a skeleton screen
    classBookAdminInfo.then((classBookAdminInfo) => {
      const canEdit = classBookAdminInfo.bookAllowsModifications && classBookAdminInfo.hasEditCurriculumAccess;
      const hasGrades =
        classBookAdminInfo.bookType === ClassBookType.Book_I_III ||
        classBookAdminInfo.bookType === ClassBookType.Book_IV ||
        classBookAdminInfo.bookType === ClassBookType.Book_V_XII ||
        classBookAdminInfo.bookType === ClassBookType.Book_CSOP;

      this.displayedColumns = [
        hasGrades && canEdit ? 'action' : null,
        'subjectName',
        'subjectTypeName',
        'teacherNames',
        hasGrades ? 'withoutGrade' : null
      ].filter(notEmpty);
    });
  }

  openCurriculumDialog(
    curriculumId: number,
    subjectName: string,
    subjectTypeName: string,
    teacherNames: string,
    withoutGrade: boolean
  ) {
    const schoolYear = tryParseInt(this.route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(this.route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(this.route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    openTypedDialog(this.dialog, BookAdminCurriculumItemDialogComponent, {
      data: {
        schoolYear: schoolYear,
        instId: instId,
        classBookId: classBookId,
        curriculumId: curriculumId,
        subjectName: subjectName,
        subjectTypeName: subjectTypeName,
        teacherNames: teacherNames,
        withoutGrade: withoutGrade
      }
    })
      .afterClosed()
      .toPromise()
      .then((ok) => {
        if (ok) {
          return this.dataSource.reload();
        }

        return Promise.resolve();
      });
  }
}
