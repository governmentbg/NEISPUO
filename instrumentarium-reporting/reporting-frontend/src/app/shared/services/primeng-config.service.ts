import { Injectable } from '@angular/core';
import { CONSTANTS } from '@shared/constants';
import { PrimeNGConfig, FilterMatchMode, FilterService } from 'primeng/api';

@Injectable({
  providedIn: 'root'
})
export class PrimengConfigService {
  constructor(private config: PrimeNGConfig, private filterService: FilterService) {}

  public filterMatchModeConfig() {
    this.config.filterMatchModeOptions = {
      text: [
        FilterMatchMode.EQUALS,
        FilterMatchMode.NOT_EQUALS,
        FilterMatchMode.CONTAINS,
        FilterMatchMode.NOT_CONTAINS,
        FilterMatchMode.STARTS_WITH,
        FilterMatchMode.ENDS_WITH
      ],
      numeric: [
        FilterMatchMode.EQUALS,
        FilterMatchMode.NOT_EQUALS,
        FilterMatchMode.LESS_THAN,
        FilterMatchMode.LESS_THAN_OR_EQUAL_TO,
        FilterMatchMode.GREATER_THAN,
        FilterMatchMode.GREATER_THAN_OR_EQUAL_TO
      ],
      date: [
        FilterMatchMode.DATE_IS,
        FilterMatchMode.DATE_IS_NOT,
        FilterMatchMode.BETWEEN,
        CONSTANTS.CUSTOM_MATCH_MODE.NOT_BETWEEN,
        FilterMatchMode.DATE_BEFORE,
        FilterMatchMode.DATE_AFTER
      ]
    };
  }

  public getfilterMatchModeOptions() {
    return this.config.filterMatchModeOptions;
  }

  public getDefaultMatchMode(fieldType) {
    if (fieldType === 'text') {
      return FilterMatchMode.CONTAINS;
    } else if (fieldType === 'time') {
      return FilterMatchMode.DATE_IS;
    } else {
      return FilterMatchMode.EQUALS;
    }
  }

  public customFiltersConfig() {
    this.filterService.register(CONSTANTS.CUSTOM_MATCH_MODE.NOT_BETWEEN, (value: any, filter: any): boolean => {
      if (filter == null || filter[0] == null || filter[1] == null) {
        return true;
      }
      if (value === undefined || value === null) {
        return false;
      }
      if (value.getTime) return filter[0].getTime() >= value.getTime() || filter[1].getTime() <= value.getTime();
      else return filter[0] >= value || filter[1] <= value;
    });
  }

  public primengTranslateConfig() {
    this.config.setTranslation({
      startsWith: 'Започва с',
      contains: 'Съдържа',
      notContains: 'Не съдържа',
      endsWith: 'Завършва с',
      equals: 'Равно на',
      notEquals: 'Не е равно на',
      noFilter: 'Изчисти филтър',
      lt: 'По-малко от',
      lte: 'По-малко или равно на',
      gt: 'По-голямо от',
      gte: 'По-голямо или равно на',
      is: 'Е',
      isNot: 'Не е',
      before: 'Преди',
      after: 'След',
      dateIs: 'Датата е',
      dateIsNot: 'Датата не е',
      dateBefore: 'Датата е преди',
      dateAfter: 'Датата е след',
      between: 'Датата е от - до',
      notBetween: 'Датата не е от - до',
      dayNames: ['Неделя', 'Понеделник', 'Вторник', 'Сряда', 'Четвъртък', 'Петък', 'Събота'],
      dayNamesShort: ['Нед', 'Пон', 'Втор', 'Ср', 'Четв', 'Пет', 'Съб'],
      dayNamesMin: ['Нд', 'Пн', 'Вт', 'Ср', 'Чт', 'Пт', 'Сб'],
      monthNames: [
        'Януари',
        'Февруари',
        'Март',
        'Април',
        'Май',
        'Юни',
        'Юли',
        'Август',
        'Септември',
        'Октомври',
        'Ноември',
        'Декември'
      ],
      monthNamesShort: ['Ян', 'Фев', 'Март', 'Апр', 'Май', 'Юни', 'Юли', 'Авг', 'Септ', 'Окт', 'Ноем', 'Дек'],
      firstDayOfWeek: 1,
      today: 'Днес',
      emptyMessage: 'Няма намерени резултати',
      emptyFilterMessage: 'Няма намерени резултати',
      clear: 'Изчисти',
      addRule: 'Добави филтър',
      removeRule: 'Премахни филтър',
      apply: 'Приложи'
    });
  }

  translate(value: string) {
    return this.config.getTranslation(value);
  }
}
