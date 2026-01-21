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
import { EnumBaseSchoolType } from '@procedures/models/base-school-type.enum';
import { ProcedureTypeEnum } from '@procedures/models/procedure-type.enum';
import { TransformTypeEnum } from '@procedures/models/transform-type.enum';
import { SingleMiChoiceComponent } from '@procedures/pages/single-mi-choice/single-mi-choice.component';
import { NomenclatureQuery } from '@shared/services/common-form-service/nomenclatures-state/nomenclatures.query';
import { ScrollService } from '@shared/services/utils/scroll.service';
import { UtilsService } from '@shared/services/utils/utils.service';
import { ConfirmationService, MessageService } from 'primeng/api';
import { Observable } from 'rxjs';
import { concatMap, filter, map } from 'rxjs/operators';
import { GuideName } from '@shared/modules/user-tour-guide/constants/user-tour-guide.constants';
import { UserTourGuideService } from '@shared/modules/user-tour-guide/user-tour-guide.service';

@Component({
  selector: 'app-mi-delete-existing',
  templateUrl: './mi-delete-existing.component.html',
  styleUrls: ['./mi-delete-existing.component.scss'],
})
export class MiDeleteExistingComponent extends MiFormUtility implements OnInit, AfterViewInit, IStatePage, OnDestroy {
  @ViewChild(MiFormComponent) miFormComponent: MiFormComponent;

  @ViewChild(SingleMiChoiceComponent) singleMiChoiceComponent: SingleMiChoiceComponent;

  form: FormGroup;

  config: IMIFormComponentState = {} as IMIFormComponentState;

  submitLabel = 'Закриване';

  BaseSchoolTypes$: Observable<BaseSchoolType[]>;

  _resp: MunicipalInstitution = null;

  cd: any;

  guideName = GuideName.DELETE_INSTITUTION;

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
    private confirmationService: ConfirmationService,
    private scrollService: ScrollService,
    private userTourService: UserTourGuideService,
  ) {
    super(nmQuery, authQuery);
  }

  ngAfterViewInit(): void {
    this.BaseSchoolTypes$ = this.nomenclatureQuery.BaseSchoolTypes$.pipe(
      filter((baseSchoolTypes) => !!baseSchoolTypes),
      map((baseSchoolTypes) => baseSchoolTypes.filter((bst) => bst.BaseSchoolTypeID === EnumBaseSchoolType.KINDERGARTEN || bst.BaseSchoolTypeID === EnumBaseSchoolType.PERSONAL_DEVELOPMENT_CENTRE)),
    );
    this.presetProcedureAndTransformType(ProcedureTypeEnum.DELETE, TransformTypeEnum.CHANGE);
    this.miFormComponent.institutionFlexFieldComponent.disableAddition = true;
    this.miFormComponent.institutionFlexFieldComponent.disableDeletion = true;

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
              ProcedureTypeID: ProcedureTypeEnum.DELETE,
            },
            RICPLRArea: {
              CPLRAreaType: mi?.RIProcedure?.RICPLRArea?.CPLRAreaType
            },
          } as RIProcedure;

          this.resp = { ...mi, RIProcedure: riProcedure };
          this.miFormComponent.institutionFlexFieldComponent.prePopulateFormArray(this.resp.RIFlexFieldValues);
          this.form.patchValue(
            this.cfgs.buildForm({
              config: this.config,
              response: this.resp,
            }).getRawValue(),
          );
          this.presetProcedureAndTransformType(ProcedureTypeEnum.DELETE, TransformTypeEnum.CLOSED);
          this.miFormComponent.institutionFlexFieldComponent.disableAddition = true;
          this.miFormComponent.institutionFlexFieldComponent.disableDeletion = true;
        },
        (err) => {
          console.log(err);
        },
      );
  }

  setFormConfig() {
    this.config = this.miAclService.getConfiguration('DELETE');
  }

  deleteMI = (e: any) => {
    this.utilsService.markAllAsDirty(this.form);
    if (this.form.invalid) {
      this.scrollService.scrollToFirstError();
      return;
    }

    this.cd = this.confirmationService.confirm({
      message: `Сигурни ли сте, че искате да закриете институция: ${this._resp.Name}(${this._resp.InstitutionID})`,
      accept: () => {
        if (this.form.invalid) {
          return;
        }

        const rawFormValue = this.form.getRawValue() as MunicipalInstitution;

        this.miService.deleteOne(`/v1/ri-institution/${this.resp.RIInstitutionID}`, {
          ...rawFormValue,
        }).subscribe(
          (s) => {
            this.messageService.add({ severity: 'success', summary: 'Успех', detail: 'Успешно закриване на институция.' });
            this.router.navigateByUrl(`/municipal-institutions/preview/${s.RIInstitutionID}`);
          },
          // eslint-disable-next-line @typescript-eslint/no-shadow
          (e) => {
            console.error(e);
            this.messageService.add({ severity: 'error', summary: 'Грешка', detail: 'Възникна грешка при закриване на институция.' });
          },
        );
      },
    });
  };

  goToPreviewPage = (e: any) => {
    this.router.navigate([`municipal-institutions/preview/${this.resp.RIInstitutionID}`]);
  };

  ngOnDestroy(): void {
    this.subs.unsubscribe();
    this.userTourService.stopGuide();
  }
}
