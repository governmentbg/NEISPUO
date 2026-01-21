import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { FontAwesomeWithConfigModule } from 'projects/shared/font-awesome-with-config.module';
import { PaginatorComponent } from './paginator.component';

@NgModule({
  declarations: [PaginatorComponent],
  imports: [CommonModule, FontAwesomeWithConfigModule, RouterModule],
  exports: [PaginatorComponent]
})
export class PaginatorModule {}
