import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { DeactivateGuard } from 'projects/shared/guards/deactivate-guard';
import { TopicPlanViewSkeletonComponent } from './topic-plan-view/topic-plan-view.component';
import { TopicPlansComponent } from './topic-plans/topic-plans.component';

const routes: Routes = [
  { path: '', component: TopicPlansComponent, canDeactivate: [DeactivateGuard] },
  { path: ':topicPlanId', component: TopicPlanViewSkeletonComponent, canDeactivate: [DeactivateGuard] }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TopicPlansRoutingModule {}
