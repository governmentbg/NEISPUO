import { Component, OnDestroy, OnInit } from "@angular/core";
import { Subscription } from "rxjs";
import { FocusedElementType } from "src/app/enums/focusedElementType";
import { Table } from "src/app/models/table.interface";
import { UpdateJsonService } from "src/app/services/update-json.service";

@Component({
  selector: "app-right-bar",
  templateUrl: "./right-bar.component.html",
  styleUrls: ["./right-bar.component.scss"]
})
export class RightBarComponent implements OnInit, OnDestroy {
  type: FocusedElementType;
  element;
  table: Table;

  private typeChangedSubscription: Subscription;

  constructor(private jsonService: UpdateJsonService) {}

  ngOnInit() {
    this.typeChangedSubscription = this.jsonService.typeChanged.subscribe(type => {
      this.type = type;
      this.element = this.jsonService.focusedElement.element;
    });
  }

  ngOnDestroy() {
    this.typeChangedSubscription && this.typeChangedSubscription.unsubscribe();
  }

  showInfo() {
    return this.type === FocusedElementType.Field;
  }
}
