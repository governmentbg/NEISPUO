import { Component } from '@angular/core';
import { OIDCService } from 'src/app/core/authentication/auth-state-manager/oidc.service';
import { PrimeNGConfig } from 'primeng/api';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {
  title = 'Общински институции';

  /**
   *
   */
  constructor(private oidcService: OIDCService, private config: PrimeNGConfig) {
    this.oidcService.start();

    /**
     * this should be moved out in a json translation file and used according to https://www.primefaces.org/primeng/showcase/#/i18n
     */
    this.config.setTranslation({
      noFilter: 'Без филтър',
      dayNames: ['Неделя', 'Понеделник', 'Вотрник', 'Сряда', 'Четвъртък', 'Петък', 'Събота'],
      dayNamesShort: ['Нед', 'Пон', 'Втор', 'Ср', 'Четв', 'Пет', 'Съб'],
      dayNamesMin: ['Н', 'П', 'В', 'С', 'Ч', 'П', 'С'],
      monthNames: [
        'Янувари',
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
        'Декември',
      ],
      monthNamesShort: [
        'Ян',
        'Фев',
        'Март',
        'Април',
        'Май',
        'Юни',
        'Юли',
        'Авг',
        'Сеп',
        'Окт',
        'Ноем',
        'Дек',
      ],
      today: 'Днес',
      clear: 'Изчисти',
    });
  }
}
