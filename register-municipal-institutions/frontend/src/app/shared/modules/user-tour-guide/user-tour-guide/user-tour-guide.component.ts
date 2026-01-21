import { Component, Input, OnInit } from '@angular/core';
import { ShepherdService } from 'angular-shepherd';
import { GUIDE_STEPS_BY_NAME } from '../constants/user-tour-guide.constants';
import { UserTourGuideQuery } from '../user-tour-guide.query';
import { UserTourGuideService } from '../user-tour-guide.service';

@Component({
  selector: 'app-user-tour-guide',
  templateUrl: './user-tour-guide.component.html',
  styleUrls: ['./user-tour-guide.component.scss'],
})

export class UserTourGuideComponent implements OnInit {
  @Input() guideName;

  @Input() typeButton: string;

  userGuideSteps: Array<Object> = [];

  constructor(
    public userTourGuideQuery: UserTourGuideQuery,
    private shepherdService: ShepherdService,
    private userTourGuideService: UserTourGuideService,
  ) { }

  ngOnInit(): void {
    this.userTourGuideService.activeUserGuide$().subscribe((guideName) => {
      if (!guideName) {
        this.userGuideSteps = [];
        return;
      }
      this.userGuideSteps = GUIDE_STEPS_BY_NAME[guideName];
      if (this.userTourGuideQuery.isFirstTimeGuide(guideName)) {
        this.startGuide();
        this.userTourGuideQuery.setGuideStarted(guideName);
      }
    });
  }

  startGuide() {
    this.userGuideSteps = GUIDE_STEPS_BY_NAME[this.guideName];
    if (this.userGuideSteps.length === 0) {
      return;
    }

    this.shepherdService.defaultStepOptions = {
      scrollTo: { behavior: 'smooth', block: 'center' },
    };
    this.shepherdService.modal = true;
    this.shepherdService.addSteps(this.userGuideSteps);
    this.shepherdService.start();
  }
}
