import { Component, Input, OnChanges, OnInit, SimpleChanges } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { AuthService } from "src/app/auth/auth.service";
import { HelperService } from "src/app/services/helpers.service";
import { environment } from "src/environments/environment";

@Component({
  selector: "app-address-control",
  templateUrl: "./address-control.component.html",
  styleUrls: ["./address-control.component.scss"]
})
export class AddressControlComponent implements OnInit, OnChanges {
  current: { code: number | string; label: string };
  isMonRuo: boolean = false;

  @Input() data: { code: number | string; label: string }[] = [];

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    private helperService: HelperService,
    private authService: AuthService
  ) {}

  ngOnInit() {
    this.isMonRuo = this.authService.isRuo() || this.authService.isMon();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes.data) {
      const queryParams = environment.production
        ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
        : { ...this.route.snapshot.queryParams };

      this.current = this.data ? this.data.find(address => address.code == queryParams.address) : null;
    }
  }

  onSelectionChange(addressCode: string | number) {
    let queryParams = environment.production
      ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
      : { ...this.route.snapshot.queryParams };

    queryParams["address"] = addressCode;
    environment.production && (queryParams = this.helperService.encodeParams(queryParams));

    this.router.navigate(["."], { relativeTo: this.route, queryParams });
  }
}
