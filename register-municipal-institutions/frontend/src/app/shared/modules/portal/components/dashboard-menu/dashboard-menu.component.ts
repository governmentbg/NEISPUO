import { Component, OnInit } from '@angular/core';
import { EnvironmentService } from '@core/services/environment.service';
import { NeispuoModule } from '@portal/neispuo-modules/neispuo-module.interface';
import { NeispuoModuleQuery } from '@portal/neispuo-modules/neispuo-module.query';
import { Observable } from 'rxjs';
import { UtilsService } from '@shared/services/utils/utils.service';
@Component({
  selector: 'app-dashboard-menu',
  templateUrl: './dashboard-menu.component.html',
  styleUrls: ['./dashboard-menu.component.scss'],
})
export class DashboardMenuComponent implements OnInit {
  categories$ = this.neispuoModuleQuery.categories$;

  modules$: Observable<NeispuoModule[]>;

  public readonly environment;

  constructor(public neispuoModuleQuery: NeispuoModuleQuery, private envService: EnvironmentService, private utilsService: UtilsService) {
    this.environment = this.envService.environment;
  }

  ngOnInit(): void { 
    this.utilsService.enableTooltip('div.dashboard-tooltip');
  }
}
