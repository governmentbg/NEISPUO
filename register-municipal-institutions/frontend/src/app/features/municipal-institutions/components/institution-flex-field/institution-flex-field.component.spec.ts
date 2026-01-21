import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InstitutionFlexFieldComponent } from './institution-flex-field.component';

describe('InstitutionFlexFieldComponent', () => {
  let component: InstitutionFlexFieldComponent;
  let fixture: ComponentFixture<InstitutionFlexFieldComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [InstitutionFlexFieldComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(InstitutionFlexFieldComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
