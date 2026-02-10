import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DivideConfirmationComponent } from './divide-confirmation.component';

describe('DivideProcedureConfirmationComponent', () => {
  let component: DivideConfirmationComponent;
  let fixture: ComponentFixture<DivideConfirmationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DivideConfirmationComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DivideConfirmationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
