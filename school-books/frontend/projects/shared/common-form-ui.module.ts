import { CdkTableModule } from '@angular/cdk/table';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ReactiveFormsModule } from '@angular/forms';
import { MatButtonModule } from '@angular/material/button';
import { MatInputModule } from '@angular/material/input';
import { MatSortModule } from '@angular/material/sort';
import { BreadcrumbModule } from './components/breadcrumb/breadcrumb.module';
import { CardSectionModule } from './components/card-section/card-section.module';
import { CardModule } from './components/card/card.module';
import { EditorPanelModule } from './components/editor-panel/editor-panel.module';
import { SimplePageSkeletonTemplateModule } from './components/skeleton/simple-page-skeleton-template.module';
import { TableModule } from './components/table/table.module';
import { TextFieldModule } from './components/text-field/text-field.module';
import { FontAwesomeWithConfigModule } from './font-awesome-with-config.module';

@NgModule({
  declarations: [],
  imports: [],
  exports: [
    BreadcrumbModule,
    CardModule,
    CardSectionModule,
    CdkTableModule,
    CommonModule,
    EditorPanelModule,
    FontAwesomeWithConfigModule,
    MatButtonModule,
    MatInputModule,
    MatSortModule,
    ReactiveFormsModule,
    SimplePageSkeletonTemplateModule,
    TableModule,
    TextFieldModule
  ]
})
export class CommonFormUiModule {}
