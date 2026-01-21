import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PremInstitutionDataComponent } from './prem-institution-data.component';

describe('PremInstitutionDataComponent', () => {
  let component: PremInstitutionDataComponent;
  let fixture: ComponentFixture<PremInstitutionDataComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [PremInstitutionDataComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(PremInstitutionDataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
