import {
  ComponentFactoryResolver,
  ComponentRef,
  Directive,
  EventEmitter,
  Input,
  OnChanges,
  OnInit,
  Output,
  SimpleChanges,
  ViewContainerRef
} from "@angular/core";
import { FormGroup } from "@angular/forms";
import { componentMapper } from "../../../../enums/constants";
import { Mode } from "../../../../enums/mode.enum";
import { FieldConfig, RegixData } from "../../../../models/field.interface";

@Directive({
  selector: "[dynamicField]"
})
export class DynamicFieldDirective implements OnInit, OnChanges {
  @Input() field: FieldConfig;
  @Input() group: FormGroup;
  @Input() mode: Mode;
  @Input() showLabel: boolean = true;

  @Output() filterTable: EventEmitter<any> = new EventEmitter();

  @Output() valueChange: EventEmitter<any> = new EventEmitter();

  @Output() performProcedure: EventEmitter<{
    procName: string;
    procParams: Object;
    canSign?: boolean;
    searchByEik?: boolean;
    requiredFields?: string[];
    groupValues?: Object;
    generateCertificate?: boolean;
    button?: FieldConfig;
  }> = new EventEmitter();

  @Output() performRegixProc: EventEmitter<{ regixData: RegixData; groupValues: any }> = new EventEmitter<{
    regixData: RegixData;
    groupValues: any;
  }>();

  componentRef: ComponentRef<any>;

  constructor(private resolver: ComponentFactoryResolver, private container: ViewContainerRef) {}

  ngOnInit() {
    const factory = this.resolver.resolveComponentFactory(componentMapper[this.field.type]);

    this.componentRef = this.container.createComponent(factory);
    this.componentRef.instance.field = this.field;
    this.componentRef.instance.group = this.group;
    this.componentRef.instance.mode = this.mode;
    this.componentRef.instance.showLabel = this.showLabel;
    this.componentRef.instance.valueChange = this.valueChange;
    this.componentRef.instance.filterTable = this.filterTable;
    this.componentRef.instance.performProcedure = this.performProcedure;
    this.componentRef.instance.performRegixProc = this.performRegixProc;
  }

  ngOnChanges(changes: SimpleChanges) {
    // if real group change happens mode will not be changed
    // mode is changed only on init and then the group is set
    if (changes.group && !changes.mode) {
      this.componentRef.instance.group = this.group;
    }
  }
}
