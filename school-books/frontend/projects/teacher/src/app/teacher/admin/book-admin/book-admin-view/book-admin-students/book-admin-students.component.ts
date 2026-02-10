import { Component, Inject, Input, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ActivatedRoute, Router } from '@angular/router';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faList as fasList } from '@fortawesome/pro-solid-svg-icons/faList';
import { faSortAmountDownAlt as fasSortAmountDownAlt } from '@fortawesome/pro-solid-svg-icons/faSortAmountDownAlt';
import { faUserCheck as fasUserCheck } from '@fortawesome/pro-solid-svg-icons/faUserCheck';
import {
  ClassBooksAdminService,
  ClassBooksAdmin_GetStudents
} from 'projects/sb-api-client/src/api/classBooksAdmin.service';
import { ClassBookType } from 'projects/sb-api-client/src/model/classBookType';
import {
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { FirstGradeBasicClassId, SecondGradeBasicClassId, ThirdGradeBasicClassId } from 'projects/shared/utils/book';
import { openTypedDialog } from 'projects/shared/utils/dialog';
import { notEmpty, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from } from 'rxjs';
import { ClassBookAdminInfoType, CLASS_BOOK_ADMIN_INFO } from '../book-admin-view.component';
import { BookAdminStudentDialogSkeletonComponent } from './book-admin-student-dialog.component';
import { BookAdminStudentNumbersDialogSkeletonComponent } from './book-admin-student-numbers-dialog.component';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class BookAdminStudentsSkeletonComponent extends SkeletonComponentBase {
  constructor(@Inject(CLASS_BOOK_ADMIN_INFO) classBookAdminInfo: Promise<ClassBookAdminInfoType>) {
    super();

    this.resolve(BookAdminStudentsComponent, {
      classBookAdminInfo: from(classBookAdminInfo)
    });
  }
}

@Component({
  selector: 'sb-book-admin-students',
  templateUrl: './book-admin-students.component.html'
})
export class BookAdminStudentsComponent implements OnInit {
  @Input() data!: {
    classBookAdminInfo: ClassBookAdminInfoType;
  };

  readonly fasList = fasList;
  readonly fadChevronCircleRight = fadChevronCircleRight;
  readonly fasSortAmountDownAlt = fasSortAmountDownAlt;
  readonly fasUserCheck = fasUserCheck;
  bookType!: ClassBookType;
  activitiesFieldName?: string | null;
  displayedColumns: string[] = [];
  canEdit = false;
  sorting = false;
  schoolYear: number;
  instId: number;
  classBookId: number;

  dataSource: TableDataSource<ClassBooksAdmin_GetStudents>;

  constructor(
    route: ActivatedRoute,
    private classBooksAdminService: ClassBooksAdminService,
    private actionService: ActionService,
    private router: Router,
    private dialog: MatDialog
  ) {
    this.schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    this.instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    this.classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');

    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      classBooksAdminService.getStudents({
        schoolYear: this.schoolYear,
        instId: this.instId,
        classBookId: this.classBookId,
        offset,
        limit
      })
    );
  }

  openChangeNumbersDialog() {
    openTypedDialog(this.dialog, BookAdminStudentNumbersDialogSkeletonComponent, {
      data: {
        schoolYear: this.schoolYear,
        instId: this.instId,
        classBookId: this.classBookId
      }
    })
      .afterClosed()
      .toPromise()
      .then((result) => {
        if (result) {
          this.dataSource.reload();
        }
      });
  }

  openStudentDialog(personId: number) {
    openTypedDialog(this.dialog, BookAdminStudentDialogSkeletonComponent, {
      data: {
        schoolYear: this.schoolYear,
        instId: this.instId,
        classBookId: this.classBookId,
        personId,
        bookType: this.bookType,
        showSpecialNeedFirstGradeResult:
          (this.bookType === ClassBookType.Book_I_III &&
            this.data.classBookAdminInfo.basicClassId !== SecondGradeBasicClassId &&
            this.data.classBookAdminInfo.basicClassId !== ThirdGradeBasicClassId) ||
          (this.bookType === ClassBookType.Book_CSOP &&
            this.data.classBookAdminInfo.basicClassId === FirstGradeBasicClassId)
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

  ngOnInit() {
    const bookType = this.data.classBookAdminInfo.bookType;
    this.bookType = bookType;

    this.canEdit =
      this.data.classBookAdminInfo.bookAllowsModifications && this.data.classBookAdminInfo.hasEditStudentAccess;

    this.displayedColumns = [
      this.canEdit ? 'action' : null,
      'classNumber',
      'studentFullName',
      bookType === ClassBookType.Book_PG || bookType === ClassBookType.Book_I_III || bookType === ClassBookType.Book_IV
        ? 'activities'
        : null,
      bookType === ClassBookType.Book_V_XII ? 'speciality' : null,
      'parentNames',
      bookType === ClassBookType.Book_I_III ||
      bookType === ClassBookType.Book_IV ||
      bookType === ClassBookType.Book_V_XII ||
      bookType === ClassBookType.Book_CSOP
        ? 'hasSpecialNeeds'
        : null,
      bookType === ClassBookType.Book_I_III ||
      bookType === ClassBookType.Book_IV ||
      bookType === ClassBookType.Book_V_XII ||
      bookType === ClassBookType.Book_CSOP
        ? 'hasGradelessSubjects'
        : null
    ].filter(notEmpty);

    this.activitiesFieldName =
      this.bookType === ClassBookType.Book_PG
        ? 'Дейности по чл. 19'
        : this.bookType === ClassBookType.Book_I_III || this.bookType === ClassBookType.Book_IV
        ? 'Дейности по интереси'
        : '';
  }

  onSortAlphabetically() {
    this.sorting = true;

    this.actionService
      .execute({
        confirmMessage:
          'Внимание! Това действие ще смени номерата на всички ученици в дневника по азбучен ред, ' +
          'а отписаните (с изключение на тези които имат отсъствия) ще бъдат без "номер в класа" ' +
          'и ще се наредят последни. Сигурни ли сте, че искате да продължите?',
        httpAction: () =>
          this.classBooksAdminService
            .sortStudentClassNumbers({
              schoolYear: this.schoolYear,
              instId: this.instId,
              classBookId: this.classBookId
            })
            .toPromise()
      })
      .then((done) => {
        if (done) {
          return this.dataSource.reload();
        }
      })
      .finally(() => {
        this.sorting = false;
      });
  }
}
