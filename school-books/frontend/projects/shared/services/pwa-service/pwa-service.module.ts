import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MatBottomSheetModule } from '@angular/material/bottom-sheet';
import { MatButtonModule } from '@angular/material/button';
import { MatCardModule } from '@angular/material/card';
import { MatToolbarModule } from '@angular/material/toolbar';
import { FontAwesomeWithConfigModule } from 'projects/shared/font-awesome-with-config.module';
import { PromptComponent } from './prompt/prompt.component';
import { PwaService } from './pwa.service';

@NgModule({
  declarations: [PromptComponent],
  imports: [
    CommonModule,
    FontAwesomeWithConfigModule,
    MatBottomSheetModule,
    MatToolbarModule,
    MatCardModule,
    MatButtonModule
  ],
  exports: [],
  providers: [{ provide: PwaService }]
})
export class PwaServiceModule {}
