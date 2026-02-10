import { Component, Input } from '@angular/core';
import { TabItem } from './tab-item';

@Component({
  selector: 'sb-tabs',
  templateUrl: './tabs.component.html'
})
export class TabsComponent {
  @Input() tabs: TabItem[] = [];
  @Input() ariaLabel: string | null = null;
}
