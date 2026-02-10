import { Component, Input, OnInit } from '@angular/core';
import { ControlContainer, FormGroup } from '@angular/forms';

@Component({
  selector: 'app-prem-institution-data',
  templateUrl: './prem-institution-data.component.html',
  styleUrls: ['./prem-institution-data.component.scss'],
})
export class PremInstitutionDataComponent implements OnInit {
  constructor(private cc: ControlContainer) { }

  form: FormGroup;

  @Input() controlName: string;

  ngOnInit(): void {
    this.form = this.cc.control.get('RIProcedure.RIPremInstitution') as FormGroup;
  }
}
