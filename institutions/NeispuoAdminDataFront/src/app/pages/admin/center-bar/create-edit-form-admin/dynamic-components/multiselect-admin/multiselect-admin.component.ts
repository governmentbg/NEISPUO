import { Component, OnInit } from "@angular/core";
import { UpdateJsonService } from "src/app/services/update-json.service";
import { DynamicComponentAdmin } from "../../dynamic-field/dynamic-component";

@Component({
  selector: "app-multiselect-admin",
  templateUrl: "./multiselect-admin.component.html",
  styleUrls: ["./multiselect-admin.component.scss"]
})
export class MultiselectAdminComponent extends DynamicComponentAdmin implements OnInit {
  appendClass: string;

  constructor(public jsonService: UpdateJsonService) {
    super(jsonService);
  }

  ngOnInit() {
    this.appendClass = ".l-container-card";
  }
}
