import { Component, Inject, InjectionToken, Input, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { IconDefinition } from '@fortawesome/fontawesome-svg-core';
import { faAnalytics as fadAnalytics } from '@fortawesome/pro-duotone-svg-icons/faAnalytics';
import { faBellSchool as fadBellSchool } from '@fortawesome/pro-duotone-svg-icons/faBellSchool';
import { faBook as fadBook } from '@fortawesome/pro-duotone-svg-icons/faBook';
import { faCabinetFiling as fadCabinetFiling } from '@fortawesome/pro-duotone-svg-icons/faCabinetFiling';
import { faCalendarAlt as fadCalendarAlt } from '@fortawesome/pro-duotone-svg-icons/faCalendarAlt';
import { faCalendarDay as fadCalendarDay } from '@fortawesome/pro-duotone-svg-icons/faCalendarDay';
import { faCalendarMinus as fadCalendarMinus } from '@fortawesome/pro-duotone-svg-icons/faCalendarMinus';
import { faCalendarStar as fadCalendarStar } from '@fortawesome/pro-duotone-svg-icons/faCalendarStar';
import { faCalendarWeek as fadCalendarWeek } from '@fortawesome/pro-duotone-svg-icons/faCalendarWeek';
import { faChalkboardTeacher as fadChalkboardTeacher } from '@fortawesome/pro-duotone-svg-icons/faChalkboardTeacher';
import { faCogs as fadCogs } from '@fortawesome/pro-duotone-svg-icons/faCogs';
import { faFileAlt as fadFileAlt } from '@fortawesome/pro-duotone-svg-icons/faFileAlt';
import { faFileCertificate as fadFileCertificate } from '@fortawesome/pro-duotone-svg-icons/faFileCertificate';
import { faLock as fadLock } from '@fortawesome/pro-duotone-svg-icons/faLock';
import { faTasksAlt as fadTasksAlt } from '@fortawesome/pro-duotone-svg-icons/faTasksAlt';
import { faUserMinus as fadUserMinus } from '@fortawesome/pro-duotone-svg-icons/faUserMinus';
import { faUserPlus as fadUserPlus } from '@fortawesome/pro-duotone-svg-icons/faUserPlus';
import { faBallPile as fasBallPile } from '@fortawesome/pro-solid-svg-icons/faBallPile';
import { faInfoSquare as fadInfoSquare } from '@fortawesome/pro-solid-svg-icons/faInfoSquare';
import { faListOl as fasListOl } from '@fortawesome/pro-solid-svg-icons/faListOl';
import { faTimes as fasTimes } from '@fortawesome/pro-solid-svg-icons/faTimes';
import { faUsersClass as fasUsersClass } from '@fortawesome/pro-solid-svg-icons/faUsersClass';
import {
  ConversationsService,
  Conversations_GetUnreadConversations
} from 'projects/sb-api-client/src/api/conversations.service';
import { InstitutionsService, Institutions_Get } from 'projects/sb-api-client/src/api/institutions.service';
import { TeacherService, Teacher_GetAllClassBooks } from 'projects/sb-api-client/src/api/teacher.service';
import { InstType } from 'projects/sb-api-client/src/model/instType';
import { AppChromeSchoolYear, UnreadConversations } from 'projects/shared/components/app-chrome/app-chrome.component';
import { MenuItem } from 'projects/shared/components/app-menu/menu-item';
import {
  APP_CHROME_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { AuthService } from 'projects/shared/services/auth.service';
import { EventService, EventType } from 'projects/shared/services/event.service';
import { LocalStorageService } from 'projects/shared/services/local-storage.service';
import { PwaService } from 'projects/shared/services/pwa-service/pwa.service';
import { groupBy, stableSort } from 'projects/shared/utils/array';
import { isGroupClassBookType } from 'projects/shared/utils/book';
import { BookTabRoutesConfig, mapBookTypeToTabs } from 'projects/shared/utils/book-tabs';
import { ArrayElementType } from 'projects/shared/utils/type';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { concat, from, interval, merge, Observable, of, Subject, Subscription } from 'rxjs';
import { filter, map, switchMap, take, takeUntil, tap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { LectureSchedulesMode } from './lecture-schedules/lecture-schedules/lecture-schedules.component';
import { TeacherAbsencesMode } from './teacher-absences/teacher-absences/teacher-absences.component';

const CLASS_LETTER_REGEX = /^[а-яА-Я]$/;
const NO_BASIC_CLASS = -999;

type ClassBook = ArrayElementType<Teacher_GetAllClassBooks>;

export type InstitutionInfoType = Institutions_Get & {
  schoolYearAllowsModifications: boolean;
};

const institutionInfoGetter = (institutionsService: InstitutionsService, route: ActivatedRoute) =>
  institutionsService
    .get({
      schoolYear: tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear'),
      instId: tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId')
    })
    .pipe(
      map(
        (institutionInfo: Institutions_Get): InstitutionInfoType => ({
          ...institutionInfo,
          schoolYearAllowsModifications: !institutionInfo.hasCBExtProvider && !institutionInfo.schoolYearIsFinalized
        })
      )
    )
    .toPromise();

export const INSTITUTION_INFO = new InjectionToken<Promise<InstitutionInfoType>>('Institution info');
export const institutionInfoProviderFactory = (institutionsService: InstitutionsService, route: ActivatedRoute) =>
  institutionInfoGetter(institutionsService, route);
@Component({
  template: APP_CHROME_SKELETON_TEMPLATE,
  providers: [
    {
      provide: INSTITUTION_INFO,
      deps: [InstitutionsService, ActivatedRoute],
      useFactory: institutionInfoProviderFactory
    }
  ]
})
export class TeacherSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    teacherService: TeacherService,
    conversationsService: ConversationsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');

    this.resolve(TeacherComponent, {
      schoolYear,
      instId,
      institutionInfo: from(institutionInfo),
      classBooks: teacherService.getAllClassBooks({ schoolYear, instId }),
      unreadConversations: conversationsService.getUnreadConversations({ limit: 5 })
    });
  }
}

@Component({
  selector: 'sb-teacher',
  templateUrl: './teacher.component.html'
})
export class TeacherComponent implements OnInit, OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    institutionInfo: InstitutionInfoType;
    classBooks: Teacher_GetAllClassBooks;
    unreadConversations: Conversations_GetUnreadConversations;
  };

  protected readonly destroyed$ = new Subject<void>();
  private pollingSubscription?: Subscription;

  isMultiInstitutionUser: boolean;
  isTeacherOrInstAdminUser: boolean;
  hasPersonId: boolean;
  bookAdminRouteCommands: unknown[] = [];
  unreadConversations: UnreadConversations[] = [];
  mobileAppBannerHidden = false;
  shouldShowEnvError = false;
  readonly fasTimes = fasTimes;
  readonly docsUrl = environment.docsUrl + 'mobile';

  constructor(
    authService: AuthService,
    teacherService: TeacherService,
    eventService: EventService,
    institutionsService: InstitutionsService,
    public localStorageService: LocalStorageService,
    route: ActivatedRoute,
    private pwaService: PwaService,
    private conversationsService: ConversationsService
  ) {
    this.isMultiInstitutionUser = authService.isMultiInstitutionUser;
    this.isTeacherOrInstAdminUser = authService.isTeacherOrInstAdminUser;
    this.hasPersonId = authService.userPersonId != null;

    eventService
      .on(EventType.ClassBooksUpdated)
      .pipe(
        switchMap(() =>
          teacherService.getAllClassBooks({ schoolYear: this.data.schoolYear, instId: this.data.instId })
        ),
        tap((classBooks) => {
          this.data.classBooks = classBooks;
          this.rebuildMenu(this.data.classBooks);
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();

    eventService
      .on([EventType.SchoolYearSettingsUpdated, EventType.ClassBooksUpdated])
      .pipe(
        switchMap(() => institutionInfoGetter(institutionsService, route)),
        tap((institutionInfo) => {
          this.data.institutionInfo = institutionInfo;
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();

    eventService
      .on(EventType.ConversationRead)
      .pipe(
        switchMap(() => this.updateUnreadConversations()),
        tap((conversations) => {
          this.unreadConversations = conversations.map((m) => ({
            conversationId: m.conversationId,
            title: m.title,
            messageDate: m.messageDate,
            conversationRouteCommands: ['./conversations', m.schoolYear, m.conversationId]
          }));
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();
  }

  showBooks = false;
  showProtocols = false;
  hasAdminReportAccess = false;
  schoolYears: AppChromeSchoolYear[] = [];
  menuItems: MenuItem[] = [];

  ngOnInit() {
    this.showBooks = this.data.institutionInfo.hasBooksReadAccess;
    this.showProtocols = this.data.institutionInfo.showProtocols && this.data.institutionInfo.hasProtocolsReadAccess;
    this.schoolYears = this.data.institutionInfo.schoolYears.map((sy) => ({
      schoolYear: sy,
      schoolYearRouteCommands: ['/', sy, this.data.instId]
    }));
    this.shouldShowEnvError = !isProdStage() && window.matchMedia('(display-mode: standalone)').matches;
    this.bookAdminRouteCommands = getBookAdminRouteCommands(this.data.institutionInfo);
    this.hasAdminReportAccess =
      this.data.institutionInfo.hasStudentsAtRiskOfDroppingOutReportAccess &&
      this.data.institutionInfo.hasGradelessStudentsReportAccess &&
      this.data.institutionInfo.hasSessionStudentsReportAccess &&
      this.data.institutionInfo.hasAbsencesByStudentsReportAccess &&
      this.data.institutionInfo.hasAbsencesByClassesReportAccess &&
      this.data.institutionInfo.hasRegularGradePointAverageByStudentsReportAccess &&
      this.data.institutionInfo.hasDateAbsencesReportAccess &&
      this.data.institutionInfo.hasExamsReportAccess &&
      this.data.institutionInfo.hasRegularGradePointAverageByClassesReportAccess &&
      this.data.institutionInfo.hasScheduleAndAbsencesByTermReportAccess &&
      this.data.institutionInfo.hasScheduleAndAbsencesByMonthReportAccess &&
      this.data.institutionInfo.hasFinalGradePointAverageByStudentsReportAccess &&
      this.data.institutionInfo.hasScheduleAndAbsencesByTermAllClassesReportAccess &&
      this.data.institutionInfo.hasFinalGradePointAverageByClassesReportAccess;

    this.rebuildMenu(this.data.classBooks);
    this.pwaService.initPwaPrompt();

    this.pollingSubscription = concat(of(null), interval(60000))
      .pipe(switchMap(() => this.updateUnreadConversations()))
      .subscribe((conversations) => {
        this.unreadConversations = conversations.map((m) => ({
          conversationId: m.conversationId,
          title: m.title,
          messageDate: m.messageDate,
          conversationRouteCommands: ['./conversations', m.schoolYear, m.conversationId]
        }));
      });

    this.mobileAppBannerHidden = this.localStorageService.getMobileAppBannerHidden();
    this.localStorageService.mobileAppBannerHidden$
      .pipe(
        filter((mobileAppBannerHidden) => mobileAppBannerHidden),
        tap(() => this.hideMobileAppBanner()),
        take(1),
        takeUntil(
          merge(
            // unsubscribe if the user hides the banner himself
            this.localStorageService.mobileAppBannerHidden$.pipe(
              filter((mobileAppBannerHidden) => mobileAppBannerHidden)
            ),
            this.destroyed$
          )
        )
      )
      .subscribe();
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
    if (this.pollingSubscription) {
      this.pollingSubscription?.unsubscribe();
    }
  }

  rebuildMenu(classBooks: ClassBook[]) {
    let bookMenuItems = null;
    let archivedBookMenuItems = null;
    if (!classBooks.length) {
      bookMenuItems = [{ text: 'Няма дневници' }];
      archivedBookMenuItems = [{ text: 'Няма дневници' }];
    } else {
      const classBookMenuItems = this.createGroupedBookMenuItems(
        classBooks.filter((b) => !isGroupClassBookType(b.bookType) && b.isValid),
        fasUsersClass
      );

      const groupBookMenuItems = this.createGroupedBookMenuItems(
        classBooks.filter((b) => isGroupClassBookType(b.bookType) && b.isValid),
        fasBallPile
      );

      const archivedClassBookMenuItems = this.createGroupedBookMenuItems(
        classBooks.filter((b) => !isGroupClassBookType(b.bookType) && !b.isValid),
        fasUsersClass
      );

      const archivedGroupBookMenuItems = this.createGroupedBookMenuItems(
        classBooks.filter((b) => isGroupClassBookType(b.bookType) && !b.isValid),
        fasBallPile
      );

      bookMenuItems = [...classBookMenuItems, ...groupBookMenuItems];
      archivedBookMenuItems = [...archivedClassBookMenuItems, ...archivedGroupBookMenuItems];
    }

    this.menuItems = [
      {
        text: 'Инф. табло',
        icon: fadInfoSquare,
        routeCommands: ['./info-board', { status: 'published' }]
      },
      ...(this.hasPersonId
        ? [
            {
              text: 'Моето разписание',
              icon: fadCalendarAlt,
              routeCommands: ['./my-schedule']
            }
          ]
        : []),
      ...(this.hasPersonId && this.data.schoolYear >= 2023
        ? [
            {
              text: 'Моите часове',
              icon: fadChalkboardTeacher,
              routeCommands: ['./my-hour']
            }
          ]
        : []),
      {
        text: 'Дневници',
        icon: fadBook,
        menuItems: bookMenuItems
      },
      ...(this.showBooks
        ? [
            {
              text: 'Книги',
              icon: fadCabinetFiling,
              menuItems: [
                ...(this.data.institutionInfo.showSpbsBook
                  ? [
                      {
                        text: 'Книга за движението на учениците от СПИ/ВУИ',
                        icon: fadFileAlt,
                        routeCommands: ['./spbs-book']
                      }
                    ]
                  : []),
                ...(this.data.institutionInfo.showReqBookQualification
                  ? [
                      {
                        text: 'Регистрационна книга за издадените документи',
                        icon: fadFileCertificate,
                        routeCommands: ['./reg-book-qualification']
                      },
                      {
                        text: 'Регистрационна книга за издадените дубликати на документи',
                        icon: fadFileCertificate,
                        routeCommands: ['./reg-book-qualification-duplicates']
                      }
                    ]
                  : []),
                {
                  text: 'Регистрационна книга за издадените удостоверения',
                  icon: fadFileCertificate,
                  routeCommands: ['./reg-book-certificates']
                },
                {
                  text: 'Регистрационна книга за издадените дубликати на удостоверения',
                  icon: fadFileCertificate,
                  routeCommands: ['./reg-book-certificate-duplicates']
                }
              ]
            }
          ]
        : []),
      ...(this.showProtocols
        ? [
            {
              text: 'Протоколи',
              icon: fadCabinetFiling,
              menuItems: [
                {
                  text: 'Протоколи за допускане до изпити',
                  icon: fadCabinetFiling,
                  routeCommands: ['./adm-protocols']
                },
                {
                  text: 'Протоколи за провеждане на изпити',
                  icon: fadCabinetFiling,
                  routeCommands: ['./exam-duty-protocols']
                },
                {
                  text: 'Протоколи за резултати от изпити',
                  icon: fadCabinetFiling,
                  routeCommands: ['./exam-result-protocols']
                }
              ]
            }
          ]
        : []),

      {
        text: 'Лекторски часове',
        icon: fadCalendarStar,
        menuItems: [
          ...(this.hasPersonId
            ? [
                {
                  text: 'Мои лекторски',
                  icon: fadUserPlus,
                  routeCommands: ['./lecture-schedules', { mode: LectureSchedulesMode.My }]
                }
              ]
            : []),

          ...(this.data.institutionInfo.hasTeacherAbsencesAllReadAccess
            ? [
                {
                  text: 'Всички лекторски',
                  icon: fadCalendarStar,
                  routeCommands: ['./lecture-schedules', { mode: LectureSchedulesMode.All }]
                }
              ]
            : [])
        ]
      },

      {
        text: 'Учителски отсъствия',
        icon: fadCalendarMinus,
        menuItems: [
          ...(this.hasPersonId
            ? [
                {
                  text: 'Мои отсъствия',
                  icon: fadUserMinus,
                  routeCommands: ['./teacher-absences', { mode: TeacherAbsencesMode.My }]
                },
                {
                  text: 'Мои замествания',
                  icon: fadUserPlus,
                  routeCommands: ['./teacher-absences', { mode: TeacherAbsencesMode.MyRepl }]
                }
              ]
            : []),

          ...(this.data.institutionInfo.hasTeacherAbsencesAllReadAccess
            ? [
                {
                  text: 'Всички отсъствия',
                  icon: fadCalendarMinus,
                  routeCommands: ['./teacher-absences', { mode: TeacherAbsencesMode.All }]
                }
              ]
            : [])
        ]
      },
      ...(this.isTeacherOrInstAdminUser
        ? [
            {
              text: 'Тематични разпр.',
              icon: fasListOl,
              routeCommands: ['./topic-plans']
            }
          ]
        : []),
      ...(archivedBookMenuItems.length > 0
        ? [
            {
              text: 'Дневници - архив',
              icon: fadBook,
              menuItems: archivedBookMenuItems
            }
          ]
        : []),
      {
        text: 'Администрация',
        icon: fadCogs,
        menuItems: [
          ...(this.data.institutionInfo.hasShiftReadAccess
            ? [
                {
                  text: 'Смени',
                  icon: fadBellSchool,
                  routeCommands: ['./shifts']
                }
              ]
            : []),
          ...(this.data.institutionInfo.hasOffDayReadAccess
            ? [
                {
                  text: 'Неучебни дни',
                  icon: fadCalendarDay,
                  routeCommands: ['./off-days']
                }
              ]
            : []),
          ...(this.data.institutionInfo.showSchoolYearSettings &&
          this.data.institutionInfo.hasSchoolYearSettingsReadAccess
            ? [
                {
                  text: 'Настройки на учебната година',
                  icon: fadCalendarStar,
                  routeCommands: ['./school-year-settings']
                }
              ]
            : []),
          {
            text: 'Дневници',
            icon: fadBook,
            routeCommands: this.bookAdminRouteCommands
          },
          ...(this.isTeacherOrInstAdminUser || this.isMultiInstitutionUser
            ? [
                {
                  text: 'Справки',
                  icon: fadAnalytics,
                  menuItems: [
                    {
                      text: 'Невписани теми',
                      icon: fadCalendarWeek,
                      routeCommands: ['./missing-topics-reports']
                    },
                    {
                      text: 'Лекторски часове',
                      icon: fadCalendarStar,
                      routeCommands: ['./lecture-schedules-reports']
                    },
                    ...(this.hasAdminReportAccess
                      ? this.data.institutionInfo.instType === InstType.School
                        ? [
                            {
                              text: 'Ученици/деца с риск от отпадане',
                              icon: fadCalendarMinus,
                              routeCommands: ['./students-at-risk-of-dropping-out-reports']
                            },
                            {
                              text: 'Ученици без оценки',
                              icon: fadCalendarMinus,
                              routeCommands: ['./gradeless-students-reports']
                            },
                            {
                              text: 'Ученици за поправителни изпити',
                              icon: fadCalendarMinus,
                              routeCommands: ['./session-students-reports']
                            },
                            {
                              text: 'Отсъствия по ученици',
                              icon: fadCalendarMinus,
                              routeCommands: ['./absences-by-students-reports']
                            },
                            {
                              text: 'Отсъствия по класове',
                              icon: fadCalendarMinus,
                              routeCommands: ['./absences-by-classes-reports']
                            },
                            {
                              text: 'Отсъстващи за деня',
                              icon: fadCalendarMinus,
                              routeCommands: ['./date-absences-reports']
                            },
                            {
                              text: 'Среден успех от текущи оценки по ученици',
                              icon: fadCalendarMinus,
                              routeCommands: ['./regular-grade-point-average-by-students-reports']
                            },
                            {
                              text: 'Среден успех от текущи оценки по класове',
                              icon: fadCalendarMinus,
                              routeCommands: ['./regular-grade-point-average-by-classes-reports']
                            },
                            {
                              text: 'Среден успех от срочни/годишни оценки по ученици',
                              icon: fadCalendarMinus,
                              routeCommands: ['./final-grade-point-average-by-students-reports']
                            },
                            {
                              text: 'Среден успех от срочни/годишни оценки по класове',
                              icon: fadCalendarMinus,
                              routeCommands: ['./final-grade-point-average-by-classes-reports']
                            },
                            {
                              text: 'Контролни/класни',
                              icon: fadCalendarMinus,
                              routeCommands: ['./exams-reports']
                            },
                            {
                              text: 'Отсъствия/теми за срок',
                              icon: fadCalendarMinus,
                              routeCommands: ['./schedule-and-absences-by-term-reports']
                            },
                            {
                              text: 'Отсъствия/теми за месец',
                              icon: fadCalendarMinus,
                              routeCommands: ['./schedule-and-absences-by-month-reports']
                            },
                            {
                              text: 'Отсъствия/теми за срок за всички паралелки',
                              icon: fadCalendarMinus,
                              routeCommands: ['./schedule-and-absences-by-term-all-classes-reports']
                            }
                          ]
                        : [
                            ...(this.data.institutionInfo.instType === InstType.DG ||
                            this.data.institutionInfo.instType === InstType.SOZ
                              ? [
                                  {
                                    text: 'Ученици/деца с риск от отпадане',
                                    icon: fadCalendarMinus,
                                    routeCommands: ['./students-at-risk-of-dropping-out-reports']
                                  },
                                  {
                                    text: 'Отсъстващи за деня',
                                    icon: fadCalendarMinus,
                                    routeCommands: ['./date-absences-reports']
                                  }
                                ]
                              : [
                                  {
                                    text: 'Ученици без оценки',
                                    icon: fadCalendarMinus,
                                    routeCommands: ['./gradeless-students-reports']
                                  },
                                  {
                                    text: 'Отсъствия по ученици',
                                    icon: fadCalendarMinus,
                                    routeCommands: ['./absences-by-students-reports']
                                  },
                                  {
                                    text: 'Отсъствия по класове',
                                    icon: fadCalendarMinus,
                                    routeCommands: ['./absences-by-classes-reports']
                                  },
                                  {
                                    text: 'Отсъстващи за деня',
                                    icon: fadCalendarMinus,
                                    routeCommands: ['./date-absences-reports']
                                  },
                                  {
                                    text: 'Отсъствия/теми за срок',
                                    icon: fadCalendarMinus,
                                    routeCommands: ['./schedule-and-absences-by-term-reports']
                                  },
                                  {
                                    text: 'Отсъствия/теми за месец',
                                    icon: fadCalendarMinus,
                                    routeCommands: ['./schedule-and-absences-by-month-reports']
                                  },
                                  {
                                    text: 'Отсъствия/теми за срок за всички паралелки',
                                    icon: fadCalendarMinus,
                                    routeCommands: ['./schedule-and-absences-by-term-all-classes-reports']
                                  }
                                ])
                          ]
                      : [])
                  ]
                }
              ]
            : []),
          ...(this.data.institutionInfo.hasTeacherSchedulesReadAccess
            ? [
                {
                  text: 'Учителски разписания',
                  icon: fadCalendarAlt,
                  routeCommands: ['./teacher-schedules']
                }
              ]
            : []),
          ...(this.data.institutionInfo.hasVerificationReadAccess &&
          !this.data.institutionInfo.hasCBExtProvider &&
          this.data.schoolYear >= 2023
            ? [
                {
                  text: 'Проверка на дневници',
                  icon: fadTasksAlt,
                  routeCommands: ['./book-verification']
                }
              ]
            : []),
          ...(this.data.institutionInfo.hasPublicationReadAccess
            ? [
                {
                  text: 'Публикации',
                  icon: fadInfoSquare,
                  routeCommands: ['./publications']
                }
              ]
            : []),
          ...(this.data.institutionInfo.hasFinalizationAccess && !this.data.institutionInfo.hasCBExtProvider
            ? [
                {
                  text: 'Приключване',
                  icon: fadLock,
                  routeCommands: ['./finalization']
                }
              ]
            : []),
          ...(this.data.institutionInfo.hasFinalizationAccess && this.data.institutionInfo.hasCBExtProvider
            ? [
                {
                  text: 'Приключване',
                  icon: fadLock,
                  routeCommands: ['./external-finalization']
                }
              ]
            : [])
        ]
      }
    ];
  }

  createGroupedBookMenuItems(classBooks: ClassBook[], icon: IconDefinition): MenuItem[] {
    const basicClassSortMap = new Map<number, number | null | undefined>();
    for (const cb of classBooks) {
      if (!basicClassSortMap.get(cb.basicClassId ?? NO_BASIC_CLASS)) {
        basicClassSortMap.set(cb.basicClassId ?? NO_BASIC_CLASS, cb.basicClassSortOrd);
      }
    }

    const isLetterBook = (b: ClassBook) => b.basicClassId != null && b.bookName && CLASS_LETTER_REGEX.test(b.bookName);

    // letter class books come befor the non letter ones
    // thats why we need a stable sort by basicClassId
    return stableSort(
      [
        // letter books grouped by basicClassId, with letter converted to upper case
        ...groupBy(
          classBooks.filter((b) => isLetterBook(b)).map((b) => ({ ...b, bookName: b.bookName.toUpperCase() })),
          (b) => b.basicClassId ?? NO_BASIC_CLASS
        ),
        // non letter books, one in each row
        ...classBooks
          .filter((b) => !isLetterBook(b))
          .map((b) => <[number, ClassBook[]]>[b.basicClassId ?? NO_BASIC_CLASS, [b]])
      ],
      ([i1], [i2]) => (basicClassSortMap.get(i1) ?? NO_BASIC_CLASS) - (basicClassSortMap.get(i2) ?? NO_BASIC_CLASS)
    ).map(([, books]) => {
      if (!books[0].basicClassName) {
        return {
          text: books[0].bookName,
          badge: books[0].isValid ? null : 'Архивиран',
          icon: icon,
          ...getBookRoute(books[0])
        };
      }

      return {
        text: books[0].basicClassName,
        badge: books[0].isValid ? null : 'Архивиран',
        icon: icon,
        showChildrenInline: true,
        // if there is only one child, clicking on the basic class also works
        ...(books.length === 1 ? getBookRoute(books[0]) : {}),
        menuItems:
          books.length === 1 && !books[0].bookName
            ? undefined // an unnamed class book, so hide children
            : books.map((b) => ({
                text: b.bookName,
                ...(books.length === 1 ? {} : getBookRoute(b))
              }))
      };
    });
  }

  updateUnreadConversations(): Observable<Conversations_GetUnreadConversations> {
    return this.conversationsService.getUnreadConversations({ limit: 5 });
  }

  hideMobileAppBanner() {
    this.mobileAppBannerHidden = true;
    this.localStorageService.setMobileAppBannerHidden(true);
  }

  getBannerToShow(): string {
    if (!this.data.institutionInfo.hasCBExtProvider && this.data.institutionInfo.showDefaultSettingsBanner) {
      return 'defaultSettings';
    }
    if (!this.data.institutionInfo.hasCBExtProvider && this.data.institutionInfo.showSchoolYearSettingsBanner) {
      return 'schoolYearSettings';
    }
    if (!this.data.institutionInfo.hasCBExtProvider && this.data.institutionInfo.showClassBooksBanner) {
      return 'classBooks';
    }
    return ''; // Няма банер за показване
  }
}

function getBookRoute(book: ClassBook) {
  const firstTabRoute = mapBookTypeToTabs(false, BookTabRoutesConfig, {
    bookType: book.bookType,
    basicClassId: book.basicClassId
  })[0];

  return {
    routeCommands: ['./book', book.classBookId, ...firstTabRoute],
    isActiveRouteCommands: ['./book', book.classBookId]
  };
}

function getBookAdminRouteCommands(institutionInfo: InstitutionInfoType): unknown[] {
  switch (institutionInfo.instType) {
    case InstType.School:
    case InstType.CSOP:
    case InstType.DG:
      return ['./book-admin', 'class'];

    case InstType.CPLR:
    case InstType.SOZ:
      return ['./book-admin', 'other'];

    default:
      throw new Error('Invalid InstType');
  }
}

function isProdStage(): boolean {
  if (environment.production) {
    return environment.envType.toLowerCase() === 'production';
  } else {
    return false;
  }
}
