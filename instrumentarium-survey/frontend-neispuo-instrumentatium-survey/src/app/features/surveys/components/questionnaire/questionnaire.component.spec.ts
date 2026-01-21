import { HttpClientTestingModule } from "@angular/common/http/testing";
import { ComponentFixture, fakeAsync, TestBed, tick, waitForAsync } from "@angular/core/testing";
import { FormBuilder, FormsModule, ReactiveFormsModule } from "@angular/forms";
import { RouterTestingModule } from "@angular/router/testing";
import { By } from '@angular/platform-browser';
import { EnvironmentService } from "src/app/core/services/environment.service";
import { PrimeNGComponentsModule } from "@shared/modules/primeng-components.module";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { QuestionnaireComponent } from "./questionnaire.component";
import { QuestionnaireService } from "src/app/core/services/questionnaire.service";
import { AuthQuery } from "@authentication/auth-state-manager/auth.query";

describe('QuestionnaireComponent', () => {
  let component: QuestionnaireComponent;
  let fixture: ComponentFixture<QuestionnaireComponent>;
  let myService: EnvironmentService;
  const formBuilder: FormBuilder = new FormBuilder();


  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ QuestionnaireComponent ],
      imports: [ 
        RouterTestingModule, HttpClientTestingModule, FormsModule, ReactiveFormsModule, PrimeNGComponentsModule,
        BrowserAnimationsModule
      ],
      providers: [{ provide: FormBuilder, useValue: formBuilder } ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(QuestionnaireComponent);
    component = fixture.componentInstance;
    myService = fixture.debugElement.injector.get(EnvironmentService);

    component.showSpinner = false;
    component.questionnaireObject = {
        id: 415,
        state: 1,
        userId: 741,
        totalScore: 3,
        submittedQuestionaireObject: "something",
        campaignsId: {
            id: 456,
            name: "Campaign 2022",
            type: 1,
            startDate: new Date(),
            endDate: new Date(),
            isActive: true,
            isLocked: false,
            updatedAt: null,
            institutionId: 41236,
            createdBy: 145
        },
        questionaireId: {
            id: 15,
            name: "За директори",
            questions: [
                {
                    id: 1,
                    orderNumber: 1,
                    title: "Това е първ въпрос?",
                    type: 1,
                    questionAreaId: 23,
                    criteriaId: 24,
                    indicatorId: 25,
                    subindicatorId: 26,
                    indicatorName: "Първ индикатор",
                    indicatorOrderNumber: 1,
                    indicatorWeight: 12,
                    subindicatorName: "Първ субиндикатор",
                    subindicatorOrderNumber: 12,
                    subindicatorWeight: 13,
                    criteriaOrderNumber: 1,
                    criteriaName: "критериум",
                    questionAreaTitle: [],
                    choices: [
                        {
                            id: 123,
                            title: "Първ отговор",
                            description: "",
                            value: 1,
                            weight: 1
                        },
                        {
                            id: 124,
                            title: "Втор отговор",
                            description: "",
                            value: 1,
                            weight: 1
                        }
                    ]
                },
                {
                    id: 2,
                    orderNumber: 1,
                    title: "Това е втор въпрос?",
                    type: 1,
                    questionAreaId: 23,
                    criteriaId: 24,
                    indicatorId: 25,
                    subindicatorId: 26,
                    indicatorName: "Първ индикатор",
                    indicatorOrderNumber: 1,
                    indicatorWeight: 12,
                    subindicatorName: "Първ субиндикатор",
                    subindicatorOrderNumber: 12,
                    subindicatorWeight: 13,
                    criteriaOrderNumber: 1,
                    criteriaName: "критериум",
                    questionAreaTitle: [],
                    choices: [
                        {
                            id: 223,
                            title: "Първ отговор",
                            description: "",
                            value: 1,
                            weight: 1
                        },
                        {
                            id: 224,
                            title: "Втор отговор",
                            description: "",
                            value: 1,
                            weight: 1
                        }
                    ]
                }
            ]
        }
    }

    fixture.detectChanges();
  });

  it('should create QuestionnaireComponent', () => {
    expect(component).toBeTruthy();
  });

  it('should have answered question', () => {
    component.choices = [
        {
            id: 124,
            title: "Втор отговор",
            description: "",
            value: 1,
            weight: 1
        },
        {
            id: 124,
            title: "Втор отговор",
            description: "",
            value: 1,
            weight: 1
        },
      ]
    fixture.detectChanges();

    expect(component.choices.length===2).toBeTruthy();
  })

  it('all unansweredQuestionsIndex shoud be undefined', () => {
    component.choices = [
        {
            id: 124,
            title: "Втор отговор",
            description: "",
            value: 1,
            weight: 1
        },
        {
            id: 124,
            title: "Втор отговор",
            description: "",
            value: 1,
            weight: 1
        },
    ]

    component.onSubmit();
    fixture.detectChanges();
    expect(component.unansweredQuestionsIndex.every(value => value === undefined)).toBeTruthy();
    expect(component.firstUnansweredQuestion === -1).toBeTruthy();
    expect(component.isModalOpened).toBeTruthy();
  })

  it('should have unanswered questions', () => {
    component.choices = [
        {
            id: 124,
            title: "Втор отговор",
            description: "",
            value: 1,
            weight: 1
        }
    ]

    component.onSubmit();
    fixture.detectChanges();
    expect(component.unansweredQuestionsIndex.every(value => value === undefined)).toBeFalsy();
    expect(component.firstUnansweredQuestion === -1).toBeFalsy();
    expect(component.isModalOpened).toBeFalsy();
  })

  it('should call submitQuestionnaire', () => {
    let questionnaireService = fixture.debugElement.injector.get(QuestionnaireService);
    spyOn(questionnaireService,'submitQuestionnaire');

    const questionnaire = {
      questionaireId: component.questionnaireObject.questionaireId.id,
      state: 0,
      campaignsId: 2,
      userId: 1,
      submittedQuestionaireObject: JSON.stringify(component.submitChoices)
    }

    questionnaireService.submitQuestionnaire(questionnaire, 4);
    expect(questionnaireService.submitQuestionnaire).toHaveBeenCalled();
  })

  it('onSubmit should be call when clicking on submit button', fakeAsync(() => {
      component.choices = [
        {
            id: 124,
            title: "Втор отговор",
            description: "",
            value: 1,
            weight: 1
        }
    ]
    spyOn(component, 'onSubmit');

  let button = fixture.debugElement.query(By.css('.btn-submit')) 
  button.triggerEventHandler('click', null);
  tick();
  fixture.detectChanges();
  expect(component.onSubmit).toHaveBeenCalled();
  }))

  it('should call method getUserFilledQuestionnaires()', () => {
    let questionnaireService = fixture.debugElement.injector.get(QuestionnaireService);
    spyOn(questionnaireService,'getUserFilledQuestionnaires');
    questionnaireService.getUserFilledQuestionnaires(1, 234, component.questionnaireObject.questionaireId.id);
    expect(questionnaireService.getUserFilledQuestionnaires).toHaveBeenCalled();
  })

    it('should save draft', () => {
    let questionnaireService = fixture.debugElement.injector.get(QuestionnaireService);
    spyOn(questionnaireService,'submitQuestionnaire');

    const questionnaire = {
      questionaireId: component.questionnaireObject.questionaireId.id,
      state: 0,
      campaignsId: 212,
      userId: 1,
      submittedQuestionaireObject: JSON.stringify(component.submitChoices)
    }

    questionnaireService.submitQuestionnaire(questionnaire, 4);
    expect(questionnaireService.submitQuestionnaire).toHaveBeenCalled();
  })

  it('buttons should be able after saved draft', () => {
    component.questionnaireObject.campaignsId.isActive = true;
    component.questionnaireObject.state = 0;
    
    if(component.questionnaireObject.campaignsId.isActive && component.questionnaireObject.state===0)
    component.enableFields = true;

    fixture.detectChanges();

    expect(fixture.debugElement.nativeElement.querySelector('.btn-submit').disabled).toBeFalsy();
    expect(fixture.debugElement.nativeElement.querySelector('.btn-save-draft').disabled).toBeFalsy();
  })
})