import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { DivideProcedureQuery } from '../../state/divide-procedure.query';
import { DivideProcedureService } from '../../state/divide-procedure.service';

@Component({
  selector: 'app-divide-confirmation',
  templateUrl: './divide-confirmation.component.html',
  styleUrls: ['./divide-confirmation.component.scss'],
})
export class DivideConfirmationComponent implements OnInit {
  miToDelete$ = this.dpQuery.miToDelete$;

  misToCreate$ = this.dpQuery.misToCreate$;

  procedureDescriptionText: string;

  constructor(
    private dpQuery: DivideProcedureQuery,
    private dpService: DivideProcedureService,
    private router: Router,
    private route: ActivatedRoute,
    private messageService: MessageService,
  ) { }

  ngOnInit(): void {

  }

  join = () => {
    this.dpService.divide().subscribe(
      (res) => {
        this.messageService.add({ summary: 'Успех', detail: 'Успешно извършена процедура по разделяне', severity: 'success' });
        this.router.navigate(['/municipal-institution']);
      },
      (err) => {
        this.messageService.add({ summary: 'Грешка', detail: 'Възникна грешка при процедура по разделяне', severity: 'error' });
        console.error(err);
      },
    );
  };
}
