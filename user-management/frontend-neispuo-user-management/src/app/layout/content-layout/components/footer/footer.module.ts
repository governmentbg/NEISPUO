import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';
import { FooterComponent } from './footer.component';

@NgModule({
    declarations: [FooterComponent],
    imports: [CommonModule, SharedModule],
    exports: [FooterComponent],
})
export class FooterModule {}
