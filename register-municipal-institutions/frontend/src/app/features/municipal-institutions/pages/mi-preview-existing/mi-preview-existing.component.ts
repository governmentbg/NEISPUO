import {
  Component, Input, OnDestroy, OnInit, ViewChild,
} from '@angular/core';
import { FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthQuery } from '@core/authentication/auth-state-manager/auth.query';
import { MiFormComponent } from '@municipal-institutions/components/mi-form/mi-form.component';
import { ACLMunicipalityInstitutionService } from '@municipal-institutions/services/acl-mi.service';
import { MICommonFormGroupService } from '@municipal-institutions/services/mi-common-form-group.service';
import { IMIFormComponentState } from '@municipal-institutions/services/mi-form-component-state.interface';
import { MiFormUtility } from '@municipal-institutions/services/mi-form-utility';
import { IStatePage } from '@municipal-institutions/services/state-page.inteface';
import { MunicipalInstitution } from '@municipal-institutions/state/municipal-institutions/municipal-institution.interface';
import { MunicipalInstitutionQuery } from '@municipal-institutions/state/municipal-institutions/municipal-institution.query';
import { MunicipalInstitutionService } from '@municipal-institutions/state/municipal-institutions/municipal-institution.service';
import { ProcedureTypeEnum } from '@procedures/models/procedure-type.enum';
import { NomenclatureQuery } from '@shared/services/common-form-service/nomenclatures-state/nomenclatures.query';
import { combineLatest, Observable } from 'rxjs';
import { UtilsService } from '@shared/services/utils/utils.service';
import {
  concatMap, map, shareReplay, tap,
} from 'rxjs/operators';
import { GuideName } from '@shared/modules/user-tour-guide/constants/user-tour-guide.constants';
import { UserTourGuideService } from '@shared/modules/user-tour-guide/user-tour-guide.service';

@Component({
  selector: 'app-mi-preview-existing',
  templateUrl: './mi-preview-existing.component.html',
  styleUrls: ['./mi-preview-existing.component.scss'],
})
export class MiPreviewExistingComponent extends MiFormUtility implements OnInit, IStatePage, OnDestroy {
  @ViewChild(MiFormComponent) miFormComponent: MiFormComponent;

  form: FormGroup;

  config: IMIFormComponentState = {} as IMIFormComponentState;

  BaseSchoolTypes$ = this.nmQuery.BaseSchoolTypes$;

  _resp: MunicipalInstitution = null;

  isEditButtonVisible$: Observable<boolean> = this.decideEditable();

  isClosed = false;

  isHistoryDropdownVisible = false;

  historyEntries$: Observable<MunicipalInstitution[]>;

  selectedHistoryEntry: MunicipalInstitution;

  isLatestVersion = false;

  display: boolean = false;

  latestVersionID = null;

  latestVersion = null;

  guideName = GuideName.PREVIEW_INSTITUTION;

  private decideEditable() {
    return combineLatest([this.authQuery.isMon$, this.authQuery.isRuo$, this.authQuery.isMunicipality$]).pipe(
      map(([isMon, isRuo, isMunicipality]) => {
        if (isMon || isRuo) {
          return false;
        }
        return isMunicipality; // always true in current logic
      }),
      shareReplay(1),
    );
  }

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
    private router: Router,
    private route: ActivatedRoute,
    private miService: MunicipalInstitutionService,
    private cfgs: MICommonFormGroupService,
    private miAclService: ACLMunicipalityInstitutionService,
    nmQuery: NomenclatureQuery,
    authQuery: AuthQuery,
    private utilsService: UtilsService,
    private userTourService: UserTourGuideService,
  ) {
    super(nmQuery, authQuery);
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe();
    this.userTourService.stopGuide();
  }

  ngOnInit() {
    this.setFormConfig();
    this.form = this.cfgs.buildForm({
      config: this.config,
      response: this.resp,
    });
    this.subs.sink = this.miQuery.selectedRIInstitutionID$
      .pipe(
        tap(
          (id) => (this.historyEntries$ = this.miService.getHistory(id).pipe(
            tap((entries) => {
              this.isLatestVersion = Number(id) === entries[0].RIInstitutionID;
              this.latestVersionID = entries[0].RIInstitutionID;
              this.latestVersion = entries[0];
              this.selectedHistoryEntry = entries.find((entry) => entry.RIInstitutionID === Number(id));
            }),
          )),
        ),
        concatMap((id) => this.miService.getOne(`/v1/ri-institution/${id}`)),
        concatMap(() => this.miQuery.selectFirst$),
      )
      .subscribe(
        (mi) => {
          this.resp = mi;
          this.miFormComponent.institutionFlexFieldComponent.prePopulateFormArray(this.resp.RIFlexFieldValues);
          this.form.patchValue(
            this.cfgs
              .buildForm({
                config: this.config,
                response: this.resp,
              })
              .getRawValue(),
          );
          this.disableRIFlexFieldArray(this.form);
          this.isClosed = this.resp.RIProcedure.ProcedureType.ProcedureTypeID === ProcedureTypeEnum.DELETE;
          this.miFormComponent.institutionFlexFieldComponent.disableAddition = true;
          this.miFormComponent.institutionFlexFieldComponent.disableDeletion = true;
        },
        (err) => {
          console.log(err);
        },
      );
    this.utilsService.enableTooltip('i.bi-clock-history');
  }

  setFormConfig() {
    this.config = this.miAclService.getConfiguration('READ');
  }

  btnClick = (pathTo: string) => {
    const navigateTo = [`municipal-institutions/${pathTo}/${this._resp.RIInstitutionID}`];
    this.router.navigate(navigateTo);
  };

  onHistoryEntryChange(ev: MunicipalInstitution) {
    this.selectedHistoryEntry = ev;
    this.selectedHistoryEntry.RIInstitutionID === this.latestVersionID ? this.isLatestVersion = true : this.isLatestVersion = false;
    this.router.navigate(['..', ev.RIInstitutionID], { relativeTo: this.route });
  }

  getLatestVersion() {
    this.isLatestVersion = true;
    this.selectedHistoryEntry = this.latestVersion;
    this.router.navigate(['..', this.latestVersionID], { relativeTo: this.route });
  }

  navigateBackward = () => {
    this.router.navigate(['..'], { relativeTo: this.route });
    if (this.isClosed) {
      this.router.navigate(['/municipal-institutions/closed'], { relativeTo: this.route });
    }
  };

  showHistory() {
    this.isHistoryDropdownVisible = true;
    this.display = true;
    if (this.isHistoryDropdownVisible) {
      this.userTourService.stopGuide();
    }
  }

  hideHistory() {
    this.display = false;
    this.isHistoryDropdownVisible = false;
    this.getLatestVersion();
  }
}
