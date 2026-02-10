import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReportingSidebarToggleComponent } from './components/reporting-sidebar-toggle/reporting-sidebar-toggle.component';
import { ActionConfirmationDirective } from './directives/action-confirmation.directive';

@NgModule({
  declarations: [ReportingSidebarToggleComponent, ActionConfirmationDirective],
  imports: [CommonModule],
  exports: [ReportingSidebarToggleComponent, ActionConfirmationDirective]
})
export class SharedModule {}
