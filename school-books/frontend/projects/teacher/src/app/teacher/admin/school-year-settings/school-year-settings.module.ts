import { NgModule } from '@angular/core';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { DateFieldModule } from 'projects/shared/components/date-field/date-field.module';
import { NomSelectModule } from 'projects/shared/components/nom-select/nom-select.module';
import { NumberFieldModule } from 'projects/shared/components/number-field/number-field.module';
import { SelectFieldModule } from 'projects/shared/components/select-field/select-field.module';
import { DeactivateGuard } from 'projects/shared/guards/deactivate-guard';
import { DatePipesModule } from 'projects/shared/pipes/date-pipes/date-pipes.module';
import { ActionServiceModule } from 'projects/shared/services/action-service/action-service.module';
import { SchoolYearSettingsRoutingModule } from './school-year-settings-routing.module';
import {
  SchoolYearSettingsViewComponent,
  SchoolYearSettingsViewSkeletonComponent
} from './school-year-settings-view/school-year-settings-view.component';
import { SchoolYearSettingsComponent } from './school-year-settings/school-year-settings.component';

@NgModule({
  declarations: [SchoolYearSettingsComponent, SchoolYearSettingsViewComponent, SchoolYearSettingsViewSkeletonComponent],
  imports: [
    SchoolYearSettingsRoutingModule,
    CommonFormUiModule,
    ActionServiceModule,
    DatePipesModule,
    DateFieldModule,
    NomSelectModule,
    SelectFieldModule,
    NumberFieldModule
  ],
  providers: [DeactivateGuard]
})
export class SchoolYearSettingsModule {}
