import { Component, OnInit } from "@angular/core";
import { UpdateJsonService } from "src/app/services/update-json.service";
import { DynamicComponentAdmin } from "../../dynamic-field/dynamic-component";

@Component({
  selector: "app-textarea-admin",
  templateUrl: "./textarea-admin.component.html",
  styleUrls: ["./textarea-admin.component.scss"]
})
export class TextareaAdminComponent extends DynamicComponentAdmin implements OnInit {
  constructor(public jsonService: UpdateJsonService) {
    super(jsonService);
  }

  ngOnInit(): void {}
}
