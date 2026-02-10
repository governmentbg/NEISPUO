import { NgModule } from '@angular/core';
import { UppyAngularDragDropModule, UppyAngularProgressBarModule } from '@uppy/angular';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { DateFieldModule } from 'projects/shared/components/date-field/date-field.module';
import { NomSelectModule } from 'projects/shared/components/nom-select/nom-select.module';
import { TextareaFieldModule } from 'projects/shared/components/textarea-field/textarea-field.module';
import { DeactivateGuard } from 'projects/shared/guards/deactivate-guard';
import { DatePipesModule } from 'projects/shared/pipes/date-pipes/date-pipes.module';
import { ActionServiceModule } from 'projects/shared/services/action-service/action-service.module';
import {
  PublicationViewComponent,
  PublicationViewSkeletonComponent
} from './publication-view/publication-view.component';
import { PublicationsRoutingModule } from './publications-routing.module';
import { PublicationsComponent } from './publications/publications.component';

@NgModule({
  declarations: [PublicationsComponent, PublicationViewComponent, PublicationViewSkeletonComponent],
  imports: [
    PublicationsRoutingModule,
    CommonFormUiModule,
    ActionServiceModule,
    DatePipesModule,
    DateFieldModule,
    NomSelectModule,
    TextareaFieldModule,
    UppyAngularDragDropModule,
    UppyAngularProgressBarModule
  ],
  providers: [DeactivateGuard]
})
export class PublicationsModule {}
