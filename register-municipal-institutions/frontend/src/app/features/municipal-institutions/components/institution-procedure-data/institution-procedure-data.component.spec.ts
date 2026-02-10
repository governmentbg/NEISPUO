import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InstitutionProcedureDataComponent } from './institution-procedure-data.component';

describe('InstitutionProcedureDataComponent', () => {
  let component: InstitutionProcedureDataComponent;
  let fixture: ComponentFixture<InstitutionProcedureDataComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [InstitutionProcedureDataComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(InstitutionProcedureDataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
