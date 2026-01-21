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
import { ParentRegisterPage } from './pages/parent-register/parent-register.page';
import { ToastModule } from '@shared/components/toast/toast.module';
import { AuthHeaderComponent } from './components/auth-header/auth-header.component';

@NgModule({
  declarations: [LoginPage, SilentSigninCallbackPage, SignInCallbackPage, ParentRegisterPage, AuthHeaderComponent],
  imports: [CommonModule, AuthenticationRoutingModule, SharedModule, FormsModule, ReactiveFormsModule, ToastModule],
  providers: [OIDCService, AuthService]
})
export class AuthenticationModule {}
