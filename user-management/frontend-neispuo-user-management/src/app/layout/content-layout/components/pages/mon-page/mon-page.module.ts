import { NgModule } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';
import { MonPageComponent } from './mon-page.component';

@NgModule({
    declarations: [MonPageComponent],
    imports: [SharedModule],
    exports: [MonPageComponent],
})
export class MonPageModule {}
