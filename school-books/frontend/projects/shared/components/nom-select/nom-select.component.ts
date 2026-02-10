import { Component, Injector, Input, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { NG_VALUE_ACCESSOR } from '@angular/forms';
import { NgSelectComponent } from '@ng-select/ng-select';
import { GlobalErrorHandler } from 'projects/shared/other/global-error-handler';
import { deepEqual } from 'projects/shared/utils/various';
import { BehaviorSubject, of, Subject, Subscription, throwError } from 'rxjs';
import {
  catchError,
  debounceTime,
  distinctUntilChanged,
  exhaustMap,
  map,
  switchMap,
  takeUntil,
  tap
} from 'rxjs/operators';
import { BaseField } from '../base-field';
import { INomService, INomVO } from './nom-service';

export const ALL_GROUP_ID = 'ALL';
const ALL_GROUP_NAME = 'Всички';
const ALL_GROUP_ITEM = {
  id: ALL_GROUP_ID,
  name: ALL_GROUP_NAME
};

const SLICE_SIZE = 20;

@Component({
  selector: 'sb-nom-select',
  templateUrl: './nom-select.component.html',
  // eslint-disable-next-line @angular-eslint/no-inputs-metadata-property
  inputs: ['label', 'validations', 'readonly', 'hint'],
  providers: [
    {
      provide: NG_VALUE_ACCESSOR,
      useExisting: NomSelectComponent,
      multi: true
    }
  ]
})
export class NomSelectComponent extends BaseField implements OnInit, OnDestroy {
  @Input() nomService!: INomService<any, Record<string, unknown>>;
  @Input() multiple = false;
  @Input() complexValue = false;
  @Input() searchable = true;
  @Input() showAllGroup = false;
  @Input() params: Record<string, unknown> = {};
  @Input() theme: 'default' | 'material' = 'material';

  @ViewChild(NgSelectComponent, { static: true }) ngSelectComponent!: NgSelectComponent;

  readonly deepEqual = deepEqual;

  typeahead$ = new Subject<string>();
  items = [] as INomVO<any>[];
  loading = false;

  private hasMore = true;
  private fetchItemsSubscription?: Subscription;
  private slice$?: BehaviorSubject<[number, number]>;
  private sliceSize = SLICE_SIZE;

  allGroup = () => ALL_GROUP_NAME;
  allGroupValue = () => ALL_GROUP_ITEM;

  constructor(injector: Injector) {
    super(injector);
  }

  ngOnInit() {
    if (this.nomService == null) {
      throw new Error('Required Input nomService is null or undefined.');
    }
    if (this.label == null) {
      throw new Error('Required Input label is null or undefined.');
    }
    if (this.showAllGroup && !this.multiple) {
      throw new Error('When "showAllGroup" is true, "multiple" must also be true.');
    }

    if (this.showAllGroup) {
      // unfortunately when "hideSelected" is enabled and we select a few items
      // we no longer have a scroll and we cannot trigger the "onScrollToEnd" method to fetch the rest
      // so we must load all items on open (or at least the first 1000)
      this.sliceSize = 1000;
    }

    if (this.complexValue || this.showAllGroup) {
      // monkey patch the findItem function, to take into account the bindValue and search groups as well
      this.ngSelectComponent.itemsList.findItem = (value: any): any => {
        const compare = this.complexValue ? this.ngSelectComponent.compareWith : (a: any, b: any) => a === b;
        return this.ngSelectComponent.itemsList.items.find((item) =>
          compare(this.ngSelectComponent.itemsList.resolveNested(item.value, this.ngSelectComponent.bindValue), value)
        );
      };
    }

    if (this.showAllGroup) {
      // monkey patch the mapSelectedItems function, to apply _hideSelected for all selected items
      // instead of it's current broken logic, which doesn't correctly support groups
      const originalMapSelectedItems = this.ngSelectComponent.itemsList.mapSelectedItems.bind(
        this.ngSelectComponent.itemsList
      );

      this.ngSelectComponent.itemsList.mapSelectedItems = () => {
        originalMapSelectedItems();
        const _this = this.ngSelectComponent.itemsList as any;

        if (_this._ngSelect.hideSelected) {
          for (const selectItem of _this.selectedItems) {
            _this._hideSelected(selectItem);
          }
        }
      };
    }

    super.ngOnInit();
  }

  ngOnDestroy() {
    super.ngOnDestroy();
  }

  fetchItems() {
    return this.typeahead$.pipe(
      debounceTime(200),
      distinctUntilChanged(),
      switchMap((term) => {
        let items: INomVO<any>[] = [];
        this.items = [];
        this.hasMore = true;
        this.slice$ = new BehaviorSubject([0, this.sliceSize - 1]);
        return this.slice$.pipe(
          exhaustMap(([start, end]) => {
            const offset = start;
            const limit = end - start + 1;

            if (limit <= 0) {
              return of(items);
            }

            this.loading = true;

            return this.nomService.getNomsByTerm(Object.assign({}, this.params, { term, offset, limit })).pipe(
              map((noms) => {
                items = items.concat(noms);
                this.items = items;
                this.hasMore = noms.length === limit;
                this.loading = false;

                return items;
              }),
              catchError((err) => {
                //TODO: show the error in an error template inside the ng-select
                GlobalErrorHandler.instance.handleError(err, true);
                this.loading = false;

                return throwError(err);
              })
            );
          })
        );
      }),
      takeUntil(this.destroyed$)
    );
  }

  onOpened() {
    if (!this.fetchItemsSubscription || this.fetchItemsSubscription.closed) {
      this.fetchItemsSubscription = this.fetchItems().subscribe();
    }

    this.typeahead$.next('');
  }

  onClosed() {
    this.typeahead$.next('');
  }

  onScrollToEnd() {
    if (!this.slice$ || !this.hasMore || this.loading) {
      return;
    }

    const [, end] = this.slice$.getValue();
    this.slice$.next([end + 1, end + this.sliceSize]);
  }

  writeValue(value: any): void {
    if ((this.multiple && value && (value as []).length) || (!this.multiple && value != null)) {
      if (this.multiple && this.showAllGroup && value.includes(ALL_GROUP_ID)) {
        this.nomService
          .getNomsByTerm(Object.assign({}, this.params, { term: '', offset: 0, limit: this.sliceSize }))
          .pipe(
            tap((noms) => {
              this.items = noms;
            }),
            catchError((err) => {
              //TODO: show the error in an error template inside the ng-select
              GlobalErrorHandler.instance.handleError(err, true);
              return of([]);
            }),
            takeUntil(this.destroyed$)
          )
          .subscribe();

        return;
      }

      this.nomService
        .getNomsById(Object.assign({}, this.params, { ids: this.multiple ? value : [value] }))
        .pipe(
          tap((noms) => {
            this.items = noms;

            if (this.complexValue && noms?.length > 0) {
              // when the value is a complex object, ng-select doesn't treat it as the bindValue ('id') and uses it instead as the whole item
              // consequently when the new items are set, it cannot find the matching item, so we must set the selected items manually
              this.ngSelectComponent.writeValue(this.multiple ? noms : noms[0]);
            }
          }),
          catchError((err) => {
            //TODO: show the error in an error template inside the ng-select
            GlobalErrorHandler.instance.handleError(err, true);
            return of([]);
          }),
          takeUntil(this.destroyed$)
        )
        .subscribe();
    }

    super.writeValue(value);
  }
}
