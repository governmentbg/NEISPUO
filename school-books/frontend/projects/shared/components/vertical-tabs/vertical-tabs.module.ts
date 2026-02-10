import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FontAwesomeWithConfigModule } from 'projects/shared/font-awesome-with-config.module';
import { VerticalTabsComponent } from './vertical-tabs.component';

@NgModule({
  declarations: [VerticalTabsComponent],
  imports: [CommonModule, FontAwesomeWithConfigModule, RouterModule],
  exports: [VerticalTabsComponent]
})
export class VerticalTabsModule {}
