import { HttpClient } from '@angular/common/http';
import { Component, OnDestroy, OnInit } from '@angular/core';
import {
  FormBuilder, FormControl, FormGroup, Validators,
} from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import {
  combineLatest, Observable, of, throwError,
} from 'rxjs';
import {
  catchError, concatMap, distinctUntilChanged, filter, map, startWith, tap,
} from 'rxjs/operators';
import { SubSink } from 'subsink';
import { MessageService, SelectItem } from 'primeng/api';
import { ScrollService } from '@shared/services/utils/scroll.service';
import { EnvironmentService } from '@core/services/environment.service';
import { AuthQuery } from '../../../../core/authentication/auth-state-manager/auth.query';
import { UtilsService } from '../../../../shared/services/utils/utils.service';

type FlexFieldDataType = 'number' | 'text' | 'single_choice' | 'multiple_select' | 'radio';

interface FlexFieldData {
  type: FlexFieldDataType;
  label: string;
  choices?: { label: string; value: string }[];
  maxLength: number;
  maxValue: number;
}

export interface FlexFieldDTO {
  // TODO: Move to models folder
  RIFlexFieldID: number;
  Data: FlexFieldData;
  Mandatory: boolean;
  /**
   * Relations
   */
  SysUser: any; // TODO: remove anys
  Municipality: any;
  RIFlexFieldValues: any[];
  _name?: string;
  _type?: string;
}

@Component({
  selector: 'app-flex-field-detail',
  templateUrl: './flex-field-detail.page.html',
  styleUrls: ['./flex-field-detail.page.scss'],
})
export class FlexFieldDetailPage implements OnInit, OnDestroy {
  flexFieldTypes: SelectItem<FlexFieldDataType>[] = [
    { label: 'Число', value: 'number' },
    { label: 'Свободен текст', value: 'text' },
    { label: 'Единичен избор', value: 'single_choice', disabled: true },
    { label: 'Множестен избор', value: 'multiple_select', disabled: true },
  ];

  selectedFlexFieldType = new FormControl(this.flexFieldTypes[0]);

  mandatoryOptions: SelectItem<boolean>[] = [
    { label: 'Да', value: true },
    { label: 'Не', value: false },
  ];

  form: FormGroup = this.formBuilder.group({
    RIFlexFieldID: [null],
    Data: this.formBuilder.group({
      type: ['number', Validators.required],
      label: ['', Validators.required],
      choices: [null], // This control is rebuilt dynamically
      maxLength: [null], // This control is rebuilt dynamically
      maxValue: [null], // This control is rebuilt dynamically
    }),
    Mandatory: [null, Validators.required],
    SysUser: [null, Validators.required],
    Municipality: [null, Validators.required],
  });

  formLoaded = false;

  private flexFieldTypeChanges$ = this.form
    .get('Data.type')
    .valueChanges.pipe(startWith(this.form.get('Data.type').value), distinctUntilChanged());

  pageMode: 'edit' | 'create' = 'create';

  private subs = new SubSink();

  private readonly environment = this.envService.environment;

  constructor(
    private formBuilder: FormBuilder,
    private utilsService: UtilsService,
    private authQuery: AuthQuery,
    private httpClient: HttpClient,
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private messageService: MessageService,
    private scrollService: ScrollService,
    private envService: EnvironmentService,
  ) {}

  ngOnInit(): void {
    this.subs.sink = combineLatest([this.flexFieldTypeChanges$, this.loadFlexFieldEntity()]).subscribe((args) => this.initFormLogic(args));

    this.autoFillUserMunicipality();
  }

  private loadFlexFieldEntity(): Observable<FlexFieldDTO> {
    return this.activatedRoute.paramMap.pipe(
      map((pm) => pm.get('id')),
      tap((id) => (this.pageMode = +id ? 'edit' : 'create')),
      concatMap((id) => {
        if (!+id) {
          // Object for patching form, providing only sane defaults
          return of({ RIFlexFieldID: null, Data: { type: 'number' } } as FlexFieldDTO);
        }

        // If id is present, return observable that loads the entity
        return this.httpClient.get<FlexFieldDTO>(`${this.environment.BACKEND_URL}/v1/ri-flex-field/${id}`).pipe(
          catchError((err) => {
            this.router.navigate(['../..'], { relativeTo: this.activatedRoute });
            return throwError(err);
          }),
        );
      }),
    );
  }

  private initFormLogic([flexFieldDataType, initialForm]: [FlexFieldDataType, FlexFieldDTO]) {
    if (flexFieldDataType === 'number') {
      const choicesControl = this.form.get('Data.choices');
      choicesControl.setValidators([]);
      choicesControl.reset();

      const maxLengthControl = this.form.get('Data.maxLength');
      maxLengthControl.setValidators([]);
      maxLengthControl.reset();

      const maxValueControl = this.form.get('Data.maxValue');
      maxValueControl.setValidators([Validators.required]);
      maxValueControl.reset();
    }

    if (flexFieldDataType === 'text') {
      const choicesControl = this.form.get('Data.choices');
      choicesControl.setValidators([]);
      choicesControl.reset();

      const maxLengthControl = this.form.get('Data.maxLength');
      maxLengthControl.setValidators([Validators.required]);
      maxLengthControl.reset();

      const maxValueControl = this.form.get('Data.maxValue');
      maxValueControl.setValidators([]);
      maxValueControl.reset();
    }

    this.form.markAsPristine();
    if (!this.formLoaded) {
      this.formLoaded = true;
      this.form.patchValue(initialForm);
    }

    if (initialForm.RIFlexFieldValues?.length > 0) {
      this.form.disable();
    }
  }

  private autoFillUserMunicipality() {
    this.subs.sink = combineLatest([
      this.authQuery.mySysUser$.pipe(filter((v) => !!v)),
      this.authQuery.myMunicipality$.pipe(filter((v) => !!v)),
    ]).subscribe(([mySysUser, myMunicipality]: [any, any]) => {
      this.form.patchValue({ SysUser: mySysUser, Municipality: myMunicipality });
    });
  }

  onSubmit() {
    this.utilsService.markAllAsDirty(this.form);

    if (this.form.invalid) {
      this.scrollService.scrollToFirstError();
      return;
    }

    const formValue = this.form.getRawValue();
    // TODO: Add isloading.
    if (this.pageMode === 'create') {
      this.createFlexField(formValue);
    } else {
      this.updateFlexField(formValue);
    }
  }

  createFlexField(formValue: any) {
    this.httpClient.post(`${this.environment.BACKEND_URL}/v1/ri-flex-field`, formValue).subscribe(
      (s: any) => {
        this.messageService.add({ severity: 'success', summary: 'Създаване', detail: 'Успешно създадохте флекс поле.' });

        this.router.navigate(['..', 'edit', s.RIFlexFieldID], { relativeTo: this.activatedRoute });
      },
      (e) => this.messageService.add({
        severity: 'error',
        summary: 'Грешка',
        detail: 'Възникна грешка, моля опитайте отново.',
      }),
    );
  }

  updateFlexField(formValue: any) {
    const { RIFlexFieldID } = formValue;
    this.httpClient.put(`${this.environment.BACKEND_URL}/v1/ri-flex-field/${RIFlexFieldID}`, formValue).subscribe(
      (s: any) => {
        this.messageService.add({
          severity: 'success',
          summary: 'Редактиране',
          detail: 'Вашите промените бяха запазени.',
        });

        this.utilsService.reloadSameRoute(this.router);
      },
      (e) => this.messageService.add({
        severity: 'error',
        summary: 'Грешка',
        detail: 'Възникна грешка, моля опитайте отново.',
      }),
    );
  }

  /** TODO: 1. Prompt confirmation. 2. add is loading */
  onDelete() {
    const { RIFlexFieldID } = this.form.getRawValue();
    this.httpClient.delete(`${this.environment.BACKEND_URL}/v1/ri-flex-field/${RIFlexFieldID}`).subscribe(
      (s: any) => {
        this.messageService.add({ severity: 'info', summary: 'Изтриване', detail: 'Успешно изтрихте флекс поле.' });

        this.router.navigate(['../..'], { relativeTo: this.activatedRoute });
      },
      (e) => this.messageService.add({
        severity: 'error',
        summary: 'Грешка',
        detail: 'Възникна грешка, моля опитайте отново.',
      }),
    );
  }

  ngOnDestroy() {
    this.subs.unsubscribe();
  }
}
