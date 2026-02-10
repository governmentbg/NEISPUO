import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ScrollService } from '@shared/services/utils/scroll.service';
import { UtilsService } from '@shared/services/utils/utils.service';
import { DivideMiPreviewRiDocumentComponent } from '../../components/divide-mi-preview-ri-document/divide-mi-preview-ri-document.component';
import { DivideProcedureQuery } from '../../state/divide-procedure.query';
import { DivideProcedureService } from '../../state/divide-procedure.service';

@Component({
  selector: 'app-divide-ri-document',
  templateUrl: './divide-ri-document.component.html',
  styleUrls: ['./divide-ri-document.component.scss'],
})
export class DivideRiDocumentComponent implements OnInit {
  @ViewChild(DivideMiPreviewRiDocumentComponent) divideMiPreviewRiProcedureComponent: DivideMiPreviewRiDocumentComponent;

  miToDelete$ = this.dpQuery.miToDelete$;

  constructor(
    private dpQuery: DivideProcedureQuery,
    private dpService: DivideProcedureService,
    private router: Router,
    private route: ActivatedRoute,
    private scrollService: ScrollService,
    private utilsService: UtilsService,

  ) { }

  ngOnInit(): void {
  }

  forwardCallback = (e: any) => {
    this.utilsService.markAllAsDirty(this.divideMiPreviewRiProcedureComponent.form);
    if (this.divideMiPreviewRiProcedureComponent.form.invalid) {
      this.scrollService.scrollToFirstError();
      return;
    }

    this.dpService.addRIDocumentToMIsToCreate(this.divideMiPreviewRiProcedureComponent.form.getRawValue());
    this.dpService.addRIDocumentToMIToDelete(this.divideMiPreviewRiProcedureComponent.form.getRawValue());
    this.router.navigate(['..', 'confirm'], { relativeTo: this.route });
  };
}
