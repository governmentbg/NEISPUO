import { ComponentFixture, TestBed } from '@angular/core/testing';

import { JoinRiDocumentComponent } from './join-ri-document.component';

describe('JoinRiDocumentComponent', () => {
  let component: JoinRiDocumentComponent;
  let fixture: ComponentFixture<JoinRiDocumentComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [JoinRiDocumentComponent],
    })
      .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(JoinRiDocumentComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
