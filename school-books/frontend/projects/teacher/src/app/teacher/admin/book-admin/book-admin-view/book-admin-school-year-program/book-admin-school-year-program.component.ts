import { Component, Inject, Input, OnInit } from '@angular/core';
import { UntypedFormBuilder } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faArrowLeft as fasArrowLeft } from '@fortawesome/pro-solid-svg-icons/faArrowLeft';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import {
  ClassBooksAdminService,
  ClassBooksAdmin_GetSchoolYearProgram
} from 'projects/sb-api-client/src/api/classBooksAdmin.service';
import { SaveToken } from 'projects/shared/components/editor-panel/editor-panel.component';
import {
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from, Subject } from 'rxjs';
import { ClassBookAdminInfoType, CLASS_BOOK_ADMIN_INFO } from '../book-admin-view.component';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class BookAdminSchoolYearProgramSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(CLASS_BOOK_ADMIN_INFO) classBookAdminInfo: Promise<ClassBookAdminInfoType>,
    classBooksAdminService: ClassBooksAdminService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    this.resolve(BookAdminSchoolYearProgramComponent, {
      schoolYear,
      instId,
      classBookId,
      schoolYearProgram: classBooksAdminService.getSchoolYearProgram({
        schoolYear,
        instId,
        classBookId
      }),
      classBookAdminInfo: from(classBookAdminInfo)
    });
  }
}

@Component({
  selector: 'sb-book-admin-school-year-program',
  templateUrl: './book-admin-school-year-program.component.html'
})
export class BookAdminSchoolYearProgramComponent implements OnInit {
  @Input() data!: {
    schoolYearProgram: ClassBooksAdmin_GetSchoolYearProgram;
    schoolYear: number;
    instId: number;
    classBookId: number;
    classBookAdminInfo: ClassBookAdminInfoType;
  };
  private readonly destroyed$ = new Subject<void>();

  fadChevronCircleRight = fadChevronCircleRight;
  fasCheck = fasCheck;
  fasArrowLeft = fasArrowLeft;
  canEdit = false;

  form = this.fb.group({
    schoolYearProgram: { value: null }
  });

  constructor(
    private fb: UntypedFormBuilder,
    private actionService: ActionService,
    private classBooksAdminService: ClassBooksAdminService
  ) {}

  ngOnInit() {
    const schoolYearProgram = this.data.schoolYearProgram;

    if (schoolYearProgram != null) {
      this.form.setValue({
        schoolYearProgram: schoolYearProgram.schoolYearProgram
      });

      this.canEdit =
        this.data.classBookAdminInfo.bookAllowsModifications &&
        this.data.classBookAdminInfo.hasEditSchoolYearProgramAccess;
    }
  }

  onSave(save: SaveToken) {
    const value = this.form.value;
    const classBookSchoolYearProgram = {
      schoolYearProgram: <string>value.schoolYearProgram
    };

    this.actionService
      .execute({
        httpAction: () => {
          const updateArgs = {
            schoolYear: this.data.schoolYear,
            instId: this.data.instId,
            classBookId: this.data.classBookId,
            schoolYearProgram: this.data.schoolYearProgram.schoolYearProgram
          };
          return this.classBooksAdminService
            .updateClassBookSchoolYearProgram({
              updateClassBookSchoolYearProgramCommand: classBookSchoolYearProgram,
              ...updateArgs
            })
            .toPromise()
            .then(() => this.classBooksAdminService.get(updateArgs).toPromise())
            .then((schoolYearProgram) => {
              this.data.schoolYearProgram = schoolYearProgram;
            });
        }
      })
      .then((success) => save.done(success));
  }
}
