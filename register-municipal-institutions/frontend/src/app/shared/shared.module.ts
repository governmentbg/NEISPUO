import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FlexLayoutModule } from '@angular/flex-layout';
import { RouterModule } from '@angular/router';
import { FileUploadModule } from 'primeng/fileupload';
import { NgxFilesizeModule } from 'ngx-filesize';
import { ConfirmationService, MessageService } from 'primeng/api';
import { ClickOutsideDirective } from './directive/click-outside.directive';
import { FileComponent } from './components/file/file.component';
import { StepperActionsComponent } from './components/stepper-actions/stepper-actions.component';
import { DotNotatedPipe } from './modules/pipes/dot-notated.pipe';
import { ActionButtonsComponent } from './components/action-buttons/action-buttons.component';
import { UserTourGuideComponent } from './modules/user-tour-guide/user-tour-guide/user-tour-guide.component';
import { TooltipModule } from 'primeng/tooltip';

@NgModule({
  declarations: [
    DotNotatedPipe,
    StepperActionsComponent,
    FileComponent,
    ActionButtonsComponent,
    ClickOutsideDirective,
    UserTourGuideComponent
  ],
  imports: [CommonModule, RouterModule, FlexLayoutModule, FileUploadModule, NgxFilesizeModule, TooltipModule],
  exports: [
    FlexLayoutModule,
    StepperActionsComponent,
    FileComponent,
    NgxFilesizeModule,
    ActionButtonsComponent,
    ClickOutsideDirective,
    UserTourGuideComponent
  ],
  providers: [DotNotatedPipe, ConfirmationService, MessageService]
})
export class SharedModule {}
