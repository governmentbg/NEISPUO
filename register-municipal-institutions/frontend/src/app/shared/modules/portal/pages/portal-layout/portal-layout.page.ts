import { Component, OnInit } from '@angular/core';
import { MediaObserver } from '@angular/flex-layout';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { AuthQuery } from '@core/authentication/auth-state-manager/auth.query';
import { RoleEnum } from '@core/authentication/models/role.enum';
import { EnvironmentService } from '@core/services/environment.service';
import { CONSTANTS } from '@shared/contstants';
import { NomenclatureQuery } from '@shared/services/common-form-service/nomenclatures-state/nomenclatures.query';
import { Observable } from 'rxjs';
import { filter, take, map } from 'rxjs/operators';
import { SubSink } from 'subsink';
import { GuideName } from '@shared/modules/user-tour-guide/constants/user-tour-guide.constants';
import { UserTourGuideService } from '@shared/modules/user-tour-guide/user-tour-guide.service';
import pjson from '../../../../../../../package.json';
import { ISideMenuConfig, SideMenuConfig } from './portal-layout.config';

@Component({
  selector: 'app-portal-layout',
  templateUrl: './portal-layout.page.html',
  styleUrls: ['./portal-layout.page.scss'],
})
export class PortalLayoutPage implements OnInit {
  public readonly environment;

  isMon$ = this.authQuery.isMon$;

  isRuo$ = this.authQuery.isRuo$;

  isMunicipality$ = this.authQuery.isMunicipality$;

  subs = new SubSink();

  BackendVersion$ = this.nomenclatureQuery.BackendVersion$;

  frontEndVersion = pjson.version;

  frontEndName = pjson.name;

  applicationVersions = '';

  displaySidemenu: boolean = false;

  sideMenuList!: ISideMenuConfig[];

  currentRoute: string;

  activeMenuList: ISideMenuConfig;

  selectedRole$ = this.authQuery.selectedRole$;

  publicRegisterLabel = CONSTANTS.SIDEMENU_TITLE_PUBLIC_REGISTER;

  guideName = GuideName.DASHBOARD_MENU;

  constructor(public media: MediaObserver, private authQuery: AuthQuery, private envService: EnvironmentService, private nomenclatureQuery: NomenclatureQuery, private router: Router, public activatedRoute: ActivatedRoute, private userGuideService: UserTourGuideService) {
    this.environment = this.envService.environment;
  }

  ngOnInit(): void {
    this.getApplicationVersions();
    this.sideMenuList = SideMenuConfig;
    this.currentRoute = this.activatedRoute.snapshot.firstChild?.routeConfig?.path;
    this.router.events.pipe(filter((event) => event instanceof NavigationEnd)).subscribe((event) => {
      this.currentRoute = this.activatedRoute.snapshot.firstChild?.routeConfig?.path;
    });
  }

  ngAfterViewInit(): void {
    this.userGuideService.setUserGuide(this.guideName);
  }

  hasRights(roles: RoleEnum[]): Observable<boolean> {
    return this.selectedRole$.pipe(
      filter((resp) => !!resp),
      take(1),
      map(
        (role) => roles?.includes(role?.SysRoleID as RoleEnum),
      ),
    );
  }

  toggleSideNav() {
    this.displaySidemenu = !this.displaySidemenu;
  }

  private getApplicationVersions() {
    this.subs.sink = this.nomenclatureQuery.BackendVersion$.subscribe((backendVersion) => {
      if (!backendVersion) {
        return;
      }
      this.applicationVersions = `Versions: ${this.frontEndName} ${this.frontEndVersion}, ${backendVersion.name} ${backendVersion.version}`;
    });
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe();
    this.userGuideService.stopGuide();
  }
}
