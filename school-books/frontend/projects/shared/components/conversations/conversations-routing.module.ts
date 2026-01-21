import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { ConversationsNewComponent } from './conversations-new/conversations-new.component';
import { ConversationViewSkeletonComponent } from './conversations-view/conversations-view.component';
import { ConversationsComponent } from './conversations/conversations.component';

const routes: Routes = [
  {
    path: 'new',
    component: ConversationsNewComponent
  },
  {
    path: '',
    component: ConversationsComponent,
    children: [
      {
        path: ':conversationSchoolYear/:conversationId',
        component: ConversationViewSkeletonComponent
      }
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ConversationsRoutingModule {}
