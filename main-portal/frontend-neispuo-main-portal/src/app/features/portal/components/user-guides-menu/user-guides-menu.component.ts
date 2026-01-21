import { Component, OnInit } from '@angular/core';
import { NeispuoModuleQuery } from '@portal/neispuo-modules/neispuo-module.query';
import { NeispuoUserGuide } from '@portal/neispuo-modules/neispuo-user-guide.interface';
import { Observable } from 'rxjs';
import { IsUserGuideClickedQuery } from '@portal/neispuo-modules/is-user-guide-clicked.query';
import { Tooltip } from 'bootstrap';
import { UserGuideManagementService } from '@portal/services/user-guide-management.service';

@Component({
  selector: 'app-user-guides-menu',
  templateUrl: './user-guides-menu.component.html',
  styleUrls: ['./user-guides-menu.component.scss']
})
export class UserGuidesMenuComponent implements OnInit {
  categories$ = this.neispuoModuleQuery.categories$;
  userGuides$: Observable<NeispuoUserGuide[]>;
  isUserGuideClickedOnce$ = this.isUserGuideClickedQuery.isUserGuideClicked$;
  copyLinkMessage: string = 'Копирай връзка';
  constructor(
    public userGuideManagementService: UserGuideManagementService,
    public neispuoModuleQuery: NeispuoModuleQuery,
    public isUserGuideClickedQuery: IsUserGuideClickedQuery
  ) {}

  ngOnInit(): void {}

  onOpenUserGuideMenu() {
    this.isUserGuideClickedQuery.updateValue();

    let tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    tooltipTriggerList.map(function (tooltipTriggerEl) {
      return new Tooltip(tooltipTriggerEl);
    });
  }

  shouldShowCopyLink(userGuide: NeispuoUserGuide): boolean {
    return userGuide && userGuide.URLOverride && userGuide.URLOverride.trim() !== '';
  }
}
