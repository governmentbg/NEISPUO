import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MergeMIsToDeleteComponent } from './merge-mis-to-delete.component';

describe('MergeMIsToDeleteComponent', () => {
  let component: MergeMIsToDeleteComponent;
  let fixture: ComponentFixture<MergeMIsToDeleteComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [MergeMIsToDeleteComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MergeMIsToDeleteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
