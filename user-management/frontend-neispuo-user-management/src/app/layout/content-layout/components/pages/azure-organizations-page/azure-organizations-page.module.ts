import { NgModule } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';
import { AzureOrganizationsPageComponent } from './azure-organizations-page.component';

@NgModule({
    declarations: [AzureOrganizationsPageComponent],
    imports: [SharedModule],
    exports: [AzureOrganizationsPageComponent],
})
export class AzureOrganizationsPageModule {}
