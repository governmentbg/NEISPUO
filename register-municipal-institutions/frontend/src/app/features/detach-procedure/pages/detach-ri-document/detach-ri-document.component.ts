import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ScrollService } from '@shared/services/utils/scroll.service';
import { UtilsService } from '@shared/services/utils/utils.service';
import { DetachMiPreviewRiDocumentComponent } from '../../components/detach-mi-preview-ri-document/detach-mi-preview-ri-document.component';
import { DetachProcedureQuery } from '../../state/detach-procedure.query';
import { DetachProcedureService } from '../../state/detach-procedure.service';

@Component({
  selector: 'app-detach-ri-document',
  templateUrl: './detach-ri-document.component.html',
  styleUrls: ['./detach-ri-document.component.scss'],
})
export class DetachRiDocumentComponent implements OnInit {
  @ViewChild(DetachMiPreviewRiDocumentComponent) DetachMiPreviewRiDocumentComponent: DetachMiPreviewRiDocumentComponent;

  miToUpdate$ = this.dpQuery.miToUpdate$;

  constructor(
    private dpQuery: DetachProcedureQuery,
    private dpService: DetachProcedureService,
    private router: Router,
    private route: ActivatedRoute,
    private scrollService: ScrollService,
    private utilsService: UtilsService,
  ) { }

  ngOnInit(): void {
  }

  forwardCallback = (e: any) => {
    this.utilsService.markAllAsDirty(this.DetachMiPreviewRiDocumentComponent.form);

    if (this.DetachMiPreviewRiDocumentComponent.form.invalid) {
      this.scrollService.scrollToFirstError();
      return;
    }
    this.dpService.addRIDocumentToMIsToCreate(this.DetachMiPreviewRiDocumentComponent.form.getRawValue());
    this.dpService.addRIDocumentToMIsToUpdate(this.DetachMiPreviewRiDocumentComponent.form.getRawValue());
    this.router.navigate(['..', 'confirm'], { relativeTo: this.route });
  };
}
