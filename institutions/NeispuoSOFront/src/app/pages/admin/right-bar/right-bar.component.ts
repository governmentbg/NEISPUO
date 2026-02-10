import { Component, OnDestroy, OnInit } from "@angular/core";
import { Subscription } from "rxjs";
import { FieldType } from "src/app/enums/fieldType.enum";
import { FocusedElementType } from "src/app/enums/focusedElementType";
import { FieldConfig } from "src/app/models/field.interface";
import { Section } from "src/app/models/section.interface";
import { Subsection } from "src/app/models/subsection.interface";
import { Table } from "src/app/models/table.interface";
import { UpdateJsonService } from "src/app/services/update-json.service";

@Component({
  selector: "app-right-bar",
  templateUrl: "./right-bar.component.html",
  styleUrls: ["./right-bar.component.scss"]
})
export class RightBarComponent implements OnInit, OnDestroy {
  type: FocusedElementType;
  labelOnly: boolean = false;
  element;
  table: Table;

  private typeChangedSubscription: Subscription;

  constructor(private jsonService: UpdateJsonService) {}

  ngOnInit() {
    this.typeChangedSubscription = this.jsonService.typeChanged.subscribe(type => {
      this.type = type;
      this.labelOnly = (<Section | Subsection>this.jsonService.focusedElement.element).labelOnly;
      this.element = this.jsonService.focusedElement.element;
    });
  }

  ngOnDestroy() {
    this.typeChangedSubscription && this.typeChangedSubscription.unsubscribe();
  }

  showInfo() {
    return (
      (this.type === FocusedElementType.Field || this.labelOnly) &&
      (<FieldConfig>this.element).type !== FieldType.Info &&
      (<FieldConfig>this.element).type !== FieldType.Breakpoint 
    );
  }
}
