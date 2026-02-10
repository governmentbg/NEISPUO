import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { Event as RouteEvent, NavigationEnd, Router } from '@angular/router';
import { faMinus as fasMinus } from '@fortawesome/pro-solid-svg-icons/faMinus';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { Subscription } from 'rxjs';
import { VerticalTabItem } from './vertical-tab-item';

@Component({
  selector: 'sb-vertical-tabs',
  templateUrl: './vertical-tabs.component.html'
})
export class VerticalTabsComponent implements OnInit, OnDestroy {
  @Input() tabs: VerticalTabItem[] = [];
  @Input() ariaLabel: string | null = null;

  selectedTab = '';
  flattenedTabs: VerticalTabItem[] = [];

  readonly fasMinus = fasMinus;
  readonly fasPlus = fasPlus;
  private routerEventsSubscription: Subscription;

  constructor(private router: Router) {
    this.routerEventsSubscription = router.events.subscribe((s: RouteEvent) => {
      if (s instanceof NavigationEnd) {
        this.updateSelectedTab();
      }
    });
  }

  ngOnInit() {
    this.flattenedTabs = [];
    for (const tab of this.tabs) {
      this.flattenedTabs.push(tab);
      for (const childTab of tab.tabItems ?? []) {
        childTab.isChild = true;
        this.flattenedTabs.push(childTab);
      }
    }

    this.updateSelectedTab();
  }

  ngOnDestroy() {
    this.routerEventsSubscription.unsubscribe();
  }

  toggleTab(event: Event, tab: VerticalTabItem) {
    event.preventDefault();
    event.stopPropagation();

    tab.isClosed = !tab.isClosed;

    for (const childTab of tab.tabItems ?? []) {
      childTab.isHidden = tab.isClosed;
    }
  }

  onChange(event: Event) {
    this.selectedTab = (event.target as HTMLSelectElement)?.value;

    const selectedTabIndex = parseInt(this.selectedTab);

    if (!isNaN(selectedTabIndex) && this.flattenedTabs[selectedTabIndex]) {
      Promise.resolve().then(() => {
        this.router.navigate(
          this.flattenedTabs[selectedTabIndex].routeCommands,
          this.flattenedTabs[selectedTabIndex].routeExtras
        );
      });
    }
  }

  private updateSelectedTab() {
    if (!this.flattenedTabs || !this.router.navigated) return;

    Promise.resolve().then(() => {
      const selectedTabIndex = this.flattenedTabs.findIndex(
        (t) => t.routeCommands && this.router.isActive(this.router.createUrlTree(t.routeCommands, t.routeExtras), false)
      );

      this.selectedTab = selectedTabIndex !== -1 ? selectedTabIndex.toString() : '';
    });
  }
}
