import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MunicipalInstitution } from '@municipal-institutions/state/municipal-institutions/municipal-institution.interface';
import { DialogService } from 'primeng/dynamicdialog';
import { DivideMIModalPreviewComponent } from '../../components/divide-mi-modal-preview/divide-mi-modal-preview.component';
import { DivideProcedureQuery } from '../../state/divide-procedure.query';
import { DivideProcedureService } from '../../state/divide-procedure.service';

@Component({
  selector: 'app-divide-mis-to-create',
  templateUrl: './divide-mis-to-create.component.html',
  styleUrls: ['./divide-mis-to-create.component.scss'],
  providers: [DialogService],
})

export class DivideMIsToCreateComponent implements OnInit {
  cols: any[] = [
    { field: 'Bulstat', header: 'Булстат/ЕИК' },
    { field: 'Name', header: 'Наименование' },
  ];

  misToCreate$ = this.dpQuery.misToCreate$;

  constructor(
    private dpQuery: DivideProcedureQuery,
    private dialogService: DialogService,
    private dpService: DivideProcedureService,
    private route: ActivatedRoute,
  ) { }

  ngOnInit(): void {
  }

  addInstitution() {
    this.dpService.isEditMode = false
    const ref = this.dialogService.open(DivideMIModalPreviewComponent, {
      data: {
        mi: {} as MunicipalInstitution,
        submitLabel: 'Добави',
        clickCallback: (mi) => {
          this.dpService.addMIToCreate(mi);
          ref.destroy();
        },
      },
      header: 'Създай институция',
      width: '70%',
    });
  }

  editInstitution(institution, rowIndex) {
    this.dpService.isEditMode = true
    const ref = this.dialogService.open(DivideMIModalPreviewComponent, {
      data: {
        mi: institution as MunicipalInstitution,
        submitLabel: 'Запази',
        rowIndex,
        clickCallback: (mi) => {
          this.dpService.updateMIToCreate(mi, rowIndex);
          ref.destroy();
        },
      },
      header: 'Създай институция',
      width: '70%',
    });
  }

  removeInstitution(mi) {
    this.dpService.removeMIToCreate(mi);
  }
}
