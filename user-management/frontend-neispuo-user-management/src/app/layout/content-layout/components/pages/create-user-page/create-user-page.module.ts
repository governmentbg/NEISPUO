import { NgModule } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';
import { CreateUserPageComponent } from './create-user-page.component';

@NgModule({
    declarations: [CreateUserPageComponent],
    imports: [SharedModule],
    exports: [CreateUserPageComponent],
})
export class CreateUserPageModule {}
