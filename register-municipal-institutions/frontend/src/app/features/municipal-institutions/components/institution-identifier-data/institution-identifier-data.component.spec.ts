import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InstitutionIdentifierDataComponent } from './institution-identifier-data.component';

describe('InstitutionIdentifierDataComponent', () => {
  let component: InstitutionIdentifierDataComponent;
  let fixture: ComponentFixture<InstitutionIdentifierDataComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [InstitutionIdentifierDataComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(InstitutionIdentifierDataComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
