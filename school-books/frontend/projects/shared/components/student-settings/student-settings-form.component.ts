import { Platform } from '@angular/cdk/platform';
import { Component, OnDestroy, OnInit } from '@angular/core';
import { UntypedFormBuilder, UntypedFormGroup, Validators } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { SwPush } from '@angular/service-worker';
import { faInfoCircle as fadInfoCircle } from '@fortawesome/pro-duotone-svg-icons/faInfoCircle';
import { PushNotificationsService } from 'projects/sb-api-client/src/api/pushNotifications.service';
import { StudentSettingsService } from 'projects/sb-api-client/src/api/studentSettings.service';
import { GlobalErrorHandler } from 'projects/shared/other/global-error-handler';
import { AuthService, SysRole } from 'projects/shared/services/auth.service';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { Subject } from 'rxjs';
import { takeUntil, tap } from 'rxjs/operators';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'sb-student-settings-form',
  templateUrl: './student-settings-form.component.html'
})
export class StudentSettingsFormComponent implements OnInit, OnDestroy {
  readonly destroyed$ = new Subject<void>();
  schoolYear!: number;
  readonly fadInfoCircle = fadInfoCircle;
  form!: UntypedFormGroup;
  showEmailSettings = false;
  showNotificationSettings = false;
  subscription!: PushSubscription;
  private VAPID_PUBLIC_KEY = environment.vapidPubKey;

  constructor(
    private fb: UntypedFormBuilder,
    private studentSettingsService: StudentSettingsService,
    private authService: AuthService,
    private route: ActivatedRoute,
    private swPush: SwPush,
    private pushNotificationsService: PushNotificationsService,
    private platform: Platform
  ) {
    this.form = this.fb.group({
      allowGradeEmails: [false, Validators.required],
      allowAbsenceEmails: [false, Validators.required],
      allowRemarkEmails: [false, Validators.required],
      allowMessageEmails: [false, Validators.required],
      allowGradeNotifications: [false, Validators.required],
      allowAbsenceNotifications: [false, Validators.required],
      allowRemarkNotifications: [false, Validators.required],
      allowMessageNotifications: [false, Validators.required]
    });
  }

  ngOnInit() {
    this.showEmailSettings = this.authService.tokenPayload.selected_role.SysRoleID === SysRole.Parent;
    this.showNotificationSettings = !this.platform.IOS && (!this.platform.WEBKIT || !this.platform.SAFARI);
    this.schoolYear = tryParseInt(this.route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');

    this.studentSettingsService
      .getStudentSettings()
      .toPromise()
      .then((settings) => {
        if (settings) {
          if (
            settings.allowAbsenceNotifications ||
            settings.allowGradeNotifications ||
            settings.allowRemarkNotifications
          ) {
            this.subscribeToPushNotifications();
          }

          this.form.setValue(
            {
              allowGradeEmails: settings.allowGradeEmails,
              allowAbsenceEmails: settings.allowAbsenceEmails,
              allowRemarkEmails: settings.allowRemarkEmails,
              allowMessageEmails: settings.allowMessageEmails,
              allowGradeNotifications: settings.allowGradeNotifications,
              allowAbsenceNotifications: settings.allowAbsenceNotifications,
              allowRemarkNotifications: settings.allowRemarkNotifications,
              allowMessageNotifications: settings.allowMessageNotifications
            },
            { emitEvent: false }
          );
        }
      });

    this.form.valueChanges
      .pipe(
        tap((value) =>
          this.studentSettingsService
            .createUpdateStudentSettings({
              createUpdateStudentSettingsCommand: {
                allowGradeEmails: value.allowGradeEmails,
                allowAbsenceEmails: value.allowAbsenceEmails,
                allowRemarkEmails: value.allowRemarkEmails,
                allowMessageEmails: value.allowMessageEmails,
                allowGradeNotifications: value.allowGradeNotifications,
                allowAbsenceNotifications: value.allowAbsenceNotifications,
                allowRemarkNotifications: value.allowRemarkNotifications,
                allowMessageNotifications: value.allowMessageNotifications
              }
            })
            .toPromise()
            .catch((err) => GlobalErrorHandler.instance.handleError(err))
        ),
        takeUntil(this.destroyed$)
      )
      .subscribe();
  }

  stopPropagation(e: Event, subscribe = false) {
    e.stopPropagation();
    if (subscribe) {
      this.subscribeToPushNotifications();
    }
  }

  subscribeToPushNotifications() {
    if (!('Notification' in window) || !('serviceWorker' in navigator)) {
      console.error('This browser does not support notifications or service workers.');
      return;
    }

    if (Notification.permission === 'default') {
      Notification.requestPermission().then((permission) => {
        if (permission !== 'granted') {
          console.error('Permission for notifications was denied.');
          return;
        }
        this.requestSubscription();
      });
    } else if (Notification.permission === 'granted') {
      this.requestSubscription();
    } else {
      console.error('Notifications are blocked by the user.');
    }
  }

  requestSubscription() {
    this.swPush
      .requestSubscription({
        serverPublicKey: this.VAPID_PUBLIC_KEY
      })
      .then((sub) => {
        this.subscription = sub;

        this.pushNotificationsService
          .subscribeToPushNotifications({
            schoolYear: this.schoolYear,
            pushSubscription: sub.toJSON()
          })
          .subscribe(
            () => console.log('Sent push subscription object to server.'),
            (err: any) => console.log('Could not send subscription object to server: ', err)
          );
      })
      .catch((err) => console.error('Could not subscribe to notifications', err));
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
  }
}
