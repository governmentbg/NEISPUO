import { BACKSPACE, COMMA, ENTER, SPACE } from '@angular/cdk/keycodes';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MatChipInputEvent } from '@angular/material/chips';

export type AbsenceChip = {
  scheduleLessonId?: number;
  absenceId?: number;
  classNumber?: number | null;
  readonly?: boolean;
  excused?: boolean;
  removed?: boolean;
  createDate?: Date;
  hasUndoAccess?: boolean;
};

@Component({
  selector: 'sb-absence-chips',
  templateUrl: './absence-chips.component.html',
  styleUrls: ['./absence-chips.component.scss']
})
export class AbsenceChipsComponent {
  @Input()
  absenceChips: AbsenceChip[] = [];

  @Input()
  disableInput = false;

  @Output() selected = new EventEmitter<AbsenceChip>();

  @Output() added = new EventEmitter<string>();

  @Output() removedLast = new EventEmitter();

  readonly separatorKeysCodes: number[] = [ENTER, COMMA, SPACE];

  chipSelected(absenceChip: AbsenceChip) {
    this.selected.emit(absenceChip);
  }

  add(event: MatChipInputEvent) {
    const input = event.chipInput.inputElement;
    const value = event.value;

    if ((value || '').trim()) {
      this.added.emit(value.trim());
    }

    // Reset the input value
    input.value = '';
  }

  chipInputKeydown(input: HTMLInputElement, event: KeyboardEvent) {
    if (event.keyCode === BACKSPACE && !input.value) {
      this.removedLast.emit();
      event.preventDefault();
      return;
    }
  }
}
