import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { GradePipesModule } from 'projects/shared/pipes/grade-pipes/grade-pipes.module';
import { QualitativeGradePickerComponent } from './qualitative-grade-picker.component';

@NgModule({
  declarations: [QualitativeGradePickerComponent],
  imports: [CommonModule, GradePipesModule],
  exports: [QualitativeGradePickerComponent]
})
export class QualitativeGradePickerModule {}
