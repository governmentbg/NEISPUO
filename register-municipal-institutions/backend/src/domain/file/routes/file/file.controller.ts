import {
    Controller,
    UseGuards,
    Post,
    UseInterceptors,
    UploadedFiles,
    Get,
    Param,
    Res,
    Req,
    NotFoundException,
    InternalServerErrorException,
    Logger,
    Inject,
    Delete,
    Query
} from '@nestjs/common';
import { FilesInterceptor } from '@nestjs/platform-express';
import { FileService } from './file.service';
import { FileGuard } from './file.guard';
import { Response } from 'express';
import * as path from 'path';
import * as fs from 'fs';
const fsPromise = fs.promises;
import { Connection } from 'typeorm';
import { AuthedRequest } from '../../../../shared/middleware/auth.middleware';
import { File } from '../../file.entity';
import { MaxFileUploadInterceptor } from '../../interceptors/max-file-upload.interceptor';
import { HashesService } from '../../../../shared/services/hashes/hashes.service';

@UseGuards(FileGuard)
@Controller('v1/file')
export class FileController {
    constructor(
        private hashesService: HashesService,
        private connection: Connection,
        @Inject('winston') private readonly logger: Logger
    ) {}

    // @Post('upload')
    // @UseInterceptors(MaxFileUploadInterceptor)
    // @UseInterceptors(
    //     FilesInterceptor('files', Number.POSITIVE_INFINITY, { dest: process.env.FILE_STORAGE })
    // )
    // async uploadFile(
    //     @Req() req: AuthedRequest,
    //     @UploadedFiles() multerFiles: Express.Multer.File[]
    // ) {
    //     const createdFiles = await this.connection.transaction(async transactionManager => {
    //         const fileRepo = transactionManager.getRepository(File);

    //         const filesToSave: File[] = [];
    //         for (const multerFile of multerFiles) {
    //             const fileRecord: File = {
    //                 // user: req._authObject.user,
    //                 fileSize: multerFile.size,
    //                 name: multerFile.originalname,
    //                 fsName: multerFile.filename,
    //                 mimeType: multerFile.mimetype,
    //                 contentMd5: await this.hashesService.md5File(multerFile.path),
    //                 multerMetadata: multerFile
    //             };

    //             filesToSave.push(fileRepo.create(fileRecord));
    //         }

    //         return fileRepo.save(filesToSave);
    //     });

    //     return createdFiles;
    // }

    // @Get('download/:fileId')
    // async downloadFile(
    //     @Param('fileId') fileId: string,
    //     @Res() response: Response,
    //     @Query('inBrowser') inBrowser: string
    // ) {
    //     // TODO: Implement recaptcha to avoid crawlers
    //     // TODO: bypass recaptcha if authenticated?
    //     // TODO: Determine UI (download JS) mechanism for saving with filename.

    //     // Get file from db
    //     const fileRepo = this.connection.getRepository(File);
    //     const dbFile = await fileRepo.findOne({ id: fileId });

    //     // Check filesystem
    //     const filePath = path.resolve(process.env.FILE_STORAGE, dbFile.fsName);
    //     try {
    //         await fsPromise.access(filePath, fs.constants.F_OK);
    //     } catch (e) {
    //         this.logger.error(`File with uuid ${fileId} was not found in filesystem`); // TODO create exception filter and move this there
    //         throw new InternalServerErrorException(
    //             `Could not find file with uuid ${fileId} in fs!`
    //         );
    //     }

    //     // return stream
    //     const stream = fs.createReadStream(filePath);
    //     if (inBrowser !== 'true') {
    //         // -> default sends as attachment
    //         response.set({
    //             'Content-Type': dbFile.mimeType,
    //             'Content-Disposition': `attachment; filename*=UTF-8''${encodeURI(dbFile.name)}`
    //         });
    //     }
    //     stream.pipe(response);
    // }

    // @Delete(':fileId')
    // async deleteFile(
    //     @Param('fileId') fileId: string,
    //     @Res() response: Response,
    //     @Req() req: AuthedRequest
    // ) {
    //     const fileRepo = this.connection.getRepository(File);
    //     const file = await fileRepo.findOne({ id: fileId });
    //     const filePath = path.resolve(process.env.FILE_STORAGE, file.fsName);
    //     await fileRepo.delete({ id: fileId });
    //     await fsPromise.unlink(filePath);
    //     this.logger.warn(`File with uuid ${fileId} was deleted by ${req._authObject?.user?.SysUserID}`);

    //     response.status(204).send();
    // }
}
