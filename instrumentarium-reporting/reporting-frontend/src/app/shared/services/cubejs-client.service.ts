import { Injectable } from '@angular/core';
import { AuthQuery } from '@authentication/auth-state-manager/auth.query';
import { CubejsClient } from '@cubejs-client/ngx';
import { BehaviorSubject, shareReplay } from 'rxjs';
import { EnvironmentService } from './environment.service';

/**
 * Idea was taken from the cubejs github issue https://github.com/cube-js/cube.js/issues/1992
 */
@Injectable({ providedIn: 'root' })
export class CubeJsClientService {
  private _cubejsSubject = new BehaviorSubject<CubejsClient>(null);
  public cubeJs$ = this._cubejsSubject.asObservable().pipe(shareReplay(1));

  private environment = this.envService.environment;
  constructor(private authQuery: AuthQuery, private envService: EnvironmentService) {
    let client: CubejsClient = null;
    client = new CubejsClient({
      token: `Bearer ${this.authQuery.getValue().oidcAccessToken}`,
      options: {
        apiUrl: this.environment.CUBEJS_API_URL
      }
    });

    this._cubejsSubject.next(client);
  }
}
