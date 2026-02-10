import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MatDialogModule } from '@angular/material/dialog';
import { AppChromeModule } from 'projects/shared/components/app-chrome/app-chrome.module';
import { AppMenuModule } from 'projects/shared/components/app-menu/app-menu.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { AppChromeSkeletonTemplateModule } from 'projects/shared/components/skeleton/app-chrome-skeleton-template.module';
import { SnackbarRootModule } from 'projects/shared/components/snackbar-root/snackbar-root.module';
import { AdminRoutingModule } from './admin-routing.module';
import { AdminComponent } from './admin.component';

@NgModule({
  imports: [
    CommonModule,
    AppChromeModule,
    AppMenuModule,
    AppChromeSkeletonTemplateModule,
    AdminRoutingModule,
    BannerModule,
    SnackbarRootModule,
    MatDialogModule
  ],
  declarations: [AdminComponent]
})
export class AdminModule {}
