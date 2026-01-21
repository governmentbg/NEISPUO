import { Component, OnInit } from '@angular/core';
import { UtilsService } from '@shared/services/utils/utils.service';

@Component({
  selector: 'app-dashboard-menu',
  templateUrl: './dashboard-menu.component.html',
  styleUrls: ['./dashboard-menu.component.scss']
})
export class DashboardMenuComponent implements OnInit {
  displayDashboard = false;

  constructor(private utilsService: UtilsService) { }

  ngOnInit(): void {
    this.utilsService.enableTooltip('div.dashboard-tooltip');
  }
}
