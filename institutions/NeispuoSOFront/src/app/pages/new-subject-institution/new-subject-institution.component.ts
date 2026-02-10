import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { AuthService } from 'src/app/auth/auth.service';
import { FormTypeInt } from 'src/app/enums/formType.enum';
import { Menu } from 'src/app/enums/menu.enum';
import { Mode, ModeInt } from 'src/app/enums/mode.enum';
import { Form } from 'src/app/models/form.interface';
import { FormDataService } from 'src/app/services/form-data.service';
import { HelperService } from 'src/app/services/helpers.service';
import { environment } from 'src/environments/environment';

@Component({
  selector: 'app-new-subject-institution',
  templateUrl: './new-subject-institution.component.html',
  styleUrls: ['./new-subject-institution.component.scss']
})
export class NewSubjectInstitutionComponent implements OnInit {
  isLoading: boolean = false;
  subjectName: string;
  data: Form;
  formGroup: FormGroup;
  message: string = '';
  dataTaken: boolean = false;
  regex: string = `[а-яА-Я\-,.:;()/"„“”\' \\\\]*`;

  private searchedSubjectName;

  get modes() {
    return Mode;
  }

  constructor(
    private formDataService: FormDataService,
    private router: Router,
    private authService: AuthService,
    private route: ActivatedRoute,
    private helperService: HelperService,
    private fb: FormBuilder
  ) {}

  ngOnInit() {}

  getData() {
    this.dataTaken = true;
    this.message = '';
    this.data = null;
    this.isLoading = true;
    this.subjectName = this.subjectName.trim();
    const queryParams = environment.production
      ? this.helperService.decodeParams(this.route.snapshot.queryParams['q'])
      : { ...this.route.snapshot.queryParams };
    this.searchedSubjectName = this.subjectName;

    const body = {
      data: { subjectName: this.subjectName, instid: queryParams.instid },
      procedureName: 'inst_basic.checkSubjectInstitution',
      operationType: 2
    };

    this.formDataService.performProcedure(body).subscribe(
      async (res: any) => {
        try {
          res && (res = JSON.parse(res));
        } catch (err) {}

        await this.getMessage(res);

        let status;

        if (res) {
          if (res.length) {
            status = res[0].status;
          } else {
            status = res.status;
          }
        }

        if (status === 1) {
          let operationType = ModeInt.view;
          const instType = FormTypeInt[this.authService.getType()] + 1;

          const data: any = await this.formDataService.fillData(operationType, instType, res, 'subjectFoundInstitution').toPromise();
          this.formGroup = this.fb.group({});
          this.data = data;
        }

        this.isLoading = false;
      },
      () => (this.isLoading = false)
    );
  }

  private async getMessage(res) {
    const messageCode = res.length ? res[0].messageCode : res.messageCode;

    if (messageCode) {
      const messages = await this.formDataService.getMessages().toPromise();
      this.message = messages[messageCode];
    }
  }

  navigate() {
    const subjectName = this.searchedSubjectName ? this.searchedSubjectName : this.subjectName;

    let queryParams = environment.production
      ? this.helperService.decodeParams(this.route.snapshot.queryParams['q'])
      : { ...this.route.snapshot.queryParams };

    queryParams.subjectName = subjectName;
    queryParams.isInstitution = 99;
    queryParams.isValid = 0;
    environment.production && (queryParams = this.helperService.encodeParams(queryParams));

    this.router.navigate(['/', Menu.Create, 'subejctInstitutionData'], { queryParams });
  }

  goBack() {
    let path = '/';
    let queryParams = {};

    if (this.authService.getPrevUrlData()) {
      [path, queryParams] = this.authService.getPrevUrlData();
      this.authService.removeUrl();
    }

    this.router.navigate([path], { queryParams });
  }
}
