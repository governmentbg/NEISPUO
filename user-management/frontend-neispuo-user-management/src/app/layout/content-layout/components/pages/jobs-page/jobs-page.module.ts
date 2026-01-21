import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { InputSwitchModule } from 'primeng/inputswitch';
import { SharedModule } from 'src/app/shared/shared.module';
import { JobsPageComponent } from './jobs-page.component';

@NgModule({
    declarations: [JobsPageComponent],
    imports: [CommonModule, SharedModule, InputSwitchModule],
    exports: [JobsPageComponent],
})
export class JobsPageModule {}
