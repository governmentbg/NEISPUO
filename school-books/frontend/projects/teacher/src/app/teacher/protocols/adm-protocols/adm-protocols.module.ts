import { NgModule } from '@angular/core';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatDialogModule } from '@angular/material/dialog';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { DateFieldModule } from 'projects/shared/components/date-field/date-field.module';
import { NomSelectModule } from 'projects/shared/components/nom-select/nom-select.module';
import { SimpleDialogSkeletonTemplateModule } from 'projects/shared/components/skeleton/simple-dialog-skeleton-template.module';
import { DeactivateGuard } from 'projects/shared/guards/deactivate-guard';
import { DatePipesModule } from 'projects/shared/pipes/date-pipes/date-pipes.module';
import { ActionServiceModule } from 'projects/shared/services/action-service/action-service.module';
import { StateExamsAdmProtocolsRoutingModule } from './adm-protocols-routing.module';
import {
  AdmProtocolsTypeDialogComponent,
  AdmProtocolsTypeDialogSkeletonComponent
} from './adm-protocols-type-dialog.component';
import { AdmProtocolsComponent } from './adm-protocols.component';
import {
  AddOrUpdateStudentGceAdmProtocolDialogComponent,
  AddOrUpdateStudentGceAdmProtocolDialogSkeletonComponent
} from './grade-change-exams-adm-protocols/add-or-update-student-gce-adm-protocol-dialog/add-or-update-student-gce-adm-protocol-dialog.component';
import {
  GradeChangeExamsAdmProtocolViewComponent,
  GradeChangeExamsAdmProtocolViewSkeletonComponent
} from './grade-change-exams-adm-protocols/grade-change-exams-adm-protocol-view/grade-change-exams-adm-protocol-view.component';
import {
  AddOrUpdateStudentSeAdmProtocolDialogComponent,
  AddOrUpdateStudentSeAdmProtocolDialogSkeletonComponent
} from './state-exams-adm-protocols/add-or-update-student-se-adm-protocol-dialog/add-or-update-student-se-adm-protocol-dialog.component';
import {
  StateExamsAdmProtocolViewComponent,
  StateExamsAdmProtocolViewSkeletonComponent
} from './state-exams-adm-protocols/state-exams-adm-protocol-view/state-exams-adm-protocol-view.component';

@NgModule({
  declarations: [
    AdmProtocolsComponent,
    StateExamsAdmProtocolViewSkeletonComponent,
    StateExamsAdmProtocolViewComponent,
    AddOrUpdateStudentSeAdmProtocolDialogComponent,
    AddOrUpdateStudentSeAdmProtocolDialogSkeletonComponent,
    GradeChangeExamsAdmProtocolViewSkeletonComponent,
    GradeChangeExamsAdmProtocolViewComponent,
    AddOrUpdateStudentGceAdmProtocolDialogComponent,
    AddOrUpdateStudentGceAdmProtocolDialogSkeletonComponent,
    AdmProtocolsTypeDialogSkeletonComponent,
    AdmProtocolsTypeDialogComponent
  ],
  imports: [
    StateExamsAdmProtocolsRoutingModule,
    CommonFormUiModule,
    ActionServiceModule,
    BannerModule,
    DateFieldModule,
    DatePipesModule,
    MatCheckboxModule,
    MatDialogModule,
    NomSelectModule,
    SimpleDialogSkeletonTemplateModule
  ],
  providers: [DeactivateGuard]
})
export class AdmProtocolsModule {}
