import { NgModule } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';
import { DashboardMenuModule } from './components/dashboard-menu/dashboard-menu.module';
import { UserMenuModule } from './components/user-menu/user-menu.module';
import { HeaderComponent } from './header.component';

@NgModule({
    declarations: [HeaderComponent],
    imports: [UserMenuModule, DashboardMenuModule, SharedModule],
    exports: [HeaderComponent],
})
export class HeaderModule {}
