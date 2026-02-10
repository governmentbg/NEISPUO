import { CdkVirtualScrollViewport } from "@angular/cdk/scrolling";
import { Component, ElementRef, Input, OnInit, ViewChild } from "@angular/core";
import { FormControl } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { map, startWith } from "rxjs/operators";
import { HelperService } from "src/app/services/helpers.service";
import { environment } from "../../../../../environments/environment";

@Component({
  selector: "app-switch-control",
  templateUrl: "./switch-control.component.html",
  styleUrls: ["./switch-control.component.scss"]
})
export class SwitchControlComponent implements OnInit {
  isOpen = false;
  height: string;

  @Input() data: { code: number | string; label: string; additionalParams: Object }[] = [];
  @Input() current: { code: number | string; label: string; additionalParams: Object };
  @Input() switchLabel: string;
  @Input() paramName: string;

  @ViewChild("autocompleteInput", { static: false }) input: ElementRef;
  @ViewChild(CdkVirtualScrollViewport) scroll: CdkVirtualScrollViewport;

  myControl = new FormControl();
  filteredOptions: { code: number | string; label: string }[];

  constructor(private router: Router, private route: ActivatedRoute, private helperService: HelperService) {}

  ngOnInit() {
    this.myControl.valueChanges
      .pipe(
        startWith(""),
        map(value => {
          value = typeof value === "string" ? value : value.label;

          this.filteredOptions = value ? this._filter(value) : this.data.slice();

          if (this.filteredOptions.length < 4) {
            this.height = this.filteredOptions.length * 50 + "px";
          } else {
            this.height = "200px";
          }
        })
      )
      .subscribe();
  }

  optionSelected(event) {
    const currentOption = event.option.value;
    this.current = currentOption;
    this.isOpen = false;
    this.myControl.setValue("");

    let queryParams = environment.production
      ? this.helperService.decodeParams(this.route.snapshot.queryParams["q"])
      : { ...this.route.snapshot.queryParams };

    queryParams[this.paramName] = currentOption.code;
    queryParams = { ...queryParams, ...currentOption.additionalParams };
    environment.production && (queryParams = this.helperService.encodeParams(queryParams));

    this.router.navigate(["."], { relativeTo: this.route, queryParams });
  }

  displayFn(val: { code: number | string; label: string }): string {
    return val && val.label ? val.label : "";
  }

  openAutocomplete() {
    this.isOpen = !this.isOpen;

    if (this.isOpen) {
      setTimeout(() => {
        this.input.nativeElement.focus();
        const index = this.data.findIndex(option => option.code === this.current.code);
        this.scroll.scrollToIndex(index);
      }, 0);
    }
  }

  private _filter(value: string): { code: number | string; label: string }[] {
    const filterValue = value.toLowerCase();

    return this.data.filter(option => option.label.toLowerCase().indexOf(filterValue) != -1);
  }
}
