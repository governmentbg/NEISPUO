import { AfterViewInit, Component, OnInit, ViewChild } from "@angular/core";
import { ActivatedRoute, Router } from "@angular/router";
import { GetManyDefaultResponse } from "@nestjsx/crud";
import { ScrollService } from "@shared/services/utils/scroll.service";
import { LazyLoadEvent } from "primeng/api";
import { CampaignService } from "src/app/core/services/campaign.service";
import { QuestionnaireService } from "src/app/core/services/questionnaire.service";
import { QuestionaireQuestion } from "src/app/resources/models/questionaire-question.model";
import { PaginatedTableList } from '@shared/classes/paginated-table-list';
import { DotNotatedPipe } from "@shared/modules/pipes/dot-notated.pipe";
import { TranslatePipe } from "@shared/modules/pipes/translate.pipe";
import { DynamicColumnService } from "@shared/services/dynamic-column.service";
import { ColumnServiceConfig } from "src/app/resources/models/column-service.model";
import { Table } from "primeng/table";
import { CondOperator } from "@nestjsx/crud-request";

@Component({
  selector: 'app-filled-questionnaires-list',
  templateUrl: './filled-questionnaires-list.component.html',
  styleUrls: ['./filled-questionnaires-list.component.scss']
})
export class FilledQuestionnairesListComponent extends PaginatedTableList<QuestionaireQuestion> implements OnInit, AfterViewInit {

  showSpinner: boolean = true;
  totalUsersSubmitted: number;
  totalUsersAssignedTo: number;
  totalRecords: number;
  campaignId: string;
  campaignName: string;

  lastLazyLoadEvent: LazyLoadEvent;
  @ViewChild('filledQuestionnaire') tableRef: Table = {} as Table;
      
  columnConfig: ColumnServiceConfig<QuestionaireQuestion> = {
    localStorageKey: 'filled-questionnaires-lis',
    allColumns: [
      { field: 'id', header: 'ID на попълнен въпросник' },
      { field: 'campaignName', header: 'Име на кампания' },
      { field: 'questionnaireName', header: 'Име на въпросник' },
      { field: 'state', header: 'Статус' }
    ],
    fixedVisibleColumnFields: ['id', 'campaignName', 'questionnaireName', 'state']
  };

  constructor(
    public scrollService: ScrollService,
    private readonly campaignService: CampaignService,
    private readonly questionnaireService: QuestionnaireService,
    route: ActivatedRoute,
    router: Router,
    dotNotatedPipe: DotNotatedPipe,
    translatePipe: TranslatePipe,
    columnService: DynamicColumnService<QuestionaireQuestion>
  ) {
    super(router, route, dotNotatedPipe, columnService, translatePipe);
    this.columnService.setConfig(this.columnConfig);
  }

  ngOnInit() {
    this.columnService.reloadVisibleColumns();
    this.campaignId = this.route.snapshot.paramMap.get('id');
    this.numberOfPeopleWhoFilledInTheCampaign(this.campaignId);
  }

  ngAfterViewInit(): void {
    this.loadByUrl();
  }

  public load(event: LazyLoadEvent) {
    this.lastLazyLoadEvent = event;

    event.filters = {
      ...event.filters,
      "campaignsId.id": {
        value: this.campaignId,
        matchMode: CondOperator.EQUALS
      }, 
      "submittedQuestionaireObject": {
        value: "{}",
        matchMode: CondOperator.NOT_EQUALS
      }
    }

    this.scrollService.scrollTo('body');
    this.questionnaireService
    .getSubmittedQuestionnaires(this.createQueryParams(event))
      .subscribe(
        (success: GetManyDefaultResponse<QuestionaireQuestion>) => {
          this.paginatedEntries = success;
          this.paginatedEntries.data[0]
          ? this.campaignName = this.paginatedEntries.data[0].campaignsId.name
          : this.campaignName = ""
        },
        (error) => {
          console.log(error);
        }
      )
      .add(() => (this.showSpinner = false));
  }

  goToQuestionnaire(submittedQuestionnaire) {
    this.router.navigateByUrl(`survey/campaign/${this.campaignId}/questionnaire/${submittedQuestionnaire.id}`);
  }

  private numberOfPeopleWhoFilledInTheCampaign(campaignId) {
    this.campaignService.getNumberOfUsersWhoFilledInTheCampaign(+campaignId).subscribe((data: any) => {
      this.totalUsersSubmitted = data.totalUsersSubmitted;
      this.totalUsersAssignedTo = data.totalUsersAssignedTo;
    });
  }
}