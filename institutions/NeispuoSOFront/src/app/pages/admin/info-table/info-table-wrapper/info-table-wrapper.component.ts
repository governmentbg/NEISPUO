import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AuthService } from 'src/app/auth/auth.service';
import { Table } from 'src/app/models/table.interface';
import { FormDataService } from 'src/app/services/form-data.service';
import { HelperService } from 'src/app/services/helpers.service';
import { environment } from 'src/environments/environment';

interface ReportConfig {
  id: string; // e.g., "report1"
  label: string; // Tab label
  meta?: any;
  data?: any;
}

@Component({
  selector: 'app-info-table-wrapper',
  templateUrl: './info-table-wrapper.component.html',
  styleUrls: ['./info-table-wrapper.component.scss']
})

export class InfoTableWrapperComponent implements OnInit {
  reports: ReportConfig[] = [
    { id: 'reportHNY', label: 'Справка за институции преминали в нова учебна година' },
    { id: 'reportSOB', label: 'Справка за институции подали данни за Списък-образец в кампания' }
  ];

  constructor(private formDataService: FormDataService,
    private route: ActivatedRoute,
    private helperService: HelperService,
    private authService: AuthService) { }

  ngOnInit() {
    this.loadAllReports();
  }

  loadAllReports() {
    this.reports.forEach(report => {
      this.formDataService.getInstitutionReport(report.id).subscribe((res: any) => {

        report.meta = res;

        const actualSysUserId = this.authService.getSysUserId();
        const actualSysRoleId = this.authService.getSysRoleId();
        const queryParams = environment.production
          ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
          : { ...this.route.snapshot.queryParams };
        const sysuserid = queryParams.sysuserid ?? actualSysUserId;
        const sysroleid = queryParams.sysroleid ?? actualSysRoleId;

        this.formDataService.getInstitutionReportData(report.id, sysuserid, sysroleid).subscribe((data: any) => {
          const decodedData = report.id === "reportHNY" ? data.reportHNY : data.reportSOB;
          report.data = decodedData;
        });
      });
    });
  }
}
