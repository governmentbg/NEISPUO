export interface MenuItem {
  path: string;
  label: string;
  children: MenuItem[];
  icon?: string;
  history?: boolean; // whether or not to be shown in history mode
  extData?: boolean; // if true lock tab for edit if extdata = 1
  showTab?: boolean; // show/hide tabs based on conditions
}
