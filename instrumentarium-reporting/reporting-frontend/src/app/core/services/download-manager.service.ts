import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

export interface DownloadItem {
  id: string;
  fileName: string;
  status: 'pending' | 'downloading' | 'completed' | 'error';
  progress?: number; // Not used for streaming, but reserved for future
  blob?: Blob;
  error?: any;
  startedAt: Date;
  finishedAt?: Date;
}

@Injectable({ providedIn: 'root' })
export class DownloadManagerService {
  private downloadsSubject = new BehaviorSubject<DownloadItem[]>([]);
  downloads$: Observable<DownloadItem[]> = this.downloadsSubject.asObservable();

  get downloads(): DownloadItem[] {
    return this.downloadsSubject.value;
  }

  addDownload(item: DownloadItem) {
    this.downloadsSubject.next([...this.downloads, item]);
  }

  updateDownload(id: string, patch: Partial<DownloadItem>) {
    this.downloadsSubject.next(
      this.downloads.map(d => d.id === id ? { ...d, ...patch } : d)
    );
  }
}
