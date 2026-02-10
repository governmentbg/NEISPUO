export interface TreeNode {
  name: string;
  children?: TreeNode[];
  instid: string;
  menuPath: string;
  color: string;
  status?: string;
  formName: string;

  ok?: boolean;
  childOk?: boolean;
  parent?: TreeNode;
  active?: boolean;
  position?: number;
}
