import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { NavigationExtras } from '@angular/router';

export interface SaveToken {
  done(success: boolean): void;
}

@Component({
  selector: 'sb-editor-panel',
  templateUrl: './editor-panel.component.html'
})
export class EditorPanelComponent implements OnInit {
  @Input() editBtnHidden = false;
  @Input() editBtnDisabled = false;

  @Input() backBtnHidden = false;
  @Input() backBtnDisabled = false;
  @Input() backBtnRouteCommands: any[] = ['../'];
  @Input() backBtnRouteExtras: NavigationExtras | null = null;

  // There is no "@Input() editable" as the value is decided in this component.
  // If we allow the parent to tell us what the value of "editable" should be we
  // need to reconsider the idea of providing a default value and always require
  // it to be explicitly defined or else cycles occur.
  @Output() editableChange = new EventEmitter<boolean>();
  @Output() save = new EventEmitter<SaveToken>();

  @Input() isNew!: boolean;

  saveBtnDisabled = false;

  editable!: boolean;

  ngOnInit() {
    if (this.isNew == null) {
      throw new Error('Missing required input parameter isNew');
    }

    this.editable = this.isNew;
    this.editableChange.emit(this.editable);
  }
}
