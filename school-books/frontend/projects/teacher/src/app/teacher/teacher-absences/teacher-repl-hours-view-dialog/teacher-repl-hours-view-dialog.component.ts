import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ActivatedRoute } from '@angular/router';
import { TypedDialog } from 'projects/shared/utils/dialog';
import { MappedScheduleCurriculumGroup } from '../teacher-absence-view/teacher-absence-view.component';

export type TeacherReplHoursViewDialogData = {
  schoolYear: number;
  instId: number;
  group: MappedScheduleCurriculumGroup;
  absences: {
    absenceId?: number | null;
    dates: string;
  }[];
  route: ActivatedRoute;
};

@Component({
  selector: 'sb-teacher-repl-hours-view-dialog.component',
  templateUrl: './teacher-repl-hours-view-dialog.component.html'
})
export class TeacherReplHoursViewDialogComponent implements TypedDialog<TeacherReplHoursViewDialogData, void> {
  d!: TeacherReplHoursViewDialogData;
  r!: void;

  constructor(
    @Inject(MAT_DIALOG_DATA)
    public data: TeacherReplHoursViewDialogData
  ) {}
}
