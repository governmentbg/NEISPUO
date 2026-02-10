import { Injectable, NgModule } from '@angular/core';
import {
  ActivatedRouteSnapshot,
  CanActivate,
  Router,
  RouterModule,
  RouterStateSnapshot,
  Routes,
  UrlTree
} from '@angular/router';
import { SnackbarRootComponent } from 'projects/shared/components/snackbar-root/snackbar-root.component';
import { AuthService, SysRole } from 'projects/shared/services/auth.service';
import { Project } from 'projects/shared/services/config.service';
import { StudentComponent } from './student.component';

@Injectable({
  providedIn: 'root'
})
export class StudentConversationsGuard implements CanActivate {
  constructor(private authService: AuthService, private router: Router) {}

  canActivate(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): boolean | UrlTree {
    const hasAccess = this.authService.tokenPayload.selected_role.SysRoleID === SysRole.Parent;
    if (!hasAccess) {
      return this.router.createUrlTree(['/not-found']);
    }
    return true;
  }
}

const routes: Routes = [
  {
    path: '',
    component: SnackbarRootComponent,
    children: [
      {
        path: ':schoolYear',
        component: StudentComponent,
        children: [
          // empty route
          { path: '', children: [] },
          {
            path: 'medical-notices',
            loadChildren: () =>
              import('./student-medical-notices/student-medical-notices.module').then(
                (m) => m.StudentMedicalNoticesModule
              )
          },
          {
            path: 'info-board',
            loadChildren: () =>
              import('./student-info-board/student-info-board.module').then((m) => m.StudentInfoBoardModule)
          },
          // book
          {
            path: 'book',
            loadChildren: () => import('./book/book.module').then((m) => m.BookModule)
          },
          // conversations
          {
            path: 'conversations',
            canActivate: [StudentConversationsGuard],
            loadChildren: () =>
              import('../../../shared/components/conversations/conversations.module').then(
                (m) => m.ConversationsModule
              ),
            data: { project: Project.StudentsApp }
          }
        ]
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class StudentRoutingModule {}
