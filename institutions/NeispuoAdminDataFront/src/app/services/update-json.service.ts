import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Subject } from "rxjs";
import { tap } from "rxjs/operators";
import { FocusedElementType } from "../enums/focusedElementType";
import { FocusedElement } from "../models/focusedElement";

@Injectable({ providedIn: "root" })
export class UpdateJsonService {
  counter: number = 1;
  focusedElement: FocusedElement;
  focusedElements: FocusedElement[] = [];

  typeChanged: Subject<FocusedElementType> = new Subject<FocusedElementType>();

  constructor(private http: HttpClient) {}

  updateJson(jsonName: string, json: Object) {
    return this.http.post(`/sys/savemetafile/${jsonName}`, json);
  }

  getAllForms() {
    return this.http.get("/uimeta/get/__formNames");
  }

  getJson(jsonName: string) {
    return this.http.get(`/uimeta/get/${jsonName}`);
  }
}
