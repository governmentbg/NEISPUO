import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetachRiDocumentComponent } from './detach-ri-document.component';

describe('DetachRiDocumentComponent', () => {
  let component: DetachRiDocumentComponent;
  let fixture: ComponentFixture<DetachRiDocumentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [DetachRiDocumentComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DetachRiDocumentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
