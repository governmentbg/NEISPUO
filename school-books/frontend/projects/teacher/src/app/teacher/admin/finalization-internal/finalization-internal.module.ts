import { NgModule } from '@angular/core';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatTooltipModule } from '@angular/material/tooltip';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { DeactivateGuard } from 'projects/shared/guards/deactivate-guard';
import { ActionServiceModule } from 'projects/shared/services/action-service/action-service.module';
import { FinalizationInternalRoutingModule } from './finalization-internal-routing.module';
import {
  FinalizationInternalComponent,
  FinalizationInternalSkeletonComponent
} from './finalization-internal/finalization-internal.component';

@NgModule({
  declarations: [FinalizationInternalComponent, FinalizationInternalSkeletonComponent],
  imports: [
    FinalizationInternalRoutingModule,
    CommonFormUiModule,
    ActionServiceModule,
    MatCheckboxModule,
    MatTooltipModule,
    BannerModule
  ],
  providers: [DeactivateGuard]
})
export class FinalizationInternalModule {}
