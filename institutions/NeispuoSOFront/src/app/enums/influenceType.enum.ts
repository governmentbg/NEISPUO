export enum InfluenceType {
  Options = "options",
  Disable = "disable",
  Hide = "hide",
  HideClear = "hideAndClear",
  Render = "render", //opposite of hide
  Require = "require",
  Fill = "fill",
  DefaultValue = "defaultValue", // should be reset on influencer reset if no db value provided
  SetValue = "setValue",
  SetValueOppositeCondition = "setValueOppositeCondition"
}

export enum InfluenceElement {
  Field = "fieldName",
  Fields = "fields",
  Subsection = "subsectionName",
  Section = "sectionName",
  Table = "createNewTableName"
}
