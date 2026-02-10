import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { AppChromeModule } from 'projects/shared/components/app-chrome/app-chrome.module';
import { AppMenuModule } from 'projects/shared/components/app-menu/app-menu.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { CardModule } from 'projects/shared/components/card/card.module';
import { SimplePageSkeletonTemplateModule } from 'projects/shared/components/skeleton/simple-page-skeleton-template.module';
import { SnackbarRootModule } from 'projects/shared/components/snackbar-root/snackbar-root.module';
import { FontAwesomeWithConfigModule } from 'projects/shared/font-awesome-with-config.module';
import { ActionServiceModule } from 'projects/shared/services/action-service/action-service.module';
import { StudentRoutingModule } from './student-routing.module';
import { StudentComponent } from './student.component';

@NgModule({
  imports: [
    CommonModule,
    FontAwesomeWithConfigModule,
    AppChromeModule,
    AppMenuModule,
    SimplePageSkeletonTemplateModule,
    StudentRoutingModule,
    SnackbarRootModule,
    ActionServiceModule,
    BannerModule,
    CardModule
  ],
  declarations: [StudentComponent]
})
export class StudentModule {}
