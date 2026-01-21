import { ScrollingModule } from '@angular/cdk/scrolling';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { MatExpansionModule } from '@angular/material/expansion';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatTooltipModule } from '@angular/material/tooltip';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { NomSelectModule } from 'projects/shared/components/nom-select/nom-select.module';
import { SimpleTabSkeletonTemplateModule } from 'projects/shared/components/skeleton/simple-tab-skeleton-template.module';
import { SlideToggleModule } from 'projects/shared/components/slide-toggle/slide-toggle.module';
import { TextareaFieldModule } from 'projects/shared/components/textarea-field/textarea-field.module';
import { DeactivateGuard } from 'projects/shared/guards/deactivate-guard';
import { DatePipesModule } from 'projects/shared/pipes/date-pipes/date-pipes.module';
import { ActionServiceModule } from 'projects/shared/services/action-service/action-service.module';
import { SelectFieldModule } from '../select-field/select-field.module';
import { ConversationsNewComponent } from './conversations-new/conversations-new.component';
import { ConversationsRoutingModule } from './conversations-routing.module';
import {
  ConversationViewComponent,
  ConversationViewSkeletonComponent
} from './conversations-view/conversations-view.component';
import { ConversationsComponent } from './conversations/conversations.component';

@NgModule({
  declarations: [
    ConversationsNewComponent,
    ConversationsComponent,
    ConversationViewSkeletonComponent,
    ConversationViewComponent
  ],
  imports: [
    ActionServiceModule,
    BannerModule,
    CommonFormUiModule,
    CommonModule,
    ConversationsRoutingModule,
    DatePipesModule,
    MatExpansionModule,
    MatMenuModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    MatTooltipModule,
    NomSelectModule,
    ReactiveFormsModule,
    ScrollingModule,
    SelectFieldModule,
    SimpleTabSkeletonTemplateModule,
    SlideToggleModule,
    TextareaFieldModule
  ],
  exports: [],
  providers: [DeactivateGuard]
})
export class ConversationsModule {}
