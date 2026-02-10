import { Component, Input, OnInit } from '@angular/core';
import { UtilsService } from '@shared/services/utils/utils.service';

@Component({
  selector: 'app-stepper-actions',
  templateUrl: './stepper-actions.component.html',
  styleUrls: ['./stepper-actions.component.scss'],
})
export class StepperActionsComponent implements OnInit {
  @Input() showBack: boolean;

  @Input() backLabel: string;

  @Input() backRouterLink: string | Array<string>;

  @Input() backwardCallback = (event: any) => {};

  @Input() showForward: boolean;

  @Input() forwardLabel: string;

  @Input() forwardRouterLink: string;

  @Input() disableForward: boolean;

  @Input() forwardCallback = (event: any) => {};

  @Input() disableForwardButtonTooltip: string;

  @Input() showSubmit: boolean;

  @Input() submitCallback = (event: any) => {};

  @Input() submitLabel: string;

  constructor(private utilsService: UtilsService) { }

  ngOnInit(): void {
  }
}
