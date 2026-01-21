import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FontAwesomeWithConfigModule } from 'projects/shared/font-awesome-with-config.module';
import { BreadcrumbComponent } from './breadcrumb.component';

@NgModule({
  declarations: [BreadcrumbComponent],
  imports: [CommonModule, FontAwesomeWithConfigModule, RouterModule],
  exports: [BreadcrumbComponent]
})
export class BreadcrumbModule {}
