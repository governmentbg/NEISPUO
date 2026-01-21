import { NgModule } from '@angular/core';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { AppChromeModule } from 'projects/shared/components/app-chrome/app-chrome.module';
import { InstitutionsRoutingModule } from './institutions-routing.module';
import { InstitutionsComponent } from './institutions.component';

@NgModule({
  declarations: [InstitutionsComponent],
  imports: [InstitutionsRoutingModule, AppChromeModule, CommonFormUiModule]
})
export class InstitutionsModule {}
