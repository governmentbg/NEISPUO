import { Injectable, NgZone, OnDestroy } from '@angular/core';
import { Subject } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class BreakpointService implements OnDestroy {
  private smMediaQuery: MediaQueryList;
  private mdMediaQuery: MediaQueryList;
  private lgMediaQuery: MediaQueryList;

  public readonly change$ = new Subject();

  constructor(private zone: NgZone) {
    this.onChange = this.onChange.bind(this);

    // breakpoints are defined in tailwind.config.js
    // sm: '768px', // tablets in portrait
    // md: '992px', // tablets in landscape, small desktops
    // lg: '1200px' // large desktops and up
    this.smMediaQuery = matchMedia('(min-width: 768px)');
    this.mdMediaQuery = matchMedia('(min-width: 992px)');
    this.lgMediaQuery = matchMedia('(min-width: 1200px)');

    // using deprecated addListener/removeListener
    // as support for addEventListener/removeEventListener is not very good (ios 14 +)
    this.smMediaQuery.addListener(this.onChange);
    this.mdMediaQuery.addListener(this.onChange);
    this.lgMediaQuery.addListener(this.onChange);
  }

  public get sm() {
    return this.smMediaQuery.matches;
  }

  public get md() {
    return this.mdMediaQuery.matches;
  }

  public get lg() {
    return this.lgMediaQuery.matches;
  }

  onChange() {
    this.zone.run(() => this.change$.next());
  }

  ngOnDestroy() {
    this.smMediaQuery.removeListener(this.onChange);
    this.mdMediaQuery.removeListener(this.onChange);
    this.lgMediaQuery.removeListener(this.onChange);
    this.change$.complete();
  }
}
