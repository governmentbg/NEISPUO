import { NgModule } from '@angular/core';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { DateAdapter, MAT_DATE_FORMATS } from '@angular/material/core';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatDialogModule } from '@angular/material/dialog';
import { MatMenuModule } from '@angular/material/menu';
import { MatProgressSpinnerModule } from '@angular/material/progress-spinner';
import { MatSelectModule } from '@angular/material/select';
import { MatTooltipModule } from '@angular/material/tooltip';
import { NgSelectModule } from '@ng-select/ng-select';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { BookSettingsModule } from 'projects/shared/components/book-settings/book-settings.module';
import { SbDateAdapter, SB_DATE_FORMATS } from 'projects/shared/components/date-field/SbDateAdapter';
import { SbDatepickerIntl } from 'projects/shared/components/date-field/SbDatepickerIntl';
import { DecimalGradePickerModule } from 'projects/shared/components/decimal-grade-picker/decimal-grade-picker.module';
import { GradeLinkModule } from 'projects/shared/components/grade-link/grade-link.module';
import { NomSelectModule } from 'projects/shared/components/nom-select/nom-select.module';
import { PlaceholderFieldModule } from 'projects/shared/components/placeholder-field/placeholder-field.module';
import { QualitativeGradePickerModule } from 'projects/shared/components/qualitative-grade-picker/qualitative-grade-picker.module';
import { SimpleDialogSkeletonTemplateModule } from 'projects/shared/components/skeleton/simple-dialog-skeleton-template.module';
import { SimpleSkeletonTemplateModule } from 'projects/shared/components/skeleton/simple-skeleton-template.module';
import { TextareaFieldModule } from 'projects/shared/components/textarea-field/textarea-field.module';
import { VerticalTabsModule } from 'projects/shared/components/vertical-tabs/vertical-tabs.module';
import { DeactivateGuard } from 'projects/shared/guards/deactivate-guard';
import { DatePipesModule } from 'projects/shared/pipes/date-pipes/date-pipes.module';
import { GradePipesModule } from 'projects/shared/pipes/grade-pipes/grade-pipes.module';
import { ActionServiceModule } from 'projects/shared/services/action-service/action-service.module';
import { AddAbsencesComponent, AddAbsencesSkeletonComponent } from './add-absences/add-absences.component';
import { AddGradesComponent, AddGradesSkeletonComponent } from './add-grades/add-grades.component';
import { AddTopicComponent, AddTopicSkeletonComponent } from './add-topic/add-topic.component';
import {
  MyHourLessonViewContentComponent,
  MyHourLessonViewContentSkeletonComponent
} from './my-hour-lesson-view-content/my-hour-lesson-view-content.component';
import {
  MyHourLessonViewComponent,
  MyHourLessonViewSkeletonComponent
} from './my-hour-lesson-view/my-hour-lesson-view.component';
import { MyHourRoutingModule } from './my-hour-routing.module';

@NgModule({
  declarations: [
    MyHourLessonViewComponent,
    MyHourLessonViewSkeletonComponent,
    MyHourLessonViewContentComponent,
    MyHourLessonViewContentSkeletonComponent,
    AddTopicComponent,
    AddTopicSkeletonComponent,
    AddGradesSkeletonComponent,
    AddGradesComponent,
    AddAbsencesSkeletonComponent,
    AddAbsencesComponent
  ],
  imports: [
    MyHourRoutingModule,
    CommonFormUiModule,
    MatCheckboxModule,
    MatDatepickerModule,
    MatDialogModule,
    MatSelectModule,
    SimpleSkeletonTemplateModule,
    SimpleDialogSkeletonTemplateModule,
    VerticalTabsModule,
    BannerModule,
    DecimalGradePickerModule,
    GradeLinkModule,
    GradePipesModule,
    QualitativeGradePickerModule,
    ActionServiceModule,
    TextareaFieldModule,
    DatePipesModule,
    NomSelectModule,
    PlaceholderFieldModule,
    NgSelectModule,
    MatProgressSpinnerModule,
    MatTooltipModule,
    MatMenuModule,
    BookSettingsModule
  ],
  exports: [MyHourLessonViewContentComponent],
  providers: [
    DeactivateGuard,
    SbDatepickerIntl,
    { provide: MAT_DATE_FORMATS, useValue: SB_DATE_FORMATS },
    { provide: DateAdapter, useClass: SbDateAdapter }
  ]
})
export class MyHourModule {}
