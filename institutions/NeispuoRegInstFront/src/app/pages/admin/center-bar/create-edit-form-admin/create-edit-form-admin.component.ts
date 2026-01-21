import { ChangeDetectorRef, Component, Input, OnInit } from "@angular/core";
import { FocusedElementType } from "src/app/enums/focusedElementType";
import { Form } from "src/app/models/form.interface";
import { Section } from "src/app/models/section.interface";
import { Subsection } from "src/app/models/subsection.interface";
import { UpdateJsonService } from "src/app/services/update-json.service";

@Component({
  selector: "app-create-edit-form-admin",
  templateUrl: "./create-edit-form-admin.component.html",
  styleUrls: ["./create-edit-form-admin.component.scss"]
})
export class CreateEditFormAdminComponent implements OnInit {
  @Input() form: Form;

  constructor(private cdr: ChangeDetectorRef, public updateJsonService: UpdateJsonService) {}

  ngOnInit() {}

  ngAfterContentChecked() {
    this.cdr.detectChanges();
  }

  focus(section: Section) {
    this.updateJsonService.focusedElements.push({
      index: section.idntfr,
      type: FocusedElementType.Section,
      element: section
    });
  }

  formFocus() {
    this.updateJsonService.focusedElements.push({
      index: this.form.idntfr,
      type: FocusedElementType.Form,
      element: this.form
    });

    const focusedElement = this.updateJsonService.focusedElements[0];
    this.updateJsonService.focusedElement = focusedElement;
    this.updateJsonService.typeChanged.next(focusedElement.type);
    this.updateJsonService.focusedElements = [];
  }

  hasRenderedField(subsection: Subsection) {
    for (const fld of subsection.fields) {
      if (fld.rendered !== false) {
        return true;
      }
    }

    for (const sub of subsection.subsections) {
      if (this.hasRenderedField(sub)) {
        return true;
      }
    }

    return false;
  }

  hasRenderedElement(section: Section) {
    if (!section.table && (!section.subsections || !section.subsections.length) && section.label) {
      return true;
    }

    if (section.table && section.table.rendered !== false) {
      return true;
    }

    for (const sub of section.subsections) {
      if (this.hasRenderedField(sub)) {
        return true;
      }
    }

    return false;
  }
}
