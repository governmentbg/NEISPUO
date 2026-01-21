import { Component, Inject, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faChild as fadChild } from '@fortawesome/pro-duotone-svg-icons/faChild';
import { faUsersClass as fadUsersClass } from '@fortawesome/pro-duotone-svg-icons/faUsersClass';
import { ClassBookType } from 'projects/sb-api-client/src/model/classBookType';
import {
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { TabItem } from 'projects/shared/components/tabs/tab-item';
import { from } from 'rxjs';
import { InstitutionInfoType, INSTITUTION_INFO } from 'src/app/teacher/teacher.component';
import { ClassBookAdminInfoType, CLASS_BOOK_ADMIN_INFO } from '../../book-admin-view.component';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class BookAdminPrintTabsSkeletonComponent extends SkeletonComponentBase {
  constructor(
    @Inject(INSTITUTION_INFO) institutionInfo: Promise<InstitutionInfoType>,
    @Inject(CLASS_BOOK_ADMIN_INFO) classBookAdminInfo: Promise<ClassBookAdminInfoType>
  ) {
    super();

    this.resolve(BookAdminPrintTabsComponent, {
      institutionInfo: from(institutionInfo),
      classBookAdminInfo: from(classBookAdminInfo)
    });
  }
}

@Component({
  selector: 'sb-book-admin-print-tabs',
  templateUrl: './book-admin-print-tabs.component.html'
})
export class BookAdminPrintTabsComponent implements OnInit {
  @Input() data!: {
    institutionInfo: InstitutionInfoType;
    classBookAdminInfo: ClassBookAdminInfoType;
  };

  tabs!: TabItem[];
  fadChild = fadChild;

  constructor(public route: ActivatedRoute) {}

  ngOnInit() {
    const showStudentPrint =
      this.data.classBookAdminInfo.bookType === ClassBookType.Book_IV ||
      this.data.classBookAdminInfo.bookType === ClassBookType.Book_V_XII;

    this.tabs = [
      { text: 'Дневник', icon: fadUsersClass, routeCommands: ['./classBook'], routeExtras: { relativeTo: this.route } },
      ...(showStudentPrint
        ? [
            {
              text: 'Ученическа книжка',
              icon: fadChild,
              routeCommands: ['./studentBook'],
              routeExtras: { relativeTo: this.route }
            }
          ]
        : [])
    ];
  }
}
