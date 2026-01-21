import { CdkVirtualScrollViewport } from '@angular/cdk/scrolling';
import { AfterViewInit, Component, Input, OnInit, ViewChild } from '@angular/core';
import { UntypedFormBuilder } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { faInfoCircle as fadInfoCircle } from '@fortawesome/pro-duotone-svg-icons/faInfoCircle';
import {
  ConversationsService,
  Conversations_GetConversationInfo
} from 'projects/sb-api-client/src/api/conversations.service';
import { ConversationsGetConversationMessagesVO } from 'projects/sb-api-client/src/model/conversationsGetConversationMessagesVO';
import {
  SIMPLE_TAB_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { EventService, EventType } from 'projects/shared/services/event.service';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { BehaviorSubject } from 'rxjs';

@Component({
  template: SIMPLE_TAB_SKELETON_TEMPLATE
})
export class ConversationViewSkeletonComponent extends SkeletonComponentBase {
  constructor(conversationsService: ConversationsService, route: ActivatedRoute) {
    super();
    const schoolYear =
      tryParseInt(route.snapshot.paramMap.get('conversationSchoolYear')) ?? throwParamError('schoolYear');
    const conversationId =
      tryParseInt(route.snapshot.paramMap.get('conversationId')) ?? throwParamError('conversationId');

    this.resolve(ConversationViewComponent, {
      conversationInfo: conversationsService.getConversationInfo({
        conversationSchoolYear: schoolYear,
        conversationId: conversationId
      }),
      conversationSchoolYear: schoolYear,
      conversationId: conversationId
    });
  }
}

@Component({
  selector: 'sb-conversations-messages',
  templateUrl: './conversations-view.component.html',
  styleUrls: ['./conversations-view.component.scss']
})
export class ConversationViewComponent implements OnInit, AfterViewInit {
  @ViewChild(CdkVirtualScrollViewport) viewport!: CdkVirtualScrollViewport;
  @Input() data!: {
    conversationInfo: Conversations_GetConversationInfo;
    conversationSchoolYear: number;
    conversationId: number;
  };

  readonly form = this.fb.group({
    message: [null]
  });

  messagesData$ = new BehaviorSubject<ConversationsGetConversationMessagesVO[]>([]);

  private offset = 0;
  private limit = 50;
  private hasMoreData = true;
  isFetching = false;
  expendMainPanel = false;

  readonly fadInfoCircle = fadInfoCircle;

  constructor(
    private fb: UntypedFormBuilder,
    public conversationsService: ConversationsService,
    private actionService: ActionService,
    private route: ActivatedRoute,
    private eventService: EventService
  ) {}

  ngOnInit(): void {
    this.loadMessages(true);
  }

  ngAfterViewInit() {
    if (this.messagesData$.getValue().length > 0) {
      this.scrollToIndex(this.messagesData$.getValue().length - 1);
    }
  }

  getParticipantName(participantId: number): string {
    const participant = this.data.conversationInfo?.participants.find((p) => p.participantId === participantId);
    return participant ? participant.title : 'Неизвестен';
  }

  sendMessage(): void {
    const messageText = this.form.get('message')?.value;
    if (messageText && messageText.trim()) {
      this.isFetching = true;

      this.actionService
        .execute({
          httpAction: () =>
            this.conversationsService
              .addMessage({
                conversationSchoolYear: this.data.conversationSchoolYear,
                addConversationMessageCommand: {
                  conversationId: this.data.conversationId,
                  message: messageText
                }
              })
              .toPromise()
              .then(() => {
                this.form.get('message')?.reset();
                this.isFetching = false;
                this.offset = 0;
                this.limit = 20;
                this.hasMoreData = true;
                this.loadMessages(true);
              })
              .then(() => {
                this.scrollToIndex(this.messagesData$.getValue().length - 1);
              })
        })
        .finally(() => {
          this.isFetching = false;
        });
    }
  }

  loadMessages(clearForm = false): void {
    if (!this.hasMoreData || this.isFetching) {
      return;
    }

    this.isFetching = true;

    this.conversationsService
      .getConversationMessages({
        conversationSchoolYear: this.data.conversationSchoolYear,
        conversationId: this.data.conversationId,
        offset: this.offset,
        limit: this.limit
      })
      .toPromise()
      .then((data) => {
        const currentData = this.messagesData$.getValue();
        const revertedData = data.reverse();
        const newMessages = clearForm ? revertedData : [...revertedData, ...currentData];
        this.messagesData$.next(newMessages);

        if (data.length < this.limit) {
          this.hasMoreData = false;
        }
      })
      .finally(() => {
        this.isFetching = false;

        if (clearForm) {
          this.scrollToIndex(this.messagesData$.getValue().length - 1);
        }

        this.eventService.dispatch({
          type: EventType.ConversationRead,
          args: { conversationId: this.data.conversationId }
        });
      });
  }

  onScrolledIndexChange(index: number): void {
    if (index - 5 < 0 && this.hasMoreData && !this.isFetching) {
      this.offset += this.limit;
      this.loadMessages();
    }
  }

  scrollToIndex(index: number): void {
    if (this.viewport) {
      setTimeout(() => this.viewport.scrollToIndex(index));
    }
  }

  getCreator() {
    return this.data.conversationInfo.participants.find((participant) => participant.isCreator)?.title;
  }

  getReadParticipants(): string {
    const participants = this.data.conversationInfo.participants
      .filter((participant) => participant.didReadLastMessage && participant.isCreator === false)
      .map((participant) => participant.title);

    return participants.length > 0 ? participants.join(', ') : 'няма прочели последното съобщение';
  }

  togglePanel(): void {
    this.expendMainPanel = !this.expendMainPanel;
  }
}
