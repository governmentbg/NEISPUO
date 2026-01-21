import {
  AfterViewInit, ChangeDetectorRef, Component, Input, OnDestroy, OnInit, SimpleChanges, ViewChild,
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
import { BehaviorSubject } from 'rxjs';

@Component({
  selector: 'app-mi-preview-only',
  templateUrl: './mi-preview-only.component.html',
  styleUrls: ['./mi-preview-only.component.scss'],
})
export class MIPreviewOnlyComponent extends MiFormUtility implements OnInit, AfterViewInit, IStatePage, OnDestroy {
  /**
   * Set default mode
   */
  @Input() displayRIPremInstitution: boolean = false;

  @Input() displayRIProcedure: boolean = false;

  @Input() displayProcedureFields: boolean;

  @ViewChild(MiFormComponent) miFormComponent: MiFormComponent;

  form: FormGroup;

  @Input() config: IMIFormComponentState = {} as IMIFormComponentState;

  BaseSchoolTypes$ = this.nmQuery.BaseSchoolTypes$;

  _resp: BehaviorSubject<MunicipalInstitution> = new BehaviorSubject(null);

  @Input() set resp(v: MunicipalInstitution) {
    if (!v || v === this.resp) {
      return;
    }
    this._resp.next(v);
  }

  get resp() {
    return this._resp.getValue();
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

  ngAfterViewInit(): void {
    this.subs.sink = this._resp.subscribe((resp) => {
      this.form.patchValue(
        this.cfgs.buildForm({
          config: this.config,
          response: this.resp,
        }).getRawValue(),
      );
      this.configureForm();
    });
  }

  // ngAfterViewInit(): void {
  //   this.form.patchValue(
  //     this.cfgs.buildForm({
  //       config: this.config,
  //       response: this.resp
  //     }).getRawValue()
  //   );
  // this.configureForm();
  // }

  ngOnChanges(changes: SimpleChanges): void {
    if (!this.form) { return; }
    this.form.patchValue(
      this.cfgs.buildForm({
        config: this.config,
        response: changes.resp.currentValue,
      }).getRawValue(),
    );
  }

  private configureForm() {
    if (this.miFormComponent.institutionFlexFieldComponent) {
      this.miFormComponent.institutionFlexFieldComponent.prePopulateFormArray(this.resp.RIFlexFieldValues);
      this.miFormComponent.institutionFlexFieldComponent.disableAddition = true;
      this.miFormComponent.institutionFlexFieldComponent.disableDeletion = true;
    }
    this.miFormComponent.displayRIPremInstitution = !!this.displayRIPremInstitution;
    this.disableRIFlexFieldArray(this.form);
    this.miFormComponent.displayRIProcedure = !!this.displayRIProcedure;
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }

  ngOnInit() {
    this.setFormConfig();
    this.form = this.cfgs.buildForm({
      config: this.config,
      response: this.resp,
    });
  }

  setFormConfig() {
    this.config = this.miAclService.getConfiguration('READ');
  }
}
