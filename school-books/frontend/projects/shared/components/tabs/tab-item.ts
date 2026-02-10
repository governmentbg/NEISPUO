import { NavigationExtras } from '@angular/router';
import { IconDefinition } from '@fortawesome/fontawesome-svg-core';

export interface TabItem {
  text: string;
  badge?: string | null;
  icon?: IconDefinition;
  routeCommands: any[];
  routeExtras?: NavigationExtras;
}
