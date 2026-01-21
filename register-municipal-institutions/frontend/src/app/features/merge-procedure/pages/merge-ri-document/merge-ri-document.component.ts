import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ScrollService } from '@shared/services/utils/scroll.service';
import { UtilsService } from '@shared/services/utils/utils.service';
import { MergeMiPreviewRiDocumentComponent } from '../../components/merge-mi-preview-ri-document/merge-mi-preview-ri-document.component';
import { MergeProcedureQuery } from '../../state/merge-procedure.query';
import { MergeProcedureService } from '../../state/merge-procedure.service';

@Component({
  selector: 'app-merge-ri-document',
  templateUrl: './merge-ri-document.component.html',
  styleUrls: ['./merge-ri-document.component.scss'],
})
export class MergeRiDocumentComponent implements OnInit {
  @ViewChild(MergeMiPreviewRiDocumentComponent) mergeMiPreviewRiDocumentComponent: MergeMiPreviewRiDocumentComponent;

  miToCreate$ = this.dpQuery.miToCreate$;

  constructor(
    private dpQuery: MergeProcedureQuery,
    private dpService: MergeProcedureService,
    private router: Router,
    private route: ActivatedRoute,
    private scrollService: ScrollService,
    private utilsService: UtilsService,

  ) { }

  ngOnInit(): void {
  }

  forwardCallback = (e: any) => {
    const RIProcedureForm = this.mergeMiPreviewRiDocumentComponent.form.get('RIProcedure');
    this.utilsService.markAllAsDirty(RIProcedureForm);
    if (RIProcedureForm.invalid) {
      this.scrollService.scrollToFirstError();
      return;
    }

    this.dpService.addRIDocumentToMIsToCreate(this.mergeMiPreviewRiDocumentComponent.form.getRawValue());
    this.dpService.addRIDocumentToMIToDelete(this.mergeMiPreviewRiDocumentComponent.form.getRawValue());
    this.router.navigate(['..', 'confirm'], { relativeTo: this.route });
  };
}
