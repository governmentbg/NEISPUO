import { Injectable } from '@angular/core';
import { BehaviorSubject } from 'rxjs';

@Injectable()
export class SideMenuService {
  private sideMenuVisibilitySource = new BehaviorSubject(false);
  displaySidemenu = this.sideMenuVisibilitySource.asObservable();

  constructor() {}

  changeSideMenuVisibility(displaySidemenu: boolean) {
    this.sideMenuVisibilitySource.next(displaySidemenu);
  }
}
