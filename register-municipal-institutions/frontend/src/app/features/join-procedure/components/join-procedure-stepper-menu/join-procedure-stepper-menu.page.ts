import { Component, OnInit } from '@angular/core';
import { MenuItem } from 'primeng/api';
import { JoinProcedureQuery } from '../../state/join-procedure.query';
import { JoinProcedureService } from '../../state/join-procedure.service';
import { JoinProcedureStore } from '../../state/join-procedure.store';

@Component({
  selector: 'app-join-procedure-stepper-menu',
  templateUrl: './join-procedure-stepper-menu.page.html',
  styleUrls: ['./join-procedure-stepper-menu.page.scss'],
  providers: [JoinProcedureStore, JoinProcedureQuery, JoinProcedureService],
})
export class JoinProcedureStepperMenuPage implements OnInit {
  constructor(private jpService: JoinProcedureService) { }

  items: MenuItem[] = [
    { label: 'Избери институции, които ще се влеят', routerLink: 'mis-to-delete' },
    { label: 'Избери институция, към която се в влива', routerLink: 'mi-to-update' },
    { label: 'Данни за заповед', routerLink: 'ri-document' },
    { label: 'Потвърди вливане', routerLink: 'confirm' },
  ];

  ngOnInit(): void {
    this.jpService.updateActiveInstitutions();
  }
}
