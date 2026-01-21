import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JoinMIToUpdateComponent } from './join-mi-to-update.component';

describe('JoinMIToUpdateComponent', () => {
  let component: JoinMIToUpdateComponent;
  let fixture: ComponentFixture<JoinMIToUpdateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [JoinMIToUpdateComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(JoinMIToUpdateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
