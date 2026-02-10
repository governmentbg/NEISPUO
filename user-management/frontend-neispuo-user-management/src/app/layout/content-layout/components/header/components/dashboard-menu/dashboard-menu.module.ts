import { NgModule } from '@angular/core';
import { DashboardMenuConfig } from 'src/app/configs/dashboard-menu.config';
import { SharedModule } from 'src/app/shared/shared.module';
import { DashboardMenuComponent } from './dashboard-menu.component';

@NgModule({
    declarations: [DashboardMenuComponent],
    imports: [SharedModule],
    exports: [DashboardMenuComponent],
    providers: [DashboardMenuConfig],
})
export class DashboardMenuModule {}
