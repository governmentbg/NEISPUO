import {
  Component, OnInit, QueryList, ViewChildren,
} from '@angular/core';
import { ProcedureType } from '@municipal-institutions/models/procedure-type';
import { RIProcedure } from '@municipal-institutions/models/ri-procedure';
import { MunicipalInstitution } from '@municipal-institutions/state/municipal-institutions/municipal-institution.interface';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ActivatedRoute, Router } from '@angular/router';
import { ScrollService } from '@shared/services/utils/scroll.service';
import { UtilsService } from '@shared/services/utils/utils.service';
import { ProcedureTypeEnum } from '../../../procedures/models/procedure-type.enum';
import { MergeMiPreviewDeleteComponent } from '../../components/merge-mi-preview-delete/merge-mi-preview-delete.component';
import { MergeProcedureQuery } from '../../state/merge-procedure.query';
import { MergeProcedureService } from '../../state/merge-procedure.service';
import { TransformTypeEnum } from '@procedures/models/transform-type.enum';
import { TransformType } from '@municipal-institutions/models/transform-type';

@Component({
  selector: 'app-merge-mis-to-delete',
  templateUrl: './merge-mis-to-delete.component.html',
  styleUrls: ['./merge-mis-to-delete.component.scss'],
})
export class MergeMIsToDeleteComponent implements OnInit {
  @ViewChildren(MergeMiPreviewDeleteComponent) mergeMiPreviewDeleteComponents: QueryList<MergeMiPreviewDeleteComponent>;

  selectedMIs: MunicipalInstitution[];

  misToDelete$: Observable<MunicipalInstitution[]> = this.mpQuery.misToDelete$;

  activeMIs$ = this.mpQuery.activeMIs$
    .pipe(
      map(
        (mis) => mis.filter((mi) => !!mi.InstitutionID).map((mi) => ({ ...mi, DisplayValue: `${mi?.InstitutionID} ${mi.Abbreviation}` })),
      ),
    );

  procedureDescriptionText: string;

  miPickerLabel: string;

  constructor(
    private mpQuery: MergeProcedureQuery,
    private mpService: MergeProcedureService,
    private scrollService: ScrollService,
    private route: ActivatedRoute,
    private utilsService: UtilsService,
    private router: Router,
  ) { }

  ngOnInit(): void {
    this.procedureDescriptionText = `При процедура
        "Преобразуване чрез сливане" избраните институции ще бъдат закрити и ще се "слеят" в нова институция.`;
    this.miPickerLabel = 'Изберете институции, които ще се слеят';
  }

  onMIsToDeleteChange(event) {
    const valueToUpdate: MunicipalInstitution = event.value.find(
      (v: MunicipalInstitution) => v.RIInstitutionID === (event?.itemValue as MunicipalInstitution)?.RIInstitutionID,
    );
    const deleteProcedureTypeObj = {
      ProcedureType: {
        ProcedureTypeID: ProcedureTypeEnum.DELETE,
      } as ProcedureType,
      TransformType: {
        TransformTypeID: TransformTypeEnum.MERGE
      } as TransformType,
      RICPLRArea: {
        CPLRAreaType: valueToUpdate?.RIProcedure?.RICPLRArea?.CPLRAreaType
      }
    };
    if (valueToUpdate) {
      valueToUpdate.RIProcedure = deleteProcedureTypeObj as RIProcedure;
    }
    this.mpService.updateMIsToDelete(event.value);
  }

  forwardCb = () => {
    for (const mergeMiPreviewDeleteComponent of this.mergeMiPreviewDeleteComponents) {
      this.utilsService.markAllAsDirty(mergeMiPreviewDeleteComponent?.form);
      if (mergeMiPreviewDeleteComponent?.form?.invalid) {
        this.scrollService.scrollToFirstError();
        return;
      }
    }

    const misToDelete = this.mergeMiPreviewDeleteComponents.toArray()
      .map(
        (jmpdc) => {
          const updatedComponentValue = jmpdc.form.getRawValue();
          const miInStore = this.mpQuery.getValue().misToDelete.find((smi) => smi.RIInstitutionID === updatedComponentValue.RIInstitutionID);
          return { ...miInStore, RIProcedure: updatedComponentValue.RIProcedure, CPLRAreaType: miInStore.RIProcedure.RICPLRArea.CPLRAreaType };
        },
      );

    this.router.navigate(['..', 'mi-to-create'], { relativeTo: this.route });
    this.mpService.updateMIsToDelete(misToDelete);
  };
}
