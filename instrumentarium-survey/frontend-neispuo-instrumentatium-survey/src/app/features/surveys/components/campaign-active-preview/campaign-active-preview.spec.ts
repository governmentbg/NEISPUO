import { HttpClientTestingModule } from "@angular/common/http/testing";
import { async, ComponentFixture, TestBed, waitForAsync } from "@angular/core/testing";
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from "@angular/forms";
import { RouterTestingModule } from "@angular/router/testing";
import { Observable, of } from "rxjs";
import { By } from '@angular/platform-browser';
import { EnvironmentService } from "src/app/core/services/environment.service";
import { CampaignActivePreviewComponent } from "./campaign-active-preview.component";
import { TEXT_CONTENT_CONSTANTS } from "@shared/constants/text-content.constants";
import moment from "moment";
import { PrimeNGComponentsModule } from "@shared/modules/primeng-components.module";
import { BrowserAnimationsModule } from "@angular/platform-browser/animations";
import { CampaignService } from "src/app/core/services/campaign.service";
import { Campaign } from "src/app/resources/models/campaign.model";

describe('CampaignActivePreviewComponent', () => {
  let component: CampaignActivePreviewComponent;
  let fixture: ComponentFixture<CampaignActivePreviewComponent>;
  let myService: EnvironmentService;
  const formBuilder: FormBuilder = new FormBuilder();


  beforeEach(waitForAsync(() => {
    TestBed.configureTestingModule({
      declarations: [ CampaignActivePreviewComponent ],
      imports: [ 
        RouterTestingModule, HttpClientTestingModule, FormsModule, ReactiveFormsModule, PrimeNGComponentsModule,
        BrowserAnimationsModule
      ],
      providers: [{ provide: FormBuilder, useValue: formBuilder } ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(CampaignActivePreviewComponent);
    component = fixture.componentInstance;
    myService = fixture.debugElement.injector.get(EnvironmentService);

    component.isInstitution$ = of(true);
    component.isMon$ = of(false);
    component.showSpinner = false;
    component.campaign = {
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
    }
    component.questionnaire = {
      id: 456,
      state: 745,
      userId: 485,
      totalScore: 4,
      submittedQuestionaireObject: "nesto",
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
        id: 458,
        name: "За директори",
        questions: []
      }
    }

    component.startDate = new Date();
    component.todaysDate = new Date();
    component.form = formBuilder.group({
      name: [component.campaign.name, [Validators.required, Validators.maxLength(100), Validators.pattern('[a-zA-ZА-я ][a-zA-ZА-я0-9 ]*')]],
      startDate: [moment(component.campaign.startDate).format('DD/MM/YYYY'), [Validators.required]],
      endDate: [moment(component.campaign.endDate).format('DD/MM/YYYY'), [Validators.required]]
    });
    component.content = TEXT_CONTENT_CONSTANTS.CAMPAIGN_ACTIVE_PREVIEW;

    fixture.detectChanges();
    console.log(fixture.debugElement.query(By.css('.edit-btn')))
  });

  it('should create CampaignActivePreviewComponent', () => {
    expect(component).toBeTruthy();
  });

  it('should have button edit', () => {
    const editButton = fixture.debugElement.query(By.css('.edit-btn'));
    expect(editButton).toBeTruthy();
  });

  it('should open edit dialog', () => {
    component.isDialogDisplayed = true;
    fixture.detectChanges();
    const formDetails =  fixture.debugElement.query(By.css('.form-details'));
    expect(formDetails).toBeTruthy();
  })

  it('should edit campaign', () => {
    let campaignService = fixture.debugElement.injector.get(CampaignService);
    spyOn(campaignService,'editCampaign');
    campaignService.editCampaign(456,new Campaign("New", new Date(), new Date()));
    expect(campaignService.editCampaign).toHaveBeenCalled();
  })

  it('shoud not be disabled, valid name', () => {
    component.isDialogDisplayed = true;
    fixture.detectChanges();
    const validName =  fixture.debugElement.query(By.css('.campaign-name.ng-valid'));
    expect(validName).toBeTruthy();
  })

  it('should be disabled, incorrect name', () => {
    component.isDialogDisplayed = true;
    component.form.patchValue({
      name: "222"
    })
 
    fixture.detectChanges();

    const validForm =  fixture.debugElement.query(By.css('.campaign-name.ng-valid'));
    expect(validForm).toBeFalsy();
  })

  it('tooltip should be Попълни въпросник', () => {
    component.questionnaire.state = 0;

    fixture.detectChanges();

    expect(fixture.debugElement.query(By.css('.criteria-name')).properties.outerHTML).toContain('Попълни въпросник');
  })

  it('tooltip should be Прегледай въпросник', () => {
    component.questionnaire.state = 1;

    fixture.detectChanges();

    expect(fixture.debugElement.query(By.css('.criteria-name')).properties.outerHTML).toContain('Прегледай въпросник');
  })

  it('totalUsersSubmitted should be empty and getNumberOfUsersWhoFilledInTheCampaign should be called', async( () => {
    let campaignService = fixture.debugElement.injector.get(CampaignService);
    component.ngOnInit();
    spyOn(campaignService,'getNumberOfUsersWhoFilledInTheCampaign').and.returnValue(new Observable())
    campaignService.getNumberOfUsersWhoFilledInTheCampaign(4);
    expect(campaignService.getNumberOfUsersWhoFilledInTheCampaign).toHaveBeenCalled();
    fixture.detectChanges();
    fixture.whenStable().then(() => {
      expect(fixture.debugElement.componentInstance.data).toBe(undefined);
      expect(component.totalUsersSubmitted).toBe(undefined);
    })
  }))

  it('totalUsersSubmitted should have data and getNumberOfUsersWhoFilledInTheCampaign should be called', () => {
    let campaignService = fixture.debugElement.injector.get(CampaignService);
    component.ngOnInit();
    spyOn(campaignService,'getNumberOfUsersWhoFilledInTheCampaign');
    campaignService.getNumberOfUsersWhoFilledInTheCampaign(4);
    expect(campaignService.getNumberOfUsersWhoFilledInTheCampaign).toHaveBeenCalled();
    component.totalUsersSubmitted=2;
    fixture.detectChanges();
    expect(component.totalUsersSubmitted).toBe(2);
  })

  it('should navigate to quetionnaire', () => {
    spyOn(component,'goToQuestionnaire');
    component.goToQuestionnaire();
    expect(component.goToQuestionnaire).toHaveBeenCalled();
  })
})