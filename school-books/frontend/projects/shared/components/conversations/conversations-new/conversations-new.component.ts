import { Component, OnDestroy, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import {
  ConversationParticipantsNomService,
  ConversationParticipantsNom_GetNomsById
} from 'projects/sb-api-client/src/api/conversationParticipantsNom.service';
import { ConversationsService } from 'projects/sb-api-client/src/api/conversations.service';
import { StudentClassBooksService } from 'projects/sb-api-client/src/api/studentClassBooks.service';
import { INomService, INomVO, NomServiceWithParams } from 'projects/shared/components/nom-select/nom-service';
import { IShouldPreventLeave } from 'projects/shared/components/skeleton/skeleton.component';
import { ActionService } from 'projects/shared/services/action-service/action.service';
import { Project } from 'projects/shared/services/config.service';
import { ArrayElementType } from 'projects/shared/utils/type';
import { throwParamError, tryParseInt } from 'projects/shared/utils/various';
import { Subject } from 'rxjs';
import { takeUntil, tap } from 'rxjs/operators';

type ParticipantNomVO = ArrayElementType<ConversationParticipantsNom_GetNomsById>['id'];

@Component({
  selector: 'sb-conversations-new',
  templateUrl: './conversations-new.component.html',
  styleUrls: ['./conversations-new.component.scss']
})
export class ConversationsNewComponent implements OnInit, OnDestroy, IShouldPreventLeave {
  private readonly destroyed$ = new Subject<void>();
  conversationParticipantsService!: INomService<ParticipantNomVO, { instId: number }>;
  saving = false;
  showNoInstFoundError = false;
  showCreateForm = false;
  isStudentApp = false;
  instIds: INomVO<any>[] = [];

  readonly form = this.fb.group({
    participants: this.fb.control<ParticipantNomVO[]>([], { validators: [Validators.required] }),
    isLocked: this.fb.control<boolean>(false),
    title: this.fb.control<string | null>('', { validators: [Validators.required] }),
    message: this.fb.control<string | null>('', { validators: [Validators.required] }),
    instId: this.fb.control<number | null>(null, { validators: [Validators.required] })
  });

  constructor(
    private fb: FormBuilder,
    private conversationParticipantsNomService: ConversationParticipantsNomService,
    private conversationsService: ConversationsService,
    private route: ActivatedRoute,
    private router: Router,
    private actionService: ActionService,
    private studentClassBooksService: StudentClassBooksService
  ) {}

  ngOnDestroy(): void {
    this.destroyed$.next();
    this.destroyed$.complete();
  }

  ngOnInit(): void {
    const project = this.route.snapshot.data['project'] as Project;

    if (project === Project.TeachersApp) {
      this.showCreateForm = true;
      const instId = tryParseInt(this.route.snapshot.paramMap.get('instId')) ?? throwParamError('instId');
      this.form.get('instId')?.setValue(instId);
      this.conversationParticipantsService = new NomServiceWithParams(this.conversationParticipantsNomService, () => ({
        instId: instId
      }));
    } else if (project === Project.StudentsApp) {
      const currentSchoolYear = this.getCurrentSchoolYear();
      this.isStudentApp = true;
      this.studentClassBooksService
        .getAllClassBooks({ schoolYear: currentSchoolYear })
        .toPromise()
        .then((r) => {
          if (r.length === 0) {
            this.showNoInstFoundError = true;
            return;
          }

          const uniqueInstitutions = Array.from(
            r
              .filter((c) => !c.isTransferred && c.instId && c.instName)
              .reduce((map, c) => {
                if (!map.has(c.instId)) {
                  map.set(c.instId, { id: c.instId, name: c.instName });
                }
                return map;
              }, new Map<number, { id: number; name: string }>())
              .values()
          );

          if (uniqueInstitutions.length === 1) {
            this.form.get('instId')?.setValue(uniqueInstitutions[0].id);
          }

          uniqueInstitutions.forEach((inst) => {
            this.instIds.push({
              id: inst.id,
              name: inst.name
            });
          });
        });

      this.form
        .get('instId')
        ?.valueChanges.pipe(
          tap((selectedValue: number | null) => {
            if (selectedValue != null) {
              this.conversationParticipantsService = new NomServiceWithParams(
                this.conversationParticipantsNomService,
                () => ({
                  instId: selectedValue
                })
              );

              this.showCreateForm = true;
            }
          }),
          takeUntil(this.destroyed$)
        )
        .subscribe();
    }
  }

  shouldPreventLeave() {
    return this.form.dirty;
  }

  onSave() {
    if (this.form.invalid || this.saving) {
      return;
    }
    this.saving = true;
    const value = this.form.value;
    const conversation = {
      title: value.title!,
      message: value.message!,
      isLocked: value.isLocked!,
      participants:
        value.participants?.map((p: any) => ({
          instId: value.instId!,
          sysUserId: p.sysUserId!,
          title: p.title!,
          classBookId: p.classBookId!,
          participantType: p.participantType!
        })) || []
    };
    this.actionService
      .execute({
        httpAction: () => {
          return this.conversationsService
            .createConversation({
              instId: value.instId!,
              createConversationCommand: conversation
            })
            .toPromise()
            .then((conversation) => {
              this.form.markAsPristine();
              this.router.navigate(['../', conversation.schoolYear, conversation.conversationId], {
                relativeTo: this.route
              });
            });
        }
      })
      .finally(() => (this.saving = false));
  }

  stopPropagation(e: Event) {
    e.stopPropagation();
  }

  getCurrentSchoolYear() {
    const date = new Date();
    return date.getMonth() < 9 ? date.getFullYear() - 1 : date.getFullYear();
  }
}
