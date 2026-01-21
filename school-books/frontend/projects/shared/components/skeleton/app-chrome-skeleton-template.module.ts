import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { DynamicIoModule, DynamicModule } from 'ng-dynamic-component';
import { AppChromeModule } from '../app-chrome/app-chrome.module';
import { AppMenuModule } from '../app-menu/app-menu.module';
import { AppChromeSkeletonTemplateComponent } from './app-chrome-skeleton-template.component';

@NgModule({
  declarations: [AppChromeSkeletonTemplateComponent],
  imports: [CommonModule, AppChromeModule, AppMenuModule, DynamicIoModule, DynamicModule, RouterModule],
  exports: [AppChromeSkeletonTemplateComponent]
})
export class AppChromeSkeletonTemplateModule {}
