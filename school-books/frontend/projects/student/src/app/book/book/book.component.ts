import { Component, Inject, InjectionToken, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faCalendarCheck as fadCalendarCheck } from '@fortawesome/pro-duotone-svg-icons/faCalendarCheck';
import { faCalendarEdit as fadCalendarEdit } from '@fortawesome/pro-duotone-svg-icons/faCalendarEdit';
import { faCalendarTimes as fadCalendarTimes } from '@fortawesome/pro-duotone-svg-icons/faCalendarTimes';
import { faClipboard as fadClipboard } from '@fortawesome/pro-duotone-svg-icons/faClipboard';
import { faClipboardUser as fadClipboardUser } from '@fortawesome/pro-duotone-svg-icons/faClipboardUser';
import { faClock as fadClock } from '@fortawesome/pro-duotone-svg-icons/faClock';
import { faCog as fadCog } from '@fortawesome/pro-duotone-svg-icons/faCog';
import { faGavel as fadGavel } from '@fortawesome/pro-duotone-svg-icons/faGavel';
import { faGraduationCap as fadGraduationCap } from '@fortawesome/pro-duotone-svg-icons/faGraduationCap';
import { faHandshake as fadHandshake } from '@fortawesome/pro-duotone-svg-icons/faHandshake';
import { faPencil as fadPencil } from '@fortawesome/pro-duotone-svg-icons/faPencil';
import { faPencilAlt as fadPencilAlt } from '@fortawesome/pro-duotone-svg-icons/faPencilAlt';
import { faStarHalfAlt as fadStarHalfAlt } from '@fortawesome/pro-duotone-svg-icons/faStarHalfAlt';
import { faTheaterMasks as fadTheaterMasks } from '@fortawesome/pro-duotone-svg-icons/faTheaterMasks';
import { faUserEdit as fadUserEdit } from '@fortawesome/pro-duotone-svg-icons/faUserEdit';
import { faTasks as fasTasks } from '@fortawesome/pro-solid-svg-icons/faTasks';
import {
  StudentClassBookService,
  StudentClassBook_GetClassBookInfo
} from 'projects/sb-api-client/src/api/studentClassBook.service';
import {
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { TabItem } from 'projects/shared/components/tabs/tab-item';
import { AuthService, SysRole } from 'projects/shared/services/auth.service';
import { BookTabRoutesConfig, mapBookTypeToTabs } from 'projects/shared/utils/book-tabs';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from } from 'rxjs';

export const CLASS_BOOK_INFO = new InjectionToken<Promise<StudentClassBook_GetClassBookInfo>>('Class book info');
export const classBookInfoProviderFactory = (studentClassBookService: StudentClassBookService, route: ActivatedRoute) =>
  studentClassBookService
    .getClassBookInfo({
      schoolYear: tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear'),
      classBookId: tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId'),
      personId: tryParseInt(route.snapshot.paramMap.get('personId')) ?? throwParamError('personId')
    })
    .toPromise();

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE,
  providers: [
    {
      provide: CLASS_BOOK_INFO,
      deps: [StudentClassBookService, ActivatedRoute],
      useFactory: classBookInfoProviderFactory
    }
  ]
})
export class BookSkeletonComponent extends SkeletonComponentBase {
  constructor(@Inject(CLASS_BOOK_INFO) classBookInfo: Promise<StudentClassBook_GetClassBookInfo>) {
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
    classBookInfo: StudentClassBook_GetClassBookInfo;
  };

  readonly fadCog = fadCog;

  tabs!: TabItem[];
  showSettings = false;
  readonly maxAbcencesByFamilyReasons = 15;

  constructor(private authService: AuthService) {}

  ngOnInit() {
    this.showSettings =
      this.authService.tokenPayload.selected_role.SysRoleID === SysRole.Parent ||
      this.authService.tokenPayload.selected_role.SysRoleID === SysRole.Student;
    this.tabs = mapBookTypeToTabs(true, tabConfig, {
      bookType: this.data.classBookInfo.bookType,
      basicClassId: this.data.classBookInfo.basicClassId
    });
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
    text: 'Взети теми',
    icon: fasTasks,
    routeCommands: ['./', ...BookTabRoutesConfig.topics]
  },
  topicsDplr: {
    text: 'Взети теми',
    icon: fasTasks,
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
    // does not exist in students book
    // added to make TS happy
    text: 'Тематични разпр.',
    icon: fadHandshake,
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
