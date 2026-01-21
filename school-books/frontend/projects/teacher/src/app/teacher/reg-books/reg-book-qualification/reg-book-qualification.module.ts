import { NgModule } from '@angular/core';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { RegBookQualificationRoutingModule } from './reg-book-qualification-routing.module';
import { RegBookQualificationComponent } from './reg-book-qualification/reg-book-qualification.component';

@NgModule({
  declarations: [RegBookQualificationComponent],
  imports: [RegBookQualificationRoutingModule, CommonFormUiModule, BannerModule]
})
export class RegBookQualificationModule {}
