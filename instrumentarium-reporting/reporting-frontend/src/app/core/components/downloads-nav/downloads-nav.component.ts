import { Component } from '@angular/core';
import { DownloadManagerService, DownloadItem } from '../../services/download-manager.service';

@Component({
  selector: 'app-downloads-nav',
  templateUrl: './downloads-nav.component.html',
  styleUrls: ['./downloads-nav.component.scss']
})
export class DownloadsNavComponent {
  showDownloads = false;
  downloads: DownloadItem[] = [];

  constructor(private downloadManager: DownloadManagerService) {
    this.downloadManager.downloads$.subscribe(list => this.downloads = list);
  }

  toggleDownloads() {
    this.showDownloads = !this.showDownloads;
  }

  saveFile(d: DownloadItem) {
    if (d.blob) {
      const url = window.URL.createObjectURL(d.blob);
      const a = document.createElement('a');
      a.href = url;
      a.download = d.fileName;
      a.click();
      window.URL.revokeObjectURL(url);
    }
  }
}
