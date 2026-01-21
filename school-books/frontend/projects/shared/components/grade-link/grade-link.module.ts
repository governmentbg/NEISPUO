import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { GradePipesModule } from 'projects/shared/pipes/grade-pipes/grade-pipes.module';
import { GradeLinkComponent } from './grade-link.component';

@NgModule({
  declarations: [GradeLinkComponent],
  imports: [CommonModule, GradePipesModule],
  exports: [GradeLinkComponent]
})
export class GradeLinkModule {}
