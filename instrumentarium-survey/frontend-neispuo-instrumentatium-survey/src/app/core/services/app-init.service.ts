import { BehaviorSubject } from 'rxjs';
import { shareReplay } from 'rxjs/operators';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { EnvironmentService } from './environment.service';

@Injectable({ providedIn: 'root' })
export class AppInitService {
  private readonly CONFIG_URL = '/assets/config/config.json';

  private configSubject$: BehaviorSubject<any> = new BehaviorSubject(null);

  public config$ = this.configSubject$.pipe(shareReplay(1));

  constructor(private http: HttpClient, private environmentService: EnvironmentService) {}

  public async getConfiguration() {
    this.configSubject$.next(await this.http.get(this.CONFIG_URL).pipe(shareReplay()).toPromise());
    return this.configSubject$.getValue();
  }

  public async loadConfiguration() {
    this.configSubject$.next(await this.http.get(this.CONFIG_URL).pipe(shareReplay()).toPromise());
    this.environmentService.setEnvironment(this.configSubject$.getValue());
    return this.configSubject$.getValue();
  }
}
