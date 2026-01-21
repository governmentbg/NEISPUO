import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MatDialogModule } from '@angular/material/dialog';
import { AppChromeModule } from 'projects/shared/components/app-chrome/app-chrome.module';
import { AppMenuModule } from 'projects/shared/components/app-menu/app-menu.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { AppChromeSkeletonTemplateModule } from 'projects/shared/components/skeleton/app-chrome-skeleton-template.module';
import { SnackbarRootModule } from 'projects/shared/components/snackbar-root/snackbar-root.module';
import { FontAwesomeWithConfigModule } from 'projects/shared/font-awesome-with-config.module';
import { TeacherRoutingModule } from './teacher-routing.module';
import { TeacherComponent, TeacherSkeletonComponent } from './teacher.component';

@NgModule({
  imports: [
    CommonModule,
    FontAwesomeWithConfigModule,
    AppChromeModule,
    AppMenuModule,
    AppChromeSkeletonTemplateModule,
    TeacherRoutingModule,
    BannerModule,
    SnackbarRootModule,
    MatDialogModule
  ],
  declarations: [TeacherComponent, TeacherSkeletonComponent]
})
export class TeacherModule {}
