import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { MergeProcedureQuery } from '../../state/merge-procedure.query';
import { MergeProcedureService } from '../../state/merge-procedure.service';

@Component({
  selector: 'app-merge-confirmation',
  templateUrl: './merge-confirmation.component.html',
  styleUrls: ['./merge-confirmation.component.scss'],
})
export class MergeConfirmationComponent implements OnInit {
  miToCreate$ = this.jpQuery.miToCreate$;

  misToDelete$ = this.jpQuery.misToDelete$;

  procedureDescriptionText: string;

  constructor(
    private jpQuery: MergeProcedureQuery,
    private jpService: MergeProcedureService,
    private router: Router,
    private route: ActivatedRoute,
    private messageService: MessageService,
  ) { }

  ngOnInit(): void {

  }

  join = () => {
    this.jpService.merge().subscribe(
      (res) => {
        this.messageService.add({ summary: 'Успех', detail: 'Успешно извършена процедура за сливане', severity: 'success' });
        this.router.navigate(['/municipal-institution']);
      },
      (err) => {
        this.messageService.add({ summary: 'Грешка', detail: 'Възникна грешка при процедура за сливане', severity: 'error' });
        console.error(err);
      },
    );
  };
}
