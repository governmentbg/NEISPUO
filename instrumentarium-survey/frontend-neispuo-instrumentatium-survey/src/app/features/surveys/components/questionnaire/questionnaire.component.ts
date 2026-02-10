import { ViewportScroller } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, NavigationExtras, Router } from '@angular/router';
import { AuthQuery } from '@authentication/auth-state-manager/auth.query';
import { SelectedRole } from '@authentication/auth-state-manager/interfaces/selected-role.interface';
import { ROUTING_CONSTANTS } from '@shared/constants/routing.constants';
import { TEXT_CONTENT_CONSTANTS } from '@shared/constants/text-content.constants';
import { map } from 'rxjs/operators';
import { QuestionnaireService } from 'src/app/core/services/questionnaire.service';
import { SubmittedQuestionaireState } from 'src/app/resources/enums/submitted-questionnaire-state.enum';
import { Choice } from 'src/app/resources/models/choice.model';
import { QuestionaireQuestion } from 'src/app/resources/models/questionaire-question.model';
import { SubmitChoice } from 'src/app/resources/models/submit-choice.model';

@Component({
  selector: 'app-questionnaire',
  templateUrl: './questionnaire.component.html',
  styleUrls: ['./questionnaire.component.scss']
})
export class QuestionnaireComponent implements OnInit {
  private campaignId: number;

  choices: Choice[] = [];
  submitChoices: SubmitChoice[] = [];
  questionnaireObject: QuestionaireQuestion;
  state = SubmittedQuestionaireState;

  $loggedUser;
  submittedQuestionnaireId: number;
  displayModal: boolean;
  showSpinner: boolean = true;
  isModalOpened: boolean = false;
  unansweredQuestionsIndex: number[] = [];
  countQuestionsIndexes: number = 0;
  firstUnansweredQuestion: number = -1;

  enableFields = false;

  content = TEXT_CONTENT_CONSTANTS.QUESTIONNAIRE;

  constructor(
    private readonly router: Router,
    private readonly authQuery: AuthQuery,
    private readonly route: ActivatedRoute,
    private readonly questionnaireService: QuestionnaireService,
    private scroller: ViewportScroller
  ) {}

  ngOnInit(): void {
    this.route.params.subscribe(params => {
      this.campaignId = params.campaignId;
      this.submittedQuestionnaireId = params.submittedQuestionnaireId;
      this.getQuestionnaireQuestions();
      this.getLoggedUser();
    });

  }

  onSubmit() {
    this.getIndexOfUnansweredQuestions();

    if (this.countQuestionsIndexes === 0) {
      this.isModalOpened =  true;
    } else {
      const navigateToUnasweredCard: string = 'card-' + this.firstUnansweredQuestion;
      this.scroller.scrollToAnchor(navigateToUnasweredCard);
    }
  }

  submitQuestionnaire(){
    const questionnaire = this.fillQuestionnaire();

    this.questionnaireService.submitQuestionnaire(questionnaire, this.questionnaireObject.id).subscribe(() => {
      const navigationExtras: NavigationExtras = {
        state: { isQuestionnaireAnswered: true }
      };

      this.router.navigate([`${ROUTING_CONSTANTS.SURVEY}/${ROUTING_CONSTANTS.CAMPAIGN}`], navigationExtras);
    });
  }

  saveDraft() {
    const questionnaire = this.fillQuestionnaire(0);
    this.questionnaireService.submitQuestionnaire(questionnaire,this.questionnaireObject.id).subscribe(() => {
      const navigationExtras: NavigationExtras = {
          state: { isQuestionnaireAnswered: false }
        };
      this.router.navigate([`${ROUTING_CONSTANTS.SURVEY}/${ROUTING_CONSTANTS.CAMPAIGN}`], navigationExtras);
    });
  }

  private fillQuestionnaire(state: number = 1) {
    this.toSubmitChoices();

    return {
      questionaireId: this.questionnaireObject.questionaireId.id,
      state,
      campaignsId: this.campaignId,
      userId: this.$loggedUser.SysUserID,
      submittedQuestionaireObject: JSON.stringify(this.submitChoices)
    };
  }

  private getLoggedUser() {
    this.authQuery.selectedRole$.subscribe((selectedReponse: SelectedRole) => {
      this.$loggedUser = selectedReponse;
    });
  }

  private getQuestionnaireQuestions() {
    this.questionnaireService.getQuestionnaireAndQuestions(this.campaignId, this.submittedQuestionnaireId).subscribe((questionnaire: QuestionaireQuestion) => {
      this.questionnaireObject = questionnaire;
      if (this.questionnaireObject.state === this.state.DRAFT && this.questionnaireObject.campaignsId.isActive) this.enableFields = true;
      if (this.submittedQuestionnaireId) this.enableFields = false;
      this.getAnswers();
      this.showSpinner = false;
    });
  }

  private getIndexOfUnansweredQuestions() {
    this.resetValues();

    const questions = this.questionnaireObject.questionaireId.questions;
    questions.forEach((_, index) => {
      if (!this.choices[index]) {
        if (this.firstUnansweredQuestion === -1) {
          this.firstUnansweredQuestion = index;
        }

        this.unansweredQuestionsIndex.push(index);
        this.countQuestionsIndexes++;
      } else {
        this.unansweredQuestionsIndex.push(undefined);
      }
    });
  }

  private resetValues() {
    this.firstUnansweredQuestion = -1;
    this.unansweredQuestionsIndex = [];
    this.countQuestionsIndexes = 0;
  }

  private getAnswers() {
      this.questionnaireService.getUserFilledQuestionnaires(this.questionnaireObject.userId, this.campaignId, this.questionnaireObject.questionaireId.id)
      .pipe(map(answers => {
        // To be fixed;
        if (answers[0].submittedQuestionaireObject !== '{}') {
          JSON.parse(answers[0].submittedQuestionaireObject).forEach((answer: SubmitChoice, index: number) => {
            this.questionnaireObject.questionaireId.questions[index].choices.forEach((choice: Choice) => {
              if (answer)
              answer.choiceId === choice.id ? this.choices[index] = choice : '';
            });
          });
        }
      })).subscribe();
  }

  private toSubmitChoices() {
    this.choices.forEach((choice, index) => {

      this.submitChoices[index] = new SubmitChoice(
        choice.id,
        choice.value,
        choice.weight,
        this.questionnaireObject.questionaireId.questions[index].indicatorId,
        this.questionnaireObject.questionaireId.questions[index].id
      );
    })
  }
}
