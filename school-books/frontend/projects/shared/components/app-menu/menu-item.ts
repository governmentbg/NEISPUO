import { NavigationExtras } from '@angular/router';
import { IconDefinition } from '@fortawesome/fontawesome-svg-core';

export interface MenuItem {
  id?: string;
  text: string;
  badge?: string | null;
  icon?: IconDefinition;
  routeCommands?: any[];
  routeExtras?: NavigationExtras;
  isActiveRouteCommands?: any[];
  showChildrenInline?: boolean;
  menuItems?: MenuItem[];
  isOpen?: boolean;
  isSelected?: boolean;
  isSkeleton?: boolean;
}
