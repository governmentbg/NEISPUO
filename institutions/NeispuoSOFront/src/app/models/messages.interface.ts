export interface Messages {
  successMessages: SuccessMessages;
  errorMessages: ErrorMessages;
  modalQuestions: ModalQuestions;
  fileMessages: FileMessages;
}

export interface SuccessMessages {
  saveSuccess: string;
}

export interface ErrorMessages {
  error: string;
  noSuchData: string;
  dataError: string;
  invalidInput: string;
  regixDown: string;
}

export interface ModalQuestions {
  unsavedChanges: string;
  restoreChanges: string;
  deleteRecord: string;
}

export interface FileMessages {
  fileSize: string;
  fileType: string;
  fileNumber: string;
  fileAccepted: string;
  fileDragDropText: string;
  fileUploadErr: string;
}
