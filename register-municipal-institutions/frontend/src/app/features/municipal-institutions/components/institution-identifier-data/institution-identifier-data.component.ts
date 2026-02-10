import { Component, Input, OnInit } from '@angular/core';
import { AbstractControl, ControlContainer, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-institution-identifier-data',
  templateUrl: './institution-identifier-data.component.html',
  styleUrls: ['./institution-identifier-data.component.scss'],
})
export class InstitutionIdentifierDataComponent implements OnInit {
  public form: FormGroup | AbstractControl;
  @Input() isBulstatLookupEnabled: boolean;
  @Input() displayBulstatMessage: boolean;
  @Input() onBulstatLoad: Function = (event: any) => {};

  constructor(public cc: ControlContainer) {
  }

  ngOnInit(): void {
    this.form = this.cc.control;
  }

}
