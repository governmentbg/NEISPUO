import { Injectable } from '@nestjs/common';
import { CONSTANTS } from 'src/common/constants/constants';
import { TelelinkCheckStatusResponseEnum } from 'src/common/constants/enum/telelink-check-status-response.enum';
import { TelelinkCreateEventResponseEnum } from 'src/common/constants/enum/telelink-create-event-response.enum';
import { WorkflowType } from 'src/common/constants/enum/workflow-type.enum';
import { AzureUsersResponseDTO } from 'src/common/dto/responses/azure-users-response.dto';
import { TelelinkService } from 'src/models/telelink/routing/telelink.service';

@Injectable()
export class UsersErrorService {
    constructor(private telelinkService: TelelinkService) {}

    generateCreationErrorMessage(dto: AzureUsersResponseDTO, status: TelelinkCreateEventResponseEnum): string {
        const { telelinkResponseDto } = dto;
        switch (status) {
            case TelelinkCreateEventResponseEnum.CREATED:
                return `Error in await this.azureUsersRepository.processCreate(dto)`;
            case TelelinkCreateEventResponseEnum.NOT_CREATED:
                return telelinkResponseDto?.response?.data ?? telelinkResponseDto?.response;
            case TelelinkCreateEventResponseEnum.VALIDATION_ERROR:
                return this.telelinkService.getValidationErrors(telelinkResponseDto.response);
            case TelelinkCreateEventResponseEnum.EXCEPTION:
                return JSON.stringify(telelinkResponseDto?.response);
            case TelelinkCreateEventResponseEnum.ERROR:
                delete telelinkResponseDto?.response?.url;
                delete telelinkResponseDto?.response?.config;
                delete telelinkResponseDto?.response?.headers;
                return JSON.stringify(telelinkResponseDto);
            default:
                return '';
        }
    }

    generateCheckErrorMessage(dto: AzureUsersResponseDTO, status: TelelinkCheckStatusResponseEnum): string {
        const { telelinkResponseDto } = dto;
        switch (status) {
            case TelelinkCheckStatusResponseEnum.DLQ:
                return this.telelinkService.getDLQErrors(telelinkResponseDto.response);
            case TelelinkCheckStatusResponseEnum.FAILED:
                delete telelinkResponseDto?.response?.request;
                delete telelinkResponseDto?.response?.config;
                delete telelinkResponseDto?.response?.headers;
                return telelinkResponseDto?.response?.data;
            case TelelinkCheckStatusResponseEnum.NOT_FOUND:
                return CONSTANTS.DB_ERROR_MESSAGE_NOT_FOUND;
            case TelelinkCheckStatusResponseEnum.OTHER:
                delete telelinkResponseDto?.response?.url;
                delete telelinkResponseDto?.response?.config;
                delete telelinkResponseDto?.response?.headers;
                return telelinkResponseDto?.response;
            case TelelinkCheckStatusResponseEnum.EXCEPTION:
                return telelinkResponseDto?.response;
            default:
                return '';
        }
    }

    generateSyncErrorMessage(dto: AzureUsersResponseDTO, status: WorkflowType) {
        const { telelinkResponseDto } = dto;
        //if in the future there is a need for a custom handling of a sync message. place it here
    }

    async handleFailedCreationErrorMessage(dtos: AzureUsersResponseDTO[], status: TelelinkCreateEventResponseEnum) {
        for (const dto of dtos) {
            const errorMessage = this.generateCreationErrorMessage(dto, status);
            dto.errorMessage = JSON.stringify(errorMessage);
            dto.errorMessage = `${status}: ${dto.errorMessage}`;
            dto.errorMessage = dto.errorMessage.substring(0, CONSTANTS.DB_ERROR_MESSAGE_EXCEPTION_LENGTH_MAX);
            dto.errorMessage = dto.errorMessage.replace(/'/g, `"`);
        }
    }

    async handleFailedCheckErrorMessage(dtos: AzureUsersResponseDTO[], status: TelelinkCheckStatusResponseEnum) {
        for (const dto of dtos) {
            const errorMessage = this.generateCheckErrorMessage(dto, status);
            dto.errorMessage = JSON.stringify(errorMessage);
            dto.errorMessage = `${status}: ${dto.errorMessage}`;
            dto.errorMessage = dto.errorMessage.substring(0, CONSTANTS.DB_ERROR_MESSAGE_EXCEPTION_LENGTH_MAX);
            dto.errorMessage = dto.errorMessage.replace(/'/g, `"`);
        }
    }

    async handleFailedSyncErrorMessage(dtos: AzureUsersResponseDTO[], status: WorkflowType) {
        for (const dto of dtos) {
            const errorMessage = this.generateSyncErrorMessage(dto, status);
            dto.errorMessage = dto.errorMessage.substring(
                0,
                CONSTANTS.DB_ERROR_MESSAGE_FAILED_SYNCRONIZATION_LENGTH_MAX,
            );
            dto.errorMessage = dto.errorMessage.replace(/'/g, `"`);
        }
    }
}
