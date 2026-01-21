import { Component, Input, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faFile as farFile } from '@fortawesome/pro-regular-svg-icons/faFile';
import { faFileExcel as farFileExcel } from '@fortawesome/pro-regular-svg-icons/faFileExcel';
import { faFilePdf as farFilePdf } from '@fortawesome/pro-regular-svg-icons/faFilePdf';
import { faFileWord as farFileWord } from '@fortawesome/pro-regular-svg-icons/faFileWord';
import { faArrowLeft as fasArrowLeft } from '@fortawesome/pro-solid-svg-icons/faArrowLeft';
import { faInfoSquare as fadInfoSquare } from '@fortawesome/pro-solid-svg-icons/faInfoSquare';
import { InfoBoardService, InfoBoard_Get } from 'projects/sb-api-client/src/api/infoBoard.service';
import {
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { BreakpointService } from 'projects/shared/services/breakpoint.service';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class InfoBoardViewSkeletonComponent extends SkeletonComponentBase {
  constructor(infoBoardService: InfoBoardService, route: ActivatedRoute) {
    super();

    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    const publicationId = tryParseInt(route.snapshot.paramMap.get('publicationId')) ?? throwParamError('publicationId');

    this.resolve(InfoBoardViewComponent, {
      schoolYear,
      instId,
      publication: infoBoardService.get({
        schoolYear,
        instId,
        publicationId: publicationId
      })
    });
  }
}

@Component({
  selector: 'sb-info-board-view',
  templateUrl: './info-board-view.component.html'
})
export class InfoBoardViewComponent implements OnInit {
  @Input() data!: {
    schoolYear: number;
    instId: number;
    publication: InfoBoard_Get;
  };

  readonly fadInfoSquare = fadInfoSquare;
  readonly fasArrowLeft = fasArrowLeft;
  readonly farFile = farFile;
  readonly farFileExcel = farFileExcel;
  readonly farFilePdf = farFilePdf;
  readonly farFileWord = farFileWord;

  constructor(public breakpointService: BreakpointService) {}

  ngOnInit() {
    this.data.publication = {
      ...this.data.publication,
      files: this.data.publication.files.map((f) => ({
        ...f,
        extension: f.extension.toLowerCase()
      }))
    };
  }
}
