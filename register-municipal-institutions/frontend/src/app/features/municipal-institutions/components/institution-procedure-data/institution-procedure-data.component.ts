import { Component, Input, OnInit } from '@angular/core';
import { ControlContainer, FormGroup } from '@angular/forms';
import { NomenclatureQuery } from '@shared/services/common-form-service/nomenclatures-state/nomenclatures.query';
import { MessageService } from 'primeng/api';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-institution-procedure-data',
  templateUrl: './institution-procedure-data.component.html',
  styleUrls: ['./institution-procedure-data.component.scss'],
})
export class InstitutionProcedureDataComponent implements OnInit {
  form: FormGroup;

  @Input() controlName: string;

  @Input() isView: boolean;

  @Input() displayDocument: boolean;

  @Input() displayProcedureFields: boolean;

  TransformTypes$ = this.nmQuery.TransformTypes$;

  ProcedureTypes$ = this.nmQuery.ProcedureTypes$;

  uploadedFiles: any[] = [];

  currentYear: number;

  CurrentYear$ = this.nmQuery.CurrentYear$.pipe(map((response) => response?.[0]?.CurrentYearID))

  constructor(private cc: ControlContainer, private messageService: MessageService, private nmQuery: NomenclatureQuery) { }

  ngOnInit(): void {
    this.form = this.cc.control.get(this.controlName) as FormGroup;
  }

  getYearRange(): string {
    this.CurrentYear$.subscribe(year => this.currentYear = year);
    const startYear = 2020;

    return `${startYear}:${this.currentYear}`
  }
}
