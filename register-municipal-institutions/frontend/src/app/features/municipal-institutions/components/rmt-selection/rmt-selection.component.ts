import { Component, OnDestroy, OnInit } from '@angular/core';
import { AbstractControl, ControlContainer, FormGroup } from '@angular/forms';
import { LocalArea } from '@municipal-institutions/models/local-area';
import { Municipality } from '@municipal-institutions/models/municipality';
import { Region } from '@municipal-institutions/models/region';
import { Town } from '@municipal-institutions/models/town';
import { LocalAreaService } from '@municipal-institutions/services/local-area.service';
import { TownsService } from '@municipal-institutions/services/towns.service';
import { NomenclatureQuery } from '@shared/services/common-form-service/nomenclatures-state/nomenclatures.query';
import { combineLatest, Observable } from 'rxjs';
import {
  filter, map, mergeMap, startWith, tap,
} from 'rxjs/operators';
import { SubSink } from 'subsink';

@Component({
  selector: 'app-rmt-selection',
  templateUrl: './rmt-selection.component.html',
  styleUrls: ['./rmt-selection.component.scss'],
})

export class RmtSelectionComponent implements OnInit, OnDestroy {
  public form: FormGroup | AbstractControl;

  subs = new SubSink();

  Regions$: Observable<Region[]> = this.nomenclatureQuery.Regions$;

  Municipalities$: Observable<Municipality[]> = this.nomenclatureQuery.Municipalities$;

  Towns: Town[];

  LocalAreas: LocalArea[];

  selectedRegion$: Observable<Region>;

  selectedMunicipality$: Observable<Municipality>;

  selectedTown$: Observable<Town>;

  constructor(
    private nomenclatureQuery: NomenclatureQuery,
    public cc: ControlContainer,
    private localAreaService: LocalAreaService,
    private townsService: TownsService,
  ) {
  }

  ngOnInit(): void {
    this.form = this.cc.control;
    this.selectedRegion$ = this.form.get('Region').valueChanges;
    this.selectedMunicipality$ = this.form.get('Municipality').valueChanges;
    this.selectedTown$ = this.form.get('Town').valueChanges;

    this.setMunicipalityFiltering();

    this.selectedMunicipality$
      .pipe(
        mergeMap((municipality) => this.townsService.getTowns(municipality)),
      )
      .subscribe((towns) => {
        this.Towns = towns;
      });

    this.selectedTown$
      .pipe(
        mergeMap((town) => this.localAreaService.getLocalAreas(town)),
      )
      .subscribe((localAreas) => {
        this.LocalAreas = localAreas;
      });
  }

  setMunicipalityFiltering() {
    this.Municipalities$ = combineLatest([
      this.nomenclatureQuery.Municipalities$,
      this.selectedRegion$.pipe(startWith(this.form.value.Region)),
    ])
      .pipe(
        filter(([municipalities, sr]) => !!municipalities && !!sr),
        map(([municipalities, sr]) => municipalities.filter((m) => m.Region.RegionID === sr.RegionID)),
        tap(() => console.log(this.form.value)),
      );
  }

  ngOnDestroy(): void {
    this.subs.unsubscribe();
  }
}
