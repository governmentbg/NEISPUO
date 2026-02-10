import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { AppChromeModule } from 'projects/shared/components/app-chrome/app-chrome.module';
import { NotFoundComponent } from './not-found.component';

@NgModule({
  declarations: [NotFoundComponent],
  imports: [CommonModule, AppChromeModule]
})
export class NotFoundModule {}
