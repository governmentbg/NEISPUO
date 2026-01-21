import { Component, Input, OnChanges, OnInit, SimpleChanges } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { AuthService } from "src/app/auth/auth.service";
import { HelperService } from "src/app/services/helpers.service";
import { environment } from "src/environments/environment";

@Component({
  selector: "app-period-filters",
  templateUrl: "./period-filters.component.html",
  styleUrls: ["./period-filters.component.scss"]
})
export class PeriodFiltersComponent implements OnInit, OnChanges {
  year: { code: number | string; label: string };
  period: { code: number | string; label: string };
  isMonRuo: boolean = false;

  @Input() years: { code: number | string; label: string }[] = [];
  @Input() periods: { code: number | string; label: string }[] = [];

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private helperService: HelperService,
    private authService: AuthService
  ) {}

  ngOnInit() {
    this.isMonRuo = this.authService.isMon() || this.authService.isRuo();
  }

  ngOnChanges(changes: SimpleChanges) {
    const queryParams = environment.production
      ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
      : { ...this.route.snapshot.queryParams };

    if (changes.years) {
      this.year = this.years ? this.years.find(year => year.code == queryParams.year) : null;
    }

    if (changes.periods) {
      this.period = this.periods ? this.periods.find(period => period.code == queryParams.period) : null;
    }
  }

  onYearChange(yearCode: string | number) {
    let queryParams = environment.production
      ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
      : { ...this.route.snapshot.queryParams };

    queryParams["year"] = yearCode;
    environment.production && (queryParams = this.helperService.encodeParams(queryParams));

    this.router.navigate(["."], { relativeTo: this.route, queryParams });
  }

  onPeriodChange(periodCode: string | number) {
    let queryParams = environment.production
      ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
      : { ...this.route.snapshot.queryParams };

    queryParams["period"] = periodCode;
    environment.production && (queryParams = this.helperService.encodeParams(queryParams));

    this.router.navigate(["."], { relativeTo: this.route, queryParams });
  }
}
