import { NgModule } from '@angular/core';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { PaginatorModule } from 'projects/shared/components/paginator/paginator.module';
import { SimplePageSkeletonTemplateModule } from 'projects/shared/components/skeleton/simple-page-skeleton-template.module';
import { SimpleTabSkeletonTemplateModule } from 'projects/shared/components/skeleton/simple-tab-skeleton-template.module';
import { TabsModule } from 'projects/shared/components/tabs/tabs.module';
import { VerticalTabsModule } from 'projects/shared/components/vertical-tabs/vertical-tabs.module';
import { DatePipesModule } from 'projects/shared/pipes/date-pipes/date-pipes.module';
import { StudentInfoRoutingModule } from './student-info-routing.module';
import { StudentInfoComponent, StudentInfoSkeletonComponent } from './student-info/student-info.component';

@NgModule({
  declarations: [StudentInfoComponent, StudentInfoSkeletonComponent],
  imports: [
    StudentInfoRoutingModule,
    CommonFormUiModule,
    BannerModule,
    SimpleTabSkeletonTemplateModule,
    SimplePageSkeletonTemplateModule,
    TabsModule,
    VerticalTabsModule,
    DatePipesModule,
    PaginatorModule
  ]
})
export class StudentInfoModule {}
