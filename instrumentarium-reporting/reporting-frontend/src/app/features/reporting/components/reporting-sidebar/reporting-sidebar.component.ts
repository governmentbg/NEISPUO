import { Component, OnInit } from '@angular/core';
import { ReportingSidebarService } from './reporting-sidebar.service';

@Component({
  selector: 'app-reporting-sidebar',
  templateUrl: './reporting-sidebar.component.html',
  styleUrls: ['./reporting-sidebar.component.scss']
})
export class ReportingSidebarComponent implements OnInit {
  constructor(public sidebarService: ReportingSidebarService) {}

  ngOnInit(): void {
    this.sidebarService.change.subscribe((v) => {
      this.sidebarService.isSidebarOpen = v.isSidebarOpen;
      this.sidebarService.visualization = v.visualization;
      this.sidebarService.options = v.options;
    });
  }
}
