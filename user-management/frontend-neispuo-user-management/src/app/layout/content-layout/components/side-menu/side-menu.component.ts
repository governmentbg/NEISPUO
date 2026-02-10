import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { RoleEnum } from '@shared/enums/roles.enum';
import { SideMenuService } from '@shared/services/side-menu.service';
import { Observable } from 'rxjs';
import { filter, map, take } from 'rxjs/operators';

import { ISideMenuConfig, ISideMenuConfigItem, SideMenuConfig } from 'src/app/configs/side-menu.config';
import { AuthQuery } from 'src/app/core/authentication/auth.query';

@Component({
    selector: 'app-side-menu',
    templateUrl: './side-menu.component.html',
    styleUrls: ['./side-menu.component.scss'],
})
export class SideMenuComponent implements OnInit {
    selectedRole$ = this.authQuery.selectedRole$;

    sideMenuList!: ISideMenuConfig[];

    currentRoute: any;

    displaySidemenu!: boolean;

    constructor(
        private authQuery: AuthQuery,
        private router: Router,
        public activatedRoute: ActivatedRoute,
        public sideMenuData: SideMenuService,
    ) {}

    ngOnInit() {
        this.sideMenuList = SideMenuConfig;
        this.sideMenuData.displaySidemenu.subscribe((displaySidemenu) => (this.displaySidemenu = displaySidemenu));

        this.currentRoute = this.activatedRoute.snapshot.firstChild?.routeConfig?.path;
        this.router.events.pipe(filter((event) => event instanceof NavigationEnd)).subscribe((event) => {
            this.currentRoute = this.activatedRoute.snapshot.firstChild?.routeConfig?.path;
        });
    }

    hasActiveRoute(menuItems: ISideMenuConfigItem[]) {
        return menuItems.some((item) => item.routerLink[0].split('/').pop() === this.currentRoute);
    }

    hasRights(roles: RoleEnum[], isLeadTeacher?: boolean): Observable<boolean> {
        return this.selectedRole$.pipe(
            filter((resp) => !!resp),
            take(1),
            map(
                (role) =>
                    roles?.includes(role?.SysRoleID as RoleEnum) &&
                    // lead teacher is not needed or lead teacher has to be true?
                    (!isLeadTeacher || role?.IsLeadTeacher === true),
            ),
        );
    }
}
