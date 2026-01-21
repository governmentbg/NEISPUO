import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FontAwesomeWithConfigModule } from 'projects/shared/font-awesome-with-config.module';
import { BannerComponent } from './banner.component';

@NgModule({
  declarations: [BannerComponent],
  imports: [CommonModule, FontAwesomeWithConfigModule],
  exports: [BannerComponent]
})
export class BannerModule {}
