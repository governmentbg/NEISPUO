export enum InfluenceType {
  Options = "options",
  Disable = "disable",
  Hide = "hide",
  SetValue = "setValue",
  DefaultValue = "defaultValue", // should be reset on influencer reset if no db value provided
  Render = "render" //opposite of hide
}
