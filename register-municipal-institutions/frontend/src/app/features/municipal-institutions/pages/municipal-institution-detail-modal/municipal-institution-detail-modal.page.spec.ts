import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MunicipalInstitutionDetailModalPage } from './municipal-institution-detail-modal.page';

describe('MunicipalInstitutionDetailModalPage', () => {
  let component: MunicipalInstitutionDetailModalPage;
  let fixture: ComponentFixture<MunicipalInstitutionDetailModalPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [MunicipalInstitutionDetailModalPage],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MunicipalInstitutionDetailModalPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
