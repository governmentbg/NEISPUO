import { Observable } from "rxjs";
import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, RouterStateSnapshot, CanDeactivate } from "@angular/router";
import { BodyComponent } from "../pages/body/body.component";
import { KeepFiltersService } from "../services/keep-filters.service";

@Injectable({
  providedIn: "root"
})
export class SaveToService implements CanDeactivate<any> {
  constructor(private keepFiltersService: KeepFiltersService) {}

  canDeactivate(
    component: BodyComponent,
    route: ActivatedRouteSnapshot,
    state: RouterStateSnapshot
  ): Observable<boolean> | boolean {
    if (
      component &&
      ((component.filters && !component.atLeastOneChosen(component.filtersFormGroup)) ||
        component.table ||
        component.keyWord)
    ) {
      this.keepFiltersService.table = component.table || null;
      // this.keepFiltersService.filters = component.filters || null;
      // this.keepFiltersService.keyWord = component.keyWord || "";

      // this.keepFiltersService.filters.forEach(filter => {
      //   const control = component.filtersFormGroup.get(filter.name);
      //   filter.value = control.value;
      // });
    }
    return true;
  }
}
