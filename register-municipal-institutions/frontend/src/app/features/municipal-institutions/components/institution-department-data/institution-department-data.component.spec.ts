import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InstitutionDepartmentDataComponent } from './institution-department-data.component';

describe('InstitutionDepartmentDataComponent', () => {
  let component: InstitutionDepartmentDataComponent;
  let fixture: ComponentFixture<InstitutionDepartmentDataComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [InstitutionDepartmentDataComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(InstitutionDepartmentDataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
