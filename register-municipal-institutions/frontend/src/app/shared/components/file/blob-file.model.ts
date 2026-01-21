export interface BlobContent {
  Size: string;
}

export interface BlobFile {
  BlobId: number;
  FileName: string;
  BlobContent: BlobContent;
}
