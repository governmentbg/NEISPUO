import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { TranslateModule } from '@ngx-translate/core';
import { HeadingComponent } from './heading.component';

@NgModule({
    declarations: [HeadingComponent],
    imports: [CommonModule, TranslateModule],
    exports: [HeadingComponent],
})
export class HeadingModule {}
