import { Injectable } from "@angular/core";
import { FieldConfig } from "../models/field.interface";
import { Table } from "../models/table.interface";

@Injectable()
export class KeepFiltersService {
  filters: FieldConfig[] = [];
  table: Table = null;
  keyWord: string = "";
  tableFilter = {};
  sortParams = [];
  sortDirs = [];

  constructor() {}
}
