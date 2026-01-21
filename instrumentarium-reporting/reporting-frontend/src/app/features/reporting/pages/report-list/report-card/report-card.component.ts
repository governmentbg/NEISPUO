import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'app-report-card',
  templateUrl: './report-card.component.html',
  styleUrls: ['./report-card.component.scss']
})
export class ReportCardComponent implements OnInit {
  @Input() report;

  constructor(private router: Router, private route: ActivatedRoute) {}

  ngOnInit(): void {}

  goToReport() {
    this.router.navigate([`report-builder/${this.report.name}`], { relativeTo: this.route });
  }
}
