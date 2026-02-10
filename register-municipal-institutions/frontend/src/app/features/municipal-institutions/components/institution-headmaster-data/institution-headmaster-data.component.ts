import { Component, OnInit } from '@angular/core';
import { AbstractControl, ControlContainer, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-institution-headmaster-data',
  templateUrl: './institution-headmaster-data.component.html',
  styleUrls: ['./institution-headmaster-data.component.scss'],
})
export class InstitutionHeadmasterDataComponent implements OnInit {
  public form: FormGroup | AbstractControl;

  constructor(public cc: ControlContainer) {
  }

  ngOnInit(): void {
    this.form = this.cc.control;
  }
}
