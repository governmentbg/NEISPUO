import { Component, Inject } from '@angular/core';
import { MatBottomSheetRef, MAT_BOTTOM_SHEET_DATA } from '@angular/material/bottom-sheet';
import { faTimes as fasTimes } from '@fortawesome/pro-solid-svg-icons/faTimes';

@Component({
  selector: 'sb-app-prompt',
  templateUrl: './prompt.component.html',
  styleUrls: ['./prompt.component.scss']
})
export class PromptComponent {
  readonly fasTimes = fasTimes;

  constructor(
    @Inject(MAT_BOTTOM_SHEET_DATA) public data: { mobileType: 'ios' | 'android' },
    private bottomSheetRef: MatBottomSheetRef<PromptComponent>
  ) {}

  public close() {
    this.bottomSheetRef.dismiss();
  }
}
