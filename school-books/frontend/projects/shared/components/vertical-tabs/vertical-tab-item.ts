import { NavigationExtras } from '@angular/router';
import { IconDefinition } from '@fortawesome/fontawesome-svg-core';

export interface VerticalTabItem {
  text: string;
  badge?: string | null;
  icon?: IconDefinition;
  routeCommands: any[];
  routeExtras?: NavigationExtras;
  tabItems?: VerticalTabItem[];
  isClosed?: boolean;
  isHidden?: boolean;
  isChild?: boolean;
}
