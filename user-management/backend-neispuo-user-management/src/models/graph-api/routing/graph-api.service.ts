import { Injectable } from '@nestjs/common';
import axios from 'axios';
import * as https from 'https';
import { GraphApiResponseEnum } from 'src/common/constants/enum/graph-api-response.enum';
import { GraphApiUserTypeEnum } from 'src/common/constants/enum/graph-api-user-type';
import { GraphApiClassResponseDTO } from 'src/common/dto/graph-api-class-response.dto';
import { GraphApiResponseDTO } from 'src/common/dto/graph-api-response.dto';
import { GraphApiSchoolResponseDTO } from 'src/common/dto/graph-api-school-response.dto';
import { GraphApiGetClassResponseDto } from 'src/common/dto/responses/graph-api-get-class-response.dto';
import { GraphApiGetSchoolResponseDto } from 'src/common/dto/responses/graph-api-get-school-response.dto';
import { GraphApiGetUserClassEnrollmentsResponseDto } from 'src/common/dto/responses/graph-api-get-user-class-enrollments-response.dto';
import { GraphApiGetUserResponseDto } from 'src/common/dto/responses/graph-api-get-user-response.dto';
import { GraphApiGetUserSchoolEnrollmentsResponseDto } from 'src/common/dto/responses/graph-api-get-user-school-enrollments-response.dto';
import { StripZeroesService } from 'src/common/services/strip-zeroes/strip-zeroes.service';
import { WithExtras } from 'src/common/types/with-extras.type';
import { BearerTokenService } from 'src/models/bearer-token/routing/bearer-token.service';
import { GetOrganizationInfoRequestDto } from '../dtos/requests/get-organization-info-request.dto';
import { GetParentInfoRequestDto } from '../dtos/requests/get-parent-info.dto-request';
import { GetUserInfoRequestDto } from '../dtos/requests/get-user-info.dto-request';
import { OrganizationInfoResponseDto } from '../dtos/responses/organization-info-response.dto';
import { ParentInfoResponseDto } from '../dtos/responses/parent-info-response.dto';
import { UserInfoResponseDto } from '../dtos/responses/user-info-response.dto';

@Injectable()
export class GraphApiService {
    private url: string = process.env.GRAPH_API_URI;

    httpsAgent = new https.Agent({ rejectUnauthorized: false });

    private httpOptions = {
        headers: {
            'Content-Type': 'application/json',
            Authorization: '',
        },
        httpsAgent: this.httpsAgent,
    };

    constructor(private tokenBearerService: BearerTokenService) {}

    private buildGetUserInfoByPersonalIDEndpointUrl(personalID: string) {
        return `${this.url}education/users?$filter=student/externalId eq '${personalID}' OR teacher/externalId eq '${personalID}'`;
    }

    private buildGetStudentInfoByPersonalIDEndpointUrl(personalID: string) {
        return `${this.url}education/users?$filter=student/externalId eq '${personalID}'`;
    }

    private buildGetTeacherInfoByPersonalIDEndpointUrl(personalID: string) {
        return `${this.url}education/users?$filter=teacher/externalId eq '${personalID}'`;
    }

    private buildGetUserSchoolEnrollmentInfoByUsernameEndpointUrl(username: string) {
        return `${this.url}education/users/${username}/schools`;
    }

    private buildGetUserClassEnrollmentInfoByUsernameEndpointUrl(username: string) {
        return `${this.url}education/users/${username}/classes`;
    }

    private buildGetExternalUsersInfoByUsernameEndpointURL(username: string) {
        return `${this.url}users/${username}`;
    }

    private buildGetSchoolInfoBySchoolNumberEndpointUrl(schoolID: number) {
        return `${this.url}education/schools?$filter=schoolNumber eq '${schoolID}'`;
    }

    private buildGetParentInfoByEmailEndpointUrl(email: string) {
        return `${this.url}users?$filter=mail eq '${email}'`;
    }

    private buildGetClassInfoByAzureIDEndpointUrl(azureID: string) {
        return `${this.url}education/classes/${azureID}`;
    }

    private buildGetClassInfoByClassIDEndpointUrl(curriculumID: string) {
        return `${this.url}education/classes?$filter=externalId eq '${curriculumID}'`;
    }

    private buildGetUseInfoByAzureIdEndpointUrl(azureID: string) {
        return `${this.url}education/users/${azureID}`;
    }

    private buildGetUseInfoByPublicEduNumberEndpointUrl(publicEduNumber: string) {
        return `${this.url}education/users?$filter=mailNickname eq '${publicEduNumber}'`;
    }

    private async callGetUserInfoByPersonalIDEndpoint(personalID: string) {
        this.httpOptions.headers.Authorization = `Bearer ${await this.tokenBearerService.getGraphApiBearerAccessToken()}`;
        return axios.get(this.buildGetUserInfoByPersonalIDEndpointUrl(personalID), this.httpOptions);
    }

    private async callGetStudentInfoByPersonalIDEndpoint(personalID: string) {
        this.httpOptions.headers.Authorization = `Bearer ${await this.tokenBearerService.getGraphApiBearerAccessToken()}`;
        return axios.get(this.buildGetStudentInfoByPersonalIDEndpointUrl(personalID), this.httpOptions);
    }

    private async callGetTeacherInfoByPersonalIDEndpoint(personalID: string) {
        this.httpOptions.headers.Authorization = `Bearer ${await this.tokenBearerService.getGraphApiBearerAccessToken()}`;
        return axios.get(this.buildGetTeacherInfoByPersonalIDEndpointUrl(personalID), this.httpOptions);
    }

    private async callGetUserInfoByAzureIDEndpoint(azureID: string) {
        this.httpOptions.headers.Authorization = `Bearer ${await this.tokenBearerService.getGraphApiBearerAccessToken()}`;
        return axios.get(this.buildGetUseInfoByAzureIdEndpointUrl(azureID), this.httpOptions);
    }

    private async callGetUserInfoByPublicEduNumberEndpoint(publicEduNumber: string) {
        this.httpOptions.headers.Authorization = `Bearer ${await this.tokenBearerService.getGraphApiBearerAccessToken()}`;
        return axios.get(this.buildGetUseInfoByPublicEduNumberEndpointUrl(publicEduNumber), this.httpOptions);
    }

    async callGetUserSchoolEnrollmentInfoByUsernameEndpoint(username: string) {
        this.httpOptions.headers.Authorization = `Bearer ${await this.tokenBearerService.getGraphApiBearerAccessToken()}`;
        return axios.get(this.buildGetUserSchoolEnrollmentInfoByUsernameEndpointUrl(username), this.httpOptions);
    }

    async callGetExternalUsersInfoByUsernameEndpoint(username: string) {
        this.httpOptions.headers.Authorization = `Bearer ${await this.tokenBearerService.getGraphApiBearerAccessToken()}`;
        return axios.get(this.buildGetExternalUsersInfoByUsernameEndpointURL(username), this.httpOptions);
    }

    private async callGetUserClassEnrollmentInfoByUsernameEndpoint(username: string) {
        this.httpOptions.headers.Authorization = `Bearer ${await this.tokenBearerService.getGraphApiBearerAccessToken()}`;
        return axios.get(this.buildGetUserClassEnrollmentInfoByUsernameEndpointUrl(username), this.httpOptions);
    }

    private async callGetSchoolInfoBySchoolIDEndpoint(schoolID: number) {
        this.httpOptions.headers.Authorization = `Bearer ${await this.tokenBearerService.getGraphApiBearerAccessToken()}`;
        return axios.get(this.buildGetSchoolInfoBySchoolNumberEndpointUrl(schoolID), this.httpOptions);
    }

    private async callGetParentInfoByEmailEndpoint(email: string) {
        this.httpOptions.headers.Authorization = `Bearer ${await this.tokenBearerService.getParentGraphApiBearerAccessToken()}`;
        return axios.get(this.buildGetParentInfoByEmailEndpointUrl(email), this.httpOptions);
    }

    private async callGetClassInfoByCurriculumEndpoint(azureID: string) {
        this.httpOptions.headers.Authorization = `Bearer ${await this.tokenBearerService.getGraphApiBearerAccessToken()}`;
        return axios.get(this.buildGetClassInfoByAzureIDEndpointUrl(azureID), this.httpOptions);
    }

    private async callGetClassInfoByClassIDEndpoint(curriculumID: string) {
        this.httpOptions.headers.Authorization = `Bearer ${await this.tokenBearerService.getGraphApiBearerAccessToken()}`;
        return axios.get(this.buildGetClassInfoByClassIDEndpointUrl(curriculumID), this.httpOptions);
    }

    getStudentInfoByPersonalID(personalID: string) {
        return this.getUserInfoByPersonalID(personalID, GraphApiUserTypeEnum.STUDENT);
    }

    getTeacherInfoByPersonalID(personalID: string) {
        return this.getUserInfoByPersonalID(personalID, GraphApiUserTypeEnum.TEACHER);
    }

    async getUserInfoByPersonalID(personalID: string, userType: GraphApiUserTypeEnum) {
        let result;
        /*
            This is done because some personalIds in Azure that start with 0 are missing the initial zero at the beggining so we check first for the stripped personalID and then for the actualID
        */
        const strippedPersonalID = StripZeroesService.transform(personalID);
        try {
            result = await this.callUserInfoByPersonalIDEndpoint(strippedPersonalID, userType);
        } catch (e) {
            console.error(e);
            return null;
        }
        // if there is no graph api result dont update DB
        if (!result?.response) {
            try {
                result = await this.callUserInfoByPersonalIDEndpoint(personalID, userType);
            } catch (e) {
                console.error(e);
            }
            if (!result?.response) {
                return null;
            }
        }
        const { mailNickname, id, mail } = result.response;
        return { publicEduNumber: mailNickname, azureID: id, email: `${mailNickname}@edu.mon.bg` };
    }

    async getUserInfoByAzureID(azureID: string, returnAllProperties: true): Promise<GraphApiGetUserResponseDto>;
    async getUserInfoByAzureID(
        azureID: string,
        returnAllProperties?: false,
    ): Promise<{ publicEduNumber: string; azureID: string; email: string } | null>;
    async getUserInfoByAzureID(azureID: string, returnAllProperties = false) {
        if (returnAllProperties) {
            const result = new GraphApiGetUserResponseDto();
            try {
                const response = await this.callGetUserInfoByAzureIDEndpoint(azureID);
                if (!response?.data?.mailNickname) {
                    result.status = GraphApiResponseEnum.ERROR;
                } else {
                    result.status = GraphApiResponseEnum.SUCCESS;
                    result.response = response.data;
                }
            } catch (e) {
                console.error(e);
                result.status = GraphApiResponseEnum.EXCEPTION;
            } finally {
                return result;
            }
        } else {
            let result;
            try {
                result = await this.callGetUserInfoByAzureIDEndpoint(azureID);
            } catch (e) {
                return null;
            }
            if (!result?.data?.mailNickname) return null;
            const { mailNickname, id, mail } = result.data;
            return { publicEduNumber: mailNickname, azureID: id, email: mail };
        }
    }

    async getUserInfoByPublicEduNumber(
        publicEduNumber: string,
        returnAllProperties: true,
    ): Promise<GraphApiGetUserResponseDto>;
    async getUserInfoByPublicEduNumber(
        publicEduNumber: string,
        returnAllProperties?: false,
    ): Promise<{ publicEduNumber: string; azureID: string; email: string } | null>;
    async getUserInfoByPublicEduNumber(publicEduNumber: string, returnAllProperties = false) {
        if (returnAllProperties) {
            const result = new GraphApiGetUserResponseDto();
            try {
                const response = await this.callGetUserInfoByPublicEduNumberEndpoint(publicEduNumber);
                if (response?.data?.value.length === 0) {
                    result.status = GraphApiResponseEnum.ERROR;
                } else {
                    result.status = GraphApiResponseEnum.SUCCESS;
                    result.response = response.data.value[0];
                }
            } catch (e) {
                console.error(e);
                result.status = GraphApiResponseEnum.EXCEPTION;
            } finally {
                return result;
            }
        } else {
            try {
                const response = await this.callGetUserInfoByPublicEduNumberEndpoint(publicEduNumber);
                if (response?.data?.value.length === 0) return null;
                const { mailNickname, id, mail } = response.data.value[0] || {};
                return { publicEduNumber: mailNickname, azureID: id, email: mail };
            } catch (e) {
                console.error(e);
                return null;
            }
        }
    }

    async callUserInfoByPersonalIDEndpoint(personalID: string, userType: GraphApiUserTypeEnum) {
        const result = new GraphApiGetUserResponseDto();
        try {
            let response;
            if (userType === GraphApiUserTypeEnum.ALL) {
                response = await this.callGetUserInfoByPersonalIDEndpoint(personalID);
            }
            if (userType === GraphApiUserTypeEnum.STUDENT) {
                response = await this.callGetStudentInfoByPersonalIDEndpoint(personalID);
            }
            if (userType === GraphApiUserTypeEnum.TEACHER) {
                response = await this.callGetTeacherInfoByPersonalIDEndpoint(personalID);
            }

            if (!!response?.data?.value) {
                result.response = response.data.value[0];
                result.status = GraphApiResponseEnum.SUCCESS;
            } else {
                console.log(`No azure record found for ${personalID}`);
                result.status = GraphApiResponseEnum.ERROR;
            }
        } catch (e) {
            // console.error(e);
            await this.tokenBearerService.generateGraphApiBearerAccessToken();
            result.status = GraphApiResponseEnum.EXCEPTION;
        } finally {
            return result;
        }
    }

    async getUserSchoolEnrollmentInfo(username: string) {
        const result = new GraphApiGetUserSchoolEnrollmentsResponseDto();

        try {
            const response = await this.callGetUserSchoolEnrollmentInfoByUsernameEndpoint(username);
            if (!!response?.data?.value) {
                result.response = response.data.value;
                result.status = GraphApiResponseEnum.SUCCESS;
            } else {
                result.status = GraphApiResponseEnum.ERROR;
            }
        } catch (e) {
            console.error(e);
            result.status = GraphApiResponseEnum.EXCEPTION;
        } finally {
            return result;
        }
    }

    async getUserClassEnrollmentInfo(username: string) {
        const result = new GraphApiGetUserClassEnrollmentsResponseDto();
        try {
            const response = await this.callGetUserClassEnrollmentInfoByUsernameEndpoint(username);
            if (!!response?.data?.value) {
                result.response = response.data.value;
                result.status = GraphApiResponseEnum.SUCCESS;
            } else {
                result.status = GraphApiResponseEnum.ERROR;
            }
        } catch (e) {
            console.error(e);
            result.status = GraphApiResponseEnum.EXCEPTION;
        } finally {
            return result;
        }
    }

    async getSchoolInfo(schoolID: number) {
        const result = new GraphApiGetSchoolResponseDto();
        await this.callGetSchoolInfoBySchoolIDEndpoint(schoolID)
            .then(
                async (response) => {
                    result.response = response.data.value[0];
                    result.status = GraphApiResponseEnum.SUCCESS;
                },
                async (error) => {
                    console.error(error);
                    result.status = GraphApiResponseEnum.ERROR;
                },
            )
            .catch((error) => {
                console.error(error);
                result.status = GraphApiResponseEnum.EXCEPTION;
            });
        return result;
    }

    async getParentInfo(email: string) {
        const result = new GraphApiGetUserResponseDto();
        await this.callGetParentInfoByEmailEndpoint(email)
            .then(
                async (response) => {
                    result.response = response.data.value[0];
                    result.status = GraphApiResponseEnum.SUCCESS;
                },
                async (error) => {
                    console.error(error);
                    result.status = GraphApiResponseEnum.ERROR;
                },
            )
            .catch((error) => {
                console.error(error);
                result.status = GraphApiResponseEnum.EXCEPTION;
            });
        return result;
    }

    async getClassInfo(azureID: string) {
        const result = new GraphApiGetClassResponseDto();
        await this.callGetClassInfoByCurriculumEndpoint(azureID)
            .then(
                async (response) => {
                    result.response = response.data;
                    result.status = GraphApiResponseEnum.SUCCESS;
                },
                async (error) => {
                    console.error(error);
                    result.status = GraphApiResponseEnum.ERROR;
                },
            )
            .catch((error) => {
                console.error(error);
                result.status = GraphApiResponseEnum.EXCEPTION;
            });
        return result;
    }

    async getClassInfoByClassID(curriculumID: string) {
        const result = new GraphApiGetClassResponseDto();
        await this.callGetClassInfoByClassIDEndpoint(curriculumID)
            .then(
                async (response) => {
                    result.response = response.data;
                    result.status = GraphApiResponseEnum.SUCCESS;
                },
                async (error) => {
                    console.error(error);
                    result.status = GraphApiResponseEnum.ERROR;
                },
            )
            .catch((error) => {
                console.error(error);
                result.status = GraphApiResponseEnum.EXCEPTION;
            });
        return result;
    }

    async getAzureParentInfo(query: GetParentInfoRequestDto) {
        const { email } = query;

        const response = new ParentInfoResponseDto();

        const parentInfo = await this.getParentInfo(email);

        if (parentInfo.status === GraphApiResponseEnum.SUCCESS) {
            response.profileInfo = parentInfo.response as WithExtras<GraphApiResponseDTO>;
        }

        return response;
    }

    async getAzureOrganizationInfo(query: GetOrganizationInfoRequestDto) {
        const { schoolId } = query;

        const response = new OrganizationInfoResponseDto();

        const schoolInfo = await this.getSchoolInfo(+schoolId);
        const userInfo = await this.getUserInfoByPublicEduNumber(schoolId.toString(), true);

        if (schoolInfo.status === GraphApiResponseEnum.SUCCESS) {
            response.schoolInfo = schoolInfo.response as WithExtras<GraphApiSchoolResponseDTO>;
        }
        if (userInfo.status === GraphApiResponseEnum.SUCCESS) {
            response.profileInfo = userInfo.response as WithExtras<GraphApiResponseDTO>;
        }

        return response;
    }

    async getAzureUserInfo(query: GetUserInfoRequestDto) {
        const { publicEduNumber, azureId } = query;

        let profileInfo: GraphApiGetUserResponseDto;
        if (publicEduNumber) {
            profileInfo = await this.getUserInfoByPublicEduNumber(
                this.getEduNumberWithoutPostfix(publicEduNumber),
                true,
            );
        } else if (azureId) {
            profileInfo = await this.getUserInfoByAzureID(azureId, true);
        }

        const response = new UserInfoResponseDto();

        let userSchoolEnrollmentInfo: GraphApiGetUserSchoolEnrollmentsResponseDto;
        let userClassEnrollmentInfo: GraphApiGetUserClassEnrollmentsResponseDto;

        if (profileInfo.status === GraphApiResponseEnum.SUCCESS) {
            response.profileInfo = profileInfo.response as WithExtras<GraphApiResponseDTO>;
            userSchoolEnrollmentInfo = await this.getUserSchoolEnrollmentInfo(profileInfo.response.id);
            userClassEnrollmentInfo = await this.getUserClassEnrollmentInfo(profileInfo.response.id);
            if (userSchoolEnrollmentInfo.status === GraphApiResponseEnum.SUCCESS) {
                response.userSchoolEnrollmentsInfo =
                    userSchoolEnrollmentInfo.response as WithExtras<GraphApiSchoolResponseDTO>[];
            }
            if (userClassEnrollmentInfo.status === GraphApiResponseEnum.SUCCESS) {
                response.userClassEnrollmentsInfo =
                    userClassEnrollmentInfo.response as WithExtras<GraphApiClassResponseDTO>[];
            }
        }

        return response;
    }

    private getEduNumberWithoutPostfix(usernameOrEmail: string) {
        const cleanIdentifier = usernameOrEmail.trim();
        if (cleanIdentifier.includes('@')) {
            return cleanIdentifier.split('@')[0];
        }
        return cleanIdentifier;
    }
}
