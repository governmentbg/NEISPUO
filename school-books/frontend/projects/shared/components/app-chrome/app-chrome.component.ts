import { Component, Input, OnDestroy } from '@angular/core';
import { Event, NavigationEnd, Router } from '@angular/router';
import { faEnvelope as fadEnvelop } from '@fortawesome/pro-duotone-svg-icons/faEnvelope';
import { faQuestion as fadQuestion } from '@fortawesome/pro-duotone-svg-icons/faQuestion';
import { faSparkles as fadSparkles } from '@fortawesome/pro-duotone-svg-icons/faSparkles';
import { faUniversity as fasUniversity } from '@fortawesome/pro-solid-svg-icons/faUniversity';
import { AuthService, SysRole } from 'projects/shared/services/auth.service';
import { ConfigService, Project } from 'projects/shared/services/config.service';
import { Subscription } from 'rxjs';
import { environment } from 'src/environments/environment';

export interface AppChromeSchoolYear {
  schoolYear: number;
  schoolYearRouteCommands: any[];
}

export interface UnreadConversations {
  conversationId: number;
  title: string;
  messageDate: Date;
  conversationRouteCommands: any[];
}

@Component({
  selector: 'sb-app-chrome',
  templateUrl: './app-chrome.component.html',
  styleUrls: ['./app-chrome.component.scss']
})
export class AppChromeComponent implements OnDestroy {
  @Input() institutionName?: string;
  @Input() institutionRouteCommands?: any[] | null;
  @Input() currentSchoolYear?: number;
  @Input() schoolYears?: AppChromeSchoolYear[];
  @Input() unreadConversations?: UnreadConversations[];

  leftSideIsOpen = false;
  baseUri: string;
  userEmail!: string;
  userInitials!: string;
  showHelpdeskLink: boolean;
  showConversationsIcon: boolean;

  readonly fadQuestion = fadQuestion;
  readonly fadSparkles = fadSparkles;
  readonly fadEnvelop = fadEnvelop;
  readonly fasUniversity = fasUniversity;
  readonly docsUrl = environment.docsUrl;
  readonly releaseNotesUrl = environment.releaseNotesUrl;

  private routerEventsSubscription: Subscription;

  constructor(private authService: AuthService, private configService: ConfigService, router: Router) {
    this.baseUri = document.baseURI;
    this.showHelpdeskLink = configService.launchConfiguration.project === Project.TeachersApp;
    this.userEmail = authService.currentUserProfile.sub;
    this.userInitials = (
      authService.currentUserProfile.FirstName[0] + authService.currentUserProfile.LastName[0]
    ).toUpperCase();

    this.routerEventsSubscription = router.events.subscribe((s: Event) => {
      if (s instanceof NavigationEnd) {
        this.hideLeftSide();
      }
    });

    this.showConversationsIcon = this.shouldShowConversationsIcon();
  }

  ngOnDestroy(): void {
    this.routerEventsSubscription.unsubscribe();
  }

  toggleLeftIsSideOpen() {
    this.leftSideIsOpen = !this.leftSideIsOpen;
  }

  hideLeftSide() {
    this.leftSideIsOpen = false;
  }

  logout() {
    this.authService.logout();
  }

  markConversationAsRead(conversationId: number): void {
    if (!this.unreadConversations) return;
    this.unreadConversations = this.unreadConversations.filter((c) => c.conversationId !== conversationId);
  }

  shouldShowConversationsIcon(): boolean {
    const sysRoleId = this.authService.tokenPayload.selected_role.SysRoleID;

    if (
      sysRoleId === SysRole.Institution ||
      sysRoleId === SysRole.Teacher ||
      sysRoleId === SysRole.Parent ||
      sysRoleId === SysRole.InstitutionExpert
    ) {
      return true;
    }

    return false;
  }
}
