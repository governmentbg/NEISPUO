import { Component, OnInit, ViewChild } from '@angular/core';
import {
  ActivatedRoute, Router,
} from '@angular/router';
import { ProcedureType } from '@municipal-institutions/models/procedure-type';
import { RIProcedure } from '@municipal-institutions/models/ri-procedure';
import { MunicipalInstitution } from '@municipal-institutions/state/municipal-institutions/municipal-institution.interface';
import { ProcedureTypeEnum } from '@procedures/models/procedure-type.enum';
import { ProcedureEnum } from '@procedures/models/procedure.enum';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ScrollService } from '@shared/services/utils/scroll.service';
import { UtilsService } from '@shared/services/utils/utils.service';
import { DivideMiPreviewDeleteComponent } from '../../components/divide-mi-preview-delete/divide-mi-preview-delete.component';
import { DivideProcedureQuery } from '../../state/divide-procedure.query';
import { DivideProcedureService } from '../../state/divide-procedure.service';
import { TransformType } from '@municipal-institutions/models/transform-type';
import { TransformTypeEnum } from '@procedures/models/transform-type.enum';

@Component({
  selector: 'app-divide-mi-to-delete',
  templateUrl: './divide-mi-to-delete.component.html',
  styleUrls: ['./divide-mi-to-delete.component.scss'],
})
export class DivideMIToDeleteComponent implements OnInit {
  @ViewChild(DivideMiPreviewDeleteComponent) divideMiToUpdatePreviewComponent: DivideMiPreviewDeleteComponent;

  miToDelete$: Observable<MunicipalInstitution> = this.dpQuery.miToDelete$;

  activeMIs$ = this.dpQuery.activeMIs$
    .pipe(
      map(
        (mis) => mis.filter((mi) => !!mi.InstitutionID).map((mi) => ({ ...mi, DisplayValue: `${mi?.InstitutionID} ${mi.Abbreviation}` })),
      ),
    );

  mode: ProcedureEnum = this.route.snapshot.data.type;

  procedureDescriptionText: string;

  miPickerLabel: string = 'Избор на институция';

  constructor(
    private dpQuery: DivideProcedureQuery,
    private dpService: DivideProcedureService,
    private router: Router,
    private route: ActivatedRoute,
    private scrollService: ScrollService,
    private utilsService: UtilsService,
  ) { }

  ngOnInit(): void {
    this.procedureDescriptionText = `При процедура
        "Преобразуване чрез разделяне" избраната институция ще бъде закрита, а нейно място ще бъдат създадени нови.`;
    this.miPickerLabel = 'Изберете институция за разделяне';
  }

  onMIToDeleteChange(event) {
    const deleteProcedureTypeObj = {
      ProcedureType: {
        ProcedureTypeID: ProcedureTypeEnum.DELETE,
      } as ProcedureType,
      TransformType: {
        TransformTypeID: TransformTypeEnum.DIVIDE
      } as TransformType,
      RICPLRArea: {
        CPLRAreaType: event.value?.RIProcedure?.RICPLRArea?.CPLRAreaType
      },
    };
    if (event.value) {
      event.value.RIProcedure = deleteProcedureTypeObj as RIProcedure;
    }

    this.dpService.updateMIToDelete(event.value);
  }

  forwardCallback = (e: any) => {
    this.utilsService.markAllAsDirty(this.divideMiToUpdatePreviewComponent.form);

    if (this.divideMiToUpdatePreviewComponent.form.invalid) {
      this.scrollService.scrollToFirstError();
      return;
    }
    this.dpService.updateMIToDelete(this.divideMiToUpdatePreviewComponent.form.getRawValue());
    this.router.navigate(['..', 'mis-to-create'], { relativeTo: this.route });
  };
}
