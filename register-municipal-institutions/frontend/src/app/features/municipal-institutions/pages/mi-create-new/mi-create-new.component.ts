import {
  AfterViewInit, ChangeDetectorRef, Component, OnDestroy, OnInit,
} from '@angular/core';
import { Router } from '@angular/router';
import { AuthQuery } from '@authentication/auth-state-manager/auth.query';
import { FormMode } from '@municipal-institutions/models/form-mode';
import { RIInstitution } from '@municipal-institutions/models/ri-institution';
import { ACLMunicipalityInstitutionService } from '@municipal-institutions/services/acl-mi.service';
import { MICommonFormGroupService } from '@municipal-institutions/services/mi-common-form-group.service';
import { IMIFormComponentState } from '@municipal-institutions/services/mi-form-component-state.interface';
import { MunicipalInstitutionService } from '@municipal-institutions/state/municipal-institutions/municipal-institution.service';
import { ProcedureTypeEnum } from '@procedures/models/procedure-type.enum';
import { TransformTypeEnum } from '@procedures/models/transform-type.enum';
import { NomenclatureQuery } from '@shared/services/common-form-service/nomenclatures-state/nomenclatures.query';
import { UtilsService } from '@shared/services/utils/utils.service';
import { MessageService } from 'primeng/api';
import { combineLatest } from 'rxjs';
import { filter, map } from 'rxjs/operators';
import { SubSink } from 'subsink';
import { ScrollService } from '@shared/services/utils/scroll.service';
import { EnumBaseSchoolType } from '@procedures/models/base-school-type.enum';
import { BudgetingInstitutionEnum } from '@procedures/models/bugeting-institution.enum';
import { FinancialSchoolTypeEnum } from '@procedures/models/financial-school-type.enum';
import { BulstatService } from '@municipal-institutions/services/bulstat.service';
import { MunicipalInstitution } from '@municipal-institutions/state/municipal-institutions/municipal-institution.interface';
import { GuideName } from '@shared/modules/user-tour-guide/constants/user-tour-guide.constants';
import { UserTourGuideService } from '@shared/modules/user-tour-guide/user-tour-guide.service';

@Component({
  selector: 'app-mi-create-new',
  templateUrl: './mi-create-new.component.html',
  styleUrls: ['./mi-create-new.component.scss'],
})
export class MiCreateNewComponent implements OnInit, AfterViewInit, OnDestroy {
  public form: any;

  config: IMIFormComponentState = {} as IMIFormComponentState;

  isUserMunicipality = false;

  submitInstitution: Function;

  formMode: FormMode = 'CREATE';

  resp: MunicipalInstitution;

  subs = new SubSink();

  guideName = GuideName.CREATE_INSTITUTION;

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
    private miService: MunicipalInstitutionService,
    private messageService: MessageService,
    private router: Router,
    private readonly aclMiService: ACLMunicipalityInstitutionService,
    private cdr: ChangeDetectorRef,
    private scrollService: ScrollService,
    private bulstatService: BulstatService,
    private userGuideService: UserTourGuideService,
  ) { }

  ngAfterViewInit(): void {
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
    this.initForm();
    this.submitInstitution = () => {
      this.utilsService.markAllAsDirty(this.form);

      if (this.form.invalid) {
        this.scrollService.scrollToFirstError();
        return;
      }

      this.miService.createOne('/v1/ri-institution', this.form.getRawValue()).subscribe(
        (s) => {
          this.messageService.add({ severity: 'success', summary: 'Успех', detail: 'Успешно откриване на институция.' });
          this.router.navigateByUrl(`/municipal-institutions/preview/${s.RIInstitutionID}`);
        },
        (e) => {
          console.error(e);
          this.messageService.add({ severity: 'error', summary: 'Грешка', detail: 'Възникна грешка при откриване на институция.' });
        },
      );
    };
      this.loadDataByBulstat();
  }

  private initForm(response = {} as RIInstitution) {
    this.setFormConfig();
    this.form = this.cfgs.buildForm({
      config: this.config,
      response,
    });

  }

  loadDataByBulstat() {
    this.form.markAsPristine();
    this.subs.sink = this.bulstatService.municipalInstitution$.subscribe(
      (data) => {
        this.resp = data;
        this.form.setValue(
          this.cfgs.buildForm({
            config: this.config,
            response: this.resp,
          }).getRawValue(),
        );
      },
    );
  }

  setFormConfig() {
    this.config = this.aclMiService.getConfiguration(this.formMode);
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe();
    this.userGuideService.stopGuide();
  }
}
