import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { AuthenticationRoutingModule } from './authentication-routing.module';
import { LoginPage } from './pages/login/login.page';
import { SharedModule } from '@shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { OIDCService } from './auth-state-manager/oidc.service';
import { AuthService } from './auth-state-manager/auth.service';
import { SilentSigninCallbackPage } from './pages/silent-signin-callback/silent-signin-callback.page';
import { SignInCallbackPage } from './pages/signin-callback/signin-callback.page';

@NgModule({
  declarations: [LoginPage, SilentSigninCallbackPage, SignInCallbackPage],
  imports: [CommonModule, AuthenticationRoutingModule, SharedModule, FormsModule, ReactiveFormsModule],
  providers: [OIDCService, AuthService]
})
export class AuthenticationModule { }
