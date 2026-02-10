import { CdkVirtualScrollViewport } from '@angular/cdk/scrolling';
import { AfterViewChecked, Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { MatSelect, MatSelectChange } from '@angular/material/select';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { faEnvelope as fasEnvelop } from '@fortawesome/pro-solid-svg-icons/faEnvelope';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { ConversationsService } from 'projects/sb-api-client/src/api/conversations.service';
import { ConversationsGetConversationsVO } from 'projects/sb-api-client/src/model/conversationsGetConversationsVO';
import { EventService } from 'projects/shared/services/event.service';
import { tryParseInt } from 'projects/shared/utils/various';
import { BehaviorSubject, Subject } from 'rxjs';
import { filter, startWith, takeUntil } from 'rxjs/operators';

@Component({
  selector: 'sb-conversations',
  templateUrl: './conversations.component.html',
  styleUrls: ['./conversations.component.scss']
})
export class ConversationsComponent implements OnInit, OnDestroy, AfterViewChecked {
  @ViewChild(CdkVirtualScrollViewport) viewport!: CdkVirtualScrollViewport;
  @ViewChild('matSelect') matSelect!: MatSelect;

  selectedConversation = -1;
  hasConversations = false;
  conversationsData$ = new BehaviorSubject<ConversationsGetConversationsVO[]>([]);

  private readonly destroyed$ = new Subject<void>();
  private isMatSelectInitialized = false;

  readonly fasPlus = fasPlus;
  readonly fasEnvelop = fasEnvelop;

  private offset = 0;
  private limit = 20;
  private hasMoreData = true;
  private isFetching = false;

  constructor(
    private conversationsService: ConversationsService,
    private router: Router,
    private eventService: EventService,
    public route: ActivatedRoute
  ) {}

  ngOnInit() {
    this.router.events
      .pipe(
        filter((event) => event instanceof NavigationEnd),
        startWith(null),
        takeUntil(this.destroyed$)
      )
      .subscribe(() => {
        const routeId = tryParseInt(this.route.firstChild?.snapshot.paramMap.get('conversationId'));

        if (!this.hasConversations) {
          this.initialLoad();
        } else if (this.hasConversations && routeId) {
          this.navigateToConversation(routeId);
        }
      });
  }

  ngAfterViewChecked(): void {
    if (this.matSelect && !this.isMatSelectInitialized && this.hasConversations) {
      this.matSelect.openedChange.subscribe((isOpened) => {
        if (isOpened) {
          this.addScrollEventListener();
        } else {
          this.removeScrollEventListener();
        }
      });
      this.isMatSelectInitialized = true;
    }
  }

  ngOnDestroy() {
    this.destroyed$.next();
    this.destroyed$.complete();
    this.removeScrollEventListener();
  }

  initialLoad() {
    this.isFetching = true;

    this.conversationsService
      .getConversations({ offset: this.offset, limit: this.limit })
      .toPromise()
      .then((data) => {
        this.conversationsData$.next(data);
        this.hasConversations = this.conversationsData$.getValue().length > 0;

        if (data.length < this.limit) {
          this.hasMoreData = false;
        }
      })
      .then(() => {
        if (this.hasConversations) {
          const routeId = tryParseInt(this.route.firstChild?.snapshot.paramMap.get('conversationId'));

          if (routeId) {
            this.selectedConversation = routeId;
          } else {
            this.selectedConversation = this.conversationsData$.getValue()[0].conversationId;
          }

          this.navigateToConversation(this.selectedConversation);
        }
      })
      .finally(() => {
        this.isFetching = false;
      });
  }

  loadConversations() {
    if (this.isFetching || !this.hasMoreData) {
      return;
    }

    this.isFetching = true;

    this.conversationsService
      .getConversations({ offset: this.offset, limit: this.limit })
      .toPromise()
      .then((data) => {
        const currentData = this.conversationsData$.getValue();
        this.conversationsData$.next([...currentData, ...data]);
        this.hasConversations = this.conversationsData$.getValue().length > 0;

        if (data.length < this.limit) {
          this.hasMoreData = false;
        }
      })
      .finally(() => {
        this.isFetching = false;
      });
  }

  addScrollEventListener(): void {
    if (this.matSelect && this.matSelect?.panel) {
      this.matSelect.panel.nativeElement.addEventListener('scroll', this.onDropdownScroll.bind(this));
    }
  }

  removeScrollEventListener(): void {
    if (this.matSelect && this.matSelect?.panel) {
      this.matSelect.panel.nativeElement.removeEventListener('scroll', this.onDropdownScroll.bind(this));
    }
  }

  onScrolledIndexChange(index: number): void {
    const totalLoaded = this.conversationsData$.getValue().length;

    if (index + 5 >= totalLoaded) {
      this.offset += this.limit;
      this.loadConversations();
    }
  }

  onDropdownScroll(event: Event): void {
    const target = event.target as HTMLElement;
    const atBottom = target.scrollTop + target.clientHeight >= target.scrollHeight;

    if (atBottom) {
      this.offset += this.limit;
      this.loadConversations();
    }
  }

  onDropdownSelect(event: MatSelectChange): void {
    const conversationId = event.value;

    if (conversationId) {
      this.navigateToConversation(conversationId);
    }
  }

  navigateToConversation(conversationId: number): void {
    this.selectedConversation = conversationId;
    const conversation = this.conversationsData$.getValue().find((c) => c.conversationId === conversationId);

    if (conversation) {
      const index = this.conversationsData$.getValue().indexOf(conversation);
      this.scrollToIndex(index);
      this.markConversationAsRead(conversationId);
      this.router.navigate(['./', conversation?.schoolYear, conversationId], { relativeTo: this.route });
    }
  }
  markConversationAsRead(conversationId: number) {
    const conversations = this.conversationsData$.getValue();
    const conversation = conversations.find((c) => c.conversationId === conversationId);
    if (conversation) {
      conversation.hasNewMessages = false;
      this.conversationsData$.next(conversations);
    }
  }

  scrollToIndex(index: number): void {
    if (this.viewport) {
      this.viewport.scrollToIndex(index, 'smooth');
    }
  }
}
