import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FlexLayoutModule } from '@angular/flex-layout';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { UserGuideAdditionModalComponent } from '@portal/components/user-guide-addition-modal/user-guide-addition-modal.component';
import { EditorModule, TINYMCE_SCRIPT_SRC } from '@tinymce/tinymce-angular';
import { ClickOutsideDirective } from './directives/click-outside.directive';
import { PrimeNGComponentsModule } from './modules/primeng-components.module';
import { SideMenuService } from './services/side-menu.service';

@NgModule({
  declarations: [ClickOutsideDirective, UserGuideAdditionModalComponent],
  imports: [
    FormsModule,
    ReactiveFormsModule,
    CommonModule,
    FlexLayoutModule,
    FormsModule,
    ReactiveFormsModule,
    PrimeNGComponentsModule,
    EditorModule
  ],
  exports: [
    FlexLayoutModule,
    FormsModule,
    ReactiveFormsModule,
    ClickOutsideDirective,
    PrimeNGComponentsModule,
    EditorModule
  ],
  providers: [SideMenuService, { provide: TINYMCE_SCRIPT_SRC, useValue: 'tinymce/tinymce.min.js' }]
})
export class SharedModule {}
