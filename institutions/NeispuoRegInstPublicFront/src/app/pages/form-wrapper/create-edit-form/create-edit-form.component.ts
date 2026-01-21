import {
  AfterContentChecked,
  ChangeDetectorRef,
  Component,
  Input,
  OnChanges,
  OnInit,
  SimpleChanges
} from "@angular/core";
import { FormGroup, FormBuilder, FormArray } from "@angular/forms";
import { FormDataService } from "../../../services/form-data.service";
import { Form } from "../../../models/form.interface";
import { Subsection } from "../../../models/subsection.interface";
import { Mode } from "../../../enums/mode.enum";
import { Menu } from "../../../enums/menu.enum";
import { Location } from "@angular/common";

@Component({
  exportAs: "createEditForm",
  selector: "app-create-edit-form",
  templateUrl: "./create-edit-form.component.html",
  styleUrls: ["./create-edit-form.component.scss"]
})
export class CreateEditFormComponent implements OnInit, OnChanges, AfterContentChecked {
  @Input() form: Form;
  @Input() validate: boolean;
  @Input() isHistoryActive: boolean = false;
  @Input() mode: Mode;
  @Input() historyPosition: number;
  @Input() procId: number;

  formGroup: FormGroup;

  get value() {
    if (!this.formGroup) return {};
    return this.formGroup.value;
  }

  get modes() {
    return Mode;
  }

  get menu() {
    return Menu;
  }

  constructor(
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef,
    private formDataService: FormDataService,
    private location: Location
  ) {}

  ngOnInit() {
    if (!this.mode) {
      this.mode = Mode.View;
    }
  }

  ngAfterContentChecked() {
    this.cdr.detectChanges();
  }

  ngOnChanges(changes: SimpleChanges) {
    if (changes.form) {
      this.formGroup = this.createFormGroup();
    }

    if (changes.validate && changes.validate.currentValue) {
      this.formDataService.validateAllFormFields(this.formGroup);
    }
  }

  private createFormGroup() {
    const group = this.fb.group({});
    if (this.form) {
      this.form.sections.forEach(section => {
        section.subsections.forEach(subsection => {
          this.formDataService.createSubsectionGroup(subsection, group);
        });
      });
    }

    return group;
  }

  getFormGroup(subsection: Subsection) {
    let id = "";
    let formGroup = this.formGroup;
    if (subsection.canDuplicate !== undefined && subsection.canDuplicate !== null) {
      const formArr = <FormArray>this.formGroup.get(subsection.name);
      const parentGroup = formArr.controls.find((control: FormGroup) => {
        const key = Object.keys(control.controls)[0];
        const matches = key == subsection.id;
        matches && (id = key);
        return matches;
      });

      if (parentGroup) {
        formGroup = <FormGroup>parentGroup.get(id);
      }
    }
    return formGroup;
  }

  goBack() {
    this.location.back();
  }

  hasNonHiddenField(sub: Subsection) {
    return this.formDataService.hasNonHiddenField(sub);
  }
}
