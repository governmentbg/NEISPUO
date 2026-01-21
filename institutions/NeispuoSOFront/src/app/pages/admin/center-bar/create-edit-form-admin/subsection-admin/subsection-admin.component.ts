import { Component, Input, OnInit } from "@angular/core";
import { FocusedElementType } from "src/app/enums/focusedElementType";
import { FieldConfig } from "src/app/models/field.interface";
import { Section } from "src/app/models/section.interface";
import { Subsection } from "src/app/models/subsection.interface";
import { UpdateJsonService } from "src/app/services/update-json.service";

@Component({
  selector: "app-subsection-admin",
  templateUrl: "./subsection-admin.component.html",
  styleUrls: ["./subsection-admin.component.scss"]
})
export class SubsectionAdminComponent implements OnInit {
  @Input() subsection: Subsection;
  @Input() section: Section;

  get focused() {
    return FocusedElementType;
  }

  constructor(public updateJsonService: UpdateJsonService) {}

  ngOnInit() {}

  trackByName(index, field: FieldConfig) {
    return field.name ? field.name : index;
  }

  focus(subsection: Subsection) {
    this.updateJsonService.focusedElements.push({
      index: subsection.idntfr,
      type: FocusedElementType.Subsection,
      element: subsection
    });
  }
}
