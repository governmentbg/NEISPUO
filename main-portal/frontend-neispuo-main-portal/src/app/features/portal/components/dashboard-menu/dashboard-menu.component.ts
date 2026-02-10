import { Component, OnInit } from '@angular/core';
import { NeispuoModule } from '@portal/neispuo-modules/neispuo-module.interface';
import { NeispuoModuleQuery } from '@portal/neispuo-modules/neispuo-module.query';
import { Observable } from 'rxjs';

@Component({
  selector: 'app-dashboard-menu',
  templateUrl: './dashboard-menu.component.html',
  styleUrls: ['./dashboard-menu.component.scss']
})
export class DashboardMenuComponent implements OnInit {
  categories$ = this.neispuoModuleQuery.categories$;
  modules$: Observable<NeispuoModule[]>;

  constructor(public neispuoModuleQuery: NeispuoModuleQuery) {}

  ngOnInit(): void {}
}
