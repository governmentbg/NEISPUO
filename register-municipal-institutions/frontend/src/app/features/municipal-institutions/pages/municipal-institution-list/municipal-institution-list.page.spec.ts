import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MunicipalInstitutionListPage } from './municipal-institution-list.page';

describe('MunicipalInstitutionList', () => {
  let component: MunicipalInstitutionListPage;
  let fixture: ComponentFixture<MunicipalInstitutionListPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [MunicipalInstitutionListPage],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MunicipalInstitutionListPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
