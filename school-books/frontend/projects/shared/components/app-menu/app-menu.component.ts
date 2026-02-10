import { Component, Input, OnChanges, OnDestroy, OnInit, SimpleChanges } from '@angular/core';
import { ActivatedRoute, Event, NavigationEnd, Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { MenuItem } from './menu-item';

@Component({
  selector: 'sb-app-menu',
  templateUrl: './app-menu.component.html'
})
export class AppMenuComponent implements OnInit, OnChanges, OnDestroy {
  //eslint-disable-next-line @angular-eslint/no-input-rename
  @Input('menuItems')
  menuItemsInput: MenuItem[] = [] as MenuItem[];

  menuItems: MenuItem[] = [] as MenuItem[];

  private routerEventsSubscription!: Subscription;

  constructor(private router: Router, private route: ActivatedRoute) {}

  ngOnInit(): void {
    this.selectActiveState();
    this.routerEventsSubscription = this.router.events.subscribe((s: Event) => {
      if (s instanceof NavigationEnd) {
        this.selectActiveState();
      }
    });
  }

  ngOnChanges(changes: SimpleChanges) {
    // perserve open/selected items if the menu items change
    let openItems = null;
    let selectedItems = null;
    if (this.menuItems) {
      openItems = new Set([...this.iterateItems(this.menuItems)].filter((mi) => mi.isOpen).map((mi) => mi.id));
      selectedItems = new Set([...this.iterateItems(this.menuItems)].filter((mi) => mi.isSelected).map((mi) => mi.id));
    }

    this.menuItems = this.setMenuItemsIdentity(this.menuItemsInput);

    if (openItems && selectedItems) {
      for (const menuItem of this.iterateItems(this.menuItems)) {
        menuItem.isOpen = openItems.has(menuItem.id);
        menuItem.isSelected = selectedItems.has(menuItem.id);
      }
    } else {
      this.selectActiveState();
    }
  }

  ngOnDestroy() {
    this.routerEventsSubscription.unsubscribe();
  }

  private setMenuItemsIdentity(menuItems: MenuItem[], parent: MenuItem | null = null) {
    return menuItems.map((mi) => {
      const item = {
        id: (parent ? parent.id : '') + '$' + (mi.routeCommands?.join() || '') + '@' + (mi.text || ''),
        ...mi
      };

      item.menuItems = mi.menuItems && this.setMenuItemsIdentity(mi.menuItems, item);

      return item;
    });
  }

  private selectActiveState() {
    Promise.resolve().then(() => {
      if (!this.menuItems || !this.router.navigated) return;

      for (const menuItem of this.iterateItems(this.menuItems)) {
        menuItem.isSelected = false;
      }

      for (const menuItem of this.findActivePath(this.menuItems)) {
        menuItem.isSelected = true;
        if (menuItem?.menuItems?.length) {
          menuItem.isOpen = true;
        }
      }
    });
  }

  private *iterateItems(menuItems: MenuItem[]): Generator<MenuItem> {
    for (const menuItem of menuItems) {
      yield* this.iterateItems(menuItem.menuItems || []);

      yield menuItem;
    }
  }

  private *findActivePath(menuItems: MenuItem[]): Generator<MenuItem, void, undefined> {
    for (const menuItem of menuItems) {
      const foundChildren = [...this.findActivePath(menuItem.menuItems || [])];

      if (
        foundChildren.length ||
        (menuItem.isActiveRouteCommands &&
          this.router.isActive(
            this.router.createUrlTree(menuItem.isActiveRouteCommands, {
              relativeTo: this.route,
              ...menuItem.routeExtras
            }),
            { paths: 'subset', queryParams: 'subset', fragment: 'ignored', matrixParams: 'subset' }
          )) ||
        (menuItem.routeCommands &&
          this.router.isActive(
            this.router.createUrlTree(menuItem.routeCommands, { relativeTo: this.route, ...menuItem.routeExtras }),
            { paths: 'subset', queryParams: 'subset', fragment: 'ignored', matrixParams: 'subset' }
          ))
      ) {
        yield* foundChildren;
        yield menuItem;
      }
    }
  }
}
