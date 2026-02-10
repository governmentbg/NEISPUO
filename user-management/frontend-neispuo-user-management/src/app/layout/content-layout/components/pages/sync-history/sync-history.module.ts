import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';
import { AzureSyncEnrollmentsTableComponent } from './azure-sync-enrollments-table/azure-sync-enrollments-table.component';
import { AzureSyncOrganizationTableComponent } from './azure-sync-organization-table/azure-sync-organization-table.component';
import { OrganizationHistoryComponent } from './organization-history/organization-history.component';
import { ParentHistoryComponent } from './parent-history/parent-history.component';
import { AzureSyncUserTableComponent } from './shared/azure-sync-user-table/azure-sync-user-table.component';
import { JsonViewCardComponent } from './shared/json-view-card/json-view-card.component';
import { NeispuoProfileCardComponent } from './shared/neispuo-profile-card/neispuo-profile-card.component';
import { ProfileTabbarComponent } from './shared/profile-tabbar/profile-tabbar.component';
import { SysUserTableComponent } from './shared/sys-user-table/sys-user-table.component';
import { SyncHistoryComponent } from './sync-history.component';
import { UserHistoryComponent } from './user-history/user-history.component';

@NgModule({
    declarations: [
        SyncHistoryComponent,
        OrganizationHistoryComponent,
        ParentHistoryComponent,
        UserHistoryComponent,
        JsonViewCardComponent,
        ProfileTabbarComponent,
        AzureSyncUserTableComponent,
        AzureSyncOrganizationTableComponent,
        AzureSyncEnrollmentsTableComponent,
        SysUserTableComponent,
        NeispuoProfileCardComponent,
    ],
    imports: [CommonModule, SharedModule],
})
export class SyncHistoryModule {}
