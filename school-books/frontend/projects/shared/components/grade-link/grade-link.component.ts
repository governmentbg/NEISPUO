import { ChangeDetectionStrategy, Component, EventEmitter, Input, Output } from '@angular/core';
import { GradeCategory } from 'projects/sb-api-client/src/model/gradeCategory';
import { GradeType } from 'projects/sb-api-client/src/model/gradeType';
import { QualitativeGrade } from 'projects/sb-api-client/src/model/qualitativeGrade';
import { SpecialNeedsGrade } from 'projects/sb-api-client/src/model/specialNeedsGrade';

type Grade = {
  category: GradeCategory;
  type?: string | null;
  decimalGrade?: number | null;
  qualitativeGrade?: QualitativeGrade | null;
  specialGrade?: SpecialNeedsGrade | null;
};

@Component({
  selector: 'sb-grade-link',
  templateUrl: './grade-link.component.html',
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class GradeLinkComponent {
  @Input() grade!: Grade;

  @Input() isForecast = false;

  @Input() subjectTypeIsProfilingSubject = false;

  @Input() isSelected = false;

  @Output() selected = new EventEmitter();

  GRADE_CATEGORY_DECIMAL = GradeCategory.Decimal;
  GRADE_CATEGORY_QUALITATIVE = GradeCategory.Qualitative;
  GRADE_CATEGORY_SPECIAL_NEEDS = GradeCategory.SpecialNeeds;
  GRADE_TYPE_CLASS_EXAM = GradeType.ClassExam;

  click() {
    this.selected.emit();
  }
}
