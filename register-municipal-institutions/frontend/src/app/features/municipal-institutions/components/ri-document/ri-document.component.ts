import { Component, Input, OnInit } from '@angular/core';
import { ControlContainer, FormGroup } from '@angular/forms';
import { NomenclatureQuery } from '@shared/services/common-form-service/nomenclatures-state/nomenclatures.query';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-ri-document',
  templateUrl: './ri-document.component.html',
  styleUrls: ['./ri-document.component.scss'],
})
export class RiDocumentComponent implements OnInit {
  @Input() controlName: string;

  @Input() isView: boolean;

  @Input() displayDocument: boolean;

  form: FormGroup;

  CurrentYear$ = this.nmQuery.CurrentYear$.pipe(map((response) => response?.[0]?.CurrentYearID))

  currentYear: number;

  constructor(private cc: ControlContainer, private nmQuery: NomenclatureQuery) { }

  ngOnInit(): void {
    this.form = this.cc.control.get(this.controlName) as FormGroup;
  }

  getYearRange(): string {
    this.CurrentYear$.subscribe(year => this.currentYear = year);
    const startYear = 2020;

    return `${startYear}:${this.currentYear}`
  }
}
