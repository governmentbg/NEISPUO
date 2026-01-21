import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DivideMIsToCreateComponent } from './divide-mis-to-create.component';

describe('DivideMIsToCreateComponent', () => {
  let component: DivideMIsToCreateComponent;
  let fixture: ComponentFixture<DivideMIsToCreateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DivideMIsToCreateComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DivideMIsToCreateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
