import { NgModule } from '@angular/core';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { AppChromeModule } from 'projects/shared/components/app-chrome/app-chrome.module';
import { DateFieldModule } from 'projects/shared/components/date-field/date-field.module';
import { NomSelectModule } from 'projects/shared/components/nom-select/nom-select.module';
import { DatePipesModule } from 'projects/shared/pipes/date-pipes/date-pipes.module';
import {
  HisMedicalNoticeViewComponent,
  HisMedicalNoticeViewSkeletonComponent
} from './his-medical-notice-view/his-medical-notice-view.component';
import { HisMedicalNoticesRoutingModule } from './his-medical-notices-routing.module';
import { HisMedicalNoticesComponent } from './his-medical-notices/his-medical-notices.component';

@NgModule({
  declarations: [HisMedicalNoticesComponent, HisMedicalNoticeViewComponent, HisMedicalNoticeViewSkeletonComponent],
  imports: [
    HisMedicalNoticesRoutingModule,
    AppChromeModule,
    CommonFormUiModule,
    NomSelectModule,
    DateFieldModule,
    DatePipesModule
  ]
})
export class HisMedicalNoticesModule {}
