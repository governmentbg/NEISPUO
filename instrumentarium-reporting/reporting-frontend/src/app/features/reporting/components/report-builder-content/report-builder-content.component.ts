import { Component, OnInit } from '@angular/core';
import { ReportingSidebarService } from '../reporting-sidebar/reporting-sidebar.service';

@Component({
  selector: 'app-report-builder-content',
  templateUrl: './report-builder-content.component.html',
  styleUrls: ['./report-builder-content.component.scss']
})
export class ReportBuilderContentComponent implements OnInit {
  constructor(public sidebarService: ReportingSidebarService) {}

  ngOnInit(): void {}
}
