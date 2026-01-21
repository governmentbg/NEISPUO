import {
  ActivatedRoute,
  ActivatedRouteSnapshot,
  DetachedRouteHandle,
  Params,
  Router,
  RouteReuseStrategy
} from '@angular/router';
import { deepEqual } from './various';

export const reloadRoute = (router: Router, route: ActivatedRoute, targetRouteCommands: any[] = ['./']) => {
  const matrixParams = getMatrixParams(route.snapshot);

  // not passing the matrix params for the current route will break
  // the save/back button functionality in routes that use them
  // (e.g. /exams/100000;type=ControlExam)
  router.navigate([...targetRouteCommands, { ...matrixParams, r: Date.now() }], { relativeTo: route });
};

export class ParamsRouteReuseStrategy implements RouteReuseStrategy {
  /**
   * Whether the given route should detach for later reuse.
   * Always returns false.
   * */
  shouldDetach(route: ActivatedRouteSnapshot): boolean {
    return false;
  }

  /**
   * A no-op; the route is never stored since this strategy never detaches routes for later re-use.
   */
  store(route: ActivatedRouteSnapshot, detachedTree: DetachedRouteHandle): void {
    // do nothing
  }

  /** Returns `false`, meaning the route (and its subtree) is never reattached */
  shouldAttach(route: ActivatedRouteSnapshot): boolean {
    return false;
  }

  /** Returns `null` because this strategy does not store routes for later re-use. */
  retrieve(route: ActivatedRouteSnapshot): DetachedRouteHandle | null {
    return null;
  }

  /**
   * Determines if a route should be reused.
   * Always returns false.
   */
  shouldReuseRoute(future: ActivatedRouteSnapshot, curr: ActivatedRouteSnapshot): boolean {
    return (
      future.routeConfig === curr.routeConfig &&
      deepEqual(future.params, curr.params) &&
      deepEqual(future.queryParams, curr.queryParams)
    );
  }
}

function getMatrixParams(routeSnapshot: ActivatedRouteSnapshot): Params {
  const routeParams = routeSnapshot.params;
  const parentRouteParams = routeSnapshot.parent?.params || {};
  const pathParamKeys =
    routeSnapshot.routeConfig?.path
      ?.split('/')
      ?.map((segment) => segment && /:([\w-]+)/.exec(segment)?.[1])
      ?.filter((pathParam) => pathParam) || [];

  return Object.fromEntries(
    Object.entries(routeParams).filter(
      ([key]) =>
        // leave only current route params
        !Object.hasOwnProperty.call(parentRouteParams, key) &&
        // skip path params
        !pathParamKeys.includes(key)
    )
  );
}
