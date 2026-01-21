import {
  AfterViewInit, ChangeDetectorRef, Component, OnDestroy, OnInit, ViewChild,
} from '@angular/core';
import { FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthQuery } from '@authentication/auth-state-manager/auth.query';
import { MiFormComponent } from '@municipal-institutions/components/mi-form/mi-form.component';
import { FormMode } from '@municipal-institutions/models/form-mode';
import { ACLMunicipalityInstitutionService } from '@municipal-institutions/services/acl-mi.service';
import { BulstatService } from '@municipal-institutions/services/bulstat.service';
import { MICommonFormGroupService } from '@municipal-institutions/services/mi-common-form-group.service';
import { IMIFormComponentState } from '@municipal-institutions/services/mi-form-component-state.interface';
import { MunicipalInstitution } from '@municipal-institutions/state/municipal-institutions/municipal-institution.interface';
import { EnumBaseSchoolType } from '@procedures/models/base-school-type.enum';
import { BudgetingInstitutionEnum } from '@procedures/models/bugeting-institution.enum';
import { FinancialSchoolTypeEnum } from '@procedures/models/financial-school-type.enum';
import { ProcedureTypeEnum } from '@procedures/models/procedure-type.enum';
import { TransformTypeEnum } from '@procedures/models/transform-type.enum';
import { NomenclatureQuery } from '@shared/services/common-form-service/nomenclatures-state/nomenclatures.query';
import { UtilsService } from '@shared/services/utils/utils.service';
import { ConfirmationService } from 'primeng/api';
import { combineLatest } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { SubSink } from 'subsink';
import { ScrollService } from '../../../../shared/services/utils/scroll.service';
import { MergeProcedureQuery } from '../../state/merge-procedure.query';
import { MergeProcedureService } from '../../state/merge-procedure.service';

@Component({
  selector: 'app-merge-mi-to-create',
  templateUrl: './merge-mi-to-create.component.html',
  styleUrls: ['./merge-mi-to-create.component.scss'],
})
export class MergeMIToCreateComponent implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild(MiFormComponent) miFormComponent: MiFormComponent;

  public form: any;

  config: IMIFormComponentState = {} as IMIFormComponentState;

  isUserMunicipality = false;

  miToCreate: MunicipalInstitution;

  formMode: FormMode = 'CREATE';

  subs = new SubSink();

  isBulstatLookupEnabled: boolean = true;

  displayBulstatMessage: boolean = true;

  confirmDialog: any;

  displayWarnMessage: boolean;

  BaseSchoolTypes$ = this.nmQuery.BaseSchoolTypes$
    .pipe(
      filter((baseSchoolTypes) => !!baseSchoolTypes),
      map(
        (baseSchoolTypes) => baseSchoolTypes
          .filter((bst) => bst.BaseSchoolTypeID === EnumBaseSchoolType.KINDERGARTEN || bst.BaseSchoolTypeID === EnumBaseSchoolType.PERSONAL_DEVELOPMENT_CENTRE),

      ),

    );

  constructor(
    private cfgs: MICommonFormGroupService,
    private nmQuery: NomenclatureQuery,
    private authQuery: AuthQuery,
    private utilsService: UtilsService,
    private router: Router,
    private activatedRoute: ActivatedRoute,
    private readonly aclMiService: ACLMunicipalityInstitutionService,
    private mpQuery: MergeProcedureQuery,
    private mpService: MergeProcedureService,
    private cdr: ChangeDetectorRef,
    private scrollService: ScrollService,
    private bulstatService: BulstatService,
    private confirmationService: ConfirmationService,
  ) { }

  ngAfterViewInit(): void {
    this.subs.sink = this.mpQuery.miToCreate$
      .pipe(filter((mitc) => !!mitc))
      .subscribe((mitc) => {
        this.form.patchValue(this.cfgs.buildForm({
          config: this.config,
          response: mitc,
        }).getRawValue());
        this.miToCreate = mitc;
        this.miFormComponent.institutionFlexFieldComponent.prePopulateFormArray(mitc.RIFlexFieldValues);
      });

    this.presetMunicipality();
    this.presetFinancialSchoolType();
    this.presetBudgetingInstitution();
    this.presetProcedureAndTransformType();

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

  ngOnInit(): void {
    this.setFormConfig();
    this.form = this.cfgs.buildForm({
      config: this.config,
      response: {} as MunicipalInstitution,
    });
  }

  setFormConfig() {
    this.config = this.aclMiService.getConfiguration('CREATE');
  }

  forwardCallback = (e: any) => {
    const RIProcedureForm = this.form.get('RIProcedure') as FormGroup;
    RIProcedureForm.removeControl('RIDocument');
    this.utilsService.markAllAsDirty(this.form);
    if (this.form.invalid) {
      this.scrollService.scrollToFirstError();
      return;
    }

    this.mpService.updateMIToCreate(this.form.getRawValue());
    this.router.navigate(['..', 'ri-document'], { relativeTo: this.activatedRoute });
  };

  backwardCallback = (e: any) => {
    this.mpService.updateMIToCreate(this.form.getRawValue());
    this.router.navigate(['..', 'mis-to-delete'], { relativeTo: this.activatedRoute });
  };

  verifyBulstat = async () => {
    try {
      const municipalInstitution = await this.bulstatService.verify(this.form.get('Bulstat').value).toPromise();
      this.confirmDialog = this.confirmationService.confirm({
        message: 'Институция с въведения ЕИК беше намерена. Желаете ли да заредите данните?',
        accept: () => {
          this.form.setValue(
            this.cfgs.buildForm({
              config: this.config,
              response: municipalInstitution,
            }).getRawValue(),
          );
          this.displayBulstatMessage = false;
          this.displayWarnMessage = false;
        },
      });
    } catch (error) {
      this.displayWarnMessage = true;
      console.error(error);
    }
  };

  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }
}
