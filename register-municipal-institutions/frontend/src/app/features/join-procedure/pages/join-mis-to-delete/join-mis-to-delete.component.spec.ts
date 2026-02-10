import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JoinMIsToDeleteComponent } from './join-mis-to-delete.component';

describe('JoinMIsToDeleteComponent', () => {
  let component: JoinMIsToDeleteComponent;
  let fixture: ComponentFixture<JoinMIsToDeleteComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [JoinMIsToDeleteComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(JoinMIsToDeleteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
