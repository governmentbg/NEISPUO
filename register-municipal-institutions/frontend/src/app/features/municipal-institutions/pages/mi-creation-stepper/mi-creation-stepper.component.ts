import { Component, OnInit } from '@angular/core';
import { BulstatService } from '@municipal-institutions/services/bulstat.service';
import { NomenclatureService } from '@shared/services/common-form-service/nomenclatures-state/nomenclatures.service';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-mi-creation-stepper',
  templateUrl: './mi-creation-stepper.component.html',
  styleUrls: ['./mi-creation-stepper.component.scss'],
  providers: [BulstatService],
})
export class MiCreationStepperComponent implements OnInit {
  constructor(
    private nmService: NomenclatureService,
  ) { }

  items: MenuItem[] = [
    { label: 'Проверка по Булстат/ЕИК', routerLink: 'bulstat-loader' },
    { label: 'Откриване на институция', routerLink: 'mi-create' },
  ];

  ngOnInit(): void {}
}
