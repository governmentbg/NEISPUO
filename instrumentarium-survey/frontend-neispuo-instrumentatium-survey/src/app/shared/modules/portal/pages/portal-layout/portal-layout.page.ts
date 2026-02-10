import { Component, OnInit } from '@angular/core';
import { MediaObserver } from '@angular/flex-layout';
import { ApiService } from 'src/app/core/services/api.service';
import { VersionModel } from 'src/app/resources/models/version.model';
import { versionFE } from 'src/app/resources/variables/version';

@Component({
  selector: 'app-portal-layout',
  templateUrl: './portal-layout.page.html',
  styleUrls: ['./portal-layout.page.scss']
})
export class PortalLayoutPage implements OnInit {
  showSidebar = false;
  appVersion = versionFE;

  constructor(
    public media: MediaObserver,
    private readonly apiService: ApiService
  ) {}

  ngOnInit(): void {
    this.getBackendVersion();
  }

  private getBackendVersion() {
    this.apiService.get('/v1/version').subscribe((versionBE: VersionModel) => {
      this.appVersion += `, ${versionBE.name}: ${versionBE.version}`;
    });
  }
}
