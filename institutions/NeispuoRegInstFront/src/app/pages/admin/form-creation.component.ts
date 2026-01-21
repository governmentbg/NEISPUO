import { Component, OnInit } from "@angular/core";
import { Form } from "src/app/models/form.interface";
import { Subsection } from "src/app/models/subsection.interface";
import { MessagesService } from "src/app/services/messages.service";
import { SnackbarService } from "src/app/services/snackbar.service";
import { UpdateJsonService } from "src/app/services/update-json.service";

@Component({
  selector: "app-form-creation",
  templateUrl: "./form-creation.component.html",
  styleUrls: ["./form-creation.component.scss"]
})
export class FormCreationComponent implements OnInit {
  isLoading = false;
  form: Form;

  private currentForm: string = "institution";

  constructor(
    private updateJsonService: UpdateJsonService,
    private snackbarService: SnackbarService,
    private msgService: MessagesService
  ) {}

  ngOnInit() {
    this.loadForm();
  }

  loadForm() {
    this.isLoading = true;
    this.updateJsonService.getJson().subscribe((res: Form) => {
      this.form = res;
      this.isLoading = false;
    });
  }

  saveFormChanges() {
    const form = this.transformForm();
    this.updateJsonService.updateJson(this.currentForm, form).subscribe(
      res => this.snackbarService.openSuccessSnackbar(this.msgService.successMessages.saveSuccess),
      err => this.snackbarService.openErrorSnackbar(this.msgService.errorMessages.error)
    );
  }

  private transformForm() {
    const form = JSON.parse(JSON.stringify(this.form)); //copy
    delete form.idntfr;

    form.sections &&
      form.sections.forEach(section => {
        delete section.idntfr;
        delete section.pseudoLabel;

        section.subsections && !section.subsections.length && delete section.subsections;

        section.subsections &&
          section.subsections.forEach(subsection => {
            this.transformSubsection(subsection);
          });

        if (section.table) {
          delete section.table.idntfr;
          delete section.table.values;
          delete section.table.pseudoLabel;

          section.table.fields &&
            section.table.fields.forEach(fld => {
              delete fld.idntfr;
              delete fld.value;
              delete fld.pseudoLabel;
            });
        }
      });

    return form;
  }

  private transformSubsection(subsection: Subsection) {
    delete subsection.id;
    delete subsection.idntfr;
    delete subsection.pseudoLabel;

    subsection.fields && !subsection.fields.length && delete subsection.fields;
    subsection.subsections && !subsection.subsections.length && delete subsection.subsections;

    subsection.fields &&
      subsection.fields.forEach(field => {
        delete field.idntfr;
        delete field.value;
        delete field.pseudoLabel;
      });

    subsection.subsections &&
      subsection.subsections.forEach(sub => {
        this.transformSubsection(sub);
      });
  }
}
