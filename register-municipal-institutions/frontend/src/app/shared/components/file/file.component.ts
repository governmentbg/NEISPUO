import { HttpClient } from '@angular/common/http';
import {
  Component, Input, OnInit, ViewChild,
} from '@angular/core';
import { ControlContainer, FormControl } from '@angular/forms';
import { EnvironmentService } from '@core/services/environment.service';
import { MessageService } from 'primeng/api';
import { FileUpload } from 'primeng/fileupload';
import { concatMap, filter } from 'rxjs/operators';
import { SubSink } from 'subsink';
import { BlobFile } from './blob-file.model';

interface BlobServerUploadResponse {
  name: string;
  size: number;
  blobId: number;
  location: string;
}

type FileMetadata = Omit<BlobServerUploadResponse, 'location'>;

@Component({
  selector: 'app-file',
  templateUrl: './file.component.html',
  styleUrls: ['./file.component.scss'],
})
export class FileComponent implements OnInit {
  @ViewChild('pFileUpload') pFileUpload: FileUpload;

  @Input() controlName: string;

  @Input() loadMetadataErrorMessage = 'Възникна грешка при зареждане на данните за заповедта';

  @Input() uploadFileErrorMessage = 'Възникна грешка при запазване на вашят файл';

  @Input() downloadFileErrorMessage = 'Възникна грешка при зареждане на вашят файл';

  @Input() sendFilleSuccessMessage = 'Успешно прикачихте файла';

  @Input() isView: boolean;

  formControl: FormControl;

  blobServerUrl = this.envService.environment.BLOB_SERVER_URL;

  private baseEndpoint = this.envService.environment.BACKEND_URL;

  uploadInProgressErrorKey = 'uploadInProgress';

  fileMetadata: FileMetadata;

  subSink = new SubSink();

  constructor(
    private messageService: MessageService,
    private cc: ControlContainer,
    private httpClient: HttpClient,
    private envService: EnvironmentService,
  ) { }

  ngOnInit(): void {
    this.formControl = this.cc.control.get(this.controlName) as FormControl;
    this.setupFileMetadataLoading();
  }

  private setupFileMetadataLoading() {
    this.subSink.sink = this.formControl.valueChanges
      .pipe(
        filter((v) => !!v),
        concatMap((blobId: number) => this.httpClient.get<BlobFile>(`${this.baseEndpoint}/v1/blob/meta/${blobId}`)),
      )
      .subscribe(
        (s) => {
          const transformedResponse: FileMetadata = { blobId: s.BlobId, name: s.FileName, size: +s.BlobContent.Size };
          this.fileMetadata = transformedResponse;
        },
        (e) => {
          this.messageService.add({
            summary: 'Грешка',
            detail: this.loadMetadataErrorMessage,
            severity: 'error',
          });
        },
      );
  }

  private addUploadInProgressFlag() {
    const previousErrors = this.formControl.errors || {};
    this.formControl.setErrors({ ...previousErrors, [this.uploadInProgressErrorKey]: true });
  }

  private removeUploadInProgressFlag() {
    const errors = { ...(this.formControl.errors || {}) };
    delete errors[this.uploadInProgressErrorKey];
    const noErrorsLeft = Object.keys(errors).length === 0;

    if (noErrorsLeft) {
      this.formControl.setErrors(null);
    } else {
      this.formControl.setErrors(errors);
    }
  }

  onBeforeUpload() {
    this.addUploadInProgressFlag();
  }

  onUpload(event: { formData: FormData; originalEvent: { body: BlobServerUploadResponse } }) {
    const { blobId } = event.originalEvent.body;
    this.formControl.setValue(blobId);
    this.removeUploadInProgressFlag();

    this.httpClient.post(`${this.baseEndpoint}/v1/blob-temp/create/`, { blobId }).subscribe(
      (s) => {
        this.messageService.add({
          summary: 'Успех',
          detail: this.sendFilleSuccessMessage,
          severity: 'success',
        });
      },
    );
  }

  onError() {
    this.messageService.add({
      summary: 'Грешка',
      detail: this.uploadFileErrorMessage,
      severity: 'error',
    });
    this.removeUploadInProgressFlag();
  }

  removeFile() {
    this.pFileUpload.clear();
    this.pFileUpload.uploadedFileCount = 0;
    this.fileMetadata = null;
    this.formControl.setValue(null);
  }

  downloadFile() {
    const blobId = this.fileMetadata?.blobId;
    if (!blobId) {
      return;
    }

    this.httpClient.get<{ location: string }>(`${this.baseEndpoint}/v1/blob/location/${blobId}`).subscribe(
      (s) => {
        window.open(s.location);
      },
      (e) => {
        this.messageService.add({
          summary: 'Грешка',
          detail: this.downloadFileErrorMessage,
          severity: 'error',
        });
      },
    );
  }
}
