import { Component, OnDestroy, OnInit } from "@angular/core";

@Component({
  selector: "app-full-control-layout",
  templateUrl: "./full-control-layout.component.html",
  styleUrls: ["./full-control-layout.component.scss"]
})
export class FullControlLayoutComponent implements OnInit, OnDestroy {
  currentTab: string;
  isLoading: boolean;

  constructor() {}

  async ngOnInit() {}

  ngOnDestroy() {}
}
