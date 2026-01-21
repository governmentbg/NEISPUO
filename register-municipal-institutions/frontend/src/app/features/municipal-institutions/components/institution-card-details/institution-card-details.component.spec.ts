import { ComponentFixture, TestBed } from '@angular/core/testing';

import { InstitutionCardDetailsComponent } from './institution-card-details.component';

describe('InstitutionCardDetailsComponent', () => {
  let component: InstitutionCardDetailsComponent;
  let fixture: ComponentFixture<InstitutionCardDetailsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [InstitutionCardDetailsComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(InstitutionCardDetailsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
