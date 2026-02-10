import { Component, OnInit } from '@angular/core';
import { EnvironmentService } from '@core/services/environment.service';
import { CONSTANTS } from '@shared/constants';
import { VersionService } from '@shared/services/version.service';

@Component({
    selector: 'app-footer',
    templateUrl: './footer.component.html',
    styleUrls: ['./footer.component.scss'],
})
export class FooterComponent implements OnInit {
    public readonly environment;

    TEXT1 = CONSTANTS.FOOTER_CONTENT_TEXT1;

    TEXT2 = CONSTANTS.FOOTER_CONTENT_TEXT2;

    TEXT3 = CONSTANTS.FOOTER_CONTENT_TEXT3;

    appVersion = CONSTANTS.VERSION_FRONTEND;

    constructor(private envService: EnvironmentService, private versionService: VersionService) {
        this.environment = this.envService.environment;
    }

    ngOnInit() {
        this.versionService.getBackendVersion(this);
    }
}
