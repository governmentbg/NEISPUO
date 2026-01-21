import { NgModule } from '@angular/core';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { ShiftFormModule } from 'projects/shared/components/shift-form/shift-form.module';
import { DeactivateGuard } from 'projects/shared/guards/deactivate-guard';
import { ActionServiceModule } from 'projects/shared/services/action-service/action-service.module';
import { ShiftViewComponent, ShiftViewSkeletonComponent } from './shift-view/shift-view.component';
import { ShiftsRoutingModule } from './shifts-routing.module';
import { ShiftsComponent } from './shifts/shifts.component';

@NgModule({
  declarations: [ShiftsComponent, ShiftViewComponent, ShiftViewSkeletonComponent],
  imports: [ShiftsRoutingModule, ShiftFormModule, CommonFormUiModule, ActionServiceModule],
  providers: [DeactivateGuard]
})
export class ShiftsModule {}
