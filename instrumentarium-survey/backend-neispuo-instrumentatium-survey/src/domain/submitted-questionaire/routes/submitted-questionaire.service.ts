import { Injectable, InternalServerErrorException } from '@nestjs/common';
import { SubmittedQuestionaire } from '../submitted-questionaire.entity';
import { TypeOrmCrudService } from '@nestjsx/crud-typeorm';
import { InjectRepository } from '@nestjs/typeorm';
import { Connection, EntityManager, getConnection, getManager, Repository } from 'typeorm';
import { CrudRequest } from '@nestjsx/crud';
import { Choice } from '@domain/choices/choice.entity';
import { SubmittedQuestionaireStates } from '../submitted-questionaire-states.enum';

@Injectable()
export class SubmittedQuestionaireService extends TypeOrmCrudService<SubmittedQuestionaire> {
    constructor(
        public connection: Connection,
        @InjectRepository(SubmittedQuestionaire) repo: Repository<SubmittedQuestionaire>
    ) {
        super(repo);
    }

    public async generateAggregatedDataForQuestionnaires(questionaireId: number) {
        const submmitedQuestionaires = await this.connection
            .getRepository(SubmittedQuestionaire)
            .createQueryBuilder('submittedQuestionaire')
            .where('submittedQuestionaire.questionaireId = :questionaireId', {
                questionaireId
            })
            .getMany();

        const questions = [];

        let ret;
        for (const submElements of submmitedQuestionaires) {
            // foreach questions

            ret = JSON.parse(JSON.stringify(submElements.submittedQuestionaireObject));
            const jsonn = JSON.parse(
                eval('(' + JSON.stringify(submElements.submittedQuestionaireObject) + ')')
            );

            for (const ansQuestion of JSON.parse(JSON.stringify(jsonn.array))) {
                // foreach answers

                // if question is already added in array "questions"
                if (questions.length > 0) {
                    const elementInObj = questions.filter(function(questions) {
                        return questions.questionId == ansQuestion.questionId;
                    });

                    if (elementInObj.length > 0) {
                        // if question exists in array "questions"
                        const currentResultsEl = elementInObj[0].results;

                        const existingResultsInQuestion = currentResultsEl.filter(function(
                            currentResultsEl
                        ) {
                            return currentResultsEl.choiceId == ansQuestion.choiceId;
                        });

                        // if answerID is already added in the object
                        if (existingResultsInQuestion.length > 0) {
                            existingResultsInQuestion[0].count++;
                        } else {
                            currentResultsEl.push({
                                choiceId: ansQuestion.choiceId,
                                count: 1
                            });
                        }
                    } else {
                        questions.push({
                            questionId: ansQuestion.questionId,
                            results: [
                                {
                                    choiceId: ansQuestion.choiceId,
                                    count: 1
                                }
                            ]
                        });
                    }
                } else {
                    questions.push({
                        questionId: ansQuestion.questionId,
                        results: [
                            {
                                choiceId: ansQuestion.choiceId,
                                count: 1
                            }
                        ]
                    });
                }
            }
        }
        return await questions;
    }

    public async getSubmittedQuestionaireByID(SubmittedQuestionaireID: number) {
        return await getConnection()
        .createQueryBuilder()
        .select(['submittedQuestionaire', 'questionaireId', 'campaignId'])
        .from(SubmittedQuestionaire, 'submittedQuestionaire')
        .leftJoin('submittedQuestionaire.questionaireId', 'questionaireId')
        .leftJoin('submittedQuestionaire.campaignsId', 'campaignId')
        .where('submittedQuestionaire.id = :id', { id: SubmittedQuestionaireID })
        .getOne();
    }

    public async getSubmittedQuestionaireByCampaignAndUser(
        UserId: number,
        CampaignId: number,
        SysRoleID: number
    ) {
        return await getConnection()
            .createQueryBuilder()
            .select(['submittedQuestionaire', 'questionaireId', 'campaignId'])
            .from(SubmittedQuestionaire, 'submittedQuestionaire')
            .leftJoin('submittedQuestionaire.questionaireId', 'questionaireId')
            .leftJoin('submittedQuestionaire.campaignsId', 'campaignId')
            .where('userId = :user', { user: UserId })
            .andWhere('campaignId = :campaign', { campaign: CampaignId })
            .andWhere('questionaireId.SysRoleID = :SysRoleID', { SysRoleID })
            .getOne();
    }

    async getWholeQuestionaireByCampaign(submittedQuestionaire: any) {
        submittedQuestionaire.questionaireId.questions = await this.getQuestionsForQuestionaire(
            submittedQuestionaire.questionaireId.id
        );

        return submittedQuestionaire;
    }

    async getQuestionaires(campaignId: number) {
        const entityManager = getManager();
        const dbQUery = await entityManager.query(`
            select cq.*
            from [tools_assessment].[campaignsQuestionaires] cq
            where cq.campaignId = ${campaignId}
        `);

        return dbQUery;
    }

    async getQuestionsForQuestionaire(questionaireId: number) {
        const entityManager = getManager();
        let questions = await entityManager.query(`
            select q.*, ind.Name as "indicatorName", ind.orderNumber as "indicatorOrderNumber",
                ind.weight as "indicatorWeight", subind.Name as "subindicatorName", 
                subind.orderNumber as "subindicatorOrderNumber", subind.weight as "subindicatorWeight",
                crit.orderNumber as "criteriaOrderNumber", crit.name as "criteriaName",
                qa.title as "questionAreaTitle", qa.title as "questionAreaTitle"
            from [tools_assessment].[question] q
            join [tools_assessment].[questionaire_question] qq on q.id = qq.questionId
            join [tools_assessment].[indicator] ind on q.indicatorId = ind.id 
            join [tools_assessment].[subindicator] subind on q.subindicatorId = subind.id
            join [tools_assessment].[criteria] crit on q.criteriaId = crit.id
            join [tools_assessment].[questionArea] qa on q.questionAreaId = qa.id
            where qq.questionaireId = ${questionaireId}
        `);

        questions = await this.getChoicesByQuestion(questions);

        return questions;
    }

    async getChoicesByQuestion(questions: any) {
        const choiceRepo = this.connection.getRepository(Choice);
        try {
            questions = await this.connection.transaction(async (em: EntityManager) => {
                for (let q of questions) {
                    q.choices = await choiceRepo
                        .createQueryBuilder('c')
                        .where('c.questionId = :id', { id: q.id })
                        .getMany();
                }
                return questions;
            });
        } catch (err) {
            throw new InternalServerErrorException(err.code);
        }

        return questions;
    }
}
