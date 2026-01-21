import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { first, mergeAll, tap } from 'rxjs/operators';
import { JoinProcedureQuery } from '../../state/join-procedure.query';
import { JoinProcedureService } from '../../state/join-procedure.service';

@Component({
  selector: 'app-join-confirmation',
  templateUrl: './join-confirmation.component.html',
  styleUrls: ['./join-confirmation.component.scss'],
})
export class JoinConfirmationComponent implements OnInit {
  miToUpdate$ = this.jpQuery.miToUpdate$;

  misToDelete$ = this.jpQuery.misToDelete$;

  misToDeleteFirst$ = this.misToDelete$.pipe(
    tap(),
    mergeAll(),
    first(),
  );

  procedureDescriptionText: string;

  constructor(
    private jpQuery: JoinProcedureQuery,
    private jpService: JoinProcedureService,
    private router: Router,
    private route: ActivatedRoute,
    private messageService: MessageService,
  ) { }

  ngOnInit(): void {

  }

  join = () => {
    this.jpService.join().subscribe(
      (res) => {
        this.messageService.add({ summary: 'Успех', detail: 'Успешно извършена процедура за вливане', severity: 'success' });
        this.router.navigate(['/municipal-institution']);
      },
      (err) => {
        this.messageService.add({ summary: 'Грешка', detail: 'Възникна грешка при процедура за вливане', severity: 'error' });
        console.error(err);
      },
    );
  };
}
