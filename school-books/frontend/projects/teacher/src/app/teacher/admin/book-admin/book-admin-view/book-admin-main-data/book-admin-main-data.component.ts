import { Component, Inject, Input, OnInit } from '@angular/core';
import { UntypedFormBuilder } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faLock as fadLock } from '@fortawesome/pro-duotone-svg-icons/faLock';
import { faLockOpen as fadLockOpen } from '@fortawesome/pro-duotone-svg-icons/faLockOpen';
import { faArrowAltRight as fasArrowAltRight } from '@fortawesome/pro-solid-svg-icons/faArrowAltRight';
import { faArrowLeft as fasArrowLeft } from '@fortawesome/pro-solid-svg-icons/faArrowLeft';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import { ClassBooksAdminService, ClassBooksAdmin_Get } from 'projects/sb-api-client/src/api/classBooksAdmin.service';
import { SaveToken } from 'projects/shared/components/editor-panel/editor-panel.component';
import {
  IShouldPreventLeave,
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { EventService, EventType } from 'projects/shared/services/event.service';
import { BookTabRoutesConfig, mapBookTypeToTabs } from 'projects/shared/utils/book-tabs';
import { reloadRoute } from 'projects/shared/utils/router';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from, Subject } from 'rxjs';
import { ClassBookAdminInfoType, CLASS_BOOK_ADMIN_INFO } from '../book-admin-view.component';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class BookAdminMainDataSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(CLASS_BOOK_ADMIN_INFO) classBookAdminInfo: Promise<ClassBookAdminInfoType>,
    classBooksAdminService: ClassBooksAdminService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    this.resolve(BookAdminMainDataComponent, {
      schoolYear,
      instId,
      classBookId,
      classBookMainData: classBooksAdminService.get({
        schoolYear,
        instId,
        classBookId
      }),
      classBookAdminInfo: from(classBookAdminInfo)
    });
  }
}

@Component({
  selector: 'sb-book-admin-main-data',
  templateUrl: './book-admin-main-data.component.html'
})
export class BookAdminMainDataComponent implements OnInit, IShouldPreventLeave {
  @Input() data!: {
    classBookMainData: ClassBooksAdmin_Get;
    schoolYear: number;
    instId: number;
    classBookId: number;
    classBookAdminInfo: ClassBookAdminInfoType;
  };
  private readonly destroyed$ = new Subject<void>();

  fadChevronCircleRight = fadChevronCircleRight;
  fasCheck = fasCheck;
  fasArrowLeft = fasArrowLeft;

  form = this.fb.group({
    className: { value: null },
    basicClassName: { value: null },
    bookName: { value: null }
  });

  readonly fasTrashAlt = fasTrashAlt;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fasArrowAltRight = fasArrowAltRight;
  readonly fadLock = fadLock;
  readonly fadLockOpen = fadLockOpen;
  canEdit = false;
  canRemove = false;
  canFinalize = false;
  canUnfinalize = false;
  removing = false;
  finalizing = false;
  unfinalizing = false;
  bookRouteCommands: unknown[] = [];

  constructor(
    private fb: UntypedFormBuilder,
    private actionService: ActionService,
    private classBooksAdminService: ClassBooksAdminService,
    private eventService: EventService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit() {
    const classBookMainData = this.data.classBookMainData;
    this.form.setValue({
      className: classBookMainData.className,
      basicClassName: classBookMainData.basicClassName,
      bookName: classBookMainData.bookName
    });

    const firstTabRoute = mapBookTypeToTabs(false, BookTabRoutesConfig, {
      bookType: classBookMainData.bookType,
      basicClassId: classBookMainData.basicClassId
    })[0];
    this.bookRouteCommands = ['../../../../book', this.data.classBookId, ...firstTabRoute];

    this.canEdit =
      this.data.classBookAdminInfo.bookAllowsModifications &&
      this.data.classBookAdminInfo.hasEditSchoolYearProgramAccess;
    this.canRemove =
      this.data.classBookAdminInfo.bookAllowsModifications && this.data.classBookAdminInfo.hasRemoveAccess;
    this.canFinalize =
      this.data.classBookAdminInfo.bookAllowsModifications && this.data.classBookAdminInfo.hasFinalizeAccess;
    this.canUnfinalize =
      !this.data.classBookAdminInfo.schoolYearIsFinalized && this.data.classBookAdminInfo.hasUnfinalizeAccess;
  }

  shouldPreventLeave() {
    return this.form.dirty;
  }

  onSave(save: SaveToken) {
    const value = this.form.value;
    const classBookMainData = {
      bookName: <string>value.bookName
    };

    this.actionService
      .execute({
        httpAction: () => {
          const updateArgs = {
            schoolYear: this.data.schoolYear,
            instId: this.data.instId,
            classBookId: this.data.classBookId,
            bookName: this.data.classBookMainData.bookName
          };
          return this.classBooksAdminService
            .update({
              updateClassBookMainDataCommand: classBookMainData,
              ...updateArgs
            })
            .toPromise()
            .then(() => {
              this.eventService.dispatch({ type: EventType.ClassBooksUpdated });
              return this.classBooksAdminService.get(updateArgs).toPromise();
            })
            .then((classBookMainData) => {
              this.data.classBookMainData = classBookMainData;
              this.form.markAsPristine();
            });
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    this.removing = true;

    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      classBookId: this.data.classBookId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да изтриете дневника?',
        errorsMessage: 'Не може да изтриете дневника, защото:',
        httpAction: () => this.classBooksAdminService.remove(removeParams).toPromise()
      })
      .then((done) => {
        if (done) {
          this.router.navigate(['../../'], { relativeTo: this.route });

          // move the dispatch to the next tick to avoid updating the currently loaded
          // BookAdminViewComponent.classBookAdminInfo which will result in an error
          // as the ClassBook is no longer available
          setTimeout(() => {
            this.eventService.dispatch({ type: EventType.ClassBooksUpdated });
          });
        }
      })
      .finally(() => {
        this.removing = false;
      });
  }

  onFinalize() {
    this.finalizing = true;

    const finalizeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      classBookId: this.data.classBookId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да приключите дневника?',
        errorsMessage: 'Не може да приключите дневника, защото:',
        httpAction: () => this.classBooksAdminService.finalize(finalizeParams).toPromise()
      })
      .then(() => {
        this.eventService.dispatch({ type: EventType.ClassBooksUpdated });
        reloadRoute(this.router, this.route, ['../']);
      });
  }

  onUnfinalize() {
    this.unfinalizing = true;

    const unfinalizeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      classBookId: this.data.classBookId
    };

    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте, че искате да премахнете приключването дневника?',
        errorsMessage: 'Не може да премахнете приключването дневника, защото:',
        httpAction: () => this.classBooksAdminService.unfinalize(unfinalizeParams).toPromise()
      })
      .then(() => {
        this.eventService.dispatch({ type: EventType.ClassBooksUpdated });
        reloadRoute(this.router, this.route, ['../']);
      });
  }
}
