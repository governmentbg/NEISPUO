import { Component, Input, OnChanges, OnInit, SimpleChanges } from "@angular/core";
import { FocusedElementType } from "src/app/enums/focusedElementType";
import { Form } from "src/app/models/form.interface";
import { Subsection } from "src/app/models/subsection.interface";
import { UpdateJsonService } from "src/app/services/update-json.service";

@Component({
  selector: "app-center-bar",
  templateUrl: "./center-bar.component.html",
  styleUrls: ["./center-bar.component.scss"]
})
export class CenterBarComponent implements OnInit, OnChanges {
  @Input() form: Form;

  constructor(private updateJsonService: UpdateJsonService) {}

  ngOnInit() {}

  ngOnChanges(changes: SimpleChanges) {
    if (changes.form && this.form) {
      this.form.idntfr = 0;
      this.initForm(this.form);
      this.updateJsonService.focusedElement = { index: 0, element: this.form, type: FocusedElementType.Form };
      this.updateJsonService.typeChanged.next(FocusedElementType.Form);
    }
  }

  private initForm(form: Form) {
    form.sections.forEach(section => {
      section.pseudoLabel = this.transformLabel(section.label);

      section.idntfr = this.updateJsonService.counter;
      this.updateJsonService.counter++;

      !section.subsections && (section.subsections = []);

      section.subsections.forEach(sub => {
        this.initSubsectionIds(sub);
      });

      section.subsections.sort((subsection1, subsection2) => subsection1.order - subsection2.order);

      if (section.table) {
        section.table.pseudoLabel = this.transformLabel(section.table.label);
        section.table.idntfr = this.updateJsonService.counter;
        this.updateJsonService.counter++;

        section.table.fields &&
          section.table.fields.forEach(fld => {
            fld.pseudoLabel = this.transformLabel(fld.label);
            fld.idntfr = this.updateJsonService.counter;
            this.updateJsonService.counter++;
          });
      }
    });

    form.sections.sort((section1, section2) => section1.order - section2.order);
  }

  private transformLabel(label: string) {
    let newLabel: string = "";
    if (label && label.includes("if")) {
      const regExp = new RegExp(`\{([^}]+)\}`, "g");
      let match;
      while ((match = regExp.exec(label)) !== null) {
        const text = match[1].split("=")[1];
        newLabel += text.substring(1, text.length - 1) + "/";
      }
      newLabel = newLabel.substring(0, newLabel.length - 1);
    } else {
      newLabel = label;
    }
    return newLabel;
  }

  private initSubsectionIds(subsection: Subsection) {
    subsection.pseudoLabel = this.transformLabel(subsection.label);

    subsection.idntfr = this.updateJsonService.counter;

    this.updateJsonService.counter++;

    if (subsection.fields) {
      subsection.fields.forEach(fld => {
        fld.pseudoLabel = this.transformLabel(fld.label);
        fld.idntfr = this.updateJsonService.counter;

        this.updateJsonService.counter++;

        !fld.influence && (fld.influence = []);
        !fld.validations && (fld.validations = []);
      });
    } else {
      subsection.fields = [];
    }

    subsection.fields.sort((field1, field2) => field1.order - field2.order);

    if (subsection.subsections) {
      subsection.subsections.forEach(sub => {
        this.initSubsectionIds(sub);
      });
    } else {
      subsection.subsections = [];
    }

    subsection.subsections.sort((subsection1, subsection2) => subsection1.order - subsection2.order);
  }
}
