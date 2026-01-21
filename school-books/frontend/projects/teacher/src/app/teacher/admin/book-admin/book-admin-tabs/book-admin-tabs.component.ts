import { Component, Inject, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { IconDefinition } from '@fortawesome/fontawesome-svg-core';
import { faBook as fadBook } from '@fortawesome/pro-duotone-svg-icons/faBook';
import { faUserFriends as fadUserFriends } from '@fortawesome/pro-duotone-svg-icons/faUserFriends';
import { faUsersClass as fadUsersClass } from '@fortawesome/pro-duotone-svg-icons/faUsersClass';
import { faArrowAltDown as fasArrowAltDown } from '@fortawesome/pro-solid-svg-icons/faArrowAltDown';
import { faArrowAltUp as fasArrowAltUp } from '@fortawesome/pro-solid-svg-icons/faArrowAltUp';
import { faBallPile as fasBallPile } from '@fortawesome/pro-solid-svg-icons/faBallPile';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { InstType } from 'projects/sb-api-client/src/model/instType';
import {
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { TabItem } from 'projects/shared/components/tabs/tab-item';
import { from } from 'rxjs';
import { InstitutionInfoType, INSTITUTION_INFO } from 'src/app/teacher/teacher.component';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class BookAdminTabsSkeletonComponent extends SkeletonComponentBase {
  constructor(@Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>) {
    super();

    this.resolve(BookAdminTabsComponent, {
      institutionInfo: from(institutionInfo)
    });
  }
}

@Component({
  selector: 'sb-book-admin-tabs',
  templateUrl: './book-admin-tabs.component.html'
})
export class BookAdminTabsComponent implements OnInit {
  @Input() data!: {
    institutionInfo: InstitutionInfoType;
  };

  fadBook = fadBook;
  fasPlus = fasPlus;
  fasArrowAltDown = fasArrowAltDown;
  fasArrowAltUp = fasArrowAltUp;

  tabs!: TabItem[];
  canCreate = false;

  constructor(public route: ActivatedRoute) {}

  ngOnInit() {
    let tabConfig: [IconDefinition, string, string][] = [];

    switch (this.data.institutionInfo.instType) {
      case InstType.School:
      case InstType.CSOP:
        tabConfig = [
          [fadUsersClass, 'Паралелки/Групи', './class'],
          [fasBallPile, 'Групи в ЦДО', './cdo'],
          [fadUserFriends, 'Други групи', './other']
        ];
        break;

      case InstType.DG:
        tabConfig = [
          [fadUsersClass, 'Групи', './class'],
          [fadUserFriends, 'Логопедични групи', './other']
        ];
        break;

      case InstType.CPLR:
      case InstType.SOZ:
        tabConfig = [[fadUserFriends, 'Групи', './other']];
        break;

      default:
        throw new Error('Invalid InstType');
    }

    this.tabs = tabConfig.map((t) => ({
      text: t[1],
      icon: t[0],
      routeCommands: [t[2]],
      routeExtras: { relativeTo: this.route }
    }));

    this.canCreate =
      this.data.institutionInfo.schoolYearAllowsModifications && this.data.institutionInfo.hasCreateClassBooksAccess;
  }
}

// Училище/ЦСОП
// 1 - Паралелки/Групи
// 2 - Групи в ЦДО
// 3 - Други групи

// Детска градина
// 1 - Групи
// 3 - Логопедични групи

// ЦПЛР/СОЗ
// 3 - Групи
