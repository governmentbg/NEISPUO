import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormControl } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { ClassBookNomsService } from 'projects/sb-api-client/src/api/classBookNoms.service';
import { InstTeacherNomsService } from 'projects/sb-api-client/src/api/instTeacherNoms.service';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { combineLatest, Subject } from 'rxjs';
import { startWith, takeUntil, tap } from 'rxjs/operators';

@Component({
  selector: 'sb-book-verification-classbook-teacher-selector',
  templateUrl: './book-verification-classbook-teacher-selector.component.html'
})
export class BookVerificationClassbookTeacherSelectorComponent implements OnInit, OnDestroy {
  readonly destroyed$ = new Subject<void>();

  classBookId: number | null;
  teacherPersonId: number | null;

  classBookNomsService: INomService<number, { instId: number; schoolYear: number }>;
  instTeacherNomsService: INomService<number, { instId: number; schoolYear: number }>;

  classBookIdFormControl: FormControl<number | null>;
  teacherPersonIdFormControl: FormControl<number | null>;

  constructor(
    private route: ActivatedRoute,
    private router: Router,
    classBookNomsService: ClassBookNomsService,
    instTeacherNomsService: InstTeacherNomsService
  ) {
    this.classBookId = tryParseInt(this.route.snapshot.queryParamMap.get('c'));
    this.teacherPersonId = tryParseInt(this.route.snapshot.queryParamMap.get('t'));

    this.classBookIdFormControl = new FormControl<number | null>(this.classBookId);
    this.teacherPersonIdFormControl = new FormControl<number | null>(this.teacherPersonId);

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');

    this.classBookNomsService = new NomServiceWithParams(classBookNomsService, () => ({
      schoolYear: schoolYear,
      instId: instId
    }));

    this.instTeacherNomsService = new NomServiceWithParams(instTeacherNomsService, () => ({
      instId: instId,
      schoolYear: schoolYear,
      includeNotActiveTeachers: true
    }));
  }

  ngOnInit() {
    combineLatest([
      this.classBookIdFormControl.valueChanges.pipe(startWith(this.classBookId)),
      this.teacherPersonIdFormControl.valueChanges.pipe(startWith(this.teacherPersonId))
    ])
      .pipe(
        tap(([classBookId, teacherPersonId]) => {
          this.router.navigate(['./'], { queryParams: { c: classBookId, t: teacherPersonId }, relativeTo: this.route });
        }),
        takeUntil(this.destroyed$)
      )
      .subscribe();
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }
}
