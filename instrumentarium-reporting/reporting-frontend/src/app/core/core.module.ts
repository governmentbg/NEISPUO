import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { HeaderComponent } from './components/header/header.component';
import { FooterComponent } from './components/footer/footer.component';
import { SharedModule } from '../shared/shared.module';
import { SecondNavComponent } from './components/second-nav/second-nav.component';
import { RouterModule } from '@angular/router';
import { UserMenuComponent } from './components/user-menu/user-menu.component';
import { DashboardMenuComponent } from './components/dashboard-menu/dashboard-menu.component';
import { InfoStripComponent } from './components/info-strip/info-strip.component';
import { AppToastMessageService } from './services/app-toast-message.service';
import { DownloadsNavComponent } from './components/downloads-nav/downloads-nav.component';

@NgModule({
  declarations: [HeaderComponent, FooterComponent, SecondNavComponent, UserMenuComponent, DashboardMenuComponent, InfoStripComponent, DownloadsNavComponent],
  imports: [CommonModule, SharedModule, RouterModule],
  providers: [AppToastMessageService],
  exports: [HeaderComponent, SecondNavComponent, FooterComponent, InfoStripComponent, DownloadsNavComponent]
})
export class CoreModule { }
