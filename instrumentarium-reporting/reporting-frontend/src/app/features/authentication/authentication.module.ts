import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { AuthenticationRoutingModule } from './authentication-routing.module';
import { SharedModule } from '@shared/shared.module';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { LoginPage } from './pages/login/login.page';
import { SigninCallbackPage } from './pages/signin-callback/signin-callback.page';
import { SilentSigninCallbackPage } from './pages/silent-signin-callback/silent-signin-callback.page';
import { AuthHeaderComponent } from './components/auth-header/auth-header.component';

@NgModule({
  declarations: [LoginPage, SigninCallbackPage, SilentSigninCallbackPage, AuthHeaderComponent],
  imports: [CommonModule, AuthenticationRoutingModule, SharedModule, FormsModule, ReactiveFormsModule]
})
export class AuthenticationModule {}
