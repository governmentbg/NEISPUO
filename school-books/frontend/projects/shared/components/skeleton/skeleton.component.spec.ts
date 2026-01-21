import { Component, Input } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { MatDialogModule } from '@angular/material/dialog';
import { By } from '@angular/platform-browser';
import { DynamicComponent, DynamicDirectivesDirective, DynamicIoModule, DynamicModule } from 'ng-dynamic-component';
import { Subject } from 'rxjs';
import { SkeletonComponentBase } from './skeleton.component';
import { TestSkeletonTemplateComponent } from './test-skeleton-template.component';

export const TEST_SKELETON_TEMPLATE =
  '<sb-test-skeleton-template [component]="component" [inputs]="inputs"></sb-test-skeleton-template>';

@Component({
  template: TEST_SKELETON_TEMPLATE
})
class TestObservableSkeletonComponent extends SkeletonComponentBase {
  subject: Subject<string>;
  constructor() {
    const subj = new Subject<string>();
    super();
    this.resolve(TestObservableComponent, subj);
    this.subject = subj;
  }
}

@Component({
  template: TEST_SKELETON_TEMPLATE
})
class TestObservableDictionarySkeletonComponent extends SkeletonComponentBase {
  subject1: Subject<string>;
  subject2: Subject<number>;
  constructor() {
    const subj1 = new Subject<string>();
    const subj2 = new Subject<number>();
    super();
    this.resolve(TestObservableDictionaryComponent, {
      prop1: subj1,
      prop2: subj2
    });
    this.subject1 = subj1;
    this.subject2 = subj2;
  }
}

@Component({
  template: '<p>{{data}}</p>'
})
class TestObservableComponent {
  @Input()
  data!: string;
}

@Component({
  template: '<p>{{data.prop1}} {{data.prop2}}</p>'
})
class TestObservableDictionaryComponent {
  @Input()
  data!: { prop1: string; prop2: number };
}

describe('SkeletonComponent', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [DynamicModule, DynamicIoModule, MatDialogModule],
      declarations: [
        DynamicComponent,
        DynamicDirectivesDirective,
        TestSkeletonTemplateComponent,
        TestObservableComponent,
        TestObservableDictionaryComponent,
        TestObservableSkeletonComponent,
        TestObservableDictionarySkeletonComponent
      ]
    }).compileComponents();
  });

  describe('with Observable data', () => {
    it('should not create component if observable has not emitted', () => {
      const fixture = TestBed.createComponent(TestObservableSkeletonComponent);
      fixture.detectChanges();

      const wrappedComponentFixture = fixture.debugElement.query(By.directive(TestObservableComponent));
      expect(wrappedComponentFixture).toBeNull();
    });

    it('should create component with correct data when observable emits', () => {
      const fixture = TestBed.createComponent(TestObservableSkeletonComponent);
      const component = fixture.componentInstance;
      const data = 'it works!';
      component.subject.next(data);
      fixture.detectChanges();

      const wrappedComponentFixture = fixture.debugElement.query(By.directive(TestObservableComponent));
      expect(wrappedComponentFixture).not.toBeNull();
      expect(wrappedComponentFixture.componentInstance).toEqual(jasmine.any(TestObservableComponent));
      expect((wrappedComponentFixture.componentInstance as TestObservableComponent).data).toEqual('it works!');
    });
  });

  describe('with ObservableDictionary data', () => {
    it('should not create component if observable has not emitted', () => {
      const fixture = TestBed.createComponent(TestObservableDictionarySkeletonComponent);
      fixture.detectChanges();

      const wrappedComponentFixture = fixture.debugElement.query(By.directive(TestObservableDictionaryComponent));
      expect(wrappedComponentFixture).toBeNull();
    });

    it('should create component with correct data when observable emits', () => {
      const fixture = TestBed.createComponent(TestObservableDictionarySkeletonComponent);
      const component = fixture.componentInstance;
      const prop1 = 'it works!';
      const prop2 = 5;
      component.subject1.next(prop1);
      component.subject2.next(prop2);
      fixture.detectChanges();

      const wrappedComponentFixture = fixture.debugElement.query(By.directive(TestObservableDictionaryComponent));
      expect(wrappedComponentFixture).not.toBeNull();
      expect(wrappedComponentFixture.componentInstance).toEqual(jasmine.any(TestObservableDictionaryComponent));
      expect((wrappedComponentFixture.componentInstance as TestObservableDictionaryComponent).data).toEqual({
        prop1: 'it works!',
        prop2: 5
      });
    });
  });
});
