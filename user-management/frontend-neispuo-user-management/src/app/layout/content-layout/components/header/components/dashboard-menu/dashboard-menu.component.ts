import { Component } from '@angular/core';
import { CONSTANTS } from '@shared/constants';
import { DashboardMenuConfig } from 'src/app/configs/dashboard-menu.config';

@Component({
    selector: 'app-dashboard-menu',
    templateUrl: './dashboard-menu.component.html',
    styleUrls: ['./dashboard-menu.component.scss'],
})
export class DashboardMenuComponent {
    public environment = this.dashBoardConfig.environment;

    public CONSTANTS = CONSTANTS;

    constructor(private dashBoardConfig: DashboardMenuConfig) {}
}
