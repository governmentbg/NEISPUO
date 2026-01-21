import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DivideRiDocumentComponent } from './divide-ri-document.component';

describe('DivideRiDocumentComponent', () => {
  let component: DivideRiDocumentComponent;
  let fixture: ComponentFixture<DivideRiDocumentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DivideRiDocumentComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DivideRiDocumentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
