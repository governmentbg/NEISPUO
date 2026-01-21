export interface TreeNode {
  name: string;
  children?: TreeNode[];
  instid: string;
  color: string;
  status?: string;
  formName: string;
  procID: number;
  instKind: string;

  ok?: boolean;
  childOk?: boolean;
  parent?: TreeNode;
  active?: boolean;
  position?: number;
}
