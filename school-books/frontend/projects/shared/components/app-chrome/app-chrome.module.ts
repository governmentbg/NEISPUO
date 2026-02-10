import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MatBadgeModule } from '@angular/material/badge';
import { MatMenuModule } from '@angular/material/menu';
import { RouterModule } from '@angular/router';
import { FontAwesomeWithConfigModule } from 'projects/shared/font-awesome-with-config.module';
import { DatePipesModule } from '../../pipes/date-pipes/date-pipes.module';
import { AppChromeComponent } from './app-chrome.component';

@NgModule({
  declarations: [AppChromeComponent],
  imports: [CommonModule, FontAwesomeWithConfigModule, MatMenuModule, RouterModule, DatePipesModule, MatBadgeModule],
  exports: [AppChromeComponent]
})
export class AppChromeModule {}
