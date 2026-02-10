export interface CubeJSMetaResponseModel {
  cubes: CubeModel[];
}

export interface CubeModel {
  name: string;
  title: string;
  description?: string;
  measures: any[];
  dimensions: any[];
  segments: any[];
  connectedComponent: any;
}
