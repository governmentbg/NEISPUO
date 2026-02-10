import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RiDocumentComponent } from './ri-document.component';

describe('RiDocumentComponent', () => {
  let component: RiDocumentComponent;
  let fixture: ComponentFixture<RiDocumentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [RiDocumentComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(RiDocumentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
