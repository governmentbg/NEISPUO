import {
  Component, OnInit, QueryList, ViewChildren,
} from '@angular/core';
import {
  ActivatedRoute, Router,
} from '@angular/router';
import { ProcedureType } from '@municipal-institutions/models/procedure-type';
import { RIProcedure } from '@municipal-institutions/models/ri-procedure';
import { MunicipalInstitution } from '@municipal-institutions/state/municipal-institutions/municipal-institution.interface';
import { ProcedureTypeEnum } from '@procedures/models/procedure-type.enum';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { ScrollService } from '@shared/services/utils/scroll.service';
import { UtilsService } from '@shared/services/utils/utils.service';
import { JoinMiPreviewDeleteComponent } from '../../components/join-mi-preview-delete/join-mi-preview-delete.component';
import { JoinProcedureQuery } from '../../state/join-procedure.query';
import { JoinProcedureService } from '../../state/join-procedure.service';
import { TransformTypeEnum } from '@procedures/models/transform-type.enum';
import { TransformType } from '@municipal-institutions/models/transform-type';

@Component({
  selector: 'app-join-mis-to-delete',
  templateUrl: './join-mis-to-delete.component.html',
  styleUrls: ['./join-mis-to-delete.component.scss'],
})
export class JoinMIsToDeleteComponent implements OnInit {
  @ViewChildren(JoinMiPreviewDeleteComponent) joinMiPreviewDeleteComponents: QueryList<JoinMiPreviewDeleteComponent>;

  selectedMIs: MunicipalInstitution[];

  misToDelete$: Observable<MunicipalInstitution[]> = this.jpQuery.misToDelete$;

  activeMIs$ = this.jpQuery.activeMIs$
    .pipe(
      map(
        (mis) => mis.filter((mi) => !!mi.InstitutionID).map((mi) => ({ ...mi, DisplayValue: `${mi?.InstitutionID} ${mi.Abbreviation}` })),
      ),
    );

  procedureDescriptionText: string;

  miPickerLabel: string;

  constructor(
    private jpQuery: JoinProcedureQuery,
    private jpService: JoinProcedureService,
    private router: Router,
    private route: ActivatedRoute,
    private scrollService: ScrollService,
    private utilsService: UtilsService,
  ) { }

  ngOnInit(): void {
    this.procedureDescriptionText = `При процедура
    "Преобразуване чрез вливане" избраните институции ще бъдат закрити и ще се "влеят" в избрана от вас институция.`;
    this.miPickerLabel = 'Изберете институции, които ще се влеят';
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
        TransformTypeID: TransformTypeEnum.JOIN
      } as TransformType,
      RICPLRArea: {
        CPLRAreaType: valueToUpdate?.RIProcedure?.RICPLRArea?.CPLRAreaType
      }
    };
    if (valueToUpdate) {
      valueToUpdate.RIProcedure = deleteProcedureTypeObj as RIProcedure;
    }
    this.jpService.updateMIsToDelete(event.value);
  }

  forwardCb = () => {
    for (const joinMiPreviewDeleteComponent of this.joinMiPreviewDeleteComponents) {
      this.utilsService.markAllAsDirty(joinMiPreviewDeleteComponent?.form);
      if (joinMiPreviewDeleteComponent?.form?.invalid) {
        this.scrollService.scrollToFirstError();
        return;
      }
    }
    const misToDelete = this.joinMiPreviewDeleteComponents.toArray()
      .map(
        (jmpdc) => {
          const updatedComponentValue = jmpdc.form.getRawValue();
          const miInStore = this.jpQuery.getValue().misToDelete.find((smi) => smi.RIInstitutionID === updatedComponentValue.RIInstitutionID);
          return { ...miInStore, RIProcedure: updatedComponentValue.RIProcedure, CPLRAreaType: miInStore.RIProcedure.RICPLRArea.CPLRAreaType };
        },
      );

    this.router.navigate(['..', 'mi-to-update'], { relativeTo: this.route });
    this.jpService.updateMIsToDelete(misToDelete);
  };
}
