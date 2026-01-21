import { Component, OnInit } from '@angular/core';
import { OIDCService } from '@authentication/auth-state-manager/oidc.service';
import { PrimeNGConfig } from 'primeng/api';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'Оценка и самооценка';

  constructor(
    private config: PrimeNGConfig,
    private oidcService: OIDCService,
  ) {}

  ngOnInit(): void {
    this.oidcService.start();

    this.setDefaultLanguage();
  }

  private setDefaultLanguage() {
    this.config.setTranslation({
      dayNames: ['Неделя', 'Понеделник', 'Вторник', 'Сряда', 'Четвъртък', 'Петък', 'Събота'],
      dayNamesMin: ['Н', 'П', 'В', 'С', 'Ч', 'П', 'С'],
      monthNames: ['Януари', 'Февруари', 'Март', 'Април', 'Май', 'Юни', 'Юли', 'Август', 'Септември', 'Октомври', 'Ноември', 'Декември'],
    })
  }
}
