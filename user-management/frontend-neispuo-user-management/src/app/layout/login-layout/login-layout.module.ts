import { NgModule } from '@angular/core';
import { SharedModule } from 'src/app/shared/shared.module';
import { AuthHeaderComponentModule } from './components/auth-header/auth-header.component.module';
import { LoginPageModule } from './components/pages/login-page/login-page.module';
import { SignInCallbackPageModule } from './components/pages/signin-callback-page/signin-callback-page.module';
import { SilentSignInCallbackPageModule } from './components/pages/silent-signin-callback-page/silent-signin-callback-page.module';
import { LoginLayoutComponent } from './login-layout.component';

@NgModule({
    declarations: [LoginLayoutComponent],
    imports: [
        LoginPageModule,
        SignInCallbackPageModule,
        SilentSignInCallbackPageModule,
        AuthHeaderComponentModule,
        SharedModule,
    ],
    exports: [LoginLayoutComponent],
})
export class LoginLayoutModule {}
