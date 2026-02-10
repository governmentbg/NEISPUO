import { NgModule } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';
import { UpdateInstitutionPageComponent } from './update-institution-page.component';

@NgModule({
    declarations: [UpdateInstitutionPageComponent],
    imports: [SharedModule],
    exports: [UpdateInstitutionPageComponent],
})
export class UpdateInstitutionPageModule {}
