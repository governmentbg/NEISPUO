import { Component, EventEmitter, Input, OnInit, Output } from "@angular/core";
import { FormArray, FormGroup } from "@angular/forms";
import { Mode } from "../../../../enums/mode.enum";
import { FieldConfig } from "../../../../models/field.interface";
import { Influence } from "../../../../models/influence.interface";
import { Subsection } from "../../../../models/subsection.interface";
import { FormDataService } from "../../../../services/form-data.service";

@Component({
  selector: "app-subsection",
  templateUrl: "./subsection.component.html",
  styleUrls: ["./subsection.component.scss"]
})
export class SubsectionComponent implements OnInit {
  @Input() subsection: Subsection;
  @Input() formGroup: FormGroup;
  @Input() mode: Mode;
  @Input() procId: number;

  @Output() sectionChanged: EventEmitter<{ subsection: Subsection; action: string }> = new EventEmitter<{
    subsection: Subsection;
    action: string;
  }>();

  @Output() influenceChanged: EventEmitter<{ value: string; influence: Influence }> = new EventEmitter<{
    value: string;
    influence: Influence;
  }>();

  constructor(private formDataService: FormDataService) {}

  get modes() {
    return Mode;
  }

  ngOnInit() {
    if (!this.mode) {
      this.mode = Mode.View;
    }
  }

  getFormGroup(subsection: Subsection) {
    let id = "";
    let formGroup = this.formGroup;
    if (subsection.canDuplicate !== undefined && subsection.canDuplicate !== null) {
      const formArr = <FormArray>this.formGroup.get(subsection.name);
      const parentGroup = formArr.controls.find((control: FormGroup) => {
        const key = Object.keys(control.controls)[0];
        const matches = key === subsection.id + "";
        matches && (id = key);
        return matches;
      });

      if (parentGroup) {
        formGroup = <FormGroup>parentGroup.get(id);
      }
    }
    return formGroup;
  }

  trackByName(index, field: FieldConfig) {
    return field.name ? field.name : index;
  }

  hasNonHiddenField(sub: Subsection) {
    return this.formDataService.hasNonHiddenField(sub);
  }
}
