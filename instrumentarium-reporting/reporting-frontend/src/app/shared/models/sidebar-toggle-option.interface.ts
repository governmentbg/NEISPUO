import { SidebarToggleOptionValueEnum } from '@shared/enums/sidebar-toggle-option-value.enum';

export interface SidebarToggleOption {
  label: string;
  value: SidebarToggleOptionValueEnum;
  iconClass: string;
  buttonClass: string;
}
