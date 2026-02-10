import { Component, OnInit, ViewChild } from '@angular/core';
import { MunicipalInstitution } from '@municipal-institutions/state/municipal-institutions/municipal-institution.interface';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { MIPreviewOnlyComponent } from '../../components/mi-preview-only/mi-preview-only.component';
import { JoinProcedureQuery } from '../../state/join-procedure.query';
import { JoinProcedureService } from '../../state/join-procedure.service';

@Component({
  selector: 'app-join-mi-to-update',
  templateUrl: './join-mi-to-update.component.html',
  styleUrls: ['./join-mi-to-update.component.scss'],
})

export class JoinMIToUpdateComponent implements OnInit {
  @ViewChild(MIPreviewOnlyComponent) joinMiToUpdatePreviewComponent: MIPreviewOnlyComponent;

  selectedMI;

  miToUpdate$: Observable<MunicipalInstitution> = this.jpQuery.miToUpdate$;

  activeMIs$ = this.jpQuery.activeMIs$
    .pipe(
      map(
        (mis) => mis.filter((mi) => !!mi.InstitutionID).map((mi) => ({ ...mi, DisplayValue: `${mi?.InstitutionID} ${mi.Abbreviation}` })),
      ),
    );

  procedureDescriptionText: string;

  miPickerLabel: string = 'Избор на институция';

  constructor(
    private jpQuery: JoinProcedureQuery,
    private jpService: JoinProcedureService,
  ) { }

  ngOnInit(): void {
  }

  onMIToUpdateChange(event) {
    this.selectedMI = event.value;

    this.jpService.updateMIToUpdate(event.value);
  }
}
