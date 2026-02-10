import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FontAwesomeWithConfigModule } from 'projects/shared/font-awesome-with-config.module';
import { AppMenuComponent } from './app-menu.component';
import { InlineMenuComponent } from './inline-menu/inline-menu.component';
import { MenuComponent } from './menu/menu.component';

@NgModule({
  declarations: [AppMenuComponent, InlineMenuComponent, MenuComponent],
  imports: [CommonModule, FontAwesomeWithConfigModule, RouterModule],
  exports: [AppMenuComponent]
})
export class AppMenuModule {}
