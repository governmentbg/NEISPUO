import { Component, Inject, Input, OnInit } from '@angular/core';
import { FormGroup, UntypedFormBuilder, Validators } from '@angular/forms';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import { ClassBookCurriculumNomsService } from 'projects/sb-api-client/src/api/classBookCurriculumNoms.service';
import { ClassBooksService, ClassBooks_GetStudents } from 'projects/sb-api-client/src/api/classBooks.service';
import { ClassBookStudentNomsService } from 'projects/sb-api-client/src/api/classBookStudentNoms.service';
import { TopicsDplrService } from 'projects/sb-api-client/src/api/topicsDplr.service';
import { INomService, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import {
  SIMPLE_TAB_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { hourRegex } from 'projects/shared/utils/date';

export type CustomTopicDialogData = {
  schoolYear: number;
  instId: number;
  classBookId: number;
  date: Date;
};

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class AddTopicDplrDialogSkeletonComponent extends SkeletonComponentBase {
  d!: CustomTopicDialogData;
  r!: void;
  constructor(
    @Inject(MAT_DIALOG_DATA)
    data: CustomTopicDialogData,
    classBooksService: ClassBooksService,
    topicsDplrService: TopicsDplrService
  ) {
    super();

    const schoolYear = data.schoolYear;
    const instId = data.instId;
    const classBookId = data.classBookId;
    const date = data.date;

    this.resolve(AddTopicDplrDialogComponent, {
      schoolYear,
      instId,
      classBookId,
      date,
      students: classBooksService.getStudents({
        schoolYear,
        instId,
        classBookId
      }),
      existingHourNumbers: topicsDplrService.getExistingHourNumbersForDate({
        schoolYear,
        instId,
        classBookId,
        date
      })
    });
  }
}

@Component({
  selector: 'sb-add-topic-dplr-dialog',
  templateUrl: './add-topic-dplr-dialog.component.html'
})
export class AddTopicDplrDialogComponent implements OnInit {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    classBookId: number;
    date: Date;
    students: ClassBooks_GetStudents;
    existingHourNumbers: number[];
  };

  readonly predefinedDurations = [
    { id: 0, text: 'Въведи край' },
    { id: 30, text: '30' },
    { id: 35, text: '35' },
    { id: 40, text: '40' },
    { id: 45, text: '45' },
    { id: 50, text: '50' },
    { id: 55, text: '55' },
    { id: 60, text: '60' },
    { id: 90, text: '90' }
  ];

  readonly fasCheck = fasCheck;
  readonly fasSpinnerThird = fasSpinnerThird;

  saving = false;
  showHourNumberError = false;
  hourNumberErrorMessage = '';

  form: FormGroup;
  classBookCurriculumNomsService: INomService<number, { schoolYear: number; instId: number; classBookId: number }>;
  classBookStudentNomsService: INomService<number, { schoolYear: number; instId: number; classBookId: number }>;

  constructor(
    private fb: UntypedFormBuilder,
    private actionService: ActionService,
    classBookStudentNomsService: ClassBookStudentNomsService,
    private dialogRef: MatDialogRef<AddTopicDplrDialogSkeletonComponent>,
    private topicsDplrService: TopicsDplrService,
    classBookCurriculumNomsService: ClassBookCurriculumNomsService
  ) {
    this.form = this.fb.group({
      hourNumber: [null, [Validators.required, Validators.min(1)]],
      startTime: [null, [Validators.required, Validators.pattern(hourRegex)]],
      duration: [null, Validators.required],
      endTime: [null, [Validators.required, Validators.pattern(hourRegex)]],
      curriculumId: [null, Validators.required],
      location: [null],
      topicTitle: [null, Validators.required],
      students: [null]
    });

    this.classBookCurriculumNomsService = new NomServiceWithParams(classBookCurriculumNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      classBookId: this.data.classBookId
    }));
    this.classBookStudentNomsService = new NomServiceWithParams(classBookStudentNomsService, () => ({
      schoolYear: this.data.schoolYear,
      instId: this.data.instId,
      classBookId: this.data.classBookId
    }));
  }

  ngOnInit(): void {
    this.form.get('startTime')?.valueChanges.subscribe(() => this.updateEndTime());
    this.form.get('duration')?.valueChanges.subscribe(() => this.updateEndTime());
    this.form.get('hourNumber')?.valueChanges.subscribe(() => this.ValidateHourNumber());
  }

  updateEndTime(): void {
    const startTime = this.form.get('startTime')?.value;
    const duration = this.form.get('duration')?.value;

    if (startTime && hourRegex.test(startTime) && duration) {
      const [hours, minutes] = startTime.split(':').map(Number);
      const startDate = new Date();
      startDate.setHours(hours, minutes);

      const endDate = new Date(startDate.getTime() + duration * 60000);
      const endHours = endDate.getHours().toString().padStart(2, '0');
      const endMinutes = endDate.getMinutes().toString().padStart(2, '0');

      this.form.get('endTime')?.setValue(`${endHours}:${endMinutes}`);
    } else {
      this.form.get('endTime')?.setValue(null);
    }
  }

  ValidateHourNumber() {
    const existingHours = this.data.existingHourNumbers;
    const hourNumber = this.form.get('hourNumber')?.value;

    if (existingHours.indexOf(hourNumber) !== -1) {
      this.showHourNumberError = true;
      this.hourNumberErrorMessage = `Вече има създадени часове с тези номера - ${existingHours.join(
        ', '
      )}. Моля изберете друг номер на час.`;
    } else {
      this.showHourNumberError = false;
      this.hourNumberErrorMessage = '';
    }
  }

  onSave() {
    if (this.form.invalid || this.saving) {
      return;
    }

    this.saving = true;

    const value = this.form.value;
    this.actionService
      .execute({
        httpAction: async () => {
          const students =
            this.data.students
              .filter((s) => this.form.value.students && this.form.value.students.includes(s.personId))
              .map((student) => student.personId) || [];

          return this.topicsDplrService
            .createTopicDplr({
              schoolYear: this.data.schoolYear,
              instId: this.data.instId,
              classBookId: this.data.classBookId,
              createTopicDplrCommand: {
                date: this.data.date,
                day: this.data.date.getDay(),
                hourNumber: value.hourNumber,
                startTime: value.startTime,
                endTime: value.endTime,
                curriculumId: value.curriculumId,
                location: value.location,
                title: value.topicTitle,
                studentPersonIds: students ?? []
              }
            })
            .toPromise()
            .then(() => {
              this.dialogRef.close(true);
            });
        }
      })
      .finally(() => {
        this.saving = false;
      });
  }
}
