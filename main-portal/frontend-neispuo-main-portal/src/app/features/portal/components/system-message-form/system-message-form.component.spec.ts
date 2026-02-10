import { ComponentFixture, TestBed } from '@angular/core/testing';

import { SystemMessageFormComponent } from './system-message-form.component';

describe('AddSystemMessageModalComponent', () => {
  let component: SystemMessageFormComponent;
  let fixture: ComponentFixture<SystemMessageFormComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [SystemMessageFormComponent]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(SystemMessageFormComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
