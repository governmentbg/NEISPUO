import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { OAuthService } from 'angular-oauth2-oidc';
import { isBefore } from 'date-fns';
import jwtDecode, { JwtPayload } from 'jwt-decode';
import { of } from 'rxjs';
import { delay, map } from 'rxjs/operators';
import { debounceUntilVisible } from '../utils/rxjs';
import { throwError } from '../utils/various';
import { ConfigService, Project } from './config.service';

export enum SysRole {
  Institution = 0,
  MonAdmin = 1,
  RuoAdmin = 2,
  Teacher = 5,
  Student = 6,
  Parent = 7,
  RuoExpert = 9,
  CIOO = 10,
  MonExpert = 12,
  InstitutionExpert = 14
}

export type NeispuoTokenPayload = {
  sub: string;
  sessionID: string;
  selected_role: {
    Username: string;
    SysUserID: number;
    SysRoleID: SysRole;
    InstitutionID: number;
    PositionID: number;
    MunicipalityID?: number | null;
    RegionID?: number | null;
    BudgetingInstitutionID?: number | null;
    PersonID?: number | null;
  };
};

export type NeispuoUserInfoPayload = {
  sub: string;
  FirstName: string;
  LastName: string;
};

const MULTI_INSTITUTION_ROLES = [
  SysRole.CIOO,
  SysRole.MonAdmin,
  SysRole.MonExpert,
  SysRole.RuoAdmin,
  SysRole.RuoExpert
];

const TEACHER_OR_INST_ADMIN_ROLES = [SysRole.Institution, SysRole.InstitutionExpert, SysRole.Teacher];

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  private userProfile?: NeispuoUserInfoPayload;

  constructor(private oauthService: OAuthService, private configService: ConfigService, private router: Router) {}

  initialize() {
    return this.oauthService.loadUserProfile().then(
      (userProfile: { info?: NeispuoUserInfoPayload }) => {
        this.userProfile = userProfile.info;
      },
      (err) => {
        if (err instanceof HttpErrorResponse && err.status === 401) {
          // defer until 5sec after the page becomes visible
          // as this error should be handled with an event handler in
          // AuthInitializerService.initialize
          return of(true)
            .pipe(
              debounceUntilVisible(),
              delay(5000),
              map(() => {
                throw err;
              })
            )
            .toPromise();
        }

        return Promise.reject(err);
      }
    );
  }

  get currentUserProfile() {
    return this.userProfile ?? throwError('Auth service is not initialized!');
  }

  get isNavigating() {
    return this.router.getCurrentNavigation() != null;
  }

  get currentNavigation() {
    const currentNavigation = this.router.getCurrentNavigation();
    if (!currentNavigation) {
      throw new Error('canLoadOrActivate should only be called from a route guard');
    }
    return currentNavigation;
  }

  get navigatingToUrl() {
    const currentNavigation = this.currentNavigation;
    return currentNavigation.finalUrl?.toString() || currentNavigation.extractedUrl.toString();
  }

  get navigatingToPath() {
    const navigatingToUrl = this.navigatingToUrl;
    const qIndex = navigatingToUrl.indexOf('?');
    return navigatingToUrl.substring(0, qIndex === -1 ? navigatingToUrl.length : qIndex);
  }

  lastDecodedAccessToken: string | null = null;
  lastDecodedTokenPayload: NeispuoTokenPayload | null = null;
  get tokenPayload(): NeispuoTokenPayload {
    if (!this.oauthService.hasValidIdToken() || !this.oauthService.hasValidAccessToken()) {
      throw new Error('Unexpected invalid token encountered.');
    }

    const accessToken = this.oauthService.getAccessToken() ?? throwError('Unexpected invalid token encountered.');

    if (this.lastDecodedAccessToken !== accessToken) {
      this.lastDecodedAccessToken = accessToken;
      this.lastDecodedTokenPayload = jwtDecode<JwtPayload>(accessToken) as NeispuoTokenPayload;
    }

    return this.lastDecodedTokenPayload ?? throwError('Unexpected invalid token encountered.');
  }

  get isMultiInstitutionUser() {
    return MULTI_INSTITUTION_ROLES.includes(this.tokenPayload.selected_role.SysRoleID);
  }

  get isTeacherOrInstAdminUser() {
    return TEACHER_OR_INST_ADMIN_ROLES.includes(this.tokenPayload.selected_role.SysRoleID);
  }

  get userPersonId() {
    return this.tokenPayload.selected_role.PersonID;
  }

  get sysUserId() {
    return this.tokenPayload.selected_role.SysUserID;
  }

  get sessionId() {
    return this.tokenPayload.sessionID;
  }

  redirectToReturnUrl() {
    if (!this.isNavigating) {
      return false;
    }

    if (this.currentNavigation.previousNavigation != null) {
      // navigate to returnUrl on initial navigation only
      return true;
    }

    const returnUrl = this.oauthService.state && decodeURIComponent(this.oauthService.state);
    if (this.navigatingToPath !== '/' || !returnUrl || typeof returnUrl !== 'string' || returnUrl === '/') {
      // navigate to returnUrl only when opening the default route and there is a returnUrl
      return true;
    }

    return this.router.parseUrl(returnUrl);
  }

  redirectToDefaultRoute() {
    if (!this.isNavigating) {
      return false;
    }

    if (this.navigatingToPath !== '/') {
      return true;
    }

    switch (this.tokenPayload.selected_role.SysRoleID) {
      case SysRole.CIOO:
      case SysRole.MonAdmin:
      case SysRole.MonExpert:
      case SysRole.RuoAdmin:
      case SysRole.RuoExpert:
        return this.router.createUrlTree(['admin', 'institutions']);

      case SysRole.Institution:
      case SysRole.InstitutionExpert:
      case SysRole.Teacher:
        return this.router.createUrlTree([
          this.configService.currentUserConfig.tokenInstSchoolYear,
          this.configService.currentUserConfig.tokenInstId
        ]);

      case SysRole.Student:
      case SysRole.Parent: {
        // TODO redirect to the info board, or better yet, setup the redirect in the student-routing.module.ts
        const currentYear = new Date().getFullYear();
        const schoolYearCutoff = new Date(currentYear, 8, 1);
        if (isBefore(new Date(), schoolYearCutoff)) {
          return this.router.createUrlTree([currentYear - 1]);
        } else {
          return this.router.createUrlTree([currentYear]);
        }
      }

      default:
        return this.router.createUrlTree(['not-found']);
    }
  }

  redirectToCorrectApp() {
    if (!this.isNavigating) {
      return false;
    }

    const role = this.tokenPayload.selected_role.SysRoleID;
    let replaceLocation: string | null = null;
    if (
      this.configService.launchConfiguration.project === Project.TeachersApp &&
      (role === SysRole.Student || role === SysRole.Parent)
    ) {
      replaceLocation = this.configService.launchConfiguration.studentsAppUrl;
    } else if (
      this.configService.launchConfiguration.project === Project.StudentsApp &&
      (role === SysRole.CIOO ||
        role === SysRole.MonAdmin ||
        role === SysRole.MonExpert ||
        role === SysRole.RuoAdmin ||
        role === SysRole.RuoExpert ||
        role === SysRole.Institution ||
        role === SysRole.InstitutionExpert ||
        role === SysRole.Teacher)
    ) {
      replaceLocation = this.configService.launchConfiguration.teachersAppUrl;
    }

    if (replaceLocation) {
      window.location.replace(replaceLocation);

      // return a never resolving promise
      // as we wait for the browser to perform the location change
      return new Promise<boolean>(() => {
        // do nothing
      });
    }

    return true;
  }

  logout() {
    this.oauthService.logOut();
  }
}
