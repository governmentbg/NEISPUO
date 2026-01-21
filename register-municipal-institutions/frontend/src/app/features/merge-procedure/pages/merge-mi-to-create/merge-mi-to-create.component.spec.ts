import { ComponentFixture, TestBed } from '@angular/core/testing';

import { MergeMIToCreateComponent } from './merge-mi-to-create.component';

describe('MergeMIToCreateComponent', () => {
  let component: MergeMIToCreateComponent;
  let fixture: ComponentFixture<MergeMIToCreateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [MergeMIToCreateComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(MergeMIToCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
