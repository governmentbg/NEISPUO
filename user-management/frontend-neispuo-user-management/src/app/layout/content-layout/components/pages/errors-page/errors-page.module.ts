import { NgModule } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';
import { ErrorsPageComponent } from './errors-page.component';

@NgModule({
    declarations: [ErrorsPageComponent],
    imports: [SharedModule],
    exports: [ErrorsPageComponent],
})
export class ErrorsPageModule {}
