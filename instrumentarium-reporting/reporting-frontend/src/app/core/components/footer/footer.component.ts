import { Component, OnInit } from '@angular/core';
import { VersionModel } from '@core/models/version.model';
import { ApiService } from '@core/services/api.service';
import { EnvironmentService } from '@shared/services/environment.service';
import pjson from '../../../../../package.json';

@Component({
  selector: 'app-footer',
  templateUrl: './footer.component.html',
  styleUrls: ['./footer.component.scss']
})
export class FooterComponent implements OnInit {
  public readonly environment: any;
  appVersion = `${pjson.name}: ${pjson.version}`;
  beVersion: string;

  constructor(public envService: EnvironmentService, private apiService: ApiService) {
    this.environment = this.envService.environment;
    this.getBackendVersion();
  }

  private getBackendVersion() {
    this.apiService.get('/v1/version').subscribe((beVersion: VersionModel) => {
      this.appVersion += `, ${beVersion.name}: ${beVersion.version}`;
      this.beVersion = beVersion.version;
    });
  }

  ngOnInit(): void {}
}
