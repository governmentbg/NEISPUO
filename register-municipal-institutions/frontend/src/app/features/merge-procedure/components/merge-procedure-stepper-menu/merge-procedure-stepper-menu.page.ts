import { Component, OnInit } from '@angular/core';
import { BulstatService } from '@municipal-institutions/services/bulstat.service';
import { MenuItem } from 'primeng/api';
import { MergeProcedureQuery } from '../../state/merge-procedure.query';
import { MergeProcedureService } from '../../state/merge-procedure.service';
import { MergeProcedureStore } from '../../state/merge-procedure.store';

@Component({
  selector: 'app-merge-procedure-stepper-menu',
  templateUrl: './merge-procedure-stepper-menu.page.html',
  styleUrls: ['./merge-procedure-stepper-menu.page.scss'],
  providers: [MergeProcedureStore, MergeProcedureQuery, MergeProcedureService, BulstatService],
})
export class MergeProcedureStepperMenuPage implements OnInit {
  constructor(private jpService: MergeProcedureService) { }

  items: MenuItem[] = [
    { label: 'Избери институции за сливане', routerLink: 'mis-to-delete' },
    { label: 'Избери институция, към която се слива', routerLink: 'mi-to-create' },
    { label: 'Данни за заповед', routerLink: 'ri-document' },
    { label: 'Потвърди сливане', routerLink: 'confirm' },
  ];

  ngOnInit(): void {
    this.jpService.updateActiveInstitutions();
  }
}
