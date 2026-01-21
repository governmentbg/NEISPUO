import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { MunicipalInstitution } from '@municipal-institutions/state/municipal-institutions/municipal-institution.interface';
import { DialogService } from 'primeng/dynamicdialog';
import { DetachMIModalPreviewComponent } from '../../components/detach-mi-modal-preview/detach-mi-modal-preview.component';
import { DetachProcedureQuery } from '../../state/detach-procedure.query';
import { DetachProcedureService } from '../../state/detach-procedure.service';

@Component({
  selector: 'app-detach-mis-to-create',
  templateUrl: './detach-mis-to-create.component.html',
  styleUrls: ['./detach-mis-to-create.component.scss'],
  providers: [DialogService],
})

export class DetachMIsToCreateComponent implements OnInit {
  cols: any[] = [
    { field: 'Bulstat', header: 'Булстат/ЕИК' },
    { field: 'Name', header: 'Наименование' },
  ];

  misToCreate$ = this.dpQuery.misToCreate$;

  constructor(
    private dpQuery: DetachProcedureQuery,
    private dialogService: DialogService,
    private dpService: DetachProcedureService,
    private route: ActivatedRoute,
  ) { }

  ngOnInit(): void {
  }

  addInstitution() {
    this.dpService.isEditMode = false
    const ref = this.dialogService.open(DetachMIModalPreviewComponent, {
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
    const ref = this.dialogService.open(DetachMIModalPreviewComponent, {
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
