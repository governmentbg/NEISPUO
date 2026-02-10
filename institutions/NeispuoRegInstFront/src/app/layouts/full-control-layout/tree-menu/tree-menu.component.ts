import { Component, OnDestroy, OnInit } from "@angular/core";
import { NestedTreeControl } from "@angular/cdk/tree";
import { MatTreeNestedDataSource } from "@angular/material/tree";
import { ActivatedRoute, Router } from "@angular/router";
import { TreeNode } from "../../../models/tree-node.interface";
import { FormType } from "../../../enums/formType.enum";
import { Subject, Subscription } from "rxjs";
import { TreeDataService } from "../../../services/tree-data.service";
import { FormDataService } from "../../../services/form-data.service";
import { debounceTime, distinctUntilChanged } from "rxjs/operators";
import { Menu } from "../../../enums/menu.enum";
import { environment } from "../../../../environments/environment";
import { AuthService } from "src/app/auth/auth.service";

@Component({
  selector: "app-tree-menu",
  templateUrl: "./tree-menu.component.html",
  styleUrls: ["./tree-menu.component.scss"]
})
export class TreeMenuComponent implements OnInit, OnDestroy {
  treeData: TreeNode[];
  formType: FormType;
  isLoading: boolean = false;

  activeNodeId: string;
  filterValue: string;
  treeControl = new NestedTreeControl<TreeNode>(node => node.children);
  dataSource = new MatTreeNestedDataSource<TreeNode>();

  private allFilteredElementsCount = 0;
  private shownFilteredElementsCount = 0;

  private searchSub$ = new Subject<string>();
  private routeSubscription: Subscription;
  private routeParamChangedSubscription: Subscription;
  private searchSubscription: Subscription;

  constructor(
    private router: Router,
    private treeDataService: TreeDataService,
    private route: ActivatedRoute,
    private formDataService: FormDataService,
    private authService: AuthService
  ) {}

  get formTypes() {
    return FormType;
  }

  hasChild = (_: number, node: TreeNode) => !!node.children && node.children.length > 0;

  ngOnInit() {
    const path = this.route.parent.snapshot.url[0].path;

    this.routeSubscription = this.route.params.subscribe(params => {
      if (this.formType !== params["type"]) {
        this.formType = params["type"];
        this.activeNodeId = null;
        this.isLoading = true;

        this.treeDataService.getTreeData(path === Menu.Active, this.formType).subscribe(
          res => {
            this.treeData = res;
            this.treeChanged();
            this.isLoading = false;
          },
          err => (this.isLoading = false)
        );
      }
    });

    //for refresh
    this.routeParamChangedSubscription = this.formDataService.routeParamChanged.subscribe(
      (paramChange: { paramName: string; paramValue: string }) => {
        if (paramChange && paramChange.paramName === "id" && paramChange.paramValue != this.activeNodeId) {
          this.activeNodeId = paramChange.paramValue;
          this.activeNodeId && this.treeData && this.expand(this.treeData);
          !this.activeNodeId && this.treeControl && this.treeControl.collapseAll();
        }
      }
    );

    this.searchSubscription = this.searchSub$
      .pipe(debounceTime(200), distinctUntilChanged())
      .subscribe((filterValue: string) => this.filterData(filterValue));
  }

  ngOnDestroy() {
    this.routeSubscription && this.routeSubscription.unsubscribe();
    this.routeParamChangedSubscription && this.routeParamChangedSubscription.unsubscribe();
    this.searchSubscription && this.searchSubscription.unsubscribe();
  }

  treeChanged() {
    this.dataSource.data = this.treeData;

    this.treeControl.dataNodes = this.dataSource.data;
    this.dataSource.data &&
      Object.keys(this.dataSource.data).forEach(region => {
        this.addParent(this.dataSource.data[region], null);
      });

    this.filterValue = null;
    this.activeNodeId && this.expand(this.treeControl.dataNodes);
  }

  addParent(data: TreeNode, parent: TreeNode | null) {
    data.parent = parent;
    if (data.children) {
      data.children.forEach(x => {
        this.addParent(x, data);
      });
    }
  }

  filterChanged(value: string) {
    this.searchSub$.next(value);
  }

  private filterData(value: string) {
    if (this.treeData) {
      this.setChildOk(value, this.treeData);
      this.allFilteredElementsCount = 0;

      if (value) {
        this.shownFilteredElementsCount = 40;
        this.initPositions(this.treeData);
        this.dataSource.data = this.copyTree(this.treeData);
        this.treeControl.dataNodes = this.dataSource.data;
        this.treeControl.expandAll();
      } else {
        this.shownFilteredElementsCount = 0;
        this.treeControl.collapseAll();
        this.dataSource.data = this.treeData;
        this.treeControl.dataNodes = this.dataSource.data;
        this.activeNodeId && this.expand(this.treeControl.dataNodes);
      }
    }
  }

  private setChildOk(text: string, children: TreeNode[]) {
    children.forEach(node => {
      node.ok = node.parent?.ok || this.nameIncludesOne(node.name, text);
      if (node.children && node.children.length > 0) node.childOk = this.setChildOk(text, node.children);
    });
    return children.filter(node => node.ok || node.childOk).length > 0;
  }

  reroute(node: TreeNode) {
    if (node.instid && node.formName && this.activeNodeId != node.instid) {
      const id = `${node.instid}`;
      let queryParams: any = { instid: id, instKind: node.instKind, procID: node.procID, sysuserid: this.authService.getSysUserId(), region: this.authService.getRegion() };
      environment.production && (queryParams = this.formDataService.encodeParams(queryParams));

      this.router
        .navigate([node.formName], { relativeTo: this.route, queryParams })
        .then(res => res !== false && (this.activeNodeId = node.instid));
    }
  }

  private nameIncludesOne(name: string, text: string) {
    const textArr = text ? text.split(" ") : [];
    let includes = false;
    textArr.forEach(text => {
      text && (includes = includes || (name && name.toLowerCase().indexOf(text.toLowerCase()) >= 0));
    });

    return includes;
  }

  private expand(nodes: TreeNode[]) {
    let expand = false;
    nodes.forEach(node => {
      if (node.children && node.children.length > 0) {
        const expandCurrent = this.expand(node.children);
        expandCurrent && this.treeControl.expand(node);
        expand = expand || expandCurrent;
      } else if (node.instid == this.activeNodeId || node.ok) {
        this.treeControl.expand(node);
        expand = true;
      }
    });
    return expand;
  }

  private initPositions(nodes: TreeNode[]) {
    nodes.forEach(node => {
      if (node.ok || node.childOk) {
        node.position = this.allFilteredElementsCount;
        this.allFilteredElementsCount++;
        node.children && node.children.length > 0 && this.initPositions(node.children);
      }
    });
  }

  private copyTree(nodes: TreeNode[]): TreeNode[] {
    const copy = [];
    nodes.forEach(node => {
      if ((node.ok || node.childOk) && node.position < this.shownFilteredElementsCount) {
        copy.push({ ...node, children: [] });
        node.children && node.children.length > 0 && (copy[copy.length - 1].children = this.copyTree(node.children));
      }
    });
    return copy;
  }

  onScroll(event) {
    const currentScroll = event.target.scrollTop;
    const scrollTopMax = event.target.scrollHeight - event.target.clientHeight;
    const loadMore = currentScroll / scrollTopMax >= 0.85;

    if (loadMore && this.shownFilteredElementsCount < this.allFilteredElementsCount) {
      this.shownFilteredElementsCount = Math.min(this.shownFilteredElementsCount + 40, this.allFilteredElementsCount);
      this.dataSource.data = this.copyTree(this.treeData);
      this.treeControl.dataNodes = this.dataSource.data;
      this.treeControl.expandAll();
    }
  }
}
