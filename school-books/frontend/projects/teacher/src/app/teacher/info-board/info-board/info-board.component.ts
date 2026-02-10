import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faPaperclip as farPaperclip } from '@fortawesome/pro-regular-svg-icons/faPaperclip';
import { faInfoSquare as fadInfoSquare } from '@fortawesome/pro-solid-svg-icons/faInfoSquare';
import { faUniversity as fasUniversity } from '@fortawesome/pro-solid-svg-icons/faUniversity';
import { faUsers as fasUsers } from '@fortawesome/pro-solid-svg-icons/faUsers';
import {
  InfoBoardService,
  InfoBoard_GetAllPublished,
  InfoBoard_GetMetadata
} from 'projects/sb-api-client/src/api/infoBoard.service';
import { PublicationType } from 'projects/sb-api-client/src/model/publicationType';
import {
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { BreakpointService } from 'projects/shared/services/breakpoint.service';
import { enumFromStringValue, throwParamError, tryParseInt } from 'projects/shared/utils/various';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class InfoBoardSkeletonComponent extends SkeletonComponentBase {
  constructor(infoBoardService: InfoBoardService, route: ActivatedRoute) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const status = route.snapshot.paramMap.get('status') ?? throwParamError('status');

    const paramType = route.snapshot.paramMap.get('type');
    let type: PublicationType | null = null;
    if (paramType) {
      type = enumFromStringValue(PublicationType, paramType) ?? throwParamError('type');
    }

    this.resolve(InfoBoardComponent, {
      schoolYear,
      instId,
      type,
      status,
      metadata: infoBoardService.getMetadata({
        schoolYear,
        instId,
        type
      })
    });
  }
}

@Component({
  selector: 'sb-info-board',
  templateUrl: './info-board.component.html'
})
export class InfoBoardComponent implements OnInit {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    type: PublicationType | null;
    status: string;
    metadata: InfoBoard_GetMetadata;
  };

  dataSource!: TableDataSource<InfoBoard_GetAllPublished>;

  readonly fadInfoSquare = fadInfoSquare;
  readonly fasUniversity = fasUniversity;
  readonly fasUsers = fasUsers;
  readonly farPaperclip = farPaperclip;
  readonly PublicationType = PublicationType;

  constructor(public breakpointService: BreakpointService, private infoBoardService: InfoBoardService) {}

  ngOnInit() {
    if (this.data.metadata.count || this.data.metadata.archivedCount) {
      this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
        this.infoBoardService.getAllPublished({
          schoolYear: this.data.schoolYear,
          instId: this.data.instId,
          archived: this.data.status === 'archived',
          type: this.data.type,
          offset,
          limit: limit
        })
      );
    }
  }
}
