import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ChildrenCodesPage } from './children-codes.page';

describe('ChildrenCodesPage', () => {
  let component: ChildrenCodesPage;
  let fixture: ComponentFixture<ChildrenCodesPage>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ChildrenCodesPage]
    }).compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ChildrenCodesPage);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
