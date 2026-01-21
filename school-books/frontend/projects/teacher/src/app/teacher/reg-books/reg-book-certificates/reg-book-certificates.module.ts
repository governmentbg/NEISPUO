import { NgModule } from '@angular/core';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { RegBookCertificatesRoutingModule } from './reg-book-certificates-routing.module';
import { RegBookCertificatesComponent } from './reg-book-certificates/reg-book-certificates.component';

@NgModule({
  declarations: [RegBookCertificatesComponent],
  imports: [RegBookCertificatesRoutingModule, CommonFormUiModule, BannerModule]
})
export class RegBookCertificatesModule {}
