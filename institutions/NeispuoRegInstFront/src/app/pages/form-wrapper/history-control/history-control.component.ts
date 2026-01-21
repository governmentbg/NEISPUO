import { Component, EventEmitter, Input, OnChanges, Output, SimpleChanges, ViewChild } from "@angular/core";
import { MatSelect } from "@angular/material/select";
import { Option } from "../../../models/option.interface";

@Component({
  selector: "app-history-control",
  templateUrl: "./history-control.component.html",
  styleUrls: ["./history-control.component.scss"]
})
export class HistoryControlComponent implements OnChanges {
  @Input() history: Option[];
  @Input() currentOption: number;

  @Output() versionChanged: EventEmitter<number> = new EventEmitter<number>();

  @ViewChild("historySelect") historySelect: MatSelect;

  currentPos: number;

  constructor() {}

  ngOnChanges(changes: SimpleChanges): void {
    if (changes && changes.currentOption) {
      this.currentPos = this.history.findIndex(option => option.code === this.currentOption);
    }
  }

  onSelectionChange(value: number | string) {
    this.currentOption = +value;
    this.versionChanged.next(this.currentOption);
    this.historySelect.value = value;
  }

  goTo(position: number) {
    this.onSelectionChange(this.history[position].code);
  }
}
