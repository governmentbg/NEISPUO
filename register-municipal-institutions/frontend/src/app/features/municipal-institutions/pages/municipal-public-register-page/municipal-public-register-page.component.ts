import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MIListColumnService } from '@municipal-institutions/state/municipal-institutions/municipal-institution-list-column.service';
import { MunicipalInstitution } from '@municipal-institutions/state/municipal-institutions/municipal-institution.interface';
import { MunicipalInstitutionQuery } from '@municipal-institutions/state/municipal-institutions/municipal-institution.query';
import { MunicipalInstitutionService } from '@municipal-institutions/state/municipal-institutions/municipal-institution.service';
import { MunicipalInstitutionStore } from '@municipal-institutions/state/municipal-institutions/municipal-institution.store';
import { DotNotatedPipe } from '@shared/modules/pipes/dot-notated.pipe';
import { LazyLoadEvent, SelectItem } from 'primeng/api';
import { Table, ColumnFilter } from 'primeng/table';
import { AuthQuery } from '../../../../core/authentication/auth-state-manager/auth.query';

@Component({
  selector: 'app-municipal-public-register-page',
  templateUrl: './municipal-public-register-page.component.html',
  styleUrls: ['./municipal-public-register-page.component.scss'],
})
export class MunicipalPublicRegisterPageComponent implements OnInit {
  @ViewChild('miTable') miTable: Table;

  @ViewChild('colFilter') colFilter: ColumnFilter;

  loading: boolean;

  constructor(
    private miQuery: MunicipalInstitutionQuery,
    private miService: MunicipalInstitutionService,
    public columnService: MIListColumnService,
    public miStore: MunicipalInstitutionStore,
    private miColumnService: MIListColumnService,
    private router: Router,
    private route: ActivatedRoute,
    private dotNotatedPipe: DotNotatedPipe,
    public authQuery: AuthQuery,
  ) { }

  institutions$ = this.miQuery.institutions$;

  isLoading$ = this.miQuery.isLoading$;

  total$ = this.miQuery.total$;

  cols: any[] = [
    { field: 'InstitutionID', header: 'Код по НЕИСПУО' },
    { field: 'Name', header: 'Име' },
    { field: 'town_municipality_region.Name', header: 'Област' },
    { field: 'town_municipality.Name', header: 'Община' },
    { field: 'Town.Name', header: 'Населено място' },
  ];

  expandedCols: any[] = [
    { field: 'BaseSchoolType.Name', header: 'Вид според подготовка' },
    { field: 'DetailedSchoolType.Name', header: 'Детайлен вид' },
    { field: 'Bulstat', header: 'Булстат/ЕИК' },
    { field: 'TRAddress', header: 'Адрес' },
    { field: 'TRPostCode', header: 'Пощенски код' },
  ];
  flexFieldsCols: any[] = [
    { field: 'RIFlexFieldValues.RIFlexField', header: 'Флекс Полета' }
  ];

  matchModeOptions: SelectItem[] = [
    { label: 'Съдържа', value: '$cont' },
  ];

  ngOnInit(): void {
  }

  ngAfterViewInit() {
    this.loadMIsByUrl();
  }

  loadMIsByUrl() {
    this.route.queryParams.subscribe((params) => {
      const event: LazyLoadEvent = params.tableMeta
        ? JSON.parse(params.tableMeta)
        : this.miTable.createLazyLoadMetadata();
      Object.assign(this.miTable, event);
      this.loadMIs(event);
    });
  }

  navigate(event) {
    this.router.navigate([], { queryParams: { tableMeta: JSON.stringify(event) } });
  }

  async loadMIs(event: LazyLoadEvent) {
    // this.scrollService.scrollTo('body');
    await this.miService.get('/v1/ri-institution-public', this.miColumnService.createQueryParams(event));
  }

  getDisplayValue(mi: MunicipalInstitution, dotNotatedProperty: string) {
    if (dotNotatedProperty === 'town_municipality.Name') {
      return mi.Town.Municipality.Name;
    } if (dotNotatedProperty === 'town_municipality_region.Name') {
      return mi.Town.Municipality.Region.Name;
    }
    return this.dotNotatedPipe.transform([mi, dotNotatedProperty]) || '';
  }

  getDisplayFlexFields(mi: MunicipalInstitution, dotNotatedProperty: string) {
    if (dotNotatedProperty === 'RIFlexFieldValues.RIFlexField') {
      const flexFields = [];
      mi.RIFlexFieldValues.forEach(mi => {
        flexFields.push(`${mi.RIFlexField.Data.label}: ${mi.Value}`)
      });

      return flexFields
    }

    return this.dotNotatedPipe.transform([mi, dotNotatedProperty]) || '';
  }
}