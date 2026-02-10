import { NgModule } from '@angular/core';
import { MatDialogModule } from '@angular/material/dialog';
import { MatSelectModule } from '@angular/material/select';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { DateFieldModule } from 'projects/shared/components/date-field/date-field.module';
import { NomSelectModule } from 'projects/shared/components/nom-select/nom-select.module';
import { NumberFieldModule } from 'projects/shared/components/number-field/number-field.module';
import { SelectFieldModule } from 'projects/shared/components/select-field/select-field.module';
import { SimpleDialogSkeletonTemplateModule } from 'projects/shared/components/skeleton/simple-dialog-skeleton-template.module';
import { TextareaFieldModule } from 'projects/shared/components/textarea-field/textarea-field.module';
import { DatePipesModule } from 'projects/shared/pipes/date-pipes/date-pipes.module';
import { ActionServiceModule } from 'projects/shared/services/action-service/action-service.module';
import {
  SpbsBookAbsenceViewDialogComponent,
  SpbsBookAbsenceViewDialogSkeletonComponent
} from './spbs-book-absence-view-dialog/spbs-book-absence-view-dialog.component';
import { SpbsBookDownloadDialogComponent } from './spbs-book-download-dialog/spbs-book-download-dialog.component';
import {
  SpbsBookEscapeViewDialogComponent,
  SpbsBookEscapeViewDialogSkeletonComponent
} from './spbs-book-escape-view-dialog/spbs-book-escape-view-dialog.component';
import {
  SpbsBookNewDialogComponent,
  SpbsBookNewDialogSkeletonComponent
} from './spbs-book-new-dialog/spbs-book-new-dialog.component';
import { SpbsBookRoutingModule } from './spbs-book-routing.module';
import { SpbsBookViewComponent, SpbsBookViewSkeletonComponent } from './spbs-book-view/spbs-book-view.component';
import { SpbsBookComponent } from './spbs-book/spbs-book.component';

@NgModule({
  declarations: [
    SpbsBookComponent,
    SpbsBookViewComponent,
    SpbsBookViewSkeletonComponent,
    SpbsBookDownloadDialogComponent,
    SpbsBookNewDialogComponent,
    SpbsBookNewDialogSkeletonComponent,
    SpbsBookEscapeViewDialogComponent,
    SpbsBookEscapeViewDialogSkeletonComponent,
    SpbsBookAbsenceViewDialogComponent,
    SpbsBookAbsenceViewDialogSkeletonComponent
  ],
  imports: [
    SpbsBookRoutingModule,
    CommonFormUiModule,
    ActionServiceModule,
    BannerModule,
    DateFieldModule,
    DatePipesModule,
    MatDialogModule,
    MatSelectModule,
    NomSelectModule,
    NumberFieldModule,
    SelectFieldModule,
    SimpleDialogSkeletonTemplateModule,
    TextareaFieldModule
  ]
})
export class SpbsBookModule {}
