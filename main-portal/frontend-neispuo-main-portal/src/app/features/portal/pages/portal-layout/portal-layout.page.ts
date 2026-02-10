import { Component, OnInit, OnDestroy } from '@angular/core';
import { AuthQuery } from '@authentication/auth-state-manager/auth.query';
import { EnvironmentService } from '@shared/services/environment.service';
import { NeispuoModuleQuery } from '../../neispuo-modules/neispuo-module.query';
import * as pjson from '../../../../../../package.json';
import { ApiService } from '@shared/services/api.service';
import { VersionModel } from 'src/app/resources/models/version.model';
import { combineLatest, timer, Subject } from 'rxjs';
import { distinctUntilChanged, map, takeUntil } from 'rxjs/operators';
import { ShepherdService } from 'angular-shepherd';
import { UserTourQuery } from '@portal/neispuo-modules/user-tour.query';
import { steps } from '../../../../shared/constants/user-tour.constants';
import { MediaObserver } from '@angular/flex-layout';
import { SideMenuService } from '@shared/services/side-menu.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-portal-layout',
  templateUrl: './portal-layout.page.html',
  styleUrls: ['./portal-layout.page.scss']
})
export class PortalLayoutPage implements OnInit, OnDestroy {
  private destroy$ = new Subject<void>();

  showRefreshModal: boolean = false;

  displaySidemenu: boolean = false;

  isAdminPanelExpanded: boolean = false;

  isParent$ = this.authQuery.isParent$;

  neispuoModules$ = this.neispuoModuleQuery.modules$;

  categories$ = this.neispuoModuleQuery.categories$;

  startUserTour$ = this.userTourQuery.startUserTour$;

  isMon$ = this.authQuery.isMon$;

  hasUserGuideManagementAccess$ = combineLatest([this.authQuery.isMon$, this.authQuery.isHelpdesk$]).pipe(
    map(([isMon, isHelpdesk]) => isMon || isHelpdesk),
    distinctUntilChanged()
  );

  showAdminPanel$ = combineLatest([this.hasUserGuideManagementAccess$, this.isMon$]).pipe(
    map(([hasUserGuideAccess, isMon]) => hasUserGuideAccess || isMon),
    distinctUntilChanged()
  );

  environment = this.envService.environment;

  appVersion = `${pjson.name}: ${pjson.version}`;

  feVersion = pjson.version;

  beVersion: string;

  constructor(
    public media: MediaObserver,
    public neispuoModuleQuery: NeispuoModuleQuery,
    private authQuery: AuthQuery,
    private envService: EnvironmentService,
    private readonly apiService: ApiService,
    private shepherdService: ShepherdService,
    public userTourQuery: UserTourQuery,
    public sideMenuData: SideMenuService,
    private router: Router
  ) {}

  ngOnInit() {
    this.initializeComponent();
    this.setupSubscriptions();
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }

  toggleAdminPanel() {
    this.isAdminPanelExpanded = !this.isAdminPanelExpanded;
  }

  private initializeComponent() {
    this.getBackendVersion();
    this.checkAndExpandAdminPanel();
  }

  private setupSubscriptions() {
    this.sideMenuData.displaySidemenu
      .pipe(takeUntil(this.destroy$))
      .subscribe((displaySidemenu) => (this.displaySidemenu = displaySidemenu));

    timer(0, this.envService.environment.SECONDS_TO_CHECK_VERSION * 1000)
      .pipe(
        takeUntil(this.destroy$),
        map(() => this.getNewVersion())
      )
      .subscribe();

    this.startUserTour$.pipe(takeUntil(this.destroy$)).subscribe((startUserTour) => {
      if (!startUserTour) {
        this.initializeUserTour();
      }
    });
  }

  private checkAndExpandAdminPanel() {
    const currentUrl = this.router.url;
    const adminRoutes = ['/portal/user-guide-management', '/portal/system-user-messages', '/portal/sync-messages'];

    if (adminRoutes.some((route) => currentUrl.startsWith(route))) {
      this.isAdminPanelExpanded = true;
    }
  }

  private initializeUserTour() {
    this.shepherdService.defaultStepOptions = {
      scrollTo: false
    };
    this.shepherdService.modal = true;
    this.shepherdService.addSteps(steps);
    this.shepherdService.start();
    this.userTourQuery.updateValue(true);
  }

  private getBackendVersion() {
    this.apiService
      .get('/v1/version')
      .pipe(takeUntil(this.destroy$))
      .subscribe((beVersion: VersionModel) => {
        this.appVersion += `, ${beVersion.name}: ${beVersion.version}`;
        this.beVersion = beVersion.version;
      });
  }

  private getNewVersion() {
    this.apiService
      .get('/v1/version')
      .pipe(takeUntil(this.destroy$))
      .subscribe((beVersion: VersionModel) => {
        const currentBEVersion = beVersion.version;
        const currentFEVersion = pjson.version;

        if (currentBEVersion !== currentFEVersion) {
          this.showRefreshModal = true;
        }
      });
  }
}
