export enum InfluenceType {
  Options = "options",
  Disable = "disable",
  Hide = "hide",
  Render = "render", //opposite of hide
  Require = "require",
  Fill = "fill",
  DefaultValue = "defaultValue", // should be reset on influencer reset if no db value provided
  SetValue = "setValue"
}

export enum InfluenceElement {
  Field = "fieldName",
  Fields = "fields",
  Subsection = "subsectionName",
  Section = "sectionName",
}
