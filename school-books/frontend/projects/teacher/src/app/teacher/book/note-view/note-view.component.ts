import { Component, Inject, Input, OnInit } from '@angular/core';
import { UntypedFormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { faTrashAlt as fasTrashAlt } from '@fortawesome/pro-solid-svg-icons/faTrashAlt';
import { ClassBookStudentNomsService } from 'projects/sb-api-client/src/api/classBookStudentNoms.service';
import { NotesService, Notes_Get } from 'projects/sb-api-client/src/api/notes.service';
import { SaveToken } from 'projects/shared/components/editor-panel/editor-panel.component';
import { ALL_GROUP_ID } from 'projects/shared/components/nom-select/nom-select.component';
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
export class NoteViewSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(CLASS_BOOK_INFO) classBookInfo: Promise<ClassBookInfoType>,
    notesService: NotesService,
    route: ActivatedRoute
  ) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');
    const noteId = tryParseInt(route.snapshot.paramMap.get('noteId'));

    if (noteId) {
      this.resolve(NoteViewComponent, {
        schoolYear,
        instId,
        classBookId,
        classBookInfo: from(classBookInfo),
        note: notesService.get({
          schoolYear,
          instId,
          classBookId,
          noteId: noteId
        })
      });
    } else {
      this.resolve(NoteViewComponent, {
        schoolYear,
        instId,
        classBookId,
        classBookInfo: from(classBookInfo),
        note: null
      });
    }
  }
}

@Component({
  selector: 'sb-note-view',
  templateUrl: './note-view.component.html'
})
export class NoteViewComponent implements OnInit, IShouldPreventLeave {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    classBookInfo: ClassBookInfoType;
    note: Notes_Get | null;
  };
  readonly fasTrashAlt = fasTrashAlt;
  readonly fasSpinnerThird = fasSpinnerThird;

  readonly form = this.fb.group({
    studentIds: [[]],
    description: [null, Validators.required]
  });

  removing = false;
  classBookStudentNomsService: INomService<number, { schoolYear: number; instId: number; classBookId: number }>;
  canEdit = false;
  canRemove = false;

  constructor(
    private fb: UntypedFormBuilder,
    private notesService: NotesService,
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
  }

  ngOnInit() {
    const note = this.data.note;
    if (note != null) {
      this.form.setValue({
        studentIds: !note.isForAllStudents ? note.studentIds : [ALL_GROUP_ID],
        description: note.description
      });

      this.canEdit =
        this.data.classBookInfo.bookAllowsModifications &&
        (this.data.classBookInfo.hasEditNoteAccess || note.hasCreatorAccess);
      this.canRemove =
        this.data.classBookInfo.bookAllowsModifications &&
        (this.data.classBookInfo.hasRemoveNoteAccess || note.hasCreatorAccess);
    }
  }

  shouldPreventLeave() {
    return this.form.dirty;
  }

  onSave(save: SaveToken) {
    const value = this.form.value;
    const isForAllStudents = (<Array<any>>value.studentIds).includes(ALL_GROUP_ID);
    const note = {
      studentIds: !isForAllStudents ? <Array<number>>value.studentIds : [],
      description: <string>value.description,
      isForAllStudents: isForAllStudents
    };
    this.actionService
      .execute({
        httpAction: () => {
          if (this.data.note == null) {
            return this.notesService
              .createNote({
                schoolYear: this.data.schoolYear,
                instId: this.data.instId,
                classBookId: this.data.classBookId,
                createNoteCommand: note
              })
              .toPromise()
              .then((newNoteId) => {
                this.form.markAsPristine();
                this.router.navigate(['../', newNoteId], { relativeTo: this.route });
              });
          } else {
            const updateArgs = {
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              classBookId: this.data.classBookId,
              noteId: this.data.note.noteId
            };
            return this.notesService
              .update({
                updateNoteCommand: note,
                ...updateArgs
              })
              .toPromise()
              .then(() => this.notesService.get(updateArgs).toPromise())
              .then((newNote) => {
                this.data.note = newNote;
              });
          }
        }
      })
      .then((success) => save.done(success));
  }

  onRemove() {
    if (!this.data.note) {
      throw new Error('onRemove requires a note to have been loaded.');
    }
    this.removing = true;
    const removeParams = {
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      classBookId: this.data.classBookId,
      noteId: this.data.note.noteId
    };
    this.actionService
      .execute({
        confirmMessage: 'Сигурни ли сте че искате да изтриете бележката?',
        errorsMessage: 'Не може да изтриете бележката, защото:',
        httpAction: () => this.notesService.remove(removeParams).toPromise()
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
