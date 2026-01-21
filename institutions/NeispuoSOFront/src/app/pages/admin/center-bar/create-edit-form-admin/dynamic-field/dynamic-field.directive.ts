import { ComponentFactoryResolver, ComponentRef, Directive, Input, OnInit, ViewContainerRef } from "@angular/core";
import { componentMapperAdmin } from "src/app/enums/constants";
import { FieldConfig } from "src/app/models/field.interface";

@Directive({
  selector: "[dynamicFieldAdmin]"
})
export class DynamicFieldAdminDirective implements OnInit {
  @Input() field: FieldConfig;

  componentRef: ComponentRef<any>;

  constructor(private resolver: ComponentFactoryResolver, private container: ViewContainerRef) {}

  ngOnInit() {
    const factory = this.resolver.resolveComponentFactory(componentMapperAdmin[this.field.type]);

    this.componentRef = this.container.createComponent(factory);
    this.componentRef.instance.field = this.field;
  }
}
