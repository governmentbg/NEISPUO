import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-refresh',
  templateUrl: './refresh.component.html',
  styleUrls: ['./refresh.component.scss']
})
export class RefreshComponent {
  @Input() showRefreshModal;

  constructor() {}

  onClick(): void {
    location.reload();
  }
}
