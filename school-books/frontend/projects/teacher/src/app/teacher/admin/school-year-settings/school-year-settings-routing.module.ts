import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DeactivateGuard } from 'projects/shared/guards/deactivate-guard';
import { SchoolYearSettingsViewSkeletonComponent } from './school-year-settings-view/school-year-settings-view.component';
import { SchoolYearSettingsComponent } from './school-year-settings/school-year-settings.component';

const routes: Routes = [
  { path: '', component: SchoolYearSettingsComponent, canDeactivate: [DeactivateGuard] },
  {
    path: ':schoolYearSettingsId',
    component: SchoolYearSettingsViewSkeletonComponent,
    canDeactivate: [DeactivateGuard]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SchoolYearSettingsRoutingModule {}
