import { NgModule } from '@angular/core';
import { DecimalGradeColorPipe } from './decimal-grade-color.pipe';
import { DecimalGradeNamePipe } from './decimal-grade-name.pipe';
import { DecimalGradeNumberRoundedPipe } from './decimal-grade-number-rounded.pipe';
import { DecimalGradeNumberPipe } from './decimal-grade-number.pipe';
import { GradeTypeNamePipe } from './grade-type-name.pipe';
import { QualitativeGradeColorPipe } from './qualitative-grade-color.pipe';
import { QualitativeGradeEmoticonPipe } from './qualitative-grade-emoticon.pipe';
import { QualitativeGradeNamePipe } from './qualitative-grade-name.pipe';
import { QualitativeGradeShortNamePipe } from './qualitative-grade-short-name.pipe';
import { SpecialGradeNamePipe } from './special-grade-name.pipe';
import { SpecialGradeShortNamePipe } from './special-grade-short-name.pipe';

@NgModule({
  declarations: [
    DecimalGradeColorPipe,
    DecimalGradeNamePipe,
    DecimalGradeNumberPipe,
    DecimalGradeNumberRoundedPipe,
    GradeTypeNamePipe,
    QualitativeGradeColorPipe,
    QualitativeGradeEmoticonPipe,
    QualitativeGradeNamePipe,
    QualitativeGradeShortNamePipe,
    SpecialGradeNamePipe,
    SpecialGradeShortNamePipe
  ],
  imports: [],
  exports: [
    DecimalGradeColorPipe,
    DecimalGradeNamePipe,
    DecimalGradeNumberPipe,
    DecimalGradeNumberRoundedPipe,
    GradeTypeNamePipe,
    QualitativeGradeColorPipe,
    QualitativeGradeEmoticonPipe,
    QualitativeGradeNamePipe,
    QualitativeGradeShortNamePipe,
    SpecialGradeNamePipe,
    SpecialGradeShortNamePipe
  ]
})
export class GradePipesModule {}
