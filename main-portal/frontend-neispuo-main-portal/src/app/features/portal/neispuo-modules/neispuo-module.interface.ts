import { NeispuoCategory } from "./neispuo-category.interface";
import { SysRole } from "./sys-role.inteface";

export interface NeispuoModule {
  id: number;
  name: string;
  category: NeispuoCategory;
  description?: string;
  link: string;
  roles: SysRole[];
}
