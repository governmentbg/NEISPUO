import {
  AfterViewInit, ChangeDetectorRef, Component, Input, OnDestroy, OnInit, ViewChild,
} from '@angular/core';
import { FormGroup } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthQuery } from '@core/authentication/auth-state-manager/auth.query';
import { MiFormComponent } from '@municipal-institutions/components/mi-form/mi-form.component';
import { BaseSchoolType } from '@municipal-institutions/models/base-school-type';
import { RIProcedure } from '@municipal-institutions/models/ri-procedure';
import { ACLMunicipalityInstitutionService } from '@municipal-institutions/services/acl-mi.service';
import { MICommonFormGroupService } from '@municipal-institutions/services/mi-common-form-group.service';
import { IMIFormComponentState } from '@municipal-institutions/services/mi-form-component-state.interface';
import { MiFormUtility } from '@municipal-institutions/services/mi-form-utility';
import { IStatePage } from '@municipal-institutions/services/state-page.inteface';
import { MunicipalInstitution } from '@municipal-institutions/state/municipal-institutions/municipal-institution.interface';
import { MunicipalInstitutionQuery } from '@municipal-institutions/state/municipal-institutions/municipal-institution.query';
import { MunicipalInstitutionService } from '@municipal-institutions/state/municipal-institutions/municipal-institution.service';
import { ProcedureTypeEnum } from '@procedures/models/procedure-type.enum';
import { TransformTypeEnum } from '@procedures/models/transform-type.enum';
import { NomenclatureQuery } from '@shared/services/common-form-service/nomenclatures-state/nomenclatures.query';
import { UtilsService } from '@shared/services/utils/utils.service';
import { MessageService } from 'primeng/api';
import { Observable } from 'rxjs';
import { concatMap, filter, map } from 'rxjs/operators';
import { ScrollService } from '@shared/services/utils/scroll.service';
import { Location } from '@angular/common';
import { EnumBaseSchoolType } from '@procedures/models/base-school-type.enum';

@Component({
  selector: 'app-mi-edit-existing',
  templateUrl: './mi-edit-existing.component.html',
  styleUrls: ['./mi-edit-existing.component.scss'],
})
export class MiEditExistingComponent extends MiFormUtility implements OnInit, AfterViewInit, IStatePage, OnDestroy {
  @ViewChild(MiFormComponent) miFormComponent: MiFormComponent;

  form: FormGroup;

  config: IMIFormComponentState = {} as IMIFormComponentState;

  submitLabel = 'Създай';

  BaseSchoolTypes$: Observable<BaseSchoolType[]>;

  _resp: MunicipalInstitution = null;

  @Input() set resp(v: MunicipalInstitution) {
    if (!v || v === this.resp) {
      return;
    }
    this._resp = v;
  }

  get resp() {
    return this._resp;
  }

  constructor(
    public miQuery: MunicipalInstitutionQuery,
    private miService: MunicipalInstitutionService,
    private cfgs: MICommonFormGroupService,
    private utilsService: UtilsService,
    private messageService: MessageService,
    private nomenclatureQuery: NomenclatureQuery,
    private router: Router,
    private miAclService: ACLMunicipalityInstitutionService,
    nmQuery: NomenclatureQuery,
    authQuery: AuthQuery,
    private cdr: ChangeDetectorRef,
    private scrollService: ScrollService,
    private _location: Location,
  ) {
    super(nmQuery, authQuery);
  }

  ngAfterViewInit(): void {
    this.BaseSchoolTypes$ = this.nomenclatureQuery.BaseSchoolTypes$.pipe(
      filter((baseSchoolTypes) => !!baseSchoolTypes),
      map((baseSchoolTypes) => baseSchoolTypes.filter((bst) => bst.BaseSchoolTypeID === EnumBaseSchoolType.KINDERGARTEN || bst.BaseSchoolTypeID === EnumBaseSchoolType.PERSONAL_DEVELOPMENT_CENTRE)),
    );

    this.presetProcedureAndTransformType(ProcedureTypeEnum.UPDATE, TransformTypeEnum.CHANGE);
    this.cdr.detectChanges();
  }

  ngOnInit() {
    this.setFormConfig();
    this.form = this.cfgs.buildForm({
      config: this.config,
      response: this.resp,
    });
    this.subs.sink = this.miQuery.selectedRIInstitutionID$
      .pipe(
        concatMap((id) => this.miService.getOne(`/v1/ri-institution/${id}`)),
        concatMap(() => this.miQuery.selectFirst$),
      ).subscribe(
        (mi) => {
          const riProcedure = {
            ProcedureType: {
              ProcedureTypeID: ProcedureTypeEnum.UPDATE,
            },
            RICPLRArea: {
              CPLRAreaType: mi?.RIProcedure?.RICPLRArea?.CPLRAreaType
            },
            TransformType: {
              TransformTypeID: mi?.RIProcedure?.TransformType?.TransformTypeID
            }
          } as RIProcedure;
          this.resp = { ...mi, RIProcedure: riProcedure, };
          this.miFormComponent.institutionFlexFieldComponent.prePopulateFormArray(this.resp.RIFlexFieldValues);
          this.form.patchValue(
            this.cfgs.buildForm({
              config: this.config,
              response: this.resp,
            }).getRawValue(),
          );
        },
        (err) => {
          console.log(err);
        },
      );
  }

  setFormConfig() {
    this.config = this.miAclService.getConfiguration('EDIT');
  }

  updateMI = (e: any) => {
    this.utilsService.markAllAsDirty(this.form);

    if (this.form.invalid) {
      this.scrollService.scrollToFirstError();
      return;
    }

    const riInstitutionObj = this.form.getRawValue();

    this.miService.updateOne(`/v1/ri-institution/${this.resp.RIInstitutionID}`, {
      ...riInstitutionObj,
    }).subscribe(
      (s) => {
        this.messageService.add({ severity: 'success', summary: 'Успех', detail: 'Успешна промяна на институция.' });
        this.router.navigateByUrl(`/municipal-institutions/preview/${s.RIInstitutionID}`);
      },
      // eslint-disable-next-line @typescript-eslint/no-shadow
      (e) => {
        console.error(e);
        this.messageService.add({ severity: 'error', summary: 'Грешка', detail: 'Възникна грешка при промяна на институция.' });
      },
    );
  };

  navigateBackward = (e: any) => {
    this._location.back();
  };

  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }
}
