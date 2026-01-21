import { Component, Input, OnInit } from '@angular/core';
import { faCheck as fasCheck } from '@fortawesome/pro-solid-svg-icons/faCheck';

@Component({
  selector: 'sb-wizard-heading',
  templateUrl: './wizard-heading.component.html',
  styleUrls: ['./wizard-heading.component.scss']
})
export class WizardHeadingComponent implements OnInit {
  readonly fasCheck = fasCheck;

  @Input() steps!: {
    number: number;
    caption: string;
    label?: string;
    hidden?: boolean;
  }[];
  @Input() currentStep!: number;

  ngOnInit() {
    if (this.steps == null) {
      throw new Error('Required Input steps is null or undefined.');
    }
    if (this.currentStep == null) {
      throw new Error('Required Input caption is null or undefined.');
    }
  }
}
