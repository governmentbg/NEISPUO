import { Component, OnInit } from '@angular/core';
import { SideMenuService } from '@shared/services/side-menu.service';

@Component({
    selector: 'app-header',
    templateUrl: './header.component.html',
    styleUrls: ['./header.component.scss'],
})
export class HeaderComponent implements OnInit {
    displaySidemenu!: boolean;

    constructor(public sideMenuData: SideMenuService) {}

    ngOnInit() {
        this.sideMenuData.displaySidemenu.subscribe((displaySidemenu) => (this.displaySidemenu = displaySidemenu));
    }
}
