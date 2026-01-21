import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { FormGroup, UntypedFormBuilder, UntypedFormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faBook as fadBook } from '@fortawesome/pro-duotone-svg-icons/faBook';
import { faAngleLeft as fasAngleLeft } from '@fortawesome/pro-solid-svg-icons/faAngleLeft';
import { faAngleRight as fasAngleRight } from '@fortawesome/pro-solid-svg-icons/faAngleRight';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { ClassBooksCreation_GetClassGroupsForClassKind } from 'projects/sb-api-client/src/api/classBooksCreation.service';
import { ClassBooksReorganizeService } from 'projects/sb-api-client/src/api/classBooksReorganize.service';
import { ClassKind } from 'projects/sb-api-client/src/model/classKind';
import {
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { EventService, EventType } from 'projects/shared/services/event.service';
import { ArrayElementType } from 'projects/shared/utils/type';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { Subject } from 'rxjs';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class BookAdminReorganizeCombineSkeletonComponent extends SkeletonComponentBase {
  constructor(ClassBooksReorganizeService: ClassBooksReorganizeService, route: ActivatedRoute) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classKind = (route.snapshot.paramMap.get('classKind') ?? throwParamError('classKind')) as ClassKind;
    const reorganizeType = 'Combine';

    this.resolve(BookAdminReorganizeCombineComponent, {
      schoolYear,
      instId,
      classKind,
      classGroups: ClassBooksReorganizeService.getClassGroupsForClassKind({
        schoolYear,
        instId,
        classKind,
        reorganizeType
      })
    });
  }
}

type ClassGroupVO = ArrayElementType<ClassBooksCreation_GetClassGroupsForClassKind> & {
  parent?: ClassGroupVO;
  children?: ClassGroupVO[];
};

@Component({
  selector: 'sb-book-admin-reorganize-combine',
  styleUrls: ['../book-admin-reorganize.component.scss'],
  templateUrl: './book-admin-reorganize-combine.component.html'
})
export class BookAdminReorganizeCombineComponent implements OnInit, OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classKind: ClassKind;
    classGroups: ClassBooksCreation_GetClassGroupsForClassKind;
  };

  private readonly destroyed$ = new Subject<void>();

  readonly fadBook = fadBook;
  readonly fasAngleRight = fasAngleRight;
  readonly fasAngleLeft = fasAngleLeft;
  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fasCheck = fasCheck;

  form!: UntypedFormGroup;
  sortedClassGroups!: ClassGroupVO[];
  selectedClassIndex: number | null = null;
  currentStep = 1;
  lastStep = 2;
  saving = false;
  hasAny = false;
  errors: string[] = [];

  constructor(
    private fb: UntypedFormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService,
    private eventService: EventService,
    private classBooksReorganizeService: ClassBooksReorganizeService
  ) {}

  ngOnInit() {
    this.sortedClassGroups = [];

    const parents = this.data.classGroups
      .filter((cg) => cg.parentClassId == null)
      .map((p) => ({ ...p, classBookName: p.suggestedClassBookName, children: [] as ClassGroupVO[] }));

    for (const parent of parents) {
      const children = this.data.classGroups
        .filter((cg) => cg.parentClassId === parent.classId)
        .map((ccg) => ({ ...ccg, parent: parent }));

      parent.children.push(...children);

      this.sortedClassGroups.push(parent);
      this.sortedClassGroups.push(...children);
    }

    this.form = this.fb.group({
      parentClassBook: this.fb.group({
        classId: [null],
        className: [{ value: null, disabled: true }],
        basicClassName: [{ value: null, disabled: true }],
        classTypeName: [{ value: null, disabled: true }],
        bookTypeDescription: [{ value: null, disabled: true }],
        classBookName: [null]
      }),
      childClassIdToCopyData: [null]
    });

    this.hasAny =
      this.sortedClassGroups.length > 0 &&
      this.sortedClassGroups.some((scg) => !scg.classIsLvl2 && scg.bookType != null);
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  get childClassIdForDataTransfer() {
    return this.form.get('childClassIdToCopyData')?.value;
  }

  get parentClassBookFormGroupValue() {
    return this.form.get('parentClassBook')?.value;
  }

  get selectedClassBookChildren() {
    return this.sortedClassGroups[this.selectedClassIndex!].children?.filter((scg) => scg.classBookId != null);
  }

  onRowSelect(classBookIndex: number): void {
    const parentClassBookFormGroup = this.form.get('parentClassBook') as FormGroup;
    this.selectedClassIndex = classBookIndex;
    const classBook = this.sortedClassGroups[classBookIndex];
    parentClassBookFormGroup.patchValue(classBook);

    if (!parentClassBookFormGroup.get('className')!.value) {
      parentClassBookFormGroup.get('className')?.setValue('(на групи)');
    }
    if (!parentClassBookFormGroup.get('basicClassName')!.value) {
      parentClassBookFormGroup.get('basicClassName')?.setValue('(на групи)');
    }
  }

  previous() {
    this.clearData();

    if (this.currentStep === 1) {
      this.router.navigate(['../'], { relativeTo: this.route });
    }

    if (this.currentStep > 1) {
      this.currentStep--;
    }
  }

  next() {
    this.errors = [];

    if (!this.hasErrors()) {
      if (this.currentStep === this.lastStep) {
        this.save();
      } else {
        this.currentStep++;
      }
    }
  }

  clearData() {
    this.errors = [];
    this.selectedClassIndex = null;
    this.form.get('childClassIdToCopyData')?.reset();
    this.form.get('parentClassBook')?.reset();
  }

  hasErrors(): boolean {
    let hasError = false;

    if (this.currentStep === 1 && this.selectedClassIndex == null) {
      hasError = true;
      this.errors.push('Моля изберете клас за обединяване');
    }

    return hasError;
  }

  save() {
    this.errors = [];
    this.saving = true;

    this.actionService
      .execute({
        httpAction: () =>
          this.classBooksReorganizeService
            .combineClassBooks({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              combineClassBooksCommand: {
                parentClassId: this.parentClassBookFormGroupValue.classId,
                parentClassBookName: this.parentClassBookFormGroupValue.classBookName,
                childClassIdForDataTransfer: this.childClassIdForDataTransfer
              }
            })
            .toPromise()
      })
      .then((done) => {
        if (done) {
          this.eventService.dispatch({ type: EventType.ClassBooksUpdated });
          this.router.navigate(['../'], { relativeTo: this.route });
        }
      })
      .finally(() => (this.saving = false));
  }
}
