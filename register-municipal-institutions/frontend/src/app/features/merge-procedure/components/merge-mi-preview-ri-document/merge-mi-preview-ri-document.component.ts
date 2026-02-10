import {
  AfterViewInit, ChangeDetectorRef, Component, Input, OnInit, SimpleChanges,
} from '@angular/core';
import { FormGroup } from '@angular/forms';
import { ACLMunicipalityInstitutionService } from '@municipal-institutions/services/acl-mi.service';
import { MICommonFormGroupService } from '@municipal-institutions/services/mi-common-form-group.service';
import { IMIFormComponentState } from '@municipal-institutions/services/mi-form-component-state.interface';
import { MunicipalInstitution } from '@municipal-institutions/state/municipal-institutions/municipal-institution.interface';
import { MunicipalInstitutionQuery } from '@municipal-institutions/state/municipal-institutions/municipal-institution.query';
import { SubSink } from 'subsink';

@Component({
  selector: 'app-merge-mi-preview-ri-document',
  templateUrl: './merge-mi-preview-ri-document.component.html',
  styleUrls: ['./merge-mi-preview-ri-document.component.scss'],
})
export class MergeMiPreviewRiDocumentComponent implements OnInit, AfterViewInit {
  @Input() displayRIPremInstitution: boolean;

  @Input() displayRIProcedure: boolean;

  @Input() controlName: string;

  @Input() isView: boolean;

  @Input() displayDocument: boolean;

  @Input() isReadOnly: boolean;

  subs = new SubSink();

  form: FormGroup;

  config: IMIFormComponentState = {} as IMIFormComponentState;

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
    private cdr: ChangeDetectorRef,

  ) { }

  ngOnChanges(changes: SimpleChanges): void {
    if (!this.form) { return; }
    this.form.patchValue(
      this.cfgs.buildForm({
        config: this.config,
        response: changes.resp.currentValue,
      }).getRawValue(),
    );
  }

  ngAfterViewInit(): void {
    this.form.setValue(
      this.cfgs.buildForm({
        config: this.config,
        response: this.resp,
      }).getRawValue(),

    );
    const RIProcedureForm = this.form.get('RIProcedure') as FormGroup;
    RIProcedureForm.removeControl('ProcedureDate');
    RIProcedureForm.removeControl('ProcedureType');
    RIProcedureForm.removeControl('TransformType');
    RIProcedureForm.removeControl('YearDue');

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
    this.isReadOnly ? this.config = this.miAclService.getConfiguration('READ') : this.config = this.miAclService.getConfiguration('CREATE');
  }
}
