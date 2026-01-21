import { Injectable } from '@angular/core';
import { ShepherdService } from 'angular-shepherd';
import { ReplaySubject } from 'rxjs';
import { GuideName } from './constants/user-tour-guide.constants';

@Injectable({
  providedIn: 'root',
})

export class UserTourGuideService {
  constructor(private shepherdService: ShepherdService) { }

  private readonly _activeUserGuide$ = new ReplaySubject<GuideName | null>(1);

  public setUserGuide(guideName: GuideName) {
    this._activeUserGuide$.next(guideName);
  }

  public hideUserGuide() {
    this._activeUserGuide$.next(null);
  }

  public activeUserGuide$() {
    return this._activeUserGuide$;
  }

  stopGuide() {
    if (this.shepherdService.isActive) {
      this.shepherdService.cancel();
    }
  }
}
