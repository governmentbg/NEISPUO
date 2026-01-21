import { NgModule } from '@angular/core';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { DateFieldModule } from 'projects/shared/components/date-field/date-field.module';
import { NomSelectModule } from 'projects/shared/components/nom-select/nom-select.module';
import { SelectFieldModule } from 'projects/shared/components/select-field/select-field.module';
import { TextareaFieldModule } from 'projects/shared/components/textarea-field/textarea-field.module';
import { DeactivateGuard } from 'projects/shared/guards/deactivate-guard';
import { DatePipesModule } from 'projects/shared/pipes/date-pipes/date-pipes.module';
import { ActionServiceModule } from 'projects/shared/services/action-service/action-service.module';
import { OffDayViewComponent, OffDayViewSkeletonComponent } from './off-day-view/off-day-view.component';
import { OffDaysRoutingModule } from './off-days-routing.module';
import { OffDaysComponent } from './off-days/off-days.component';

@NgModule({
  declarations: [OffDaysComponent, OffDayViewComponent, OffDayViewSkeletonComponent],
  imports: [
    OffDaysRoutingModule,
    CommonFormUiModule,
    ActionServiceModule,
    DatePipesModule,
    DateFieldModule,
    NomSelectModule,
    SelectFieldModule,
    TextareaFieldModule
  ],
  providers: [DeactivateGuard]
})
export class OffDaysModule {}
