import { ComponentFixture, TestBed } from '@angular/core/testing';

import { FlexFieldDetailPage } from './flex-field-detail.page';

describe('FlexFieldDetailPage', () => {
  let component: FlexFieldDetailPage;
  let fixture: ComponentFixture<FlexFieldDetailPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [FlexFieldDetailPage],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(FlexFieldDetailPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
