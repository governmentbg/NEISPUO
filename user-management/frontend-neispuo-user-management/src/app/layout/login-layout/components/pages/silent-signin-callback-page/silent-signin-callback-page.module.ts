import { NgModule } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';
import { SilentSigninCallbackPageComponent } from './silent-signin-callback-page.component';

@NgModule({
    declarations: [SilentSigninCallbackPageComponent],
    imports: [SharedModule],
    exports: [SilentSigninCallbackPageComponent],
})
export class SilentSignInCallbackPageModule {}
