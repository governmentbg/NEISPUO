import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InstitutionTypeDataComponent } from './institution-type-data.component';

describe('InstitutionTypeDataComponent', () => {
  let component: InstitutionTypeDataComponent;
  let fixture: ComponentFixture<InstitutionTypeDataComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [InstitutionTypeDataComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(InstitutionTypeDataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
