import { Component, Input, OnInit } from '@angular/core';
import { ReportingSidebarService } from '../reporting-sidebar/reporting-sidebar.service';

@Component({
  selector: 'app-settings',
  templateUrl: './settings.component.html',
  styleUrls: ['./settings.component.scss']
})
export class SettingsComponent implements OnInit {
  constructor(public sidebarService: ReportingSidebarService) {}

  ngOnInit(): void {
    this.sidebarService.change.subscribe((v) => {
      this.sidebarService.isSidebarOpen = v.isSidebarOpen;
      this.sidebarService.visualization = v.visualization;
      this.sidebarService.options = v.options;
    });
  }
}
