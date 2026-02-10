import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormArray, UntypedFormBuilder, UntypedFormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faBook as fadBook } from '@fortawesome/pro-duotone-svg-icons/faBook';
import { faAngleRight as fasAngleRight } from '@fortawesome/pro-solid-svg-icons/faAngleRight';
import {
  ClassBooksCreationService,
  ClassBooksCreation_GetClassGroupsForClassKind
} from 'projects/sb-api-client/src/api/classBooksCreation.service';
import { ClassKind } from 'projects/sb-api-client/src/model/classKind';
import { SaveToken } from 'projects/shared/components/editor-panel/editor-panel.component';
import {
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { EventService, EventType } from 'projects/shared/services/event.service';
import { ArrayElementType } from 'projects/shared/utils/type';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { Subject } from 'rxjs';
import { takeUntil, tap } from 'rxjs/operators';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class BookAdminNewSkeletonComponent extends SkeletonComponentBase {
  constructor(classBooksCreationService: ClassBooksCreationService, route: ActivatedRoute) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classKind = (route.snapshot.paramMap.get('classKind') ?? throwParamError('classKind')) as ClassKind;

    this.resolve(BookAdminNewComponent, {
      schoolYear,
      instId,
      classKind,
      classGroups: classBooksCreationService.getClassGroupsForClassKind({
        schoolYear,
        instId,
        classKind
      })
    });
  }
}

type ClassGroupVO = ArrayElementType<ClassBooksCreation_GetClassGroupsForClassKind> & {
  parent?: ClassGroupVO;
  children?: ClassGroupVO[];
};

@Component({
  selector: 'sb-book-admin-new',
  styleUrls: ['./book-admin-new.component.scss'],
  templateUrl: './book-admin-new.component.html'
})
export class BookAdminNewComponent implements OnInit, OnDestroy {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classKind: ClassKind;
    classGroups: ClassBooksCreation_GetClassGroupsForClassKind;
  };

  private readonly destroyed$ = new Subject<void>();

  readonly fadBook = fadBook;
  readonly fasAngleRight = fasAngleRight;

  allClassBooksSelected!: boolean;
  form!: UntypedFormGroup;
  sortedClassGroups!: ClassGroupVO[];

  constructor(
    private fb: UntypedFormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService,
    private eventService: EventService,
    private classBooksCreationService: ClassBooksCreationService
  ) {}

  ngOnInit() {
    this.sortedClassGroups = [];

    const parents = this.data.classGroups
      .filter((cg) => cg.parentClassId == null)
      .map((p) => ({ ...p, children: [] as ClassGroupVO[] }));

    for (const parent of parents) {
      const children = this.data.classGroups
        .filter((cg) => cg.parentClassId === parent.classId)
        .map((ccg) => ({ ...ccg, parent: parent }));

      parent.children.push(...children);

      this.sortedClassGroups.push(parent);
      this.sortedClassGroups.push(...children);
    }

    this.form = this.fb.group({
      classBooks: this.fb.array(
        this.sortedClassGroups.map((cg, i) => {
          const isDisabled = !this.classBookAllowedAt(i);

          const classBook = this.fb.group({
            classId: [cg.classId],
            checked: [{ value: !!cg.classBookId, disabled: isDisabled }],
            classBookName: [cg.classBookName ?? cg.suggestedClassBookName]
          });

          if (isDisabled) {
            classBook.disable();
          }

          return classBook;
        })
      )
    });

    this.syncAllClassBooksSelected();

    this.form.valueChanges
      .pipe(
        tap(() => {
          this.syncAllClassBooksSelected();
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  get classBooksFormArray(): UntypedFormArray {
    return this.form.get('classBooks') as UntypedFormArray;
  }

  get classBooksFormArrayValue() {
    return this.classBooksFormArray.enabled
      ? (this.classBooksFormArray.value as {
          classId: number;
          checked: boolean;
          classBookName: string;
        }[])
      : [];
  }

  classBookAllowedAt(i: number) {
    const classGroup = this.sortedClassGroups[i];
    return (
      !classGroup.classBookId &&
      !classGroup.parent?.classBookId &&
      !classGroup.children?.find((childClassGroup) => !!childClassGroup.classBookId) &&
      classGroup.bookType
    );
  }

  classBookCheckedAt(i: number) {
    return !!this.classBooksFormArray.at(i).get('checked')?.value;
  }

  syncAllClassBooksSelected() {
    this.allClassBooksSelected =
      this.classBooksFormArrayValue.filter((cb) => cb.checked).length === this.classBooksFormArrayValue.length;
  }

  classBookSelectionChanged(i: number) {
    const parentIndex = this.parentIndexOf(i);
    if (parentIndex != null) {
      if (this.classBookCheckedAt(i)) {
        this.classBooksFormArray.at(parentIndex).disable();
      } else {
        const hasCheckedChild = [...this.childIndexesOf(parentIndex)].find((j) => this.classBookCheckedAt(j)) != null;

        if (this.classBookAllowedAt(parentIndex) && !hasCheckedChild) {
          this.classBooksFormArray.at(parentIndex).enable();
        }
      }
    } else {
      for (const j of this.childIndexesOf(i)) {
        if (this.classBookCheckedAt(i)) {
          this.classBooksFormArray.at(j).disable();
        } else if (this.classBookAllowedAt(j)) {
          this.classBooksFormArray.at(j).enable();
        }
      }
    }
  }

  toggleAllClassBooksSelected() {
    this.allClassBooksSelected = !this.allClassBooksSelected;

    for (const [i, classBookFormControl] of this.classBooksFormArray.controls.entries()) {
      if (!this.allClassBooksSelected) {
        if (!this.classBookAllowedAt(i)) {
          continue;
        }

        classBookFormControl.enable({ emitEvent: false });
        classBookFormControl.get('checked')?.setValue(false, { emitEvent: false });
      } else {
        if (classBookFormControl.disabled) {
          continue;
        }

        classBookFormControl.get('checked')?.setValue(true, { emitEvent: false });

        for (const j of this.childIndexesOf(i)) {
          this.classBooksFormArray.at(j).disable({ emitEvent: false });
        }
      }
    }
  }

  *childIndexesOf(parentIndex: number): Generator<number, void, undefined> {
    // in sortedClassGroups the children are directly after the parent
    // so iterate forwards until we find a class group that is not a child
    const classGroup = this.sortedClassGroups[parentIndex];
    for (let childIndex = parentIndex + 1; childIndex < this.sortedClassGroups.length; childIndex++) {
      if (this.sortedClassGroups[childIndex].parent === classGroup) {
        yield childIndex;
      } else {
        break;
      }
    }
  }

  parentIndexOf(childIndex: number): number | null {
    // in sortedClassGroups the parent is before the children
    // so iterate backwards until we find the parent
    const classGroup = this.sortedClassGroups[childIndex];
    for (let parentIndex = childIndex - 1; parentIndex >= 0; parentIndex--) {
      if (this.sortedClassGroups[parentIndex] === classGroup.parent) {
        return parentIndex;
      }
    }

    return null;
  }

  onSave(save: SaveToken) {
    this.actionService
      .execute({
        httpAction: () =>
          this.classBooksCreationService
            .createClassBook({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              createClassBookCommand: {
                classBooks: this.classBooksFormArrayValue
                  .filter((cb) => cb.checked)
                  .map((cb) => ({ classId: cb.classId, classBookName: cb.classBookName }))
              }
            })
            .toPromise()
            .then(() => {
              this.eventService.dispatch({ type: EventType.ClassBooksUpdated });
              this.router.navigate(['../'], { relativeTo: this.route });
            })
      })
      .then((success) => save.done(success));
  }
}
