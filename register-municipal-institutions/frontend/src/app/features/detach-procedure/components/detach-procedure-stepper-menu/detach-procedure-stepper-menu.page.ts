import { Component, OnInit } from '@angular/core';
import { BulstatService } from '@municipal-institutions/services/bulstat.service';
import { MenuItem } from 'primeng/api';
import { DetachProcedureQuery } from '../../state/detach-procedure.query';
import { DetachProcedureService } from '../../state/detach-procedure.service';
import { DetachProcedureStore } from '../../state/detach-procedure.store';

@Component({
  selector: 'app-detach-procedure-stepper-menu',
  templateUrl: './detach-procedure-stepper-menu.page.html',
  styleUrls: ['./detach-procedure-stepper-menu.page.scss'],
  providers: [DetachProcedureStore, DetachProcedureQuery, DetachProcedureService, BulstatService],
})
export class DetachProcedureStepperMenuPage implements OnInit {
  constructor(private dpService: DetachProcedureService) { }

  items: MenuItem[] = [
    { label: 'Институция, от която ще се отделя', routerLink: 'mi-to-update' },
    { label: 'Създаване на институции', routerLink: 'mis-to-create' },
    { label: 'Данни за заповед', routerLink: 'ri-document' },
    { label: 'Потвърди отделяне', routerLink: 'confirm' },
  ];

  ngOnInit(): void {
    this.dpService.updateActiveInstitutions();
  }
}
