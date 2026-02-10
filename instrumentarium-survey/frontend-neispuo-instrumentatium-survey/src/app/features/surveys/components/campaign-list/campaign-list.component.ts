import { AfterViewInit, Component, ElementRef, OnInit, Renderer2, ViewChild } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthQuery } from '@authentication/auth-state-manager/auth.query';
import { GetManyDefaultResponse } from '@nestjsx/crud';
import { CondOperator } from '@nestjsx/crud-request';
import { PaginatedTableList } from '@shared/classes/paginated-table-list';
import { ROUTING_CONSTANTS } from '@shared/constants/routing.constants';
import { TABLES_CONSTANTS } from '@shared/constants/tables.constants';
import { TEXT_CONTENT_CONSTANTS } from '@shared/constants/text-content.constants';
import { DotNotatedPipe } from '@shared/modules/pipes/dot-notated.pipe';
import { TranslatePipe } from '@shared/modules/pipes/translate.pipe';
import { DynamicColumnService } from '@shared/services/dynamic-column.service';
import { ScrollService } from '@shared/services/utils/scroll.service';
import { LazyLoadEvent, MessageService } from 'primeng/api';
import { Table } from 'primeng/table';
import { CampaignService } from 'src/app/core/services/campaign.service';
import { CampaignDateFilter } from 'src/app/resources/enums/campaign-date-filter.enum';
import { CampaignFilter } from 'src/app/resources/enums/campaign-filter.enum';
import { CampaignType } from 'src/app/resources/enums/campaign-type.enum';
import { Campaign } from 'src/app/resources/models/campaign.model';
import { ColumnServiceConfig } from 'src/app/resources/models/column-service.model';
import { DialogService } from 'primeng/dynamicdialog';
import { NewCampaignComponent } from '../new-campaign/new-campaign.component';
import { AnsweredQuestionnaireState } from 'src/app/resources/models/answered-questionnaire-state.model';
import { RoleEnum } from '@authentication/models/role.enum';

@Component({
  selector: 'app-campaign-list',
  templateUrl: './campaign-list.component.html',
  styleUrls: ['./campaign-list.component.scss'],
  providers: [MessageService]
})
export class CampaignListComponent extends PaginatedTableList<Campaign> implements OnInit, AfterViewInit {
  loading = true;

  @ViewChild('campaignsTable') tableRef: Table = {} as Table;
  @ViewChild('filterButton') filterButton: ElementRef;
  @ViewChild('filter') filter: ElementRef;
  routingConstants = ROUTING_CONSTANTS;

  form: FormGroup;
  campaignType = CampaignType;
  selectedYearFilters: CampaignDateFilter[] = [];

  selectedOption: string;
  answeredQuestionaireName: string;
  campaignsToDelete = [];

  activeFilterBtn: number = 1;

  isModalOpened: boolean = false;
  displayYearFilters: boolean = false;

  lastLazyLoadEvent: LazyLoadEvent;
  selectedRole$ = this.authQuery.selectedRole$;
  isInstitution$ = this.authQuery.isInstitution$;
  state: AnsweredQuestionnaireState;

  // Configurations
  columnConfig: ColumnServiceConfig<Campaign> = {
    localStorageKey: 'campaigns-list',
    allColumns: [
      { field: 'institutionId', header: TABLES_CONSTANTS.CAMPAIGN_LIST.INSTITUTION_CODE },
      { field: 'name', header: TABLES_CONSTANTS.CAMPAIGN_LIST.NAME },
      { field: 'type', header: TABLES_CONSTANTS.CAMPAIGN_LIST.TYPE },
      {
        field: 'startDate',
        header: TABLES_CONSTANTS.CAMPAIGN_LIST.START_DATE,
        filter: { type: 'date', operator: CondOperator.CONTAINS }
      },
      {
        field: 'endDate',
        header: TABLES_CONSTANTS.CAMPAIGN_LIST.END_DATE,
        filter: { type: 'date', operator: CondOperator.CONTAINS }
      },
      { field: 'isActive', header: TABLES_CONSTANTS.CAMPAIGN_LIST.IS_ACTIVE },
      { field: 'SubmittedQuestionaires', header: TABLES_CONSTANTS.CAMPAIGN_LIST.STATE },
      {
        field: 'updatedAt',
        header: TABLES_CONSTANTS.CAMPAIGN_LIST.UPDATED_AT,
        filter: { type: 'date', operator: CondOperator.CONTAINS }
      }
    ],
    fixedVisibleColumnFields: ['institutionId', 'name', 'type', 'startDate', 'endDate', 'isActive', 'SubmittedQuestionaires', 'updatedAt']
  };

  years = [CampaignDateFilter.CURRENT_YEAR, CampaignDateFilter.PREVIOUS_YEAR, CampaignDateFilter.TWOYEARS_BEFORE];

  content = TEXT_CONTENT_CONSTANTS.CAMPAIGN_LIST;

  constructor(
    public scrollService: ScrollService,
    private readonly formBuilder: FormBuilder,
    private readonly campaignsService: CampaignService,
    private readonly authQuery: AuthQuery,
    private readonly dialogService: DialogService,
    columnService: DynamicColumnService<Campaign>,
    router: Router,
    route: ActivatedRoute,
    dotNotatedPipe: DotNotatedPipe,
    translatePipe: TranslatePipe,
    private renderer: Renderer2,
    private messageService: MessageService
  ) {
    super(router, route, dotNotatedPipe, columnService, translatePipe);
    this.selectedRole$.subscribe((role) => {
      switch (role.SysRoleID) {
        case RoleEnum.PARENT:
        case RoleEnum.STUDENT:
        case RoleEnum.TEACHER:
          this.removeColumn('updatedAt');
          break;

        case RoleEnum.MON_ADMIN:
        case RoleEnum.MON_EXPERT:
        case RoleEnum.MON_OBGUM:
        case RoleEnum.MON_OBGUM_FINANCES:
        case RoleEnum.MON_CHRAO:
        case RoleEnum.MON_USER_ADMIN:
        case RoleEnum.NIO:
          this.removeColumn('SubmittedQuestionaires');
          break;
      }

      this.columnService.setConfig(this.columnConfig);
    });


    this.answeredQuestionnaireMessage();

    this.mouseDirective();
  }

  // Lifecycle Hooks
  ngOnInit(): void {
    this.columnService.reloadVisibleColumns();

    this.initForm();
  }

  ngAfterViewInit(): void {
    this.loadByUrl();

    if(this.state) {
      this.state.isQuestionnaireAnswered ? this.showNotificationMessage(this.content.ANSWERED_QUESTIONNAIRE) : this.showNotificationMessage(this.content.SAVED_DRAFT)
    }
  }

  // Events
  onActivateButton(activeFilterBtn: number) {
    this.activeFilterBtn = activeFilterBtn;

    this.resetFilters();
    switch (this.activeFilterBtn) {
      case 1:
        this.selectedOption = CampaignFilter.ALL;
        break;
      case 2:
        this.tableRef.filter(true, 'isActive', 'equals');
        this.selectedOption = CampaignFilter.ACTIVE;
        break;
      case 3:
        this.tableRef.filter(false, 'isActive', 'equals');
        this.selectedOption = CampaignFilter.PAST;
        break;
    }

    this.load(this.lastLazyLoadEvent);
  }

  onApplyYearSorting() {
    this.load(this.lastLazyLoadEvent);
  }

  navigateToCampaignPreview(campaign: Campaign) {
    this.selectedRole$.subscribe((selectedRole) => {
      switch (selectedRole.SysRoleID) {
        case RoleEnum.INSTITUTION:
          if (campaign.isActive) {
            if (campaign.type === CampaignType.ESUI) {
              this.router.navigateByUrl(
                `${ROUTING_CONSTANTS.SURVEY}/${ROUTING_CONSTANTS.CAMPAIGN}/${campaign.id}/${ROUTING_CONSTANTS.QUESTIONNAIRE}`
              );
            } else if (campaign.type === CampaignType.SELFEVALUATE) {
              this.router.navigateByUrl(
                `${ROUTING_CONSTANTS.SURVEY}/${ROUTING_CONSTANTS.ACTIVE}/${ROUTING_CONSTANTS.CAMPAIGN}/${campaign.id}/${ROUTING_CONSTANTS.PREVIEW}`
              );
            }
          } else if (this.isPastCampaign(campaign))
            this.router.navigateByUrl(
              campaign.type === CampaignType.SELFEVALUATE
                ? `${ROUTING_CONSTANTS.SURVEY}/${ROUTING_CONSTANTS.INACTIVE}/${ROUTING_CONSTANTS.CAMPAIGN}/${campaign.id}/${ROUTING_CONSTANTS.PREVIEW}`
                : `${ROUTING_CONSTANTS.SURVEY}/${ROUTING_CONSTANTS.CAMPAIGN}/${campaign.id}/${ROUTING_CONSTANTS.QUESTIONNAIRE}`
            );
          else this.router.navigateByUrl(
            campaign.type === CampaignType.SELFEVALUATE
              ? `${ROUTING_CONSTANTS.SURVEY}/${ROUTING_CONSTANTS.ACTIVE}/${ROUTING_CONSTANTS.CAMPAIGN}/${campaign.id}/${ROUTING_CONSTANTS.PREVIEW}`
              : ``);
          break;

        case RoleEnum.TEACHER:
        case RoleEnum.STUDENT:
        case RoleEnum.PARENT:
          if (this.isPastCampaign(campaign) || campaign.isActive) {
            this.router.navigateByUrl(
              `${ROUTING_CONSTANTS.SURVEY}/${ROUTING_CONSTANTS.CAMPAIGN}/${campaign.id}/${ROUTING_CONSTANTS.QUESTIONNAIRE}`
            );
          }
          break;

        case RoleEnum.MON_ADMIN:
        case RoleEnum.MON_EXPERT:
        case RoleEnum.MON_OBGUM:
        case RoleEnum.MON_OBGUM_FINANCES:
        case RoleEnum.MON_CHRAO:
        case RoleEnum.MON_USER_ADMIN:
        case RoleEnum.NIO:
          this.router.navigateByUrl(
            `${ROUTING_CONSTANTS.SURVEY}/${ROUTING_CONSTANTS.FILLED}/${ROUTING_CONSTANTS.QUESTIONNAIRES}/${campaign.id}`
            );
          break;

        default:
          return false;
      }
    });
  }

  onChecked(event, campaign: Campaign) {
    event.target.checked ? this.campaignsToDelete.push(campaign.id) : this.campaignsToDelete.pop();
  }

  createCampaign() {
    const dialogRef = this.dialogService.open(NewCampaignComponent, {
      header: 'Създай кампания',
      width: '60%',
    });

    dialogRef.onClose.subscribe(async (newCampaignName) => {
      if (newCampaignName) {
        this.load(this.lastLazyLoadEvent);

        this.showNotificationMessage(this.content.SUCCESSFULL_CREATE_CAMPAIGN_MESSAGE +' \"'+ newCampaignName +'\"');
      }
    });
  }

  async onDeleteCampaign() {
    this.campaignsService.deleteCampaign(this.campaignsToDelete).subscribe((_) => {
      this.load(this.lastLazyLoadEvent);
    });

    this.isModalOpened = false;
    this.campaignsToDelete = [];

    this.showNotificationMessage(this.content.SUCCESSFULL_DELETE_CAMPAIGN_MESSAGE)
  }

  // Public Methods
  public load(event: LazyLoadEvent) {
    this.lastLazyLoadEvent = event;
    this.selectedYearFilters = this.form.get('selectedYearFilters').value;

    this.scrollService.scrollTo('body');
    this.campaignsService
      .getAllCampaigns({ ...this.createQueryParams(event), years: JSON.stringify(this.selectedYearFilters) })
      .subscribe(
        (success: GetManyDefaultResponse<Campaign>) => {
          this.paginatedEntries = success;
        },
        (error) => {
          console.log(error);
        }
      )
      .add(() => (this.loading = false));
  }

  public removeFilter(value) {
    const indexOfYearFilter = this.selectedYearFilters.indexOf(value);

    indexOfYearFilter !== -1
      ? this.selectedYearFilters.splice(indexOfYearFilter, 1)
      : this.onActivateButton(1);
  }

  // Private Methods
  private initForm() {
    this.form = this.formBuilder.group({
      selectedYearFilters: ['', Validators.nullValidator]
    });

    this.form.patchValue({
      selectedYearFilters: []
    });

    this.selectedYearFilters = [];
  }

  private async answeredQuestionnaireMessage() {
    const navigation = this.router.getCurrentNavigation();
    this.state = navigation.extras.state as { isQuestionnaireAnswered: boolean };
  }

  private resetFilters() {
    this.tableRef.filter(undefined, 'type', 'equals');
    this.tableRef.filter(undefined, 'isActive', 'equals');
  }


  private timeout(ms) {
    return new Promise((resolve) => setTimeout(resolve, ms));
  }

  // To be moved to directive
  private mouseDirective() {
    this.renderer.listen('window', 'click', (e: Event) => {
      if (this.filter && this.filterButton) {
        if (!this.filterButton.nativeElement.contains(e.target) && !this.filter.nativeElement.contains(e.target) && !(e.target as Element).matches(".p-multiselect-token-icon.pi.pi-times-circle")) {
          this.displayYearFilters = false;
        }
      }
    });
  }

  private isPastCampaign(campaign: Campaign): boolean {
    return new Date(campaign.startDate) < new Date() ? true : false;
  }

  private removeColumn(field: string) {
    this.columnConfig.fixedVisibleColumnFields.reduce((acc, el, i) => {
      if (el === field) {
        this.columnConfig.fixedVisibleColumnFields.splice(i, 1);
      }

      return acc;
    }, []);
  }

  async showNotificationMessage(message: string) {
    this.messageService.add({severity:'success', detail: message });

    await this.timeout(5 * 1000);
    this.messageService.clear();
  }
}
