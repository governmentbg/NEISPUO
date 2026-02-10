import { Component, Input } from '@angular/core';
import { NavigationExtras } from '@angular/router';
import { IconDefinition } from '@fortawesome/fontawesome-svg-core';

export interface BreadcrumbItem {
  text: string;
  routeCommands: any[];
  routeExtras?: NavigationExtras;
}

@Component({
  selector: 'sb-breadcrumb',
  templateUrl: './breadcrumb.component.html',
  styleUrls: ['./breadcrumb.component.scss']
})
export class BreadcrumbComponent {
  @Input() icon?: IconDefinition;
  @Input() text?: string;
  @Input() parentItems = [] as BreadcrumbItem[];
  @Input() isSkeleton = false;
}
