export interface Messages {
  successMesages: SuccessMessages;
  errorMessages: ErrorMessages;
  modalQuestions: ModalQuestions;
}

export interface SuccessMessages {
  saveSuccess: string;
}

export interface ErrorMessages {
  error: string;
  noSuchData: string;
}

export interface ModalQuestions {
  unsavedChanges: string;
  restoreChanges: string;
}
