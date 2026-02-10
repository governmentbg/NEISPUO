import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { ProcedureEnum } from '@procedures/models/procedure.enum';
import { GuideName } from '@shared/modules/user-tour-guide/constants/user-tour-guide.constants';
import { UserTourGuideService } from '@shared/modules/user-tour-guide/user-tour-guide.service';

@Component({
  selector: 'procedure-choice',
  templateUrl: './procedure-choice.page.html',
  styleUrls: ['./procedure-choice.page.scss'],
})
export class ProcedureChoicePage implements OnInit {
  selectedProcedure: ProcedureEnum;

  procedureTypes = ProcedureEnum;

  guideName = GuideName.PROCEDURE_CHOICE;

  constructor(private router: Router, private userTourService: UserTourGuideService) { }

  ngOnInit(): void {

  }

  goToProcedure() {
    if (this.selectedProcedure === ProcedureEnum.DIVIDE) {
      this.router.navigateByUrl('/divide-procedure/mi-to-delete');
      return;
    } if (this.selectedProcedure === ProcedureEnum.DETACH) {
      this.router.navigateByUrl('/detach-procedure/mi-to-update');
      return;
    } if (this.selectedProcedure === ProcedureEnum.MERGE) {
      this.router.navigateByUrl('/merge-procedure/mis-to-delete');
      return;
    } if (this.selectedProcedure === ProcedureEnum.JOIN) {
      this.router.navigateByUrl('/join-procedure/mis-to-delete');
      return;
    }
    this.router.navigateByUrl(`/procedures/${this.selectedProcedure}`);
  }

  ngOnDestroy() {
    this.userTourService.stopGuide();
  }
}
