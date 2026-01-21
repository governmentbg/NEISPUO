import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { RouterModule } from '@angular/router';
import { FontAwesomeWithConfigModule } from 'projects/shared/font-awesome-with-config.module';
import { EditPanelComponent } from './edit-panel/edit-panel.component';
import { EditorPanelComponent } from './editor-panel.component';
import { NewPanelComponent } from './new-panel/new-panel.component';

@NgModule({
  declarations: [EditorPanelComponent, EditPanelComponent, NewPanelComponent],
  imports: [CommonModule, FontAwesomeWithConfigModule, RouterModule, MatButtonModule],
  exports: [EditorPanelComponent]
})
export class EditorPanelModule {}
