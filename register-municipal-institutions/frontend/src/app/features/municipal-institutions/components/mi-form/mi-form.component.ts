import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { BaseSchoolType } from '@municipal-institutions/models/base-school-type';
import { Observable } from 'rxjs';
import { InstitutionFlexFieldComponent } from '../institution-flex-field/institution-flex-field.component';

@Component({
  selector: 'app-mi-form',
  templateUrl: './mi-form.component.html',
  styleUrls: ['./mi-form.component.scss'],
})
export class MiFormComponent implements OnInit {
  @Input() BaseSchoolTypes$: Observable<BaseSchoolType>;

  @Input() displayRIPremInstitution: boolean;

  @Input() displayRIProcedure: boolean;

  @Input() form;

  @Input() isView: boolean;

  @Input() isViewFlexField: boolean;

  @Input() displayDocument: boolean;

  @Input() displayProcedureFields: boolean;
  @Input() isBulstatLookupEnabled: boolean;
  @Input() displayBulstatMessage: boolean;
  @Input() onBulstatLoad: Function = (event) => {};
  @Input() displayWarnMessage: boolean;
  @ViewChild(InstitutionFlexFieldComponent) institutionFlexFieldComponent: InstitutionFlexFieldComponent;

  warnMessage = "Институция с въведения ЕИК не беше намерена. Моля въведете данните ръчно.";


  ngOnInit(): void {
  }

}
