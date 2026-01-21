import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { DetachProcedureQuery } from '../../state/detach-procedure.query';
import { DetachProcedureService } from '../../state/detach-procedure.service';

@Component({
  selector: 'app-detach-confirmation',
  templateUrl: './detach-confirmation.component.html',
  styleUrls: ['./detach-confirmation.component.scss'],
})
export class DetachConfirmationComponent implements OnInit {
  miToUpdate$ = this.dpQuery.miToUpdate$;

  misToCreate$ = this.dpQuery.misToCreate$;

  procedureDescriptionText: string;

  constructor(
    private dpQuery: DetachProcedureQuery,
    private dpService: DetachProcedureService,
    private router: Router,
    private route: ActivatedRoute,
    private messageService: MessageService,
  ) { }

  ngOnInit(): void {

  }

  detach = () => {
    this.dpService.detach().subscribe(
      (res) => {
        this.messageService.add({ summary: 'Успех', detail: 'Успешно извършена процедура по отделяне', severity: 'success' });
        this.router.navigate(['/municipal-institution']);
      },
      (err) => {
        this.messageService.add({ summary: 'Грешка', detail: 'Възникна грешка при процедура по отделяне', severity: 'error' });
        console.error(err);
      },
    );
  };
}
