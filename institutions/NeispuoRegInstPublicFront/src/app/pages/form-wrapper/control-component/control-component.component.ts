import { Location } from "@angular/common";
import { Component, EventEmitter, OnInit, Output } from "@angular/core";
import { Menu } from "../../../enums/menu.enum";

@Component({
  selector: "app-control-component",
  templateUrl: "./control-component.component.html",
  styleUrls: ["./control-component.component.scss"]
})
export class ControlComponentComponent implements OnInit {
  historyMode = false;
  @Output() isHistoryActive: EventEmitter<boolean> = new EventEmitter<boolean>();

  get menu() {
    return Menu;
  }

  constructor(private location: Location) {}

  ngOnInit() {}

  historyChanged() {
    this.historyMode = !this.historyMode;
    this.isHistoryActive.emit(this.historyMode);
  }

  goBack() {
    this.location.back();
  }
}
