import { Injectable } from '@angular/core';
import { EnvironmentService } from '../core/services/environment.service';

@Injectable({ providedIn: 'root' })
export class DashboardMenuConfig {
    public environment;

    constructor(envService: EnvironmentService) {
        this.environment = envService.environment;
    }
}
