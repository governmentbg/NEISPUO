import { Component, Input, OnInit } from "@angular/core";
import { Router } from "@angular/router";
import { MenuInt } from "../../../enums/menu.enum";
import { KeepFiltersService } from "../../../services/keep-filters.service";

@Component({
  selector: "app-main-tab",
  templateUrl: "./main-tab.component.html",
  styleUrls: ["./main-tab.component.scss"]
})
export class MainTabComponent implements OnInit {
  tabIndex: number;
  @Input() path: string;

  constructor(private router: Router, private keepFiltersService: KeepFiltersService) {}

  ngOnInit() {
    this.tabIndex = MenuInt[this.path];
  }

  onPathChanged(index: number) {
    if (index !== this.tabIndex) {
      this.tabIndex = index;
      let path = `/${MenuInt[index]}`;
      this.router.navigate([path]).then(res => {
        if (res) {
          this.keepFiltersService.table = null;
          this.keepFiltersService.filters = null;
          this.keepFiltersService.keyWord = "";
          this.keepFiltersService.sortDirs = [];
          this.keepFiltersService.sortParams = [];
        }
      });
    }
  }
}
