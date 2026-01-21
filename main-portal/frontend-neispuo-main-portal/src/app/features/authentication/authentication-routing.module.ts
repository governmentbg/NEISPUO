import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { NoAuthGuard } from './guards/no-auth.guard';
import { LoginPage } from './pages/login/login.page';
import { ParentRegisterPage } from './pages/parent-register/parent-register.page';
import { SignInCallbackPage } from './pages/signin-callback/signin-callback.page';
import { SilentSigninCallbackPage } from './pages/silent-signin-callback/silent-signin-callback.page';

const routes: Routes = [
  { path: 'login', component: LoginPage, canActivate: [NoAuthGuard] },
  { path: 'signin-callback', component: SignInCallbackPage },
  { path: 'silent-signin-callback', component: SilentSigninCallbackPage },
  { path: 'parent-register', component: ParentRegisterPage, canActivate: [NoAuthGuard] }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AuthenticationRoutingModule {}
