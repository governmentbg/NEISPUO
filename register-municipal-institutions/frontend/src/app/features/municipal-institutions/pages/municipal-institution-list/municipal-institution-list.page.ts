import {
  AfterViewInit, Component, HostListener, OnDestroy, OnInit, ViewChild,
} from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MIListColumnService } from '@municipal-institutions/state/municipal-institutions/municipal-institution-list-column.service';
import { MunicipalInstitution } from '@municipal-institutions/state/municipal-institutions/municipal-institution.interface';
import { MunicipalInstitutionQuery } from '@municipal-institutions/state/municipal-institutions/municipal-institution.query';
import { MunicipalInstitutionService } from '@municipal-institutions/state/municipal-institutions/municipal-institution.service';
import { MunicipalInstitutionStore } from '@municipal-institutions/state/municipal-institutions/municipal-institution.store';
import { DotNotatedPipe } from '@shared/modules/pipes/dot-notated.pipe';
import { FilterService, LazyLoadEvent, SelectItem } from 'primeng/api';
import { ColumnFilter, Table } from 'primeng/table';
import { combineLatest, Observable, Subject } from 'rxjs';
import {
  map, shareReplay, take, takeUntil,
} from 'rxjs/operators';
import { SubSink } from 'subsink';
import { AuthQuery } from '../../../../core/authentication/auth-state-manager/auth.query';

export interface GetManyDefaultResponse<T> {
  data: T[];
  count: number;
  total: number;
  page: number;
  pageCount: number;
}

type PaginatedMunicipalInstitution = GetManyDefaultResponse<MunicipalInstitution>;

@Component({
  selector: 'app-municipal-institution-list',
  templateUrl: './municipal-institution-list.page.html',
  styleUrls: ['./municipal-institution-list.page.scss'],
})
export class MunicipalInstitutionListPage implements OnInit, AfterViewInit, OnDestroy {
  @ViewChild('miTable') miTable: Table;

  @ViewChild('colFilter') colFilter: ColumnFilter;

  loading: boolean;

  subSink = new SubSink();

  paginatedMI: PaginatedMunicipalInstitution = {
    count: 20,
    data: [],
    page: 1,
    pageCount: null,
    total: null,
  };

  municipalityStatusToLoad: Observable<'closed' | 'active'> = this.route.data.pipe(
    map(({ scope }: { scope: 'closed' | 'active' }) => scope),
  );

  isEditButtonVisible$: Observable<boolean> = this.decideEditable();

  isSmallResolution: boolean = false;

  @HostListener('window:resize', ['$event'])
  smallResolution() {
    this.isSmallResolution = window.innerWidth <= 568;
  }

  constructor(
    private miQuery: MunicipalInstitutionQuery,
    private miService: MunicipalInstitutionService,
    private filterService: FilterService,
    private router: Router,
    private route: ActivatedRoute,
    public columnService: MIListColumnService,
    public miStore: MunicipalInstitutionStore,
    private miColumnService: MIListColumnService,
    private dotNotatedPipe: DotNotatedPipe,
    private authQuery: AuthQuery,
  ) {}

  institutions$ = this.miQuery.institutions$;

  isLoading$ = this.miQuery.isLoading$;

  total$ = this.miQuery.total$;

  destroy$: Subject<any> = new Subject<any>();

  cols: any[] = [
    { field: 'InstitutionID', header: 'Код по НЕИСПУО' },
    { field: 'Name', header: 'Име' },
    { field: 'town_municipality_region.Name', header: 'Област' },
    { field: 'town_municipality.Name', header: 'Община' },
    { field: 'Town.Name', header: 'Населено място' },
    // { field: 'RIProcedure.ProcedureType.Name', header: 'Последна процедура' },
  ];

  matchModeOptions: SelectItem[] = [{ label: 'Съдържа', value: '$cont' }];

  isAdmin = false;

  ngOnInit(): void {
    this.smallResolution();
    this.authQuery.isMon$
      .pipe(takeUntil(this.destroy$))
      .subscribe((isMon) => {
        this.isAdmin = isMon;
      });
  }

  ngAfterViewInit() {
    this.loadMIsByUrl();
  }

  private decideEditable() {
    return combineLatest([
      this.authQuery.isMon$,
      this.authQuery.isRuo$,
      this.authQuery.isMunicipality$,
      this.municipalityStatusToLoad,
    ]).pipe(
      map(([isMon, isRuo, isMunicipality, scope]) => {
        this.isAdmin = isMon;
        if (isMon || isRuo || scope === 'closed') {
          return false;
        }
        return isMunicipality; // always true in current logic
      }),
      shareReplay(1),
    );
  }

  loadMIsByUrl() {
    this.route.queryParams.subscribe((params) => {
      const event: LazyLoadEvent = params.tableMeta ? JSON.parse(params.tableMeta) : this.miTable.createLazyLoadMetadata();
      Object.assign(this.miTable, event);
      this.loadMIs(event);
    });
  }

  navigate(event) {
    this.router.navigate([], { queryParams: { tableMeta: JSON.stringify(event) } });
  }

  async loadMIs(event: LazyLoadEvent) {
    // this.scrollService.scrollTo('body');
    const municipalityStatusToLoad = await this.municipalityStatusToLoad.pipe(take(1)).toPromise();
    const url = municipalityStatusToLoad === 'closed' ? '/v1/ri-institution-closed' : '/v1/ri-institution-latest-active';
    await this.miService.get(url, this.miColumnService.createQueryParams(event));
  }

  getDisplayValue(mi: MunicipalInstitution, col: string) {
    if (col === 'town_municipality.Name') {
      return mi.Town.Municipality.Name;
    } if (col === 'town_municipality_region.Name') {
      return mi.Town.Municipality.Region.Name;
    }
    return this.dotNotatedPipe.transform([mi, col]) || '';
  }

  showCol(col) {
    return (this.isAdmin && (col.field === 'town_municipality_region.Name' || col.field === 'town_municipality.Name'))
    || (col.field !== 'town_municipality_region.Name' && col.field !== 'town_municipality.Name');
  }

  ngOnDestroy() {
    this.subSink.unsubscribe();
    this.destroy$.next();
    this.destroy$.complete();
  }
}
