/* eslint-disable prettier/prettier */
import { Injectable, Logger } from '@nestjs/common';
import axios from 'axios';
import * as https from 'https';
import { TelelinkCheckStatusResponseEnum } from 'src/common/constants/enum/telelink-check-status-response.enum';
import { TelelinkCreateEventResponseEnum } from 'src/common/constants/enum/telelink-create-event-response.enum';
import { TelelinkStatusCodeEnum } from 'src/common/constants/enum/telelink-status-code.enum';
import { TelelinkCheckEventResponseDto } from 'src/common/dto/responses/telelink-check-event-response.dto';
import { TelelinkCreateEventResponseDto } from 'src/common/dto/responses/telelink-create-event-response.dto';
import { EventDto } from 'src/common/dto/telelink/event-dto';
import { LogDtoFactory } from 'src/common/factories/log-dto.factory';
import { BearerTokenService } from 'src/models/bearer-token/routing/bearer-token.service';

@Injectable()
export class TelelinkService {
    private logger = new Logger(TelelinkService.name);

    private url: string = process.env.TELELINK_API_URL;

    httpsAgent = new https.Agent({ rejectUnauthorized: false });

    private httpOptions = {
        headers: {
            'Content-Type': 'application/json',
            Authorization: '',
        },
        httpsAgent: this.httpsAgent,
    };

    constructor(private tokenBearerService: BearerTokenService) {}

    private async callCheckEventEndpoint(id: string) {
        this.httpOptions.headers.Authorization = `Bearer ${await this.tokenBearerService.getTelelinkBearerAccessToken()}`;
        const a = await axios.get(`${this.url}/${id}`, this.httpOptions);
        return a;
    }

    private async callCreateEventEndpoint(eventDto: EventDto) {
        const token = await this.tokenBearerService.getTelelinkBearerAccessToken();
        this.httpOptions.headers.Authorization = `Bearer ${token}`;
        const a = await axios.post(this.url, eventDto, this.httpOptions);
        return a;
    }

    async createEvents(eventDtos: EventDto[]) {
        this.httpOptions.headers.Authorization = `Bearer ${await this.tokenBearerService.getTelelinkBearerAccessToken()}`;
        const a = await axios.post(`${this.url}/batch`, eventDtos, this.httpOptions);
        return a.data;
    }

    getTelelinkCheckStatus(response) {
        //this extracts all statuses and their number of occurances
        //this is done because sometimes we recieve a response with more than one action and have to check if all actions are ready in order to mark the event as successful :))
        const actionsTriggered = response.data.actionsTriggered;
        const statusOccurrences = this.getStatusOccurences(actionsTriggered);

        if (Object.keys(statusOccurrences).length > 0) {
            if (statusOccurrences[TelelinkCheckStatusResponseEnum.DONE] === actionsTriggered.length) {
                return TelelinkCheckStatusResponseEnum.DONE;
            } else if (statusOccurrences[TelelinkCheckStatusResponseEnum.FAILED] > 0) {
                return TelelinkCheckStatusResponseEnum.FAILED;
            } else if (statusOccurrences[TelelinkCheckStatusResponseEnum.DLQ] > 0) {
                return TelelinkCheckStatusResponseEnum.DLQ;
            } else if (statusOccurrences[TelelinkCheckStatusResponseEnum.IN_PROGRESS] > 0) {
                return TelelinkCheckStatusResponseEnum.IN_PROGRESS;
            } else {
                return TelelinkCheckStatusResponseEnum.FAILED;
            }
        }
        return TelelinkCheckStatusResponseEnum.IN_PROGRESS;
    }

    getTelelinkStatusCode(response) {
        //this extracts all statuses and their number of occurances
        //this is done because sometimes we recieve a response with more than one action and have to check if all actions are ready in order to mark the event as successful :))
        const actionsTriggered = response.data.actionsTriggered;
        const statusCodeOccurrences = this.getStatusCodeOccurences(actionsTriggered);

        if (Object.keys(statusCodeOccurrences).length > 0) {
            if (
                statusCodeOccurrences[TelelinkStatusCodeEnum[TelelinkStatusCodeEnum.DONE]] === actionsTriggered.length
            ) {
                return TelelinkStatusCodeEnum.DONE;
            } else if (statusCodeOccurrences[TelelinkStatusCodeEnum[TelelinkStatusCodeEnum.FAILURE]] > 0) {
                return TelelinkStatusCodeEnum.FAILURE;
            } else if (statusCodeOccurrences[TelelinkStatusCodeEnum[TelelinkStatusCodeEnum.SCHOOL_NOT_FOUND]] > 0) {
                return TelelinkStatusCodeEnum.SCHOOL_NOT_FOUND;
            } else if (statusCodeOccurrences[TelelinkStatusCodeEnum[TelelinkStatusCodeEnum.CLASS_NOT_FOUND]] > 0) {
                return TelelinkStatusCodeEnum.CLASS_NOT_FOUND;
            } else if (statusCodeOccurrences[TelelinkStatusCodeEnum[TelelinkStatusCodeEnum.USER_NOT_FOUND]] > 0) {
                return TelelinkStatusCodeEnum.USER_NOT_FOUND;
            } else if (statusCodeOccurrences[TelelinkStatusCodeEnum[TelelinkStatusCodeEnum.USER_EXISTS]] > 0) {
                return TelelinkStatusCodeEnum.USER_EXISTS;
            } else if (statusCodeOccurrences[TelelinkStatusCodeEnum[TelelinkStatusCodeEnum.DEFAULT]] > 0) {
                return TelelinkStatusCodeEnum.DEFAULT;
            } else {
                return TelelinkStatusCodeEnum.FAILURE;
            }
        }
        return TelelinkStatusCodeEnum.DEFAULT;
    }

    getTelelinkCreateStatus(response) {
        if (response.data.attributes) {
            return TelelinkCreateEventResponseEnum.CREATED;
        }
        return TelelinkCreateEventResponseEnum.NOT_CREATED;
    }

    async checkEvent(guid) {
        const result = new TelelinkCheckEventResponseDto();
        await this.callCheckEventEndpoint(guid)
            .then(
                async (response) => {
                    result.response = response;
                    result.status = this.getTelelinkCheckStatus(response);
                    result.statusCode = this.getTelelinkStatusCode(response);
                },
                async (error) => {
                    result.response = error;
                    if (error?.response?.status === 404) {
                        result.status = TelelinkCheckStatusResponseEnum.NOT_FOUND;
                    } else if (error?.response?.status === 503) {
                        result.status = TelelinkCheckStatusResponseEnum.UNAVAILABLE;
                    } else {
                        result.status = TelelinkCheckStatusResponseEnum.OTHER;
                    }
                    this.logger.error(error);
                    this.logger.error(`${this.url}/${guid}`);
                },
            )
            .catch((error) => {
                result.response = error;
                result.status = TelelinkCheckStatusResponseEnum.EXCEPTION;
                this.logger.error(error);
                this.logger.error(`${this.url}/${guid}`);
            });
        return result;
    }

    async createEvent(eventDto) {
        const result = new TelelinkCreateEventResponseDto();
        await this.callCreateEventEndpoint(eventDto)
            .then(
                async (response) => {
                    result.response = response;
                    result.status = this.getTelelinkCreateStatus(response);
                },
                async (error) => {
                    result.response = error.response;
                    if (error?.response?.status === 400) {
                        result.status = TelelinkCreateEventResponseEnum.VALIDATION_ERROR;
                    } else {
                        result.status = TelelinkCreateEventResponseEnum.ERROR;
                    }
                    this.logger.error(error);
                    const dto = LogDtoFactory.createFromAzureEventObject(eventDto);
                    this.logger.error(dto);
                },
            )
            .catch((error) => {
                result.response = error?.response;
                result.status = TelelinkCreateEventResponseEnum.EXCEPTION;
                this.logger.error(error);
                this.logger.error(eventDto);
            });
        return result;
    }

    getValidationErrors(azureErrorResponse: any) {
        // returns an object containing all validation errros concatenated in a string
        return `Validation Errors: [ ${Object.values(azureErrorResponse.data.errors)} ]`;
    }

    getStatusOccurences(actionsTriggeredArray: any[]) {
        // returns an object containing all statuses in the azure response and their counts
        return actionsTriggeredArray.reduce(function (result, action) {
            return result[action.status] ? ++result[action.status] : (result[action.status] = 1), result;
        }, {});
    }

    getStatusCodeOccurences(actionsTriggeredArray: any[]) {
        // returns an object containing all statuses in the azure response and their counts
        return actionsTriggeredArray.reduce(function (result, action) {
            const statusCode = TelelinkStatusCodeEnum[action.statusCode];
            return result[statusCode] ? ++result[statusCode] : (result[statusCode] = 1), result;
        }, {});
    }

    getDLQErrors(response: any) {
        let dlqMessages = [];
        dlqMessages = response.data.actionsTriggered.reduce(function (result, action) {
            if (action.status === TelelinkCheckStatusResponseEnum.DLQ) {
                result.push(action.statusMessage);
            }
            return result;
        }, []);
        if (!dlqMessages || dlqMessages.length === 0) return `DLQ Errors: [ "UNABLE TO PARSE DLQ ERROR MESSAGES" ]`;
        return `DLQ Errors: [ ${dlqMessages.toString()} ]`;
    }
}
