import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DivideMIToDeleteComponent } from './divide-mi-to-delete.component';

describe('DivideMIToDeleteComponent', () => {
  let component: DivideMIToDeleteComponent;
  let fixture: ComponentFixture<DivideMIToDeleteComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DivideMIToDeleteComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DivideMIToDeleteComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
