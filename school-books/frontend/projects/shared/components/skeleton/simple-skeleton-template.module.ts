import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { DynamicIoModule, DynamicModule } from 'ng-dynamic-component';
import { CardModule } from 'projects/shared/components/card/card.module';
import { BannerModule } from '../banner/banner.module';
import { BreadcrumbModule } from '../breadcrumb/breadcrumb.module';
import { SimpleSkeletonTemplateComponent } from './simple-skeleton-template.component';

@NgModule({
  declarations: [SimpleSkeletonTemplateComponent],
  imports: [
    CommonModule,
    CardModule,
    DynamicIoModule,
    DynamicModule,
    MatProgressSpinnerModule,
    BannerModule,
    BreadcrumbModule
  ],
  exports: [SimpleSkeletonTemplateComponent]
})
export class SimpleSkeletonTemplateModule {}
