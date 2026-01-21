import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class EnvironmentService {
  public dynamicEnv: any;

  setEnvironment(config: any) {
    this.dynamicEnv = config;
  }

  get environment() {
    return this.dynamicEnv;
  }
}