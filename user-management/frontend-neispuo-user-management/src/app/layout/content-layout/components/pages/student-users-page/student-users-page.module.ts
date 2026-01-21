import { NgModule } from '@angular/core';
import { DotNotatedPipe } from '@shared/pipes/dot-notated.pipe';
import { SharedModule } from 'src/app/shared/shared.module';
import { StudentUsersPageComponent } from './student-users-page.component';

@NgModule({
    declarations: [StudentUsersPageComponent],
    imports: [SharedModule],
    exports: [StudentUsersPageComponent],
    providers: [DotNotatedPipe],
})
export class StudentUsersPageModule {}
