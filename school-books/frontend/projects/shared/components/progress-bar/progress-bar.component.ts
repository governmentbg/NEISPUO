import { Component, Input } from '@angular/core';

@Component({
  selector: 'sb-progress-bar',
  templateUrl: './progress-bar.component.html',
  styleUrls: ['./progress-bar.component.scss']
})
export class ProgressBarComponent {
  @Input() percentage!: number;
  @Input() color = 'primary';
}
