import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DeactivateGuard } from 'projects/shared/guards/deactivate-guard';
import { PublicationViewSkeletonComponent } from './publication-view/publication-view.component';
import { PublicationsComponent } from './publications/publications.component';

const routes: Routes = [
  { path: '', component: PublicationsComponent },
  { path: 'new', component: PublicationViewSkeletonComponent, canDeactivate: [DeactivateGuard] },
  { path: ':publicationId', component: PublicationViewSkeletonComponent, canDeactivate: [DeactivateGuard] },
  { path: '**', redirectTo: '/not-found' }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class PublicationsRoutingModule {}
