import { Component, OnDestroy, OnInit } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { faNotesMedical as fadNotesMedical } from '@fortawesome/pro-duotone-svg-icons/faNotesMedical';
import { faChild as fasChild } from '@fortawesome/pro-solid-svg-icons/faChild';
import { faInfoSquare as fadInfoSquare } from '@fortawesome/pro-solid-svg-icons/faInfoSquare';
import { faTimes as fasTimes } from '@fortawesome/pro-solid-svg-icons/faTimes';
import { faUniversity as fasUniversity } from '@fortawesome/pro-solid-svg-icons/faUniversity';
import {
  ConversationsService,
  Conversations_GetUnreadConversations
} from 'projects/sb-api-client/src/api/conversations.service';
import {
  StudentClassBooksService,
  StudentClassBooks_GetAllClassBooks
} from 'projects/sb-api-client/src/api/studentClassBooks.service';
import {
  StudentMedicalNoticesService,
  StudentMedicalNotices_GetAllStudents
} from 'projects/sb-api-client/src/api/studentMedicalNotices.service';
import { AppChromeSchoolYear, UnreadConversations } from 'projects/shared/components/app-chrome/app-chrome.component';
import { MenuItem } from 'projects/shared/components/app-menu/menu-item';
import { GlobalErrorHandler } from 'projects/shared/other/global-error-handler';
import { AuthService, SysRole } from 'projects/shared/services/auth.service';
import { ConfigService } from 'projects/shared/services/config.service';
import { EventService, EventType } from 'projects/shared/services/event.service';
import { LocalStorageService } from 'projects/shared/services/local-storage.service';
import { PwaService } from 'projects/shared/services/pwa-service/pwa.service';
import { groupBy, range } from 'projects/shared/utils/array';
import { BookTabRoutesConfig, mapBookTypeToTabs } from 'projects/shared/utils/book-tabs';
import { ArrayElementType } from 'projects/shared/utils/type';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { concat, interval, merge, Observable, of, Subject, Subscription } from 'rxjs';
import { filter, switchMap, take, takeUntil, tap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

type ClassBook = ArrayElementType<StudentClassBooks_GetAllClassBooks>;
type Student = ArrayElementType<StudentMedicalNotices_GetAllStudents>;

@Component({
  selector: 'sb-student',
  templateUrl: './student.component.html'
})
export class StudentComponent implements OnInit, OnDestroy {
  schoolYear!: number;
  schoolYears: AppChromeSchoolYear[] = [];
  menuItems: MenuItem[] = [];
  subscription!: PushSubscription;
  unreadConversations: UnreadConversations[] = [];
  mobileAppBannerHidden = false;
  isBaseUrl = false;
  shouldShowEnvError = false;
  readonly fasTimes = fasTimes;
  readonly docsUrl = environment.docsUrl + 'mobile';

  protected readonly destroyed$ = new Subject<void>();
  private pollingSubscription?: Subscription;

  constructor(
    private studentClassBooksService: StudentClassBooksService,
    private studentMedicalNoticesService: StudentMedicalNoticesService,
    private conversationsService: ConversationsService,
    private configService: ConfigService,
    private router: Router,
    private route: ActivatedRoute,
    private authService: AuthService,
    private pwaService: PwaService,
    eventService: EventService,
    public localStorageService: LocalStorageService
  ) {
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

  ngOnInit() {
    this.schoolYear = tryParseInt(this.route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');

    this.isBaseUrl = this.router.url === `/${this.schoolYear}`;

    this.router.events
      .pipe(
        filter((event) => event instanceof NavigationEnd),
        takeUntil(this.destroyed$)
      )
      .subscribe((event) => {
        this.isBaseUrl = (event as NavigationEnd).url === `/${this.schoolYear}`;
      });

    const NeispuoStartSchoolYear = 2021;
    this.schoolYears = range(NeispuoStartSchoolYear, this.configService.currentUserConfig.systemSchoolYear)
      .reverse()
      .map((sy) => ({
        schoolYear: sy,
        schoolYearRouteCommands: ['/', sy]
      }));

    this.shouldShowEnvError = !isProdStage() && window.matchMedia('(display-mode: standalone)').matches;

    Promise.all([
      this.studentMedicalNoticesService.getAllStudents({ schoolYear: this.schoolYear }).toPromise(),
      this.studentClassBooksService.getAllClassBooks({ schoolYear: this.schoolYear }).toPromise()
    ])
      .then(([students, classBooks]) => {
        this.rebuildMenu(classBooks, students);
      })
      .catch((err) => GlobalErrorHandler.instance.handleError(err));

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

  rebuildMenu(classBooks: ClassBook[], students: Student[]) {
    if (classBooks.length) {
      const classBooksByInstId = groupBy(classBooks, (cb) => cb.instId);

      this.menuItems = classBooksByInstId.map(([instId, instClassBooks]) => {
        const classBookItems = instClassBooks.map((cb) => ({
          text: `${cb.firstName} ${cb.lastName} - ${getBookDisplayName(cb)}`,
          icon: fasChild,
          ...getBookRoute(cb)
        }));

        return {
          text: instClassBooks[0].instName,
          icon: fasUniversity,
          menuItems: [
            {
              text: 'Инф. табло',
              icon: fadInfoSquare,
              routeCommands: ['./info-board', instId, { status: 'published' }],
              isActiveRouteCommands: ['./info-board', instId]
            },
            ...classBookItems
          ]
        };
      });
    } else {
      this.menuItems = [{ text: 'Няма бележници' }];
    }

    // eslint-disable-next-line no-constant-condition
    if (this.authService.tokenPayload.selected_role.SysRoleID === SysRole.Parent) {
      if (students.length > 1) {
        const studentItems = students.map((s) => ({
          text: `${s.firstName} ${s.lastName}`,
          icon: fasChild,
          routeCommands: ['./medical-notices', s.personId]
        }));

        this.menuItems.unshift({
          text: 'Медицински бележки',
          icon: fadNotesMedical,
          menuItems: studentItems
        });
      } else if (students.length === 1) {
        this.menuItems.unshift({
          text: 'Медицински бележки',
          icon: fadNotesMedical,
          routeCommands: ['./medical-notices', students[0].personId]
        });
      }
    }
  }

  updateUnreadConversations(): Observable<Conversations_GetUnreadConversations> {
    return this.conversationsService.getUnreadConversations({ limit: 5 });
  }

  hideMobileAppBanner() {
    this.mobileAppBannerHidden = true;
    this.localStorageService.setMobileAppBannerHidden(true);
  }
}

function getBookRoute(book: ClassBook) {
  const firstTabRoute = mapBookTypeToTabs(true, BookTabRoutesConfig, {
    bookType: book.bookType,
    basicClassId: book.basicClassId
  })[0];

  return {
    routeCommands: ['./book', book.classBookId, book.personId, ...firstTabRoute],
    isActiveRouteCommands: ['./book', book.classBookId, book.personId]
  };
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

function isProdStage() {
  if (environment.production) {
    return environment.envType.toLowerCase() === 'production';
  } else {
    return false;
  }
}
