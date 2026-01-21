import { AfterViewInit, ChangeDetectionStrategy, Component, ElementRef, Input } from '@angular/core';
import { faCheckCircle as fadCheckCircle } from '@fortawesome/pro-duotone-svg-icons/faCheckCircle';
import { faExclamationTriangle as fadExclamationTriangle } from '@fortawesome/pro-duotone-svg-icons/faExclamationTriangle';
import { faInfoSquare as fadInfoSquare } from '@fortawesome/pro-duotone-svg-icons/faInfoSquare';
import { faTimesCircle as fadTimesCircle } from '@fortawesome/pro-duotone-svg-icons/faTimesCircle';
import { throwError } from 'projects/shared/utils/various';

export type BannerType = 'info' | 'success' | 'warning' | 'error';

@Component({
  selector: 'sb-banner',
  templateUrl: './banner.component.html',
  styleUrls: ['./banner.component.scss'],
  changeDetection: ChangeDetectionStrategy.OnPush
})
export class BannerComponent implements AfterViewInit {
  @Input() type: BannerType = 'info';
  @Input() showIcon = true;
  @Input() small = false;

  readonly fadInfoSquare = fadInfoSquare;
  readonly fadCheckCircle = fadCheckCircle;
  readonly fadExclamationTriangle = fadExclamationTriangle;
  readonly fadTimesCircle = fadTimesCircle;

  constructor(private el: ElementRef) {}

  get icon() {
    return this.type === 'info'
      ? fadInfoSquare
      : this.type === 'success'
      ? fadCheckCircle
      : this.type === 'warning'
      ? fadExclamationTriangle
      : this.type === 'error'
      ? fadTimesCircle
      : throwError('Unknown banner type');
  }

  ngAfterViewInit() {
    if (this.type === 'error') {
      this.el.nativeElement.scrollIntoView({ behavior: 'smooth', block: 'center' });
    }
  }
}
