import { Observable, of } from 'rxjs';

export interface INomVO<I> {
  id: NonNullable<I>;
  name: string;
  badge?: string | null;
}

export type GetNomsByIdParams<I> = {
  ids: Array<NonNullable<I>>;
};

export type GetNomsByTermParams = {
  term?: string | null;
  offset?: number | null;
  limit?: number | null;
};

export enum YesNoValues {
  Yes = 'YES',
  No = 'NO'
}

export interface INomService<I, P extends object> {
  getNomsById(params: GetNomsByIdParams<I> & P): Observable<Array<INomVO<I>>>;
  getNomsByTerm(params: GetNomsByTermParams & P): Observable<Array<INomVO<I>>>;
}

export class NomServiceWithParams<I, P extends object> implements INomService<I, Record<string, unknown>> {
  constructor(private dataService: INomService<I, P>, private paramsFunc: () => P) {}

  getNomsById(params: GetNomsByIdParams<I>): Observable<Array<INomVO<I>>> {
    return this.dataService.getNomsById(Object.assign({}, this.paramsFunc.apply(null), params));
  }
  getNomsByTerm(params: GetNomsByTermParams): Observable<Array<INomVO<I>>> {
    return this.dataService.getNomsByTerm(Object.assign({}, this.paramsFunc.apply(null), params));
  }
}

export class NomServiceFromItems<I> implements INomService<I, Record<string, unknown>> {
  constructor(private items: Array<INomVO<I>>) {}

  getNomsById(params: GetNomsByIdParams<I>): Observable<Array<INomVO<I>>> {
    return of(this.items.filter((i) => params.ids.indexOf(i.id) !== -1));
  }
  getNomsByTerm({ term }: GetNomsByTermParams): Observable<Array<INomVO<I>>> {
    const words = term?.split(/\s+/);
    if (!words?.length) {
      return of(this.items);
    }

    return of(this.items.filter((i) => words.every((w) => i.name.toLowerCase().includes(w.toLowerCase()))));
  }
}

export class YesNoNomsService extends NomServiceFromItems<string> {
  constructor() {
    super([
      { id: YesNoValues.Yes, name: 'Да' },
      { id: YesNoValues.No, name: 'Не' }
    ]);
  }
}
