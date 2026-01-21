export interface Messages {
  successMessages: SuccessMessages;
  errorMessages: ErrorMessages;
  modalQuestions: ModalQuestions;
}

export interface SuccessMessages {
  saveSuccess: string;
}

export interface ErrorMessages {
  error: string;
  noSuchData: string;
  dataError: string;
  invalidInput: string;
}

export interface ModalQuestions {
  unsavedChanges: string;
  restoreChanges: string;
  deleteRecord: string;
}
