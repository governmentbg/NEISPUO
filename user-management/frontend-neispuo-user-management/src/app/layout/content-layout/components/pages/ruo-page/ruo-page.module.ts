import { NgModule } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';
import { RuoPageComponent } from './ruo-page.component';

@NgModule({
    declarations: [RuoPageComponent],
    imports: [SharedModule],
    exports: [RuoPageComponent],
})
export class RuoPageModule {}
