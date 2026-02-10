import {MigrationInterface, QueryRunner} from "typeorm";

export class initialMigration1641976633646 implements MigrationInterface {
    name = 'initialMigration1641976633646'

    public async up(queryRunner: QueryRunner): Promise<void> {
        await queryRunner.query(`CREATE SCHEMA tools_assessment;`);
        
        await queryRunner.query(`
        CREATE TABLE tools_assessment.campaignTypes (
            id smallint NOT NULL,
            [type] nvarchar(50) COLLATE Cyrillic_General_CI_AS NOT NULL,
            CONSTRAINT campaignTypes_PK PRIMARY KEY (id)
        );
        `);
        
        await queryRunner.query(`
        CREATE TABLE tools_assessment.questionArea (
            id int IDENTITY(1,1) NOT NULL,
            orderNumber smallint NOT NULL,
            title nvarchar(255) COLLATE Cyrillic_General_CI_AS NOT NULL,
            description ntext COLLATE Cyrillic_General_CI_AS NULL,
            CONSTRAINT PK_questionArea PRIMARY KEY (id)
        );
        `);
        
        await queryRunner.query(`
        CREATE TABLE tools_assessment.questionaire (
            name nvarchar(255) COLLATE Cyrillic_General_CI_AS NOT NULL,
            id int IDENTITY(1,1) NOT NULL,
            SysRoleID smallint NULL,
            CONSTRAINT PK__question__3213E83F73EA40FB PRIMARY KEY (id)
        );
        `);
        
        await queryRunner.query(`
        CREATE TABLE tools_assessment.campaigns (
            id int IDENTITY(1,1) NOT NULL,
            name nvarchar(255) COLLATE Cyrillic_General_CI_AS NOT NULL,
            [type] smallint NOT NULL,
            startDate datetime NOT NULL,
            endDate datetime NOT NULL,
            isActive bit NULL,
            isLocked bit NULL,
            createdBy int NULL,
            institutionId int NOT NULL,
            updatedAt datetime NULL,
            updatedBy int NULL,
            aggregate_results text COLLATE Cyrillic_General_CI_AS NULL,
            createdAt datetime DEFAULT getdate() NOT NULL,
            CONSTRAINT PK_campaigns PRIMARY KEY (id),
            CONSTRAINT campaigns_FK FOREIGN KEY ([type]) REFERENCES tools_assessment.campaignTypes(id)
        );
        `);
        
        await queryRunner.query(`
        CREATE TABLE tools_assessment.campaignsQuestionaires (
            campaignsId int NOT NULL,
            questionaireId int NOT NULL,
            id int IDENTITY(1,1) NOT NULL,
            CONSTRAINT campaignQuestionaires_PK PRIMARY KEY (id),
            CONSTRAINT FK_campaignsInstitutions_campaigns FOREIGN KEY (campaignsId) REFERENCES tools_assessment.campaigns(id),
            CONSTRAINT FK_campaignsInstitutions_questionaire FOREIGN KEY (questionaireId) REFERENCES tools_assessment.questionaire(id)
        );
        `);
        
        await queryRunner.query(`
        CREATE TABLE tools_assessment.criteria (
            id int IDENTITY(1,1) NOT NULL,
            orderNumber smallint NOT NULL,
            name nvarchar(255) COLLATE Cyrillic_General_CI_AS NOT NULL,
            questionAreaID int NOT NULL,
            CONSTRAINT PK_criteria PRIMARY KEY (id),
            CONSTRAINT FK_criteria_questionArea FOREIGN KEY (questionAreaID) REFERENCES tools_assessment.questionArea(id)
        );
        `);

        await queryRunner.query(`
        CREATE TABLE tools_assessment.[indicator] (
            id int IDENTITY(1,1) NOT NULL,
            orderNumber smallint NOT NULL,
            name nvarchar(255) COLLATE Cyrillic_General_CI_AS NOT NULL,
            criteriaId int NOT NULL,
            weight int NULL,
            CONSTRAINT PK_indicator PRIMARY KEY (id),
            CONSTRAINT FK_indicator_criteria FOREIGN KEY (criteriaId) REFERENCES tools_assessment.criteria(id)
        );
        `);

        await queryRunner.query(`
        CREATE TABLE tools_assessment.subindicator (
            id int IDENTITY(1,1) NOT NULL,
            orderNumber smallint NOT NULL,
            name nvarchar(255) COLLATE Cyrillic_General_CI_AS NOT NULL,
            indicatorId int NOT NULL,
            weight int NULL,
            CONSTRAINT PK_subindicator PRIMARY KEY (id),
            CONSTRAINT FK_subindicator_indicator FOREIGN KEY (indicatorId) REFERENCES tools_assessment.[indicator](id)
        );
        `);
        
        await queryRunner.query(`
        CREATE TABLE tools_assessment.submittedQuestionaire (
            id int IDENTITY(1,1) NOT NULL,
            questionaireId int NOT NULL,
            state smallint NOT NULL,
            campaignId int NOT NULL,
            userId int NOT NULL,
            submittedQuestionaireObject nvarchar COLLATE Cyrillic_General_CI_AS NULL,
            totalScore int NULL,
            CONSTRAINT PK_submittedQuestionaire PRIMARY KEY (id),
            CONSTRAINT FK_submittedQuestionaire_campaigns FOREIGN KEY (campaignId) REFERENCES tools_assessment.campaigns(id),
            CONSTRAINT FK_submittedQuestionaire_questionaire FOREIGN KEY (questionaireId) REFERENCES tools_assessment.questionaire(id)
        );
        `);
        
        await queryRunner.query(`
        CREATE TABLE tools_assessment.aggregatedResults (
            id int IDENTITY(1,1) NOT NULL,
            campaignId int NOT NULL,
            questionaireId int NOT NULL,
            indicatorId int NOT NULL,
            totalScore float NOT NULL,
            CONSTRAINT PK__aggregat__3213E83FF2D1054C PRIMARY KEY (id),
            CONSTRAINT FK__aggregate__campa__5367FEEB FOREIGN KEY (campaignId) REFERENCES tools_assessment.campaigns(id),
            CONSTRAINT FK__aggregate__indic__5550475D FOREIGN KEY (indicatorId) REFERENCES tools_assessment.[indicator](id),
            CONSTRAINT FK__aggregate__quest__545C2324 FOREIGN KEY (questionaireId) REFERENCES tools_assessment.questionaire(id)
        );
        `);
        
        await queryRunner.query(`
        CREATE TABLE tools_assessment.campaignSysUser (
            id int IDENTITY(1,1) NOT NULL,
            campaignId int NOT NULL,
            sysUserId int NOT NULL,
            CONSTRAINT PK__campaign__3213E83FEC2EB3EB PRIMARY KEY (id),
            CONSTRAINT FK__campaignS__campa__7D3E3D26 FOREIGN KEY (campaignId) REFERENCES tools_assessment.campaigns(id) ON DELETE CASCADE
        );
        `);

        await queryRunner.query(`
        CREATE TABLE tools_assessment.question (
            id int IDENTITY(1,1) NOT NULL,
            orderNumber smallint NOT NULL,
            title nvarchar(1000) COLLATE Cyrillic_General_CI_AS NULL,
            [type] nvarchar(1) COLLATE Cyrillic_General_CI_AS NULL,
            questionAreaId int NULL,
            criteriaId int NULL,
            indicatorId int NULL,
            subindicatorId int NULL,
            CONSTRAINT PK_question PRIMARY KEY (id),
            CONSTRAINT FK_question_criteria FOREIGN KEY (criteriaId) REFERENCES tools_assessment.criteria(id),
            CONSTRAINT FK_question_indicator FOREIGN KEY (indicatorId) REFERENCES tools_assessment.[indicator](id),
            CONSTRAINT FK_question_questionArea FOREIGN KEY (questionAreaId) REFERENCES tools_assessment.questionArea(id),
            CONSTRAINT FK_question_subindicator FOREIGN KEY (subindicatorId) REFERENCES tools_assessment.subindicator(id)
        );
        `);
        
        await queryRunner.query(`
        CREATE TABLE tools_assessment.questionaire_question (
            questionaireId int NOT NULL,
            questionId int NOT NULL,
            QuestionaireQuestionId int IDENTITY(1,1) NOT NULL,
            CONSTRAINT PK_questionaire_question PRIMARY KEY (QuestionaireQuestionId),
            CONSTRAINT FK_questionaire_question_question FOREIGN KEY (questionId) REFERENCES tools_assessment.question(id)
        );
        `);
        
        await queryRunner.query(`
        CREATE TABLE tools_assessment.answeredQuestion (
            id int IDENTITY(1,1) NOT NULL,
            questionId int NOT NULL,
            submittedQuestionaireId int NOT NULL,
            value varchar(255) COLLATE Cyrillic_General_CI_AS NULL,
            additionalInfo nvarchar(255) COLLATE Cyrillic_General_CI_AS NULL,
            CONSTRAINT PK_answeredQuestion PRIMARY KEY (id),
            CONSTRAINT FK_answeredQuestion_question FOREIGN KEY (questionId) REFERENCES tools_assessment.question(id),
            CONSTRAINT FK_answeredQuestion_submittedQuestionaire FOREIGN KEY (submittedQuestionaireId) REFERENCES tools_assessment.submittedQuestionaire(id)
        );
        `);
        
        await queryRunner.query(`
        CREATE TABLE tools_assessment.choices (
            id int IDENTITY(1,1) NOT NULL,
            title nvarchar(1000) COLLATE Cyrillic_General_CI_AS NULL,
            description nvarchar(250) COLLATE Cyrillic_General_CI_AS NULL,
            value nvarchar(1) COLLATE Cyrillic_General_CI_AS NULL,
            questionId int NOT NULL,
            weight int NULL,
            CONSTRAINT PK_choices_1 PRIMARY KEY (id),
            CONSTRAINT FK_choices_question FOREIGN KEY (questionId) REFERENCES tools_assessment.question(id)
        );
        `);
        
        await queryRunner.query(`
        CREATE TABLE tools_assessment.answeredQuestionChoice (
            choiceId int NOT NULL,
            answeredQuestionId int NOT NULL,
            CONSTRAINT FK_answeredQuestionChoice_answeredQuestion FOREIGN KEY (answeredQuestionId) REFERENCES tools_assessment.answeredQuestion(id),
            CONSTRAINT FK_answeredQuestionChoice_choices FOREIGN KEY (choiceId) REFERENCES tools_assessment.choices(id)
        );
        `);
    }

    public async down(queryRunner: QueryRunner): Promise<void> {
    }

}
