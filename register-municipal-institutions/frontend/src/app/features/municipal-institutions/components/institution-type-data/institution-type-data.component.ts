import { Component, Input, OnInit } from '@angular/core';
import { AbstractControl, ControlContainer, FormGroup } from '@angular/forms';
import { BaseSchoolType } from '@municipal-institutions/models/base-school-type';
import { CPLRAreaType } from '@municipal-institutions/models/cplr-area-type';
import { EnumBaseSchoolType } from '@procedures/models/base-school-type.enum';
import { NomenclatureQuery } from '@shared/services/common-form-service/nomenclatures-state/nomenclatures.query';
import { combineLatest, Observable } from 'rxjs';
import { filter, map, startWith } from 'rxjs/operators';

@Component({
  selector: 'app-institution-type-data',
  templateUrl: './institution-type-data.component.html',
  styleUrls: ['./institution-type-data.component.scss'],
})
export class InstitutionTypeDataComponent implements OnInit {
  public form: FormGroup | AbstractControl;

  public cplrAreaOptions: CPLRAreaType[] = [];

  @Input() FinancialSchoolTypes$ = this.nomenclatureQuery.FinancialSchoolTypes$;

  @Input() BaseSchoolTypes$ = this.nomenclatureQuery.BaseSchoolTypes$;

  @Input() BudgetingInstitutions$ = this.nomenclatureQuery.BudgetingInstitutions$;

  @Input() DetailedSchoolTypes$ = this.nomenclatureQuery.DetailedSchoolTypes$;

  @Input() CPLRAreaTypes$: any = this.nomenclatureQuery.CPLRAreaTypes$;

  selectedBaseSchoolType$: Observable<BaseSchoolType>;

  constructor(private nomenclatureQuery: NomenclatureQuery, public cc: ControlContainer) { }

  ngOnInit(): void {
    this.form = this.cc.control;
    this.setDetailedSchoolTypeFiltering();
  }

  setDetailedSchoolTypeFiltering() {
    this.DetailedSchoolTypes$ = combineLatest([
      this.nomenclatureQuery.DetailedSchoolTypes$,
      this.form.get('BaseSchoolType').valueChanges.pipe(startWith(this.form.value.BaseSchoolType)),
    ])
      .pipe(
        filter(([dsts, sbst]) => !!dsts && !!sbst),
        map(([dsts, sbst]) => dsts.filter((dst) => dst.BaseSchoolType.BaseSchoolTypeID === sbst.BaseSchoolTypeID)),
      );
  }

  setDetailedCPLRAreaTypeFiltering() {
    this.CPLRAreaTypes$ = combineLatest([
      this.nomenclatureQuery.CPLRAreaTypes$,
      this.form.get('BaseSchoolType').valueChanges.pipe(startWith(this.form.value.BaseSchoolType)),
    ]).pipe(
      filter(([cplrs, sbst]) => !!cplrs && !!sbst),
      map(([cplrs, sbst]) => {
        if (sbst.BaseSchoolTypeID === EnumBaseSchoolType.PERSONAL_DEVELOPMENT_CENTRE) {
          return cplrs;
        }
        this.form.get('CPLRAreaType').reset();
      }),
    ).subscribe((options) => this.cplrAreaOptions = options);
  }

  isCplrAreaHidden(): boolean {
    return this.form.get('BaseSchoolType').value?.BaseSchoolTypeID === EnumBaseSchoolType.PERSONAL_DEVELOPMENT_CENTRE;
  }
}
