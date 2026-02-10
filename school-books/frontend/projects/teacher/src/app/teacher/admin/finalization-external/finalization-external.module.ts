import { NgModule } from '@angular/core';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatTooltipModule } from '@angular/material/tooltip';
import { UppyAngularDragDropModule, UppyAngularProgressBarModule } from '@uppy/angular';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { DeactivateGuard } from 'projects/shared/guards/deactivate-guard';
import { ActionServiceModule } from 'projects/shared/services/action-service/action-service.module';
import { FinalizationExternalRoutingModule } from './finalization-external-routing.module';
import {
  FinalizationExternalComponent,
  FinalizationExternalSkeletonComponent
} from './finalization-external/finalization-external.component';

@NgModule({
  declarations: [FinalizationExternalComponent, FinalizationExternalSkeletonComponent],
  imports: [
    FinalizationExternalRoutingModule,
    CommonFormUiModule,
    ActionServiceModule,
    MatCheckboxModule,
    MatTooltipModule,
    BannerModule,
    UppyAngularDragDropModule,
    UppyAngularProgressBarModule
  ],
  providers: [DeactivateGuard]
})
export class FinalizationExternalModule {}
