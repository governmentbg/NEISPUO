import { NgModule } from '@angular/core';
import { SharedModule } from '@shared/shared.module';
import { TeacherClassPageComponent } from './teacher-class-page.component';

@NgModule({
    declarations: [TeacherClassPageComponent],
    imports: [SharedModule],
    exports: [TeacherClassPageComponent],
})
export class TeacherClassPageModule {}
