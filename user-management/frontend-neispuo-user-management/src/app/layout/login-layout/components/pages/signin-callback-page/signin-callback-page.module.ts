import { NgModule } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';
import { SignInCallbackPageComponent } from './signin-callback-page.component';

@NgModule({
    declarations: [SignInCallbackPageComponent],
    imports: [SharedModule],
    exports: [SignInCallbackPageComponent],
})
export class SignInCallbackPageModule {}
