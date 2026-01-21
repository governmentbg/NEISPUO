import { HttpClient } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable } from "rxjs";
import { tap } from "rxjs/operators";
import { ErrorMessages, Messages, ModalQuestions, SuccessMessages } from "../models/messages.interface";

@Injectable()
export class MessagesService {
  errorMessages: ErrorMessages;
  successMessages: SuccessMessages;
  modalQuestions: ModalQuestions;

  constructor(private http: HttpClient) {}

  getMessages(): Observable<any> {
    return this.http.get(`assets/messages/messages.json`).pipe(
      tap((res: Messages) => {
        this.errorMessages = res.errorMessages;
        this.successMessages = res.successMesages;
        this.modalQuestions = res.modalQuestions;
      })
    );
  }
}
