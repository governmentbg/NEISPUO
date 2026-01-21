import { Component } from '@angular/core';
import { ActivatedRoute, NavigationEnd, Router } from '@angular/router';
import { filter, map, mergeMap } from 'rxjs/operators';

@Component({
    selector: 'app-heading',
    templateUrl: './heading.component.html',
    styleUrls: ['./heading.component.scss'],
})
export class HeadingComponent {
    currentRoute!: string;

    constructor(private route: ActivatedRoute, private router: Router) {
        try {
            this.router.events
                .pipe(
                    filter((event) => event instanceof NavigationEnd),
                    map(() => {
                        let actRoute = route.firstChild;
                        let child = actRoute;
                        while (child) {
                            if (child.firstChild) {
                                child = child.firstChild;
                                actRoute = child;
                            } else {
                                child = null;
                            }
                        }
                        return actRoute;
                    }),
                    mergeMap((actRoute) => actRoute!.data),
                )
                .subscribe((data: any) => {
                    this.currentRoute = data.title;
                });
        } catch (e) {
            console.log(e);
        }
    }
}
