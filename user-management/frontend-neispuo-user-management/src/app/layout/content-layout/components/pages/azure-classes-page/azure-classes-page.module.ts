import { NgModule } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';
import { AzureClassesPageComponent } from './azure-classes-page.component';

@NgModule({
    declarations: [AzureClassesPageComponent],
    imports: [SharedModule],
    exports: [AzureClassesPageComponent],
})
export class AzureClassesPageModule {}
