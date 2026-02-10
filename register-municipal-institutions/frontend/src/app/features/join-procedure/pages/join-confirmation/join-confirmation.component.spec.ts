import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JoinConfirmationComponent } from './join-confirmation.component';

describe('DivideProcedureConfirmationComponent', () => {
  let component: JoinConfirmationComponent;
  let fixture: ComponentFixture<JoinConfirmationComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [JoinConfirmationComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(JoinConfirmationComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
