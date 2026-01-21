import { Component, OnInit } from "@angular/core";
import { UpdateJsonService } from "src/app/services/update-json.service";
import { DynamicComponentAdmin } from "../../dynamic-field/dynamic-component";

@Component({
  selector: "app-checkbox-admin",
  templateUrl: "./checkbox-admin.component.html",
  styleUrls: ["./checkbox-admin.component.scss"]
})
export class CheckboxAdminComponent extends DynamicComponentAdmin implements OnInit {
  constructor(public jsonService: UpdateJsonService) {
    super(jsonService);
  }

  ngOnInit() {}
}
