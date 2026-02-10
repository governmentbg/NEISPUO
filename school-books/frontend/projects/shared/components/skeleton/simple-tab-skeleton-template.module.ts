import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { DynamicIoModule, DynamicModule } from 'ng-dynamic-component';
import { BannerModule } from '../banner/banner.module';
import { SimpleTabSkeletonTemplateComponent } from './simple-tab-skeleton-template.component';

@NgModule({
  declarations: [SimpleTabSkeletonTemplateComponent],
  imports: [CommonModule, DynamicIoModule, DynamicModule, MatProgressSpinnerModule, BannerModule],
  exports: [SimpleTabSkeletonTemplateComponent]
})
export class SimpleTabSkeletonTemplateModule {}
