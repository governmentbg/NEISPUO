import { Injectable, NotFoundException } from '@nestjs/common';
import { InjectRepository } from '@nestjs/typeorm';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { UserGuide } from '../user-guides.entity';
import { Connection } from 'typeorm';
import { DateToUTCTransformService } from 'src/shared/services/date-to-utc-transform.service';
import { AuditModuleEnum } from 'src/shared/enums/audit-module.enum';
import { AuthedRequest } from 'src/shared/dto/authed-request.interface';
import { UserGuideDTO } from 'src/shared/dto/user-guide.dto';
import { AuditEntity } from 'src/entities/audit.entity';
import { auditAction } from 'src/shared/enums/audit-action.const';
import { UserGuideModificationDTO } from 'src/shared/dto/user-guide-modifiactions.dto';

@Injectable()
export class UserGuidesService extends TypeOrmCrudService<UserGuide> {
  constructor(
    @InjectRepository(UserGuide) repo,
    private connection: Connection,
  ) {
    super(repo);
  }

  getUserGuideByID(id: number) {
    return this.repo.findOne(id);
  }

  async createUserGuide(request: AuthedRequest, userGuideDTO: UserGuideDTO) {
    let createdUserGuide;
    await this.connection.transaction(async entityManager => {
      const userGuideRepo = entityManager.getRepository(UserGuide);

      const userGuide = new UserGuide();
      userGuide.name = userGuideDTO.name;
      userGuide.category = userGuideDTO.category as any;
      userGuide.filename = userGuideDTO.filename;
      userGuide.mimeType = userGuideDTO.mimeType;
      userGuide.fileContent = userGuideDTO.fileContent;
      userGuide.URLOverride = userGuideDTO.URLOverride;

      createdUserGuide = await userGuideRepo.save(userGuide);
      delete createdUserGuide.fileContent;
      await this.logUserGuidesModifications({
        request,
        userGuide: createdUserGuide,
        auditAction: auditAction.INSERT,
        entityManager,
      });
    });
    return createdUserGuide;
  }

  updateUserGuideByID(
    request: AuthedRequest,
    id: number,
    userGuideDTO: UserGuideDTO,
  ) {
    return this.connection.transaction(async entityManager => {
      const userGuideRepo = entityManager.getRepository(UserGuide);
      let existingUserGuide = await this.repo.findOne(id);

      if (!existingUserGuide) {
        return new NotFoundException(
          `User guide with id: ${id} was not found.`,
        );
      }

      const { filename, mimeType, fileContent } = this.handleFileUpdate(
        userGuideDTO,
        existingUserGuide,
      );

      const updatedUserGuide = await userGuideRepo.save({
        ...existingUserGuide,
        name: userGuideDTO.name,
        category: userGuideDTO.category as any,
        filename: filename,
        mimeType: mimeType,
        fileContent: fileContent,
        URLOverride:
          userGuideDTO.URLOverride !== undefined
            ? userGuideDTO.URLOverride
            : existingUserGuide.URLOverride,
      });

      await this.logUserGuidesModifications({
        request,
        userGuide: updatedUserGuide,
        auditAction: auditAction.UPDATE,
        entityManager,
      });
      return updatedUserGuide;
    });
  }

  deleteUserGuideByID(request, id: number) {
    return this.connection.transaction(async entityManager => {
      const userGuideRepo = entityManager.getRepository(UserGuide);
      const userGuide = await userGuideRepo.findOne({
        where: { id: id },
      });
      await this.logUserGuidesModifications({
        request,
        userGuide,
        auditAction: auditAction.DELETE,
        entityManager,
      });
      return userGuideRepo.delete(id);
    });
  }

  async retrieveFileFromDatabase(fileID: number) {
    const userGuide: UserGuide = await this.repo.findOne({
      where: { id: fileID },
    });

    if (!userGuide) {
      return null;
    }

    return userGuide.fileContent;
  }

  logUserGuidesModifications(
    userGuideModificationDTO: UserGuideModificationDTO,
  ) {
    const {
      request,
      userGuide,
      auditAction,
      entityManager,
    } = userGuideModificationDTO;
    const selectedRole = request.user?.selected_role;
    const { SysUserID, Username, PersonID, InstitutionID } = selectedRole;
    const { id, name, category } = userGuide;

    const auditRepo = entityManager.getRepository(AuditEntity);
    return auditRepo.insert([
      {
        SysUserId: SysUserID,
        Username: Username,
        PersonId: PersonID,
        DateUtc: DateToUTCTransformService.transform(new Date()),
        Action: auditAction,
        InstId: InstitutionID,
        AuditModuleId: AuditModuleEnum.MAIN_PORTAL,
        LoginSessionId: request.user.sessionID,
        RemoteIpAddress: request.ip || 'localohost',
        UserAgent: request.get('User-Agent'),
        ObjectName: 'UserGuide',
        ObjectId: id,
        Data: {
          UserGuideID: id,
          name: name,
          category: category,
        },
      },
    ]);
  }

  private handleFileUpdate(
    userGuideDTO: UserGuideDTO,
    existingUserGuide: UserGuide,
  ) {
    let filename = existingUserGuide.filename;
    let mimeType = existingUserGuide.mimeType;
    let fileContent = existingUserGuide.fileContent;

    if (userGuideDTO.filename === '__REMOVE_FILE__') {
      // remove file
      filename = '';
      mimeType = '';
      fileContent = null;
    } else if (userGuideDTO.filename) {
      // new file upload
      filename = userGuideDTO.filename;
      mimeType = userGuideDTO.mimeType;
      fileContent = userGuideDTO.fileContent;
    }

    return { filename, mimeType, fileContent };
  }
}
