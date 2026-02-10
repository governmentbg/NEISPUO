import { Component, Inject, Input, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import { ClassBookCurriculumNomsService } from 'projects/sb-api-client/src/api/classBookCurriculumNoms.service';
import { ClassBookStudentNomsService } from 'projects/sb-api-client/src/api/classBookStudentNoms.service';
import { RemarksService, Remarks_Get } from 'projects/sb-api-client/src/api/remarks.service';
import { RemarkType } from 'projects/sb-api-client/src/model/remarkType';
import { SaveToken } from 'projects/shared/components/editor-panel/editor-panel.component';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import {
  IShouldPreventLeave,
  SIMPLE_TAB_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { ClassBookInfoType } from 'projects/shared/utils/book';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { from } from 'rxjs';
import { CLASS_BOOK_INFO } from '../book/book.component';

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class RemarkViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(CLASS_BOOK_INFO) classBookInfo: Promise<ClassBookInfoType>,
    remarksService: RemarksService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const remarkId = tryParseInt(route.snapshot.paramMap.get('remarkId'));
    const type = <RemarkType>route.snapshot.paramMap.get('type') ?? throwParamError('type');

    if (remarkId) {
      this.resolve(RemarkViewComponent, {
        schoolYear,
        instId,
        classBookId,
        type,
        classBookInfo: from(classBookInfo),
        remark: remarksService.get({
          schoolYear,
          instId,
          classBookId,
          remarkId: remarkId
        })
      });
    } else {
      this.resolve(RemarkViewComponent, {
        schoolYear,
        instId,
        classBookId,
        classBookInfo: from(classBookInfo),
        type,
        remark: null
      });
    }
  }
}

@Component({
  selector: 'sb-remark-view',
  templateUrl: './remark-view.component.html'
})
export class RemarkViewComponent implements OnInit, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    type: RemarkType;
    classBookInfo: ClassBookInfoType;
    remark: Remarks_Get | null;
  };
  readonly fasTrashAlt = fasTrashAlt;
  readonly fasSpinnerThird = fasSpinnerThird;

  readonly form = this.fb.group({
    studentId: [null, Validators.required],
    subject: [null, Validators.required],
    description: [null, Validators.required],
    date: [null, Validators.required]
  });

  title!: string;
  removing = false;
  classBookCurriculumNomsService: INomService<number, { schoolYear: number; instId: number; classBookId: number }>;
  classBookStudentNomsService: INomService<number, { schoolYear: number; instId: number; classBookId: number }>;

  constructor(
    private fb: UntypedFormBuilder,
    private remarksService: RemarksService,
    classBookCurriculumNomsService: ClassBookCurriculumNomsService,
    classBookStudentNomsService: ClassBookStudentNomsService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService
  ) {
    this.classBookStudentNomsService = new NomServiceWithParams(classBookStudentNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      classBookId: this.data.classBookId
    }));
    this.classBookCurriculumNomsService = new NomServiceWithParams(classBookCurriculumNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      classBookId: this.data.classBookId,
      withWriteAccess: true
    }));
  }

  ngOnInit() {
    if (this.data.remark != null) {
      this.form.setValue({
        studentId: this.data.remark.personId,
        subject: this.data.remark.curriculumId,
        description: this.data.remark.description,
        date: this.data.remark.date
      });
    }

    const remarkTypeName = this.data.type === RemarkType.Bad ? 'забележка' : 'похвала';
    this.title = this.data.remark == null ? `Създаване на ${remarkTypeName}` : `Редакция на ${remarkTypeName}`;
  }

  shouldPreventLeave() {
    return this.form.dirty;
  }

  onSave(save: SaveToken) {
    const value = this.form.value;
    const curriculumId = <number>value.subject;
    const remark = {
      type: this.data.type,
      personId: <number>value.studentId,
      description: <string>value.description,
      date: <Date>value.date
    };
    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.remark == null) {
            return this.remarksService
              .createRemark({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                classBookId: this.data.classBookId,
                curriculumId,
                createRemarkCommand: remark
              })
              .toPromise()
              .then(() => {
                this.form.markAsPristine();
                this.router.navigate(['../'], { relativeTo: this.route });
              });
          } else {
            const updateArgs = {
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              classBookId: this.data.classBookId,
              curriculumId: this.data.remark.curriculumId,
              remarkId: this.data.remark.remarkId
            };
            return this.remarksService
              .update({
                updateRemarkCommand: remark,
                ...updateArgs
              })
              .toPromise()
              .then(() => this.remarksService.get(updateArgs).toPromise())
              .then(() => {
                this.form.markAsPristine();
                this.router.navigate(['../'], { relativeTo: this.route });
              });
          }
        }
      })
      .then((success) => save.done(success));
  }
}
