import { NgModule } from '@angular/core';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { RegBookCertificateDuplicatesRoutingModule } from './reg-book-certificate-duplicates-routing.module';
import { RegBookCertificateDuplicatesComponent } from './reg-book-certificate-duplicates/reg-book-certificate-duplicates.component';

@NgModule({
  declarations: [RegBookCertificateDuplicatesComponent],
  imports: [RegBookCertificateDuplicatesRoutingModule, CommonFormUiModule, BannerModule]
})
export class RegBookCertificateDuplicatesModule {}
