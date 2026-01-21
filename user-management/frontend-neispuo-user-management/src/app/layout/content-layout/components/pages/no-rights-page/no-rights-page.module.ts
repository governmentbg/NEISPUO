import { NgModule } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';
import { NoRightsPageComponent } from './no-rights-page.component';

@NgModule({
    declarations: [NoRightsPageComponent],
    imports: [SharedModule],
    exports: [NoRightsPageComponent],
})
export class NoRightsPageModule {}
