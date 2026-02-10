import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FlexFieldListPage } from './flex-field-list.page';

describe('FlexFieldListPage', () => {
  let component: FlexFieldListPage;
  let fixture: ComponentFixture<FlexFieldListPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [FlexFieldListPage],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FlexFieldListPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
