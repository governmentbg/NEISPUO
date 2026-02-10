import { HttpClient } from '@angular/common/http';
import { Component, OnInit, ViewChild } from '@angular/core';
import { SelectItem } from 'primeng/api';
import { ColumnFilter, Table } from 'primeng/table';
import { filter, take } from 'rxjs/operators';
import { SubSink } from 'subsink';
import { EnvironmentService } from '@core/services/environment.service';
import { DotNotatedPipe } from '../../../../shared/modules/pipes/dot-notated.pipe';
import { AuthQuery } from '../../../../core/authentication/auth-state-manager/auth.query';
import { MunicipalInstitution } from '../../../municipal-institutions/state/municipal-institutions/municipal-institution.interface';
import { FlexFieldDTO } from '../flex-field-detail/flex-field-detail.page';

interface TableColumnDefinition {
  field: string; // accepts dot.notated.properties
  header: string;
  filter?: { type: 'text' | 'single-select'; selectOptions?: SelectItem[] }; // Add other types as necessary
}

@Component({
  selector: 'app-flex-field-list',
  templateUrl: './flex-field-list.page.html',
  styleUrls: ['./flex-field-list.page.scss'],
})
export class FlexFieldListPage implements OnInit {
  @ViewChild('flexFieldTable') flexFieldTable: Table;

  @ViewChild('colFilter') colFilter: ColumnFilter;

  loading = false;

  subSink = new SubSink();

  flexFields: FlexFieldDTO[] = [];

  private readonly environment = this.envService.environment;

  constructor(
    private dotNotatedPipe: DotNotatedPipe,
    private httpClient: HttpClient,
    private authQuery: AuthQuery,
    private envService: EnvironmentService,
  ) {}

  cols: TableColumnDefinition[] = [
    { field: 'Data.label', header: 'Име', filter: { type: 'text' } },
    {
      field: 'Data.type',
      header: 'Вид',
      filter: {
        type: 'single-select',
        selectOptions: [
          { label: 'Число', value: 'number' },
          { label: 'Свободен текст', value: 'text' },
          { label: 'Единичен избор', value: 'single_choice', disabled: true },
          { label: 'Множествен избор', value: 'multiple_select', disabled: true },
        ],
      },
    },
  ];

  ngOnInit(): void {
    this.ensureMunicipalityIsLoaded().subscribe((municipality) => {
      this.loadMunicipalityFlexFields(municipality);
    });
  }

  private ensureMunicipalityIsLoaded() {
    return this.authQuery.myMunicipality$.pipe(
      filter((v) => !!v),
      take(1),
    );
  }

  private loadMunicipalityFlexFields(municipality: any) {
    // this.scrollService.scrollTo('body');

    this.loading = true;
    this.httpClient
      .get<FlexFieldDTO[]>(`${this.environment.BACKEND_URL}/v1/ri-flex-field`)
      .subscribe(
        (s) => {
          this.flexFields = s;
        },
        (e) => console.log(e), // TODO: show helpful messages
      )
      .add(() => (this.loading = false));
  }

  getDisplayValue(mi: MunicipalInstitution, dotNotatedProperty: string) {
    const value = this.dotNotatedPipe.transform([mi, dotNotatedProperty]) || '';

    let displayValue;
    switch (value) {
      case 'number':
        displayValue = 'Число';
        break;
      case 'text':
        displayValue = 'Свободен текст';
        break;

      default:
        // do nothing, return value
        displayValue = value;
        break;
    }
    return displayValue;
  }
}
