import { Component, OnInit } from '@angular/core';
import { BulstatService } from '@municipal-institutions/services/bulstat.service';
import { MenuItem } from 'primeng/api';
import { DivideProcedureQuery } from '../../state/divide-procedure.query';
import { DivideProcedureService } from '../../state/divide-procedure.service';
import { DivideProcedureStore } from '../../state/divide-procedure.store';

@Component({
  selector: 'app-divide-procedure-stepper-menu',
  templateUrl: './divide-procedure-stepper-menu.page.html',
  styleUrls: ['./divide-procedure-stepper-menu.page.scss'],
  providers: [DivideProcedureStore, DivideProcedureQuery, DivideProcedureService, BulstatService],
})
export class DivideProcedureStepperMenuPage implements OnInit {
  constructor(private dpService: DivideProcedureService) { }

  items: MenuItem[] = [
    { label: 'Институция, от която ще се разделяне', routerLink: 'mi-to-delete' },
    { label: 'Създаване на институции', routerLink: 'mis-to-create' },
    { label: 'Данни за заповед', routerLink: 'ri-document' },
    { label: 'Потвърди разделяне', routerLink: 'confirm' },
  ];

  ngOnInit(): void {
    this.dpService.updateActiveInstitutions();
  }
}
