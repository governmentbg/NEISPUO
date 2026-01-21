import { NgModule } from '@angular/core';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { RegBookQualificationDuplicatesRoutingModule } from './reg-book-qualification-duplicates-routing.module';
import { RegBookQualificationDuplicatesComponent } from './reg-book-qualification-duplicates/reg-book-qualification-duplicates.component';

@NgModule({
  declarations: [RegBookQualificationDuplicatesComponent],
  imports: [RegBookQualificationDuplicatesRoutingModule, CommonFormUiModule, BannerModule]
})
export class RegBookQualificationDuplicatesModule {}
