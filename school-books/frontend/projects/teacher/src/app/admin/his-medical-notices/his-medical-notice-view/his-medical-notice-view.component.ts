import { Component, Input, OnInit } from '@angular/core';
import { UntypedFormBuilder } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faArrowLeft as fasArrowLeft } from '@fortawesome/pro-solid-svg-icons/faArrowLeft';
import { faSpinnerThird as fasSpinnerThird } from '@fortawesome/pro-solid-svg-icons/faSpinnerThird';
import {
  HisMedicalNoticesService,
  HisMedicalNotices_Get
} from 'projects/sb-api-client/src/api/hisMedicalNotices.service';
import {
  SIMPLE_PAGE_SKELETON_TEMPLATE,
  SkeletonComponentBase
} from 'projects/shared/components/skeleton/skeleton.component';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';

@Component({
  template: SIMPLE_PAGE_SKELETON_TEMPLATE
})
export class HisMedicalNoticeViewSkeletonComponent extends SkeletonComponentBase {
  constructor(hisMedicalNoticesService: HisMedicalNoticesService, route: ActivatedRoute) {
    super();

    const hisMedicalNoticeId =
      tryParseInt(route.snapshot.paramMap.get('hisMedicalNoticeId')) ?? throwParamError('hisMedicalNoticeId');

    this.resolve(HisMedicalNoticeViewComponent, {
      hisMedicalNotice: hisMedicalNoticesService.get({
        hisMedicalNoticeId
      })
    });
  }
}

@Component({
  selector: 'sb-his-medical-notice-view',
  templateUrl: './his-medical-notice-view.component.html'
})
export class HisMedicalNoticeViewComponent implements OnInit {
  @Input() data!: {
    hisMedicalNotice: HisMedicalNotices_Get;
  };

  readonly fasSpinnerThird = fasSpinnerThird;
  readonly fadChevronCircleRight = fadChevronCircleRight;
  readonly fasArrowLeft = fasArrowLeft;
  loading?: boolean;

  readonly form = this.fb.group({
    schoolYears: [null],
    nrnMedicalNotice: [null],
    nrnExamination: [null],
    identifier: [null],
    givenName: [null],
    familyName: [null],
    pmi: [null],
    matchedStudentNames: [null],
    matchedStudentIdentifier: [null],
    authoredOn: [null],
    fromDate: [null],
    toDate: [null]
  });

  constructor(private fb: UntypedFormBuilder, private hisMedicalNoticesService: HisMedicalNoticesService) {}

  ngOnInit(): void {
    this.loading = true;

    const hisMedicalNotice = this.data.hisMedicalNotice;

    this.form.setValue({
      schoolYears: hisMedicalNotice.schoolYears,
      nrnMedicalNotice: hisMedicalNotice.nrnMedicalNotice,
      nrnExamination: hisMedicalNotice.nrnExamination,
      identifier: hisMedicalNotice.identifier,
      givenName: hisMedicalNotice.givenName,
      familyName: hisMedicalNotice.familyName,
      pmi: hisMedicalNotice.pmi,
      matchedStudentNames: hisMedicalNotice.matchedStudentNames,
      matchedStudentIdentifier: hisMedicalNotice.matchedStudentIdentifier,
      authoredOn: hisMedicalNotice.authoredOn,
      fromDate: hisMedicalNotice.fromDate,
      toDate: hisMedicalNotice.toDate
    });
  }
}
