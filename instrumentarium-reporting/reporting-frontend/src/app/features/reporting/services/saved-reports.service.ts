import { Injectable } from '@angular/core';
import { Report } from '@core/models/report.model';
import { ApiService } from '@core/services/api.service';
import { RoleEnum } from '@shared/enums/role.enum';
import { BehaviorSubject, Observable, tap } from 'rxjs';

@Injectable({ providedIn: 'root' })
export class SavedReportsService {
  private _savedReport = new BehaviorSubject<Report>(null);
  public savedReport$ = this._savedReport.asObservable();

  get savedReport() {
    return this._savedReport.getValue();
  }

  set savedReport(response) {
    this._savedReport.next(response);
  }

  constructor(private apiService: ApiService) {}

  public getReport(reportSection: string, reportId: number): Observable<Report> {
    return this.apiService
      .get<Report>(`/v1/${reportSection}/${reportId}`)
      .pipe(tap((response) => this._savedReport.next(response)));
  }

  public createReport(report: Report): Observable<Report> {
    return this.apiService
      .post<Report>('/v1/saved-reports', report)
      .pipe(tap((response) => this._savedReport.next(response)));
  }

  public updateReport(reportId: number, report: any): Observable<Report> {
    return this.apiService
      .put<Report>(`/v1/saved-reports/${reportId}`, report)
      .pipe(tap((response) => this._savedReport.next(response)));
  }

  public deleteReport(reportId: number) {
    return this.apiService.delete(`/v1/saved-reports/${reportId}`).pipe(tap((response) => this._savedReport.next(null)));
  }

  public scopeRolesToShareWith(currentUserRole: RoleEnum) {
    if (
      [
        RoleEnum.MON_ADMIN,
        RoleEnum.MON_CHRAO,
        RoleEnum.MON_EXPERT,
        RoleEnum.MON_OBGUM,
        RoleEnum.MON_OBGUM_FINANCES,
        RoleEnum.CIOO
      ].includes(currentUserRole)
    ) {
      return [
        RoleEnum.MON_ADMIN,
        RoleEnum.MON_EXPERT,
        RoleEnum.MON_CHRAO,
        RoleEnum.MON_OBGUM,
        RoleEnum.MON_OBGUM_FINANCES,
        RoleEnum.CIOO,
        RoleEnum.RUO,
        RoleEnum.RUO_EXPERT,
        RoleEnum.MUNICIPALITY,
        RoleEnum.INSTITUTION,
        RoleEnum.BUDGETING_INSTITUTION
      ];
    }
    //temporary disabled
    // else if (currentUserRole === RoleEnum.RUO || currentUserRole === RoleEnum.RUO_EXPERT) {
    //   return [RoleEnum.RUO, RoleEnum.MUNICIPALITY, RoleEnum.INSTITUTION];
    // } else if (currentUserRole === RoleEnum.MUNICIPALITY) {
    //   return [RoleEnum.MUNICIPALITY, RoleEnum.INSTITUTION];
    // }
    return [];
  }

  cleanUpData() {
    this.savedReport = null;
  }
}
