import { Component, Inject, Input, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import { ClassBookCurriculumNomsService } from 'projects/sb-api-client/src/api/classBookCurriculumNoms.service';
import { ExamsService, Exams_Get } from 'projects/sb-api-client/src/api/exams.service';
import { BookExamType } from 'projects/sb-api-client/src/model/bookExamType';
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
export class ExamViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(CLASS_BOOK_INFO) classBookInfo: Promise<ClassBookInfoType>,
    examsService: ExamsService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const examId = tryParseInt(route.snapshot.paramMap.get('examId'));
    const type = <BookExamType>route.snapshot.paramMap.get('type') ?? throwParamError('type');

    if (examId) {
      this.resolve(ExamViewComponent, {
        schoolYear,
        instId,
        classBookId,
        type,
        classBookInfo: from(classBookInfo),
        exam: examsService.get({
          schoolYear,
          instId,
          classBookId,
          examId: examId
        })
      });
    } else {
      this.resolve(ExamViewComponent, {
        schoolYear,
        instId,
        classBookId,
        type,
        classBookInfo: from(classBookInfo),
        exam: null
      });
    }
  }
}

@Component({
  selector: 'sb-exam-view',
  templateUrl: './exam-view.component.html'
})
export class ExamViewComponent implements OnInit, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    type: BookExamType;
    classBookInfo: ClassBookInfoType;
    exam: Exams_Get | null;
  };
  readonly fasTrashAlt = fasTrashAlt;
  readonly fasSpinnerThird = fasSpinnerThird;

  readonly form = this.fb.group({
    subject: [null, Validators.required],
    description: [null],
    date: [null, Validators.required]
  });

  title!: string;
  canEdit = false;
  canRemove = false;
  removing = false;
  classBookCurriculumNomsService: INomService<number, { schoolYear: number; instId: number; classBookId: number }>;

  constructor(
    private fb: UntypedFormBuilder,
    private examsService: ExamsService,
    classBookCurriculumNomsService: ClassBookCurriculumNomsService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService
  ) {
    this.classBookCurriculumNomsService = new NomServiceWithParams(classBookCurriculumNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      classBookId: this.data.classBookId,
      withWriteAccess: true
    }));
  }

  ngOnInit() {
    if (this.data.exam != null) {
      this.form.setValue({
        subject: this.data.exam.curriculumId,
        description: this.data.exam.description,
        date: this.data.exam.date
      });
      this.canEdit = this.data.classBookInfo.bookAllowsModifications && this.data.exam.hasEditAccess;
      this.canRemove = this.data.classBookInfo.bookAllowsModifications && this.data.exam.hasRemoveAccess;
    }

    const examTypeName = this.data.type === BookExamType.ControlExam ? 'контролна работа' : 'класна работа';
    this.title = this.data.exam == null ? `Създаване на ${examTypeName}` : `Редакция на ${examTypeName}`;
  }

  shouldPreventLeave() {
    return this.form.dirty;
  }

  onSave(save: SaveToken) {
    const value = this.form.value;
    const curriculumId = <number>value.subject;
    const exam = {
      type: this.data.type,
      description: <string>value.description,
      date: <Date>value.date
    };
    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.exam == null) {
            return this.examsService
              .createExam({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                classBookId: this.data.classBookId,
                curriculumId,
                createExamCommand: exam
              })
              .toPromise()
              .then((newExamId) => {
                this.form.markAsPristine();
                this.router.navigate(['../', newExamId, { type: this.data.type }], { relativeTo: this.route });
              });
          } else {
            const updateArgs = {
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              classBookId: this.data.classBookId,
              curriculumId: this.data.exam.curriculumId,
              examId: this.data.exam.examId
            };
            return this.examsService
              .update({
                updateExamCommand: exam,
                ...updateArgs
              })
              .toPromise()
              .then(() => this.examsService.get(updateArgs).toPromise())
              .then((newExam) => {
                this.data.exam = newExam;
              });
          }
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.exam) {
      throw new Error('onRemove requires a class exam to have been loaded.');
    }
    this.removing = true;
    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      classBookId: this.data.classBookId,
      curriculumId: this.data.exam.curriculumId,
      examId: this.data.exam.examId
    };
    const examTypeName = this.data.type === BookExamType.ControlExam ? 'контролната работа' : 'класната работа';
    this.actionService
      .execute({
        confirmMessage: `Сигурни ли сте че искате да изтриете ${examTypeName}?`,
        errorsMessage: `Не може да изтриете ${examTypeName}, защото:`,
        httpAction: () => this.examsService.remove(removeParams).toPromise()
      })
      .then((done) => {
        if (done) {
          this.router.navigate(['../'], { relativeTo: this.route });
        }
      })
      .finally(() => {
        this.removing = false;
      });
  }
}
