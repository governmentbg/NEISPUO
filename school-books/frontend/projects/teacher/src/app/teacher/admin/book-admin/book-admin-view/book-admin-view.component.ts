import { Component, Inject, InjectionToken, Input, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { IconDefinition } from '@fortawesome/fontawesome-svg-core';
import { faBallot as fadBallot } from '@fortawesome/pro-duotone-svg-icons/faBallot';
import { faBook as fadBook } from '@fortawesome/pro-duotone-svg-icons/faBook';
import { faBookOpen as fadBookOpen } from '@fortawesome/pro-duotone-svg-icons/faBookOpen';
import { faCalendarCheck as fadCalendarCheck } from '@fortawesome/pro-duotone-svg-icons/faCalendarCheck';
import { faChild as fadChild } from '@fortawesome/pro-duotone-svg-icons/faChild';
import { faClock as fadClock } from '@fortawesome/pro-duotone-svg-icons/faClock';
import { faFilePdf as fadFilePdf } from '@fortawesome/pro-duotone-svg-icons/faFilePdf';
import {
  ClassBooksAdminService,
  ClassBooksAdmin_GetInfo
} from 'projects/sb-api-client/src/api/classBooksAdmin.service';
import { ClassBookType } from 'projects/sb-api-client/src/model/classBookType';
import {
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { EventService, EventType } from 'projects/shared/services/event.service';
import { checkBookAllowsModifications } from 'projects/shared/utils/book';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from, Subject } from 'rxjs';
import { map, switchMap, takeUntil, tap } from 'rxjs/operators';

export type ClassBookAdminInfoType = ClassBooksAdmin_GetInfo & {
  bookAllowsModifications: boolean;
};

const classBookAdminInfoGetter = (classBooksAdminService: ClassBooksAdminService, route: ActivatedRoute) =>
  classBooksAdminService
    .getInfo({
      schoolYear: tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear'),
      instId: tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId'),
      classBookId: tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId')
    })
    .pipe(
      map(
        (classBookAdminInfo: ClassBooksAdmin_GetInfo): ClassBookAdminInfoType => ({
          ...classBookAdminInfo,
          bookAllowsModifications:
            !classBookAdminInfo.hasCBExtProvider &&
            classBookAdminInfo.isValid &&
            checkBookAllowsModifications(classBookAdminInfo.schoolYearIsFinalized, classBookAdminInfo.isFinalized)
        })
      )
    )
    .toPromise();

export const CLASS_BOOK_ADMIN_INFO = new InjectionToken<Promise<ClassBooksAdmin_GetInfo>>('Class book admin info');
export const classBookAdminInfoProviderFactory = (
  classBooksAdminService: ClassBooksAdminService,
  route: ActivatedRoute
) => classBookAdminInfoGetter(classBooksAdminService, route);

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE,
  providers: [
    {
      provide: CLASS_BOOK_ADMIN_INFO,
      deps: [ClassBooksAdminService, ActivatedRoute],
      useFactory: classBookAdminInfoProviderFactory
    }
  ]
})
export class BookAdminViewSkeletonComponent extends SkeletonComponentBase {
  constructor(@Inject(CLASS_BOOK_ADMIN_INFO) classBookAdminInfo: Promise<ClassBookAdminInfoType>) {
    super();

    this.resolve(BookAdminViewComponent, {
      classBookAdminInfo: from(classBookAdminInfo)
    });
  }
}

@Component({
  selector: 'sb-book-admin-view',
  templateUrl: './book-admin-view.component.html'
})
export class BookAdminViewComponent implements OnInit, OnDestroy {
  @Input() data!: {
    classBookAdminInfo: ClassBookAdminInfoType;
  };

  protected readonly destroyed$ = new Subject<void>();

  fadBallot = fadBallot;
  fadBookOpen = fadBookOpen;
  fadBook = fadBook;
  fadChild = fadChild;
  fadClock = fadClock;
  fadCalendarCheck = fadCalendarCheck;

  tabs!: { text: string; icon: IconDefinition; routeCommands: string[] }[];

  constructor(eventService: EventService, classBooksAdminService: ClassBooksAdminService, route: ActivatedRoute) {
    eventService
      .on(EventType.ClassBooksUpdated)
      .pipe(
        switchMap(() => classBookAdminInfoGetter(classBooksAdminService, route)),
        tap((classBookAdminInfo) => {
          this.data.classBookAdminInfo = classBookAdminInfo;
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();
  }

  ngOnInit(): void {
    const bookType = this.data.classBookAdminInfo.bookType;
    this.tabs = [
      {
        text: 'Основни данни',
        icon: fadBallot,
        routeCommands: ['./main']
      },
      {
        text: 'Предмети',
        icon: fadBookOpen,
        routeCommands: ['./subjects']
      },
      {
        text: 'Ученици',
        icon: fadChild,
        routeCommands: ['./students']
      },
      {
        text: 'Разписания',
        icon: fadClock,
        routeCommands: ['./schedules']
      },
      ...(bookType === ClassBookType.Book_I_III ||
      bookType === ClassBookType.Book_IV ||
      bookType === ClassBookType.Book_V_XII ||
      bookType === ClassBookType.Book_CSOP
        ? [
            {
              text: 'Разписания (ИУП)',
              icon: fadClock,
              routeCommands: ['./individual-schedules']
            }
          ]
        : []),
      ...(bookType === ClassBookType.Book_CDO ||
      bookType === ClassBookType.Book_DPLR ||
      bookType === ClassBookType.Book_CSOP
        ? [
            {
              text: 'Годишна програма',
              icon: fadCalendarCheck,
              routeCommands: ['./schoolYearProgram']
            }
          ]
        : []),
      {
        text: 'Принтиране',
        icon: fadFilePdf,
        routeCommands: ['./print']
      }
    ];
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }
}
