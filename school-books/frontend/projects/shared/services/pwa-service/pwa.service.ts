import { Platform } from '@angular/cdk/platform';
import { Injectable } from '@angular/core';
import { MatBottomSheet } from '@angular/material/bottom-sheet';
import { NavigationEnd, Router } from '@angular/router';
import { timer } from 'rxjs';
import { filter, take } from 'rxjs/operators';
import { AuthService } from '../auth.service';
import { PromptComponent } from './prompt/prompt.component';

@Injectable({
  providedIn: 'root'
})
export class PwaService {
  private promptEvent: BeforeInstallPromptEvent | undefined;

  constructor(
    private bottomSheet: MatBottomSheet,
    private platform: Platform,
    private router: Router,
    private authService: AuthService
  ) {}

  initPwaPrompt() {
    this.router.events
      .pipe(
        filter((event) => event instanceof NavigationEnd),
        take(1)
      )
      .subscribe(() => {
        if (this.platform.ANDROID) {
          window.addEventListener('beforeinstallprompt', (event: BeforeInstallPromptEvent) => {
            event.preventDefault();
            this.promptEvent = event;
            this.openPromptComponent('android');
          });
        }
        if (this.platform.IOS) {
          const isInStandaloneMode = window.matchMedia('(display-mode: standalone)').matches;
          if (!isInStandaloneMode) {
            this.openPromptComponent('ios');
          }
        }
      });
  }

  openPromptComponent(mobileType: 'ios' | 'android') {
    timer(5000)
      .pipe(take(1))
      .subscribe(() => {
        this.bottomSheet.open(PromptComponent, {
          data: { mobileType, promptEvent: this.promptEvent }
        });
      });
  }
}
