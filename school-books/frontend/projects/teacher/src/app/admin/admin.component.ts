import { Component } from '@angular/core';
import { faNotesMedical as fadNotesMedical } from '@fortawesome/pro-duotone-svg-icons/faNotesMedical';
import { faUniversity as fasUniversity } from '@fortawesome/pro-solid-svg-icons/faUniversity';
import { MenuItem } from 'projects/shared/components/app-menu/menu-item';

@Component({
  selector: 'sb-admin',
  templateUrl: './admin.component.html'
})
export class AdminComponent {
  menuItems: MenuItem[] = [
    {
      text: 'Институции',
      icon: fasUniversity,
      routeCommands: ['./institutions']
    },
    {
      text: 'Медицински бележки',
      icon: fadNotesMedical,
      routeCommands: ['./his-medical-notices']
    }
  ];
}
