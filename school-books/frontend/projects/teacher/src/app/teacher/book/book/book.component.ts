import { Component, Inject, InjectionToken, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faCalendarCheck as fadCalendarCheck } from '@fortawesome/pro-duotone-svg-icons/faCalendarCheck';
import { faCalendarEdit as fadCalendarEdit } from '@fortawesome/pro-duotone-svg-icons/faCalendarEdit';
import { faCalendarTimes as fadCalendarTimes } from '@fortawesome/pro-duotone-svg-icons/faCalendarTimes';
import { faClipboard as fadClipboard } from '@fortawesome/pro-duotone-svg-icons/faClipboard';
import { faClipboardUser as fadClipboardUser } from '@fortawesome/pro-duotone-svg-icons/faClipboardUser';
import { faClock as fadClock } from '@fortawesome/pro-duotone-svg-icons/faClock';
import { faCog as fadCog } from '@fortawesome/pro-duotone-svg-icons/faCog';
import { faCommentDots as fadCommentDots } from '@fortawesome/pro-duotone-svg-icons/faCommentDots';
import { faGavel as fadGavel } from '@fortawesome/pro-duotone-svg-icons/faGavel';
import { faGraduationCap as fadGraduationCap } from '@fortawesome/pro-duotone-svg-icons/faGraduationCap';
import { faHandshake as fadHandshake } from '@fortawesome/pro-duotone-svg-icons/faHandshake';
import { faPencilAlt as fadPencilAlt } from '@fortawesome/pro-duotone-svg-icons/faPencilAlt';
import { faStarHalfAlt as fadStarHalfAlt } from '@fortawesome/pro-duotone-svg-icons/faStarHalfAlt';
import { faTheaterMasks as fadTheaterMasks } from '@fortawesome/pro-duotone-svg-icons/faTheaterMasks';
import { faUserEdit as fadUserEdit } from '@fortawesome/pro-duotone-svg-icons/faUserEdit';
import { faBallPile as fasBallPile } from '@fortawesome/pro-solid-svg-icons/faBallPile';
import { faListOl as fasListOl } from '@fortawesome/pro-solid-svg-icons/faListOl';
import { faPencil as fadPencil } from '@fortawesome/pro-solid-svg-icons/faPencil';
import { faUsersClass as fasUsersClass } from '@fortawesome/pro-solid-svg-icons/faUsersClass';
import { ClassBooksService } from 'projects/sb-api-client/src/api/classBooks.service';
import { ClassBookType } from 'projects/sb-api-client/src/model/classBookType';
import {
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { TabItem } from 'projects/shared/components/tabs/tab-item';
import { ClassBookInfoType, extendClassBookInfo, isGroupClassBookType } from 'projects/shared/utils/book';
import { BookTabRoutesConfig, mapBookTypeToTabs } from 'projects/shared/utils/book-tabs';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from, Subject } from 'rxjs';
import { map } from 'rxjs/operators';

export const CLASS_BOOK_INFO = new InjectionToken<Promise<ClassBookInfoType>>('Class book info');
export const classBookInfoProviderFactory = (classBooksService: ClassBooksService, route: ActivatedRoute) =>
  classBooksService
    .get({
      schoolYear: tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear'),
      instId: tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId'),
      classBookId: tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId')
    })
    .pipe(map(extendClassBookInfo))
    .toPromise();

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE,
  providers: [
    {
      provide: CLASS_BOOK_INFO,
      deps: [ClassBooksService, ActivatedRoute],
      useFactory: classBookInfoProviderFactory
    }
  ]
})
export class BookSkeletonComponent extends SkeletonComponentBase {
  constructor(@Inject(CLASS_BOOK_INFO) classBookInfo: Promise<ClassBookInfoType>) {
    super();

    this.resolve(BookComponent, {
      classBookInfo: from(classBookInfo)
    });
  }
}

@Component({
  selector: 'sb-book',
  templateUrl: './book.component.html'
})
export class BookComponent implements OnInit {
  @Input() data!: {
    classBookInfo: ClassBookInfoType;
  };

  readonly destroyed$ = new Subject<void>();

  isGroup!: boolean;
  tabs!: TabItem[];
  teachersType!: string;

  readonly fasBallPile = fasBallPile;
  readonly fasUsersClass = fasUsersClass;
  readonly fadCog = fadCog;

  ngOnInit() {
    this.isGroup = isGroupClassBookType(this.data.classBookInfo.bookType);
    this.tabs = mapBookTypeToTabs(false, tabConfig, {
      bookType: this.data.classBookInfo.bookType,
      basicClassId: this.data.classBookInfo.basicClassId
    });

    this.teachersType = this.data.classBookInfo.bookType === ClassBookType.Book_PG ? 'учители' : 'класен';
  }
}

const tabConfig = {
  grades: {
    text: 'Оценки',
    icon: fadPencilAlt,
    routeCommands: ['./', ...BookTabRoutesConfig.grades]
  },
  absences: {
    text: 'Отсъствия',
    icon: fadCalendarTimes,
    routeCommands: ['./', ...BookTabRoutesConfig.absences]
  },
  absencesDplr: {
    text: 'Отсъствия',
    icon: fadCalendarTimes,
    routeCommands: ['./', ...BookTabRoutesConfig.absencesDplr]
  },
  attendancesDplr: {
    text: 'Присъствия',
    icon: fadCalendarCheck,
    routeCommands: ['./', ...BookTabRoutesConfig.attendancesDplr]
  },
  attendances: {
    text: 'Присъствия',
    icon: fadCalendarTimes,
    routeCommands: ['./', ...BookTabRoutesConfig.attendances]
  },
  topics: {
    text: 'Теми',
    icon: fadCommentDots,
    routeCommands: ['./', ...BookTabRoutesConfig.topics]
  },
  topicsDplr: {
    text: 'Теми',
    icon: fadCommentDots,
    routeCommands: ['./', ...BookTabRoutesConfig.topicsDplr]
  },
  remarks: {
    text: 'Отзиви',
    icon: fadStarHalfAlt,
    routeCommands: ['./', ...BookTabRoutesConfig.remarks]
  },
  schedule: {
    text: 'Разписание',
    icon: fadClock,
    routeCommands: ['./', ...BookTabRoutesConfig.schedule]
  },
  parentMeetings: {
    text: 'Срещи с родители',
    icon: fadCalendarCheck,
    routeCommands: ['./', ...BookTabRoutesConfig.parentMeetings]
  },
  exams: {
    text: 'Контролни',
    icon: fadCalendarEdit,
    routeCommands: ['./', ...BookTabRoutesConfig.exams]
  },
  sanctions: {
    text: 'Санкции',
    icon: fadGavel,
    routeCommands: ['./', ...BookTabRoutesConfig.sanctions]
  },
  classBookTopicPlans: {
    text: 'Тематични разпр.',
    icon: fasListOl,
    routeCommands: ['./', ...BookTabRoutesConfig.classBookTopicPlans]
  },
  supports: {
    text: 'Подкрепа',
    icon: fadHandshake,
    routeCommands: ['./', ...BookTabRoutesConfig.supports]
  },
  notes: {
    text: 'Бележки',
    icon: fadClipboard,
    routeCommands: ['./', ...BookTabRoutesConfig.notes]
  },
  firstGradeResults: {
    text: 'Общ годишен успех',
    icon: fadGraduationCap,
    routeCommands: ['./', ...BookTabRoutesConfig.firstGradeResults]
  },
  pgResults: {
    text: 'Резултати',
    icon: fadGraduationCap,
    routeCommands: ['./', ...BookTabRoutesConfig.pgResults]
  },
  individualWorks: {
    text: 'Индивидуална работа',
    icon: fadUserEdit,
    routeCommands: ['./', ...BookTabRoutesConfig.individualWorks]
  },
  gradeResults: {
    text: 'Резултати',
    icon: fadGraduationCap,
    routeCommands: ['./', ...BookTabRoutesConfig.gradeResults]
  },
  sessions: {
    text: 'Поправки',
    icon: fadPencil,
    routeCommands: ['./', ...BookTabRoutesConfig.sessions]
  },
  performances: {
    text: 'Изяви',
    icon: fadTheaterMasks,
    routeCommands: ['./', ...BookTabRoutesConfig.performances]
  },
  replrParticipations: {
    text: 'Участия в РЕПЛР',
    icon: fadClipboardUser,
    routeCommands: ['./', ...BookTabRoutesConfig.replrParticipations]
  }
};
