import { NgModule } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';
import { AzureUsersPageComponent } from './azure-users-page.component';

@NgModule({
    declarations: [AzureUsersPageComponent],
    imports: [SharedModule],
    exports: [AzureUsersPageComponent],
})
export class AzureUsersPageModule {}
