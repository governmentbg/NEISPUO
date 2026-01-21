import { NgModule } from '@angular/core';
import { CommonFormUiModule } from 'projects/shared/common-form-ui.module';
import { BannerModule } from 'projects/shared/components/banner/banner.module';
import { PaginatorModule } from 'projects/shared/components/paginator/paginator.module';
import { DatePipesModule } from 'projects/shared/pipes/date-pipes/date-pipes.module';
import { InfoBoardRoutingModule } from './info-board-routing.module';
import { InfoBoardViewComponent, InfoBoardViewSkeletonComponent } from './info-board-view/info-board-view.component';
import { InfoBoardComponent, InfoBoardSkeletonComponent } from './info-board/info-board.component';

@NgModule({
  declarations: [
    InfoBoardComponent,
    InfoBoardSkeletonComponent,
    InfoBoardViewComponent,
    InfoBoardViewSkeletonComponent
  ],
  imports: [InfoBoardRoutingModule, CommonFormUiModule, BannerModule, DatePipesModule, PaginatorModule]
})
export class InfoBoardModule {}
