import {
  AfterViewInit, ChangeDetectorRef, Component, Input, OnDestroy, OnInit, ViewChild,
} from '@angular/core';
import { FormGroup } from '@angular/forms';
import { AuthQuery } from '@core/authentication/auth-state-manager/auth.query';
import { MiFormComponent } from '@municipal-institutions/components/mi-form/mi-form.component';
import { ACLMunicipalityInstitutionService } from '@municipal-institutions/services/acl-mi.service';
import { MICommonFormGroupService } from '@municipal-institutions/services/mi-common-form-group.service';
import { IMIFormComponentState } from '@municipal-institutions/services/mi-form-component-state.interface';
import { MiFormUtility } from '@municipal-institutions/services/mi-form-utility';
import { IStatePage } from '@municipal-institutions/services/state-page.inteface';
import { MunicipalInstitution } from '@municipal-institutions/state/municipal-institutions/municipal-institution.interface';
import { MunicipalInstitutionQuery } from '@municipal-institutions/state/municipal-institutions/municipal-institution.query';
import { NomenclatureQuery } from '@shared/services/common-form-service/nomenclatures-state/nomenclatures.query';

@Component({
  selector: 'app-join-mi-preview-delete',
  templateUrl: './join-mi-preview-delete.component.html',
  styleUrls: ['./join-mi-preview-delete.component.scss'],
})
export class JoinMiPreviewDeleteComponent extends MiFormUtility implements OnInit, AfterViewInit, IStatePage, OnDestroy {
  /**
   * Set default mode
   */
  @Input() displayRIPremInstitution: boolean;

  @Input() displayProcedureFields: boolean;

  @Input() displayDocument: boolean;

  @ViewChild(MiFormComponent) miFormComponent: MiFormComponent;

  form: FormGroup;

  config: IMIFormComponentState = {} as IMIFormComponentState;

  BaseSchoolTypes$ = this.nmQuery.BaseSchoolTypes$;

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
    private cfgs: MICommonFormGroupService,
    private miAclService: ACLMunicipalityInstitutionService,
    nmQuery: NomenclatureQuery,
    authQuery: AuthQuery,
    private cdr: ChangeDetectorRef,

  ) {
    super(nmQuery, authQuery);
  }

  ngAfterViewInit() {
    this.form.setValue(
      this.cfgs
        .buildForm({
          config: this.config,
          response: this.resp,
        })
        .getRawValue(),
    );
    const RIProcedureForm = this.form.get('RIProcedure') as FormGroup;
    RIProcedureForm.removeControl('RIDocument');
    this.miFormComponent.displayRIPremInstitution = this.displayRIPremInstitution;
    this.miFormComponent.institutionFlexFieldComponent.prePopulateFormArray(this.resp.RIFlexFieldValues);
    this.disableRIFlexFieldArray(this.form);
    this.miFormComponent.institutionFlexFieldComponent.disableAddition = true;
    this.miFormComponent.institutionFlexFieldComponent.disableDeletion = true;
    this.cdr.detectChanges();
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }

  ngOnInit() {
    this.setFormConfig();
    this.form = this.cfgs.buildForm({
      config: this.config,
      response: null,
    });
  }

  setFormConfig() {
    this.config = this.miAclService.getConfiguration('TRANSFORM_JOIN_MI_TO_DELETE');
  }
}
