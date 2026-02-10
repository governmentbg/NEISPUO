import { NgModule } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';
import { BudgetInstitutionPageComponent } from './budget-institution-page.component';

@NgModule({
    declarations: [BudgetInstitutionPageComponent],
    imports: [SharedModule],
    exports: [BudgetInstitutionPageComponent],
})
export class BudgetInstitutionPageModule {}
