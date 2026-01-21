import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MatTabsModule } from '@angular/material/tabs';
import { RouterModule } from '@angular/router';
import { FontAwesomeWithConfigModule } from 'projects/shared/font-awesome-with-config.module';
import { TabsComponent } from './tabs.component';

@NgModule({
  declarations: [TabsComponent],
  imports: [CommonModule, FontAwesomeWithConfigModule, RouterModule, MatTabsModule],
  exports: [TabsComponent]
})
export class TabsModule {}
