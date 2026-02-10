import { NavigationExtras } from '@angular/router';
import { IconDefinition } from '@fortawesome/fontawesome-svg-core';

export interface PaginatorItem {
  readonly text?: string;
  readonly icon?: IconDefinition;
  readonly routeCommands?: any[];
  readonly routeExtras?: NavigationExtras;
  readonly clickable?: boolean;
  readonly state?: any;
}
