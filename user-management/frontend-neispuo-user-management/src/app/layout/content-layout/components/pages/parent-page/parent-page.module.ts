import { NgModule } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';
import { ParentPageComponent } from './parent-page.component';

@NgModule({
    declarations: [ParentPageComponent],
    imports: [SharedModule],
    exports: [ParentPageComponent],
})
export class ParentPageModule {}
