import { NgModule } from '@angular/core';
import { MatDialogModule } from '@angular/material/dialog';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { UppyAngularDragDropModule, UppyAngularProgressBarModule } from '@uppy/angular';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { NomSelectModule } from 'projects/shared/components/nom-select/nom-select.module';
import { NumberFieldModule } from 'projects/shared/components/number-field/number-field.module';
import { PaginatorModule } from 'projects/shared/components/paginator/paginator.module';
import { SimpleDialogSkeletonTemplateModule } from 'projects/shared/components/skeleton/simple-dialog-skeleton-template.module';
import { TextareaFieldModule } from 'projects/shared/components/textarea-field/textarea-field.module';
import { DeactivateGuard } from 'projects/shared/guards/deactivate-guard';
import { ActionServiceModule } from 'projects/shared/services/action-service/action-service.module';
import {
  TopicPlanViewDialogComponent,
  TopicPlanViewDialogSkeletonComponent
} from './topic-plan-view-dialog/topic-plan-view-dialog.component';
import { TopicPlanViewImportDialogComponent } from './topic-plan-view-import-dialog/topic-plan-view-import-dialog.component';
import { TopicPlanViewComponent, TopicPlanViewSkeletonComponent } from './topic-plan-view/topic-plan-view.component';
import { TopicPlansRoutingModule } from './topic-plans-routing.module';
import { TopicPlansComponent } from './topic-plans/topic-plans.component';

@NgModule({
  declarations: [
    TopicPlansComponent,
    TopicPlanViewComponent,
    TopicPlanViewSkeletonComponent,
    TopicPlanViewDialogSkeletonComponent,
    TopicPlanViewDialogComponent,
    TopicPlanViewImportDialogComponent
  ],
  imports: [
    TopicPlansRoutingModule,
    ActionServiceModule,
    BannerModule,
    CommonFormUiModule,
    NomSelectModule,
    MatDialogModule,
    MatProgressSpinnerModule,
    MatSelectModule,
    PaginatorModule,
    NumberFieldModule,
    SimpleDialogSkeletonTemplateModule,
    UppyAngularDragDropModule,
    UppyAngularProgressBarModule,
    TextareaFieldModule
  ],
  providers: [DeactivateGuard]
})
export class TopicPlansModule {}
