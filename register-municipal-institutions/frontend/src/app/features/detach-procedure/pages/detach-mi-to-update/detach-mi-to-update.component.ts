import { Component, OnInit, ViewChild } from '@angular/core';
import {
  ActivatedRoute, Router,
} from '@angular/router';
import { ProcedureType } from '@municipal-institutions/models/procedure-type';
import { RIProcedure } from '@municipal-institutions/models/ri-procedure';
import { TransformType } from '@municipal-institutions/models/transform-type';
import { MunicipalInstitution } from '@municipal-institutions/state/municipal-institutions/municipal-institution.interface';
import { ProcedureTypeEnum } from '@procedures/models/procedure-type.enum';
import { TransformTypeEnum } from '@procedures/models/transform-type.enum';
import { ScrollService } from '@shared/services/utils/scroll.service';
import { UtilsService } from '@shared/services/utils/utils.service';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { DetachMiPreviewUpdateComponent } from '../../components/detach-mi-preview-update/detach-mi-preview-update.component';
import { DetachProcedureQuery } from '../../state/detach-procedure.query';
import { DetachProcedureService } from '../../state/detach-procedure.service';

@Component({
  selector: 'app-detach-mi-to-update',
  templateUrl: './detach-mi-to-update.component.html',
  styleUrls: ['./detach-mi-to-update.component.scss'],
})

export class DetachMIToUpdateComponent implements OnInit {
  @ViewChild(DetachMiPreviewUpdateComponent) detachMiToUpdatePreviewComponent: DetachMiPreviewUpdateComponent;

  miToUpdate$: Observable<MunicipalInstitution> = this.dpQuery.miToUpdate$;

  activeMIs$ = this.dpQuery.activeMIs$
    .pipe(
      map(
        (mis) => mis.filter((mi) => !!mi.InstitutionID).map((mi) => ({ ...mi, DisplayValue: `${mi?.InstitutionID} ${mi.Abbreviation}` })),
      ),
    );

  procedureDescriptionText: string;

  miPickerLabel: string = 'Избор на институция';

  constructor(
    private dpQuery: DetachProcedureQuery,
    private dpService: DetachProcedureService,
    private router: Router,
    private route: ActivatedRoute,
    private scrollService: ScrollService,
    private utilsService: UtilsService,
  ) { }

  ngOnInit(): void {
    this.procedureDescriptionText = `При процедура
        "Преобразуване чрез отделяне" избраната институция ще бъде актуализирана и ще бъдат създадени нови.`;
    this.miPickerLabel = 'Изберете институция, от която ще отделяте';
  }

  onMIToUpdateChange(event) {
    const deleteProcedureTypeObj = {
      ProcedureType: {
        ProcedureTypeID: ProcedureTypeEnum.UPDATE,
      } as ProcedureType,
      TransformType: {
        TransformTypeID: TransformTypeEnum.DETACH,
      } as TransformType,
      RICPLRArea: {
        CPLRAreaType: event.value?.RIProcedure?.RICPLRArea?.CPLRAreaType
      },
    };
    if (event.value) {
      event.value.RIProcedure = deleteProcedureTypeObj as RIProcedure;
    }
    this.dpService.updateMIToUpdate(event.value);
  }

  forwardCallback = (e: any) => {
    this.utilsService.markAllAsDirty(this.detachMiToUpdatePreviewComponent.form);

    if (this.detachMiToUpdatePreviewComponent.form.invalid) {
      this.scrollService.scrollToFirstError();
      return;
    }
    this.dpService.updateMIToUpdate(this.detachMiToUpdatePreviewComponent.form.getRawValue());
    this.router.navigate(['..', 'mis-to-create'], { relativeTo: this.route });
  };
}
