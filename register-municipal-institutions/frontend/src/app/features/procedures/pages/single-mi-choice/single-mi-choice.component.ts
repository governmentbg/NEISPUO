import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { RIInstitution } from '@municipal-institutions/models/ri-institution';
import { MunicipalInstitution } from '@municipal-institutions/state/municipal-institutions/municipal-institution.interface';
import { ProcedureEnum } from '@procedures/models/procedure.enum';
import { ProcedureQuery } from '@procedures/state/procedure/procedure.query';
import { ProcedureService } from '@procedures/state/procedure/procedure.service';
import { ProcedureState } from '@procedures/state/procedure/procedure.store';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

@Component({
  selector: 'app-single-mi-choice',
  templateUrl: './single-mi-choice.component.html',
  styleUrls: ['./single-mi-choice.component.scss'],
})

export class SingleMiChoiceComponent implements OnInit {
  availableMis$ = this.pQuery.availableMis$
    .pipe(
      map(
        (mis) => mis.filter((mi) => !!mi.InstitutionID).map((mi) => ({ ...mi, DisplayValue: `${mi?.InstitutionID} ${mi.Abbreviation}` })),
      ),
    );

  selectedMI: RIInstitution = null;

  mode: ProcedureEnum = this.route.snapshot.data.type;

  procedureDescriptionText: string;

  miPickerLabel: string = 'Избор на институция';

  procedureStorePropertyName: keyof ProcedureState;

  displayRIPremInstitution: boolean = false;

  disableProcedureFields: boolean = false;

  storedMi$: Observable<MunicipalInstitution>;

  updateStoreCallback() {
    /**
     * Issue with persisting object when making changes as the selected object / store are not completed
     */
    if (this.mode === ProcedureEnum.DIVIDE) {
      this.selectedMI.RIProcedure.ProcedureDate = new Date();
      this.selectedMI.RIProcedure.YearDue = 2021;
      this.selectedMI.RIProcedure.TransformDetails = 'Заличаване чрез разделяне';
      this.displayRIPremInstitution = true;
      this.selectedMI.RIProcedure.RIPremInstitution.PremStudents = 'Обучението на децата ще се осъществява в съответните населени места, като групите функционират като друг адрес, на който се осъществява обучението';
      this.selectedMI.RIProcedure.RIPremInstitution.PremDocs = 'Задължителната документация се приема и съхранява от приемащата детска градина ДГ №2 "Здравец" гр. Петрич код по админ 100702';
      this.selectedMI.RIProcedure.RIPremInstitution.PremInventory = 'Имуществото се приема за стопанистване и управление от приемащата детска градина ДГ №2 "Здравец" гр. Петрич код по админ 100702';
    } else if (this.mode === ProcedureEnum.JOIN) {
      this.selectedMI.RIProcedure.TransformDetails = 'Промянва (чрез вливане)';
      this.selectedMI.RIProcedure.RIPremInstitution.PremStudents = 'Обучението на децата ще се осъществява в съответните населени места, като групите функционират като друг адрес, на който се осъществява обучението';
      this.selectedMI.RIProcedure.RIPremInstitution.PremDocs = 'Задължителната документация се приема и съхранява от приемащата детска градина ДГ №2 "Здравец" гр. Петрич код по админ 100702';
      this.selectedMI.RIProcedure.RIPremInstitution.PremInventory = 'Имуществото се приема за стопанистване и управление от приемащата детска градина ДГ №2 "Здравец" гр. Петрич код по админ 100702';
    }
    this.pService.setStoreProperty(this.procedureStorePropertyName, this.selectedMI);
  }

  constructor(
    private pQuery: ProcedureQuery,
    private pService: ProcedureService,
    private route: ActivatedRoute,
  ) { }

  ngOnInit(): void {
    this.setupComponent(this.mode);

    this.storedMi$.subscribe((mi) => {
      this.selectedMI = mi;
    });
  }

  setupComponent(mode: ProcedureEnum) {
    if (mode === ProcedureEnum.DIVIDE) {
      this.procedureDescriptionText = `При процедура
      "Преобразуване чрез разделяне" съществуващата институция ще бъде закрита и на нейно място ще бъдат открити нови.`;
      this.miPickerLabel = 'Изберете институция за преобразуване чрез разделяне';
      this.procedureStorePropertyName = 'divideMiToDelete';
      this.storedMi$ = this.pQuery.select(this.procedureStorePropertyName);
      this.disableProcedureFields = false;
    } else if (mode === ProcedureEnum.DETACH) {
      this.procedureDescriptionText = `При процедура
      "Преобразуване чрез отделяне" съществуващата институция ще се запази и ще бъдат открити нови, които се отделят от съществуващата.`;
      this.miPickerLabel = 'Изберете институция, от която ще отделяте';
      this.procedureStorePropertyName = 'detachMiToUpdate';
      this.storedMi$ = this.pQuery.select(this.procedureStorePropertyName);
    } else if (mode === ProcedureEnum.MERGE) {
      this.procedureDescriptionText = 'При .';
      this.miPickerLabel = 'Изберете институция, от която ще отделяте';
      this.procedureStorePropertyName = 'detachMiToUpdate';
      this.storedMi$ = this.pQuery.select(this.procedureStorePropertyName);
    } else if (mode === ProcedureEnum.JOIN) {
      this.procedureStorePropertyName = 'joinMiToUpdate';
      this.storedMi$ = this.pQuery.select(this.procedureStorePropertyName);
    }
  }
}
