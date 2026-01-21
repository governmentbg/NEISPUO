import { Component, OnDestroy, OnInit } from '@angular/core';
import { AuthQuery } from '@authentication/auth-state-manager/auth.query';
import { OIDCService } from '@authentication/auth-state-manager/oidc.service';
import { NeispuoModuleQuery } from '@portal/neispuo-modules/neispuo-module.query';
import { NeispuoModuleService } from '@portal/neispuo-modules/neispuo-module.service';
import { filter, switchMap, take } from 'rxjs/operators';
import { SubSink } from 'subsink';
@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.page.html',
  styleUrls: ['./dashboard.page.scss']
})
export class DashboardPage implements OnInit, OnDestroy {
  categories$ = this.neispuoModuleQuery.categories$;
  selectedCategory$ = this.neispuoModuleQuery.selectedCategory$;
  abbreviatedName$ = this.authQuery.abbreviatedName$;
  fullName$ = this.authQuery.fullName$;
  errorMessage: string;
  additionalErrorMessage: string;
  subs = new SubSink();
  isLoading$ = this.neispuoModuleQuery.selectLoading();
  error$ = this.neispuoModuleQuery.selectError();

  constructor(
    public neispuoModuleQuery: NeispuoModuleQuery,
    private nmService: NeispuoModuleService,
    private oidcService: OIDCService,
    private authQuery: AuthQuery
  ) {}

  ngOnInit() {
    this.subs.sink = this.authQuery.isLoggedIn$
      .pipe(
        filter((loggedIn) => !!loggedIn),
        take(1),
        switchMap(() => this.nmService.getCategories())
      )
      .subscribe(() => {});
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }
}
