import { NgModule } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';
import { AzureEnrollmentsPageComponent } from './azure-enrollments-page.component';

@NgModule({
    declarations: [AzureEnrollmentsPageComponent],
    imports: [SharedModule],
    exports: [AzureEnrollmentsPageComponent],
})
export class AzureEnrollmentsPageModule {}
