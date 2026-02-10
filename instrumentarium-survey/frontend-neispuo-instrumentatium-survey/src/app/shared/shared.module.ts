import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PrimeNGComponentsModule } from './modules/primeng-components.module';
import { FlexLayoutModule } from '@angular/flex-layout';
import { DotNotatedPipe } from './modules/pipes/dot-notated.pipe';
import { RouterModule } from '@angular/router';
import { ClickOutsideDirective } from './directive/click-outside.directive';

@NgModule({
  declarations: [DotNotatedPipe, ClickOutsideDirective],
  imports: [CommonModule, RouterModule, PrimeNGComponentsModule, FlexLayoutModule],
  exports: [PrimeNGComponentsModule, FlexLayoutModule, ClickOutsideDirective],
  providers: [DotNotatedPipe]
})
export class SharedModule {}
