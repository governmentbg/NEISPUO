import { NgModule } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';
import { MunicipalityPageComponent } from './municipality-page.component';

@NgModule({
    declarations: [MunicipalityPageComponent],
    imports: [SharedModule],
    exports: [MunicipalityPageComponent],
})
export class MunicipalityPageModule {}
