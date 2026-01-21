import { NgModule } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';
import { OtherAzureUsersPageComponent } from './other-azure-users-page.component';

@NgModule({
    declarations: [OtherAzureUsersPageComponent],
    imports: [SharedModule],
    exports: [OtherAzureUsersPageComponent],
})
export class OtherAzureUsersPageModule {}
