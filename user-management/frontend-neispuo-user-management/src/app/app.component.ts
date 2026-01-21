import { Component, OnInit } from '@angular/core';
import { TranslateService } from '@ngx-translate/core';
import { CONSTANTS } from '@shared/constants';
import { PrimeNGConfig } from 'primeng/api';
import { OIDCService } from './core/services/oidc.service';

@Component({
    selector: 'app-root',
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.scss'],
})
export class AppComponent implements OnInit {
    title = 'User Management';

    constructor(
        private oidcService: OIDCService,
        private config: PrimeNGConfig,
        private translateService: TranslateService,
    ) {}

    ngOnInit(): void {
        this.oidcService.start();
        this.translate();
    }

    translate(): void {
        this.translateService.addLangs([`${CONSTANTS.LANGUAGE_BG}`]);
        this.translateService.setDefaultLang(CONSTANTS.LANGUAGE_BG);
        this.translateService.use(CONSTANTS.LANGUAGE_BG);
        this.translateService.stream('PRIMENG.CALENDAR').subscribe((res) => {
            this.config.setTranslation(res);
        });
    }
}
