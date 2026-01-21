import { EventEmitter, OnChanges, SimpleChanges } from "@angular/core";
import { Component, Input, OnInit, Output } from "@angular/core";

@Component({
  selector: "app-left-bar",
  templateUrl: "./left-bar.component.html",
  styleUrls: ["./left-bar.component.scss"]
})
export class LeftBarComponent implements OnInit, OnChanges {
  @Input() jsonLs: { name: string; label: string }[] = [];

  @Output() jsonChanged: EventEmitter<string> = new EventEmitter<string>();

  filteredList: { name: string; label: string }[] = [];
  jsonSelected;

  constructor() {}

  ngOnInit() {}

  ngOnChanges(changes: SimpleChanges) {
    if (changes.jsonLs && this.jsonLs && this.jsonLs.length > 0) {
      this.filteredList = this.jsonLs;
    }
  }

  onJsonChange(jsonName: string) {
    this.jsonSelected = jsonName;
    this.jsonChanged.next(jsonName);
  }

  getList(event) {
    if (!event.target.value) {
      this.filteredList = this.jsonLs;
    } else {
      this.filteredList = [];
      this.jsonLs.forEach(
        elem => elem.label.toLowerCase().includes(event.target.value.toLowerCase()) && this.filteredList.push(elem)
      );
    }
  }
}
