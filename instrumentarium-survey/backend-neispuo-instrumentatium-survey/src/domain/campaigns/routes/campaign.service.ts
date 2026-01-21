import { BadRequestException, Injectable, InternalServerErrorException } from '@nestjs/common';
import { Campaign } from '../campaign.entity';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Connection, EntityManager, getManager, Repository } from 'typeorm';
import { CrudRequest, ParsedRequest } from '@nestjsx/crud';
import { SubmittedQuestionaireStates } from '../../submitted-questionaire/submitted-questionaire-states.enum';
import { TeachersQuestionaireReminderService } from '../../notifications/teachers-questionaire-reminder/routes/teachers-questionaire-reminder.service';
import { CampaignType } from '../enums/campaign.enum';
import moment from 'moment';
import { SubmittedQuestionaire } from '@domain/submitted-questionaire/submitted-questionaire.entity';
import { Questionaire } from '@domain/questionaire/questionaire.entity';
import { LoggedUserModel } from '@shared/models/selected-role.model';
import { SysRoleEnum } from '@domain/sys-role/enums/sys-role.enum';

@Injectable()
export class CampaignService extends TypeOrmCrudService<Campaign> {
    constructor(
        @InjectRepository(Campaign) repo: Repository<Campaign>,
        private connection: Connection,
        private notificationService: TeachersQuestionaireReminderService
    ) {
        super(repo);
    }

    public async getAll(@ParsedRequest() req: CrudRequest, years: number[], base: any) {
        // sort years
        years = years.sort((a, b) => a - b);

        // make it so startDate is in range of selected years
        if (years.length > 0) {
            req.parsed.search.$and.push(
                { startDate: { $gte: new Date(years[0], 0).toISOString() } },
                { startDate: { $lt: new Date(years[years.length - 1], 12).toISOString() } }
            );
        }

        const campaigns = await base.getManyBase(req);

        // add property if the campaign has already been fulfilled
        campaigns.data = await Promise.all(
            campaigns.data.map(async (el: any) => {
                const submittedQ = await this.connection
                    .createQueryBuilder()
                    .select('sq.id')
                    .from('tools_assessment.submittedQuestionaire', 'sq')
                    .where('sq.campaignId = :id', { id: el.id })
                    .andWhere('sq.state = :st', { st: SubmittedQuestionaireStates.FINISHED })
                    .getOne();

                el.hasSubmittedQuestionaires = submittedQ ? true : false;
                return el;
            })
        );

        return campaigns;
    }

    public async deleteCampaign(@ParsedRequest() req: CrudRequest, base: any) {
        const parsed = req.parsed;
        const urlParams = parsed.search.$and;
        const campaignId = JSON.parse(JSON.stringify(urlParams[1])).id.$eq;

        if ((await this.checkIfSubmittedQuestionairesExist(campaignId)) === false) {
            return false;
        }

        return await this.deleteCampaignsInDb([campaignId]);
    }

    public async deleteManyCampaigns(campaignIds: string[]) {
        const deleteableCampaigns: number[] = [];
        const nonDeleteableCampaigns: number[] = [];

        for (const campaign of campaignIds) {
            if ((await this.checkIfSubmittedQuestionairesExist(campaign)) === false) {
                nonDeleteableCampaigns.push(+campaign);
            } else {
                deleteableCampaigns.push(+campaign);
            }
        }

        if (deleteableCampaigns && deleteableCampaigns.length > 0) {
            await this.deleteCampaignsInDb(deleteableCampaigns);
        }

        if (nonDeleteableCampaigns && nonDeleteableCampaigns.length > 0) {
            return this.selectMultipleCampaigns(nonDeleteableCampaigns);
        }
    }

    public async deleteCampaignsInDb(campaigns: number[]) {
        await this.connection
            .createQueryBuilder()
            .delete()
            .from(SubmittedQuestionaire)
            .where('campaignId in ( :...ids ) ', { ids: campaigns })
            .execute();

        await this.connection
            .createQueryBuilder()
            .delete()
            .from(Campaign)
            .where('id in ( :...ids ) ', { ids: campaigns })
            .execute();
    }

    public async selectMultipleCampaigns(campaigns: number[]) {
        const campaignObjects = await this.connection
            .getRepository(Campaign)
            .createQueryBuilder('c')
            .where('c.id in ( :...ids ) ', { ids: campaigns })
            .getMany();

        return campaignObjects;
    }

    public async checkIfSubmittedQuestionairesExist(campaignId: string) {
        const submittedQuestionaires = await this.connection
            .getRepository(SubmittedQuestionaire)
            .createQueryBuilder('s')
            .where('s.campaignId = :id ', { id: campaignId })
            .andWhere('s.state = :st', { st: SubmittedQuestionaireStates.FINISHED })
            .getCount();

        // If there are submitted Questionaires from this campaign don't delete it
        return !submittedQuestionaires;
    }

    public async createCampaign(dto: Campaign, loggedUser: LoggedUserModel | any) {
        if (!this.validateCampaignDtoOnCreate(dto)) {
            throw new BadRequestException('Invalid campaign parameters provided!');
        }

        const startDate = moment(dto.startDate);
        const today = moment();

        dto.isActive = startDate.isSame(today, 'day') ? 1 : 0;
        dto.isLocked = 0;
        //if campaign is being created from ESUI type is already set to ESUI
        dto.type = dto.type ?? CampaignType.SELFEVALUATE;

        //if campaign is being created from ESUI institutionId is already set in the dto
        dto.institutionId = dto.institutionId ?? loggedUser.InstitutionID;
        // if campaign is being created from ESUI we don't set createdBy
        dto.createdBy = loggedUser.SysUserID ? loggedUser.SysUserID : '';

        try {
            const newCampaign = await this.connection.transaction(async (em: EntityManager) => {
                const campaignRepo = em.getRepository(Campaign);
                const questionaireRepo = em.getRepository(Questionaire);

                const newCampaign = await campaignRepo.save(dto);

                // Create SubmittedQuestionaire for TEACHERS
                const teacherQuestionaire = await questionaireRepo.findOne({
                    SysRoleID: SysRoleEnum.TEACHER
                });
                await em.query(`
                    INSERT INTO tools_assessment.submittedQuestionaire
                    (userId, questionaireId, campaignId, state, submittedQuestionaireObject)
                    SELECT s.sysUserID, ${teacherQuestionaire.id}, ${newCampaign.id}, ${SubmittedQuestionaireStates.DRAFT}, '{}'
                    FROM azure_temp.StudentTeacherUsers s
                    WHERE s.institutionID = ${dto.institutionId}
                    AND s.sysRoleID = ${SysRoleEnum.TEACHER}
                `);

                // Create SubmittedQuestionaire for STUDENTS
                const studentQuestionaire = await questionaireRepo.findOne({
                    SysRoleID: SysRoleEnum.STUDENT
                });
                await em.query(`
                    INSERT INTO tools_assessment.submittedQuestionaire
                    (userId, questionaireId, campaignId, state, submittedQuestionaireObject)
                    SELECT s.sysUserID, ${studentQuestionaire.id}, ${newCampaign.id}, ${SubmittedQuestionaireStates.DRAFT}, '{}'
                    FROM azure_temp.StudentTeacherUsers s
                    WHERE s.institutionID = ${dto.institutionId}
                    AND s.sysRoleID = ${SysRoleEnum.STUDENT}
                `);

                // Create SubmittedQuestionaire for PARENTS
                const parentQuestionaire = await questionaireRepo.findOne({
                    SysRoleID: SysRoleEnum.PARENT
                });
                await em.query(`
                    INSERT INTO tools_assessment.submittedQuestionaire
                    (userId, questionaireId, campaignId, state, submittedQuestionaireObject)
                    SELECT su.SysUserID, ${parentQuestionaire.id}, ${newCampaign.id}, ${SubmittedQuestionaireStates.DRAFT}, '{}'
                    FROM core.SysUser su 
                    JOIN core.ParentChildSchoolBookAccess p on p.ParentID = su.PersonID
                    JOIN azure_temp.StudentTeacherUsers st on st.personID = p.ChildID 
                    WHERE st.institutionID = ${dto.institutionId}
                    AND st.sysRoleID = ${SysRoleEnum.STUDENT}
                `);

                // Create SubmittedQuestionaire for the Institution DIRECTOR
                const institutionQuestionaire = await questionaireRepo.findOne({
                    SysRoleID: SysRoleEnum.INSTITUTION
                });
                await em.query(`
                    INSERT INTO tools_assessment.submittedQuestionaire
                    (userId, questionaireId, campaignId, state, submittedQuestionaireObject)
                    SELECT i.sysUserID, ${institutionQuestionaire.id}, ${newCampaign.id}, ${SubmittedQuestionaireStates.DRAFT}, '{}'
                    FROM azure_temp.InstitutionsTable i
                    WHERE i.institutionID = ${dto.institutionId}
                    AND i.SysUserID IS NOT NULL 
                    AND i.sysRoleID = ${SysRoleEnum.INSTITUTION}
                `);

                return newCampaign;
            });

            return newCampaign;
        } catch (err) {
            throw new InternalServerErrorException(err);
        }
    }

    public async sendEmailsForNewlyCreatedCampaign(dto: Campaign, newCampaign: Campaign) {
        const teachers = await this.notificationService.getAllTeachersWithoutSubmittedQuestionaireFromInstitution(
            newCampaign.id
        );

        await this.notificationService.sendEmailsToTeachers(teachers);
    }

    public validateCampaignDtoOnCreate(campaign: Campaign) {
        const startDate = moment(campaign.startDate);
        const endDate = moment(campaign.endDate);

        if (startDate >= endDate) {
            return false;
        }

        if (campaign.name.length > 100) {
            return false;
        }

        return true;
    }

    public hardcodePropertiesForESUICampaignCreation(dto: Campaign, falseProperty: number = 0) {
        dto.type = CampaignType.ESUI;

        return dto;
    }

    public validateCampaignStartDateOnEdit(dto: Campaign) {
        const startDate = moment(dto.startDate);
        const today = moment();

        return startDate.isSameOrAfter(today, 'day');
    }

    public async getAggregatedDataForCampaign(campaignId: number) {
        const totalSubmitted = await this.connection
            .createQueryBuilder()
            .select('sq.id')
            .from('tools_assessment.submittedQuestionaire', 'sq')
            .where('sq.campaignId = :id', { id: campaignId })
            .andWhere('sq.state = :state', { state: SubmittedQuestionaireStates.FINISHED })
            .getCount();

        const totalAssigned = await this.connection
            .createQueryBuilder()
            .select('sq.id')
            .from('tools_assessment.submittedQuestionaire', 'sq')
            .where('sq.campaignId = :id', { id: campaignId })
            .getCount();

        return {
            totalUsersSubmitted: totalSubmitted,
            totalUsersAssignedTo: totalAssigned
        };
    }

    public async updateCampaign(dto: Campaign, id: number) {
        return await this.connection
            .createQueryBuilder()
            .update('tools_assessment.campaigns')
            .set({ ...dto })
            .where('id = :id', { id })
            .execute();
    }
}
