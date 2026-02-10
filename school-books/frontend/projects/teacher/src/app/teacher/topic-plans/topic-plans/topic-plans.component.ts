import { Component } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { ActivatedRoute } from '@angular/router';
import { faChevronCircleRight as fadChevronCircleRight } from '@fortawesome/pro-duotone-svg-icons/faChevronCircleRight';
import { faListOl as fasListOl } from '@fortawesome/pro-solid-svg-icons/faListOl';
import { faPlus as fasPlus } from '@fortawesome/pro-solid-svg-icons/faPlus';
import { TopicPlansService, TopicPlans_GetAll } from 'projects/sb-api-client/src/api/topicPlans.service';
import { TableDataSource } from 'projects/shared/components/table/table-datasource';
import { deepEqual, throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { debounceTime, distinctUntilChanged } from 'rxjs/operators';

@Component({
  selector: 'sb-topic-plans',
  templateUrl: './topic-plans.component.html'
})
export class TopicPlansComponent {
  readonly fasPlus = fasPlus;
  readonly fasListOl = fasListOl;
  readonly fadChevronCircleRight = fadChevronCircleRight;

  dataSource: TableDataSource<TopicPlans_GetAll>;

  readonly searchForm = this.fb.nonNullable.group({
    name: this.fb.nonNullable.control<string | null | undefined>(null),
    basicClassName: this.fb.nonNullable.control<string | null | undefined>(null),
    subjectName: this.fb.nonNullable.control<string | null | undefined>(null),
    subjectTypeName: this.fb.nonNullable.control<string | null | undefined>(null)
  });

  constructor(topicPlansService: TopicPlansService, route: ActivatedRoute, private fb: FormBuilder) {
    const schoolYear = tryParseInt(route.snapshot.paramMap.get('schoolYear')) ?? throwParamError('schoolYear');
    const instId = tryParseInt(route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');

    this.dataSource = new TableDataSource((sortBy, sortDirection, offset, limit) =>
      topicPlansService.getAll({
        schoolYear,
        instId,
        offset,
        limit,
        ...this.searchForm.value
      })
    );

    this.searchForm.valueChanges
      .pipe(
        debounceTime(200),
        distinctUntilChanged((a, b) => deepEqual(a, b))
      )
      .subscribe(() => {
        this.dataSource.reload();
      });
  }
}
