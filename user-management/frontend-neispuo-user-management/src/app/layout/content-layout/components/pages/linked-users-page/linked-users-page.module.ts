import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '@shared/shared.module';
import { LinkedUsersPageComponent } from './linked-users-page.component';
import { LinkedUsersTableComponent } from './linked-users-table/linked-users-table.component';
import { LinkedUsersAddDialogComponent } from './linked-users-add-dialog/linked-users-add-dialog.component';

@NgModule({
    declarations: [LinkedUsersPageComponent, LinkedUsersTableComponent, LinkedUsersAddDialogComponent],
    imports: [CommonModule, SharedModule],
    exports: [LinkedUsersPageComponent],
})
export class LinkedUsersPageModule {}
