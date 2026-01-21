import { Component, OnInit } from '@angular/core';
import { EnvironmentService } from '@shared/services/environment.service';

@Component({
  selector: 'app-dashboard-menu',
  templateUrl: './dashboard-menu.component.html',
  styleUrls: ['./dashboard-menu.component.scss']
})
export class DashboardMenuComponent implements OnInit {
  public readonly environment: any;

  constructor(public envService: EnvironmentService) {
    this.environment = this.envService.environment;
  }

  ngOnInit(): void {}
}
