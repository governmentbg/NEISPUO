import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { faArrowLeft as fasArrowLeft } from '@fortawesome/pro-solid-svg-icons/faArrowLeft';
import { faBallPile as fasBallPile } from '@fortawesome/pro-solid-svg-icons/faBallPile';
import { faChild as fasChild } from '@fortawesome/pro-solid-svg-icons/faChild';
import { faUsersClass as fasUsersClass } from '@fortawesome/pro-solid-svg-icons/faUsersClass';
import {
  StudentInfoClassBooksService,
  StudentInfoClassBooks_GetAllClassBooks
} from 'projects/sb-api-client/src/api/studentInfoClassBooks.service';
import {
  SIMPLE_TAB_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { TabItem } from 'projects/shared/components/tabs/tab-item';
import { ArrayElementType } from 'projects/shared/utils/type';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { Subject } from 'rxjs';

type ClassBook = ArrayElementType<StudentInfoClassBooks_GetAllClassBooks>;

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class StudentInfoSkeletonComponent extends SkeletonComponentBase {
  constructor(studentInfoClassBooksService: StudentInfoClassBooksService, route: ActivatedRoute, router: Router) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const personId = tryParseInt(route.snapshot.paramMap.get('personId')) ?? throwParamError('personId');

    this.resolve(StudentInfoComponent, {
      routerBackState: router.getCurrentNavigation()?.previousNavigation?.finalUrl?.toString() ?? '',
      schoolYear,
      classBooks: studentInfoClassBooksService.getAllClassBooks({
        schoolYear,
        instId,
        classBookId,
        personId
      })
    });
  }
}

@Component({
  selector: 'sb-student-info',
  templateUrl: './student-info.component.html'
})
export class StudentInfoComponent implements OnInit {
  @Input() data!: {
    routerBackState: string;
    schoolYear: number;
    classBooks: StudentInfoClassBooks_GetAllClassBooks;
  };

  readonly destroyed$ = new Subject<void>();

  tabs!: TabItem[];

  readonly fasBallPile = fasBallPile;
  readonly fasUsersClass = fasUsersClass;
  readonly fasArrowLeft = fasArrowLeft;

  constructor(private router: Router) {}

  ngOnInit() {
    this.tabs = this.data.classBooks.map((cb) => ({
      text: `${cb.instName} - ${getBookDisplayName(cb)}`,
      badge: !cb.isValid ? 'АРХИВИРАН' : null,
      icon: fasChild,
      routeCommands: ['./student-book', cb.classBookId]
    }));
  }

  navigate() {
    this.router.navigateByUrl(this.data.routerBackState);
  }
}

function getBookDisplayName(book: ClassBook) {
  if (!book.bookName || book.bookName?.length === 1) {
    return `${book.basicClassName || ''}${book.bookName || ''}`;
  } else if (book.basicClassName) {
    return `${book.basicClassName} - ${book.bookName}`;
  } else {
    return book.bookName;
  }
}
