import {
  Component, Input, OnInit, ViewEncapsulation,
} from '@angular/core';
import { ProcedureQuery } from '@procedures/state/procedure/procedure.query';
import { ProcedureService } from '@procedures/state/procedure/procedure.service';
import { ProcedureStore } from '@procedures/state/procedure/procedure.store';
import { NomenclatureService } from '@shared/services/common-form-service/nomenclatures-state/nomenclatures.service';
import { MenuItem } from 'primeng/api';

@Component({
  selector: 'app-procedure-stepper',
  templateUrl: './procedure-stepper.component.html',
  styleUrls: ['./procedure-stepper.component.scss'],
  providers: [ProcedureStore, ProcedureQuery, ProcedureService],
  encapsulation: ViewEncapsulation.None,

})
export class ProcedureStepperComponent implements OnInit {
  constructor(
    private pStore: ProcedureStore,
    private pQuery: ProcedureQuery,
    private pService: ProcedureService,
    private nmService: NomenclatureService,
  ) { }

  @Input() items: MenuItem[];

  ngOnInit(): void {
    this.pService.setAllInstitutions();
  }
}
