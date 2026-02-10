import { DataSource } from '@angular/cdk/collections';
import { GlobalErrorHandler } from 'projects/shared/other/global-error-handler';
import { asyncScheduler, BehaviorSubject, combineLatest, Observable, of, Subscription } from 'rxjs';
import { catchError, finalize, map, observeOn, startWith, switchMap, tap } from 'rxjs/operators';

export interface TableResult<T> {
  result: T[];
  length: number;
}

export interface Page {
  pageIndex: number;
  pageSize: number;
}

export interface Sort {
  active: string;
  direction: string;
}

export interface ReloadPage {
  hideLoader?: boolean;
  done: boolean;
}

export type ElementType<TResult> = TResult extends TableResult<infer T> ? T : never;

export const DEFAULT_PAGE_SIZE = 50;

export function mapTableResult<T, U>(
  { length, result }: TableResult<T>,
  callbackfn: (value: T, index: number, array: T[]) => U,
  thisArg?: any
): TableResult<U> {
  return {
    length,
    result: result.map(callbackfn, thisArg)
  };
}

export class TableDataSource<TResult> extends DataSource<ElementType<TResult>> {
  private page$ = new BehaviorSubject<Page>({ pageIndex: 0, pageSize: DEFAULT_PAGE_SIZE });
  private sort$ = new BehaviorSubject<Sort>({ active: '', direction: '' });
  private reloadPage$ = new BehaviorSubject<ReloadPage>({ done: true });
  private _pending$ = new BehaviorSubject<number>(0);
  private _length$ = new BehaviorSubject<number>(0);
  private _empty$ = new BehaviorSubject<boolean>(true);
  private _loading$ = new BehaviorSubject<boolean>(false);
  private pageSubscription?: Subscription;
  private sortSubscription?: Subscription;

  public pending$ = this._pending$.asObservable();
  public length$ = this._length$.asObservable();
  public empty$ = this._empty$.asObservable();
  public loading$ = this._loading$.asObservable();

  constructor(
    private fetch: (
      sortBy: string,
      sortDirection: string,
      offset: number,
      limit: number
    ) => Observable<TableResult<ElementType<TResult>>>
  ) {
    super();
  }

  connect(): Observable<ElementType<TResult>[]> {
    return combineLatest([this.page$, this.sort$, this.reloadPage$]).pipe(
      observeOn(asyncScheduler), // we must not change pending$ synchronously on connect() as it trips angular change detection
      switchMap(([page, sort, reloadPage]) => {
        // copy the reloadPage info and set it to done
        const reloadingDone = reloadPage.done;
        const hideLoader = reloadPage.hideLoader;
        reloadPage.done = true;

        const incr = () => {
          if (reloadingDone || !hideLoader) {
            this.incrementPending();
            this._loading$.next(true);
          }
        };
        const decr = () => {
          if (reloadingDone || !hideLoader) {
            this.decrementPending();
            this._loading$.next(false);
          }
        };

        incr();
        return this.fetch(sort.active, sort.direction, page.pageIndex * page.pageSize, page.pageSize).pipe(
          tap((r) => {
            this._length$.next(r.length);
            this._empty$.next(r.length === 0);
          }),
          map((r) => r.result),
          catchError((err) => {
            GlobalErrorHandler.instance.handleError(err);
            this._length$.next(0);
            this._empty$.next(true);
            return of([]);
          }),
          finalize(() => decr())
        );
      }),
      startWith([]) // immediately return an empty
    );
  }

  disconnect() {
    if (this.pageSubscription) {
      this.pageSubscription.unsubscribe();
    }
    if (this.sortSubscription) {
      this.sortSubscription.unsubscribe();
    }

    this._pending$.complete();
    this._length$.complete();
    this._loading$.complete();
  }

  reload() {
    const { pageSize } = this.page$.getValue();
    this.page$.next({ pageIndex: 0, pageSize });
  }

  reloadPage(hideLoader?: boolean) {
    this.reloadPage$.next({ done: false, hideLoader });
  }

  attachPaginator(paginator$: Observable<Page>) {
    if (this.pageSubscription) {
      this.pageSubscription.unsubscribe();
    }
    this.pageSubscription = paginator$.subscribe(this.page$);
  }

  attachSort(sort$: Observable<Sort>) {
    if (this.sortSubscription) {
      this.sortSubscription.unsubscribe();
    }
    this.sortSubscription = sort$.subscribe(this.sort$);
  }

  private incrementPending = () => this._pending$.next(this._pending$.value + 1);

  private decrementPending = () => this._pending$.next(Math.max(0, this._pending$.value - 1)); // TODO fix loading of filtered fetch items
}
