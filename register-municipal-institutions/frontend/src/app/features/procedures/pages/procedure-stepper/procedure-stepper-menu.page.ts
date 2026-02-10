import { Component, OnInit } from '@angular/core';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-procedure-stepper-menu',
  templateUrl: './procedure-stepper-menu.page.html',
  styleUrls: ['./procedure-stepper-menu.page.scss'],
})
export class ProcedureStepperMenuPage implements OnInit {
  constructor() { }

  items: MenuItem[] = [
    { label: 'Избери институция(и)', routerLink: 'mi-to-split' },
    { label: 'Създай или избери институции', routerLink: 'mi-to-create' },
    { label: 'Потвърди', routerLink: 'confirm' },
  ];

  ngOnInit(): void {
  }
}
