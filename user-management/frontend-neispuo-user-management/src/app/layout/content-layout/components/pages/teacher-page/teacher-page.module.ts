import { NgModule } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';
import { TeacherPageComponent } from './teacher-page.component';

@NgModule({
    declarations: [TeacherPageComponent],
    imports: [SharedModule],
    exports: [TeacherPageComponent],
})
export class TeacherPageModule {}
