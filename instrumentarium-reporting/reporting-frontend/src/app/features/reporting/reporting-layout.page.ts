import { Component, OnInit, OnDestroy } from '@angular/core';
import { InfoStripService } from '@core/services/info-strip.service';
import { NewModuleInfoQuery } from './new-module-info/new-module-info.query';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-reporting-layout',
  templateUrl: './reporting-layout.page.html',
  styleUrls: ['./reporting-layout.page.scss']
})
export class ReportingLayoutPage implements OnInit, OnDestroy {
  showNewModuleInfoModal = false;
  showNewModuleInfoModal$ = this.newModuleInfoQuery.showNewModuleInfoModal$;
  private subscription: Subscription;

  constructor(public newModuleInfoQuery: NewModuleInfoQuery, public infoStripService: InfoStripService) {}

  ngOnInit(): void {
    this.subscription = this.showNewModuleInfoModal$.subscribe((showNewModuleInfoModal) => {
      if (!showNewModuleInfoModal) {
        this.showNewModuleInfoModal = true;
        this.newModuleInfoQuery.updateValue(true);
      }
    });
  }

  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
