import { Component, OnDestroy, OnInit } from "@angular/core";
import { NestedTreeControl } from "@angular/cdk/tree";
import { MatTreeNestedDataSource } from "@angular/material/tree";
import { ActivatedRoute, Router } from "@angular/router";
import { TreeNode } from "../../../models/tree-node.interface";
import { FormType } from "../../../enums/formType.enum";
import { Subject, Subscription } from "rxjs";
import { TreeDataService } from "../../../services/tree-data.service";
import { debounceTime } from "rxjs/operators";
import { environment } from "../../../../environments/environment";
import { HelperService } from "src/app/services/helpers.service";
import { Tabs } from "src/app/enums/tabs.enum";
import { FormDataService } from "src/app/services/form-data.service";

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
    private helperSerice: HelperService,
    private formDataService: FormDataService
  ) {}

  get formTypes() {
    return FormType;
  }

  hasChild = (_: number, node: TreeNode) => !!node.children && node.children.length > 0;

  ngOnInit() {
    this.routeSubscription = this.route.params.subscribe(params => {
      if (this.formType !== params["type"]) {
        this.formType = params["type"];
        this.activeNodeId = null;
        this.isLoading = true;

        this.treeDataService.getTreeData(this.formType).subscribe(
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
    this.routeParamChangedSubscription = this.helperSerice.routeParamChanged.subscribe(
      (paramChange: { paramName: string; paramValue: string }) => {
        if (paramChange && paramChange.paramName === "id" && paramChange.paramValue != this.activeNodeId) {
          this.activeNodeId = paramChange.paramValue;

          // for some reason without set timeout error occurs - expression changed, when go back from edit
          setTimeout(() => {
            this.activeNodeId && this.treeData && this.expand(this.treeData);
            !this.activeNodeId && this.treeControl && this.treeControl.collapseAll();
          });
        }
      }
    );

    this.searchSubscription = this.searchSub$.pipe(debounceTime(200)).subscribe((filterValue: string) => this.filterData(filterValue));
  }

  ngOnDestroy() {
    this.routeSubscription && this.routeSubscription.unsubscribe();
    this.routeParamChangedSubscription && this.routeParamChangedSubscription.unsubscribe();
    this.searchSubscription && this.searchSubscription.unsubscribe();
  }

  treeChanged() {
    this.dataSource.data = this.treeData;

    this.treeControl.dataNodes = this.treeData;
    this.dataSource.data &&
      Object.keys(this.dataSource.data).forEach(region => {
        this.addParent(this.dataSource.data[region], null);
      });

    this.filterValue = null;
    this.activeNodeId && this.expand(this.treeData);
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

  async reroute(node: TreeNode) {
    if (node.instid && node.formName && this.activeNodeId != node.instid) {
      const routeChildren = this.route.snapshot.children;
      const menuItem = routeChildren && routeChildren.length ? routeChildren[0].params["menuItem"] : node.menuPath;
      const formName =
        routeChildren && routeChildren.length
          ? routeChildren[0].params["grandParentForm"] || routeChildren[0].params["parentForm"] || routeChildren[0].params["formName"]
          : node.formName;
      const tab = routeChildren && routeChildren.length ? routeChildren[0].params["tab"] : Tabs.main;

      let queryParamsTmp = environment.production
        ? this.helperSerice.decodeParams(this.route.snapshot.queryParams["q"])
        : this.route.snapshot.queryParams;

      let queryParams: any = {
        instid: node.instid,
        sysuserid: queryParamsTmp.sysuserid,
        sysroleid: queryParamsTmp.sysroleid,
        year: queryParamsTmp.year,
        period: queryParamsTmp.period,
        address: queryParamsTmp.address
      };

      if (this.router.url.split("/").length > 3 && this.router.url.split("/")[3] === Tabs.list) {
        const campaignRes: any = await this.formDataService.isOpenCampaign(queryParams.instid).toPromise();
        queryParams.isOpenCampaign = campaignRes && campaignRes.length ? campaignRes[0].isOpen : campaignRes.isOpen;
      }

      let result: any = await this.formDataService.getIsLocked(node.instid).toPromise();
      result && result.length && (result = result[0]);
      this.formDataService.instIsLocked = !!result.isLocked;
      queryParams.isLocked = this.formDataService.instIsLocked;

      let detailedSchoolTypeRes: any = await this.formDataService.getDetailedSchoolType(queryParams.instid).toPromise();
      detailedSchoolTypeRes && detailedSchoolTypeRes.length && (detailedSchoolTypeRes = detailedSchoolTypeRes[0]);
      this.formDataService.detailedSchoolType = detailedSchoolTypeRes.detailedSchoolType;
      queryParams.detailedSchoolType = this.formDataService.detailedSchoolType;

      let schoolYearRes: any = await this.formDataService.getSchoolYear(queryParams.instid).toPromise();
      schoolYearRes && schoolYearRes.length && (schoolYearRes = schoolYearRes[0]);
      this.formDataService.schoolYear = schoolYearRes.schoolYear;
      queryParams.schoolYear = this.formDataService.schoolYear;

      environment.production && (queryParams = this.helperSerice.encodeParams(queryParams));

      if (tab && formName && menuItem) {
        this.router
          .navigate([tab, menuItem, formName], {
            relativeTo: this.route,
            queryParams
          })
          .then(res => res !== false && (this.activeNodeId = node.instid));
      } else {
        this.router
          .navigate([this.router.url.split("?")[0]], { queryParams })
          .then(res => res !== false && (this.activeNodeId = node.instid));
      }
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
