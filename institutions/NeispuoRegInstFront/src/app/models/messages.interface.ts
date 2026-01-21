export interface Messages {
  successMesages: SuccessMessages;
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
  certificateError: string;
  regixError: string;
  regixNotFound: string;
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
  certificateUploadErr: string;
}
