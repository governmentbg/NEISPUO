import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ScrollService } from '@shared/services/utils/scroll.service';
import { UtilsService } from '@shared/services/utils/utils.service';
import { first, mergeAll, tap } from 'rxjs/operators';
import { JoinMiPreviewRiDocumentComponent } from '../../components/join-mi-preview-ri-document/join-mi-preview-ri-document.component';
import { JoinProcedureQuery } from '../../state/join-procedure.query';
import { JoinProcedureService } from '../../state/join-procedure.service';

@Component({
  selector: 'app-join-ri-document',
  templateUrl: './join-ri-document.component.html',
  styleUrls: ['./join-ri-document.component.scss'],
})
export class JoinRiDocumentComponent implements OnInit {
  @ViewChild(JoinMiPreviewRiDocumentComponent) joinMiPreviewRiDocumentComponent: JoinMiPreviewRiDocumentComponent;

  miToUpdate$ = this.jpQuery.miToUpdate$;

  misToDelete$ = this.jpQuery.misToDelete$;

  misToDeleteFirst$ = this.misToDelete$.pipe(
    tap(),
    mergeAll(),
    first(),
  );

  constructor(
    private jpQuery: JoinProcedureQuery,
    private jpService: JoinProcedureService,
    private router: Router,
    private route: ActivatedRoute,
    private scrollService: ScrollService,
    private utilsService: UtilsService,

  ) { }

  ngOnInit(): void {
  }

  forwardCallback = (e: any) => {
    const RIProcedureForm = this.joinMiPreviewRiDocumentComponent.form.get('RIProcedure');
    this.utilsService.markAllAsDirty(RIProcedureForm);
    if (RIProcedureForm.invalid) {
      this.scrollService.scrollToFirstError();
      return;
    }

    this.jpService.addRIDocumentToMIsToDelete(this.joinMiPreviewRiDocumentComponent.form.getRawValue());
    this.router.navigate(['..', 'confirm'], { relativeTo: this.route });
  };
}
