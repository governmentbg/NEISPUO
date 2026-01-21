import { Component } from '@angular/core';
import { Observable } from 'rxjs';
import { InfoStripService } from '../../services/info-strip.service';

@Component({
  selector: 'app-info-strip',
  templateUrl: './info-strip.component.html',
  styleUrls: ['./info-strip.component.scss']
})
export class InfoStripComponent {
  isVisible$: Observable<boolean>;

  constructor(private infoStripService: InfoStripService) {
    this.isVisible$ = this.infoStripService.isVisible$;
  }

  hideStrip(): void {
    this.infoStripService.hide();
  }
}
