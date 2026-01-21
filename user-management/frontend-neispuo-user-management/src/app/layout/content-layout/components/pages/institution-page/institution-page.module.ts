import { NgModule } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';
import { InstitutionPageComponent } from './institution-page.component';

@NgModule({
    declarations: [InstitutionPageComponent],
    imports: [SharedModule],
    exports: [InstitutionPageComponent],
})
export class InstitutionPageModule {}
