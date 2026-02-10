import {
  AfterViewInit, ChangeDetectorRef, Component, Input, OnInit, ViewChild,
} from '@angular/core';
import { FormGroup } from '@angular/forms';
import { AuthQuery } from '@authentication/auth-state-manager/auth.query';
import { MiFormComponent } from '@municipal-institutions/components/mi-form/mi-form.component';
import { ACLMunicipalityInstitutionService } from '@municipal-institutions/services/acl-mi.service';
import { MICommonFormGroupService } from '@municipal-institutions/services/mi-common-form-group.service';
import { IMIFormComponentState } from '@municipal-institutions/services/mi-form-component-state.interface';
import { IStatePage } from '@municipal-institutions/services/state-page.inteface';
import { MunicipalInstitution } from '@municipal-institutions/state/municipal-institutions/municipal-institution.interface';
import { ProcedureTypeEnum } from '@procedures/models/procedure-type.enum';
import { TransformTypeEnum } from '@procedures/models/transform-type.enum';
import { NomenclatureQuery } from '@shared/services/common-form-service/nomenclatures-state/nomenclatures.query';
import { UtilsService } from '@shared/services/utils/utils.service';
import { DynamicDialogRef, DynamicDialogConfig } from 'primeng/dynamicdialog';
import { combineLatest } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { SubSink } from 'subsink';
import { ScrollService } from '@shared/services/utils/scroll.service';
import { BulstatService } from '@municipal-institutions/services/bulstat.service';
import { ConfirmationService } from 'primeng/api';
import { EnumBaseSchoolType } from '@procedures/models/base-school-type.enum';
import { BudgetingInstitutionEnum } from '@procedures/models/bugeting-institution.enum';
import { FinancialSchoolTypeEnum } from '@procedures/models/financial-school-type.enum';
import { DivideProcedureService } from '../../state/divide-procedure.service';
import { DivideProcedureQuery } from '../../state/divide-procedure.query';

@Component({
  selector: 'app-divide-mi-modal-preview',
  templateUrl: './divide-mi-modal-preview.component.html',
  styleUrls: ['./divide-mi-modal-preview.component.scss'],
})
export class DivideMIModalPreviewComponent implements OnInit, AfterViewInit, IStatePage {
  confirmDialog: any
  response: MunicipalInstitution
  displayWarnMessage: boolean;
  form: FormGroup;

  rowIndex: number;

  config: IMIFormComponentState = {} as IMIFormComponentState;
  submitLabel = 'Добави'
  _resp: MunicipalInstitution;
  subs = new SubSink();
  displayBulstatMessage: boolean = true
  isBulstatLookupEnabled = !this.dpService.isEditMode
  @ViewChild(MiFormComponent) miFormComponent: MiFormComponent;
  @Input() set resp(v: MunicipalInstitution) {
    if (!v || v === this.resp) {
      return;
    }
    this._resp = v;
  }

  get resp() {
    return this._resp;
  }

  @Input() clickCallback = (institution, index) => { };

  BaseSchoolTypes$ = this.nmQuery.BaseSchoolTypes$
    .pipe(
      filter((baseSchoolTypes) => !!baseSchoolTypes),
      map(
        (baseSchoolTypes) => baseSchoolTypes
          .filter((bst) => bst.BaseSchoolTypeID === EnumBaseSchoolType.KINDERGARTEN || bst.BaseSchoolTypeID === EnumBaseSchoolType.PERSONAL_DEVELOPMENT_CENTRE),

      ),

    );

  constructor(
    public ref: DynamicDialogRef,
    public dconfig: DynamicDialogConfig,
    private cfgs: MICommonFormGroupService,
    private nmQuery: NomenclatureQuery,
    private authQuery: AuthQuery,
    private utilsService: UtilsService,
    private readonly aclMiService: ACLMunicipalityInstitutionService,
    private dpQuery: DivideProcedureQuery,
    private dpService: DivideProcedureService,
    private cdr: ChangeDetectorRef,
    private scrollService: ScrollService,
    private bulstatService: BulstatService,
    private confirmationService: ConfirmationService
  ) { }

  ngOnInit() {
    this.resp = this.dconfig.data.mi as MunicipalInstitution;
    this.rowIndex = this.dconfig.data.rowIndex as number;
    this.clickCallback = this.dconfig.data.clickCallback;
    this.initForm();
  }

  addMi() {
    this.utilsService.markAllAsDirty(this.form);
    const RIProcedureForm = this.form.get('RIProcedure') as FormGroup;
    RIProcedureForm.removeControl('RIDocument');
    if (this.form.invalid) {
      this.scrollService.scrollTo('small.invalid-feedback', document.querySelector('.p-dialog-content'));
      return;
    }
    this.clickCallback(this.form.getRawValue(), this.rowIndex);
  }

  private initForm() {
    this.setFormConfig();
    this.form = this.cfgs.buildForm({
      config: this.config,
      response: this.resp,
    });
  }

  ngAfterViewInit(): void {
    this.presetMunicipality();
    this.presetFinancialSchoolType();
    this.presetBudgetingInstitution();
    this.presetProcedureAndTransformType();
    this.miFormComponent.institutionFlexFieldComponent.prePopulateFormArray(this.resp.RIFlexFieldValues);
    this.cdr.detectChanges();
  }

  /**
  * Preset municipality
  */
  presetMunicipality() {
    this.subs.sink = combineLatest([this.authQuery.selectedRole$, this.nmQuery.Municipalities$])
      .pipe(filter(([sr, mun]) => !!sr && !!mun)).subscribe(([sr, muns]) => {
        const userMunicipality = muns.find((m) => m.MunicipalityID === sr.MunicipalityID);
        this.form.patchValue({
          Region: userMunicipality.Region,
          Municipality: userMunicipality,
        });
      });
  }

  presetBudgetingInstitution() {
    this.subs.sink = this.nmQuery.BudgetingInstitutions$.pipe(filter((bis) => !!bis)).subscribe((bis) => {
      this.form.patchValue({ BudgetingInstitution: bis.find((bi) => bi.BudgetingInstitutionID === BudgetingInstitutionEnum.MUNICIPALITY) });
    });
  }

  presetFinancialSchoolType() {
    this.subs.sink = this.nmQuery.FinancialSchoolTypes$.pipe(filter((fsts) => !!fsts)).subscribe((fsts) => {
      this.form.patchValue({ FinancialSchoolType: fsts.find((fst) => fst.FinancialSchoolTypeID === FinancialSchoolTypeEnum.MUNICIPAL) });
    });
  }

  presetProcedureAndTransformType() {
    this.subs.sink = this.nmQuery.ProcedureTypes$.pipe(filter((pts) => !!pts)).subscribe((pt) => {
      this.form.patchValue({ RIProcedure: { ProcedureType: pt.find((pts) => pts.ProcedureTypeID === ProcedureTypeEnum.CREATE) } });
    });
    this.subs.sink = this.nmQuery.TransformTypes$.pipe(filter((tts) => !!tts)).subscribe((tt) => {
      this.form.patchValue({ RIProcedure: { TransformType: tt.find((tts) => tts.TransformTypeID === TransformTypeEnum.CREATE) } });
    });
  }

  setFormConfig() {
    this.config = this.aclMiService.getConfiguration('CREATE');
  }

  verifyBulstat = async () => {
    try {
      let mi = await this.bulstatService.verify(this.form.get('Bulstat').value).toPromise();
      this.confirmDialog = this.confirmationService.confirm({
        message: 'Институция с въведения ЕИК беше намерена. Желаете ли да заредите данните?',
        accept: () => {
          this.form.setValue(
            this.cfgs.buildForm({
              config: this.config,
              response: mi
            }).getRawValue())
          this.displayBulstatMessage = false;
          this.displayWarnMessage = false;
        }
      })

    } catch (error) {
      this.displayWarnMessage = true;
      console.error(error);
    }
  }

}
