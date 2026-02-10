import { Component, Inject } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { IndividualWorksService, IndividualWorks_GetAll } from 'projects/sb-api-client/src/api/individualWorks.service';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { ClassBookInfoType } from 'projects/shared/utils/book';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { CLASS_BOOK_INFO } from '../book/book.component';

@Component({
  selector: 'sb-individual-works',
  templateUrl: './individual-works.component.html'
})
export class IndividualWorksComponent {
  readonly fasPlus = fasPlus;
  readonly fadChevronCircleRight = fadChevronCircleRight;

  dataSource: TableDataSource<IndividualWorks_GetAll>;
  canCreate = false;
  classBookId: number;
  constructor(
    @Inject(CLASS_BOOK_INFO) classBookInfo: Promise<ClassBookInfoType>,
    individualWorksService: IndividualWorksService,
    route: ActivatedRoute
  ) {
    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
    this.classBookId = tryParseInt(route.snapshot.paramMap.get('classBookId')) ?? throwParamError('classBookId');

    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      individualWorksService.getAll({ schoolYear, instId, classBookId: this.classBookId, offset, limit })
    );

    // the promise should be resolved by this stage,
    // so no need for a skeleton screen
    classBookInfo.then((classBookInfo) => {
      this.canCreate = classBookInfo.bookAllowsModifications && classBookInfo.hasCreateIndividualWorkAccess;
    });
  }
}
