import { Factory, Seeder } from 'typeorm-seeding';
import { Connection } from 'typeorm';
import { QuestionArea } from '../../domain/question-area/question-area.entity'
import { Criterion } from '../../domain/criteria/criteria.entity'
import { Indicator } from '../../domain/indicator/indicator.entity';
import { Subindicator } from '../../domain/subindicator/subindicator.entity';
import { Question } from '../../domain/question/question.entity';
import { Choice } from '../../domain/choices/choice.entity';
import { Questionaire } from '../../domain/questionaire/questionaire.entity';
import { QuestionaireQuestion } from '../../domain/questionaire-question/questionaire-question.entity';

export default class CreateQuestionaireTeacher implements Seeder {
    public async run(factory: Factory, connection: Connection): Promise<any> {
        let QuestionaireID = await connection
            .createQueryBuilder()
            .insert()
            .into(Questionaire)
            .values([
                { name: 'Въпросник - педагогически специалисти' },
            ])
            .returning(['id'])
            .execute();

        let QuestionAreaId = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionArea)
            .values([
                { orderNumber: 1, title: 'ОБЛАСТ „ОБРАЗОВАТЕЛЕН ПРОЦЕС“' },
            ])
            .returning(['id'])
            .execute();
        
        let criteriaId = await connection
            .createQueryBuilder()
            .insert()
            .into(Criterion)
            .values([
                { 
                    orderNumber: 1, 
                    name: 'Ефективност на взаимодействието за личностно развитие на децата/учениците', 
                    questionAreaID: QuestionAreaId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        let indicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Indicator)
            .values([
                { 
                    orderNumber: 1, 
                    name: 'Осигуряване на обща подкрепа',
                    criteriaId: criteriaId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        let subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 1, 
                    name: 'Осъществяват се дейности за превенция на обучителните затруднения',
                    indicatorId: indicatorId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        let questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 1, 
                    title: 'Осъществявате ли дейности за превенция на обучителни затруднения?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                    subindicatorId: subindicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        let answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'не',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'рядко',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'често',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'да',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        let questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();


        criteriaId = await connection
            .createQueryBuilder()
            .insert()
            .into(Criterion)
            .values([
                { 
                    orderNumber: 2, 
                    name: 'Социализация и възпитание в образователния процес', 
                    questionAreaID: QuestionAreaId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        indicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Indicator)
            .values([
                { 
                    orderNumber: 2, 
                    name: 'Насоченост на педагогическата ситуация/урокът към социализацията и възпитанието на детето/ученика ',
                    criteriaId: criteriaId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 2, 
                    name: 'Учителят прилага техники за развиване на социални умения у децата/учениците.',
                    indicatorId: indicatorId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 2, 
                    title: 'В каква степен развивате социални умения у децата/ учениците?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                    subindicatorId: subindicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'в  незадоволителна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();

        
        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 3, 
                    name: 'Учителят формира умения за управление на собственото образователно и професионално развитие у децата/учениците',
                    indicatorId: indicatorId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 3, 
                    title: 'В каква степен формирате умения у децата/учениците за собственото им образователно и професионално развитие?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                    subindicatorId: subindicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'в  незадоволителна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();

        
        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 4, 
                    name: 'В обучението са включени компоненти с възпитателно въздействие.',
                    indicatorId: indicatorId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 4, 
                    title: 'При провеждане на педагогически ситуации/уроци включвате ли възпитателни елементи?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                    subindicatorId: subindicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'не',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'рядко',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'често',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'да',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();


        
        criteriaId = await connection
            .createQueryBuilder()
            .insert()
            .into(Criterion)
            .values([
                { 
                    orderNumber: 3, 
                    name: 'Обхващане и включване и предотвратяване на отпадането от образователната система на деца и ученици в задължителна предучилищна и училищна възраст', 
                    questionAreaID: QuestionAreaId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        indicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Indicator)
            .values([
                { 
                    orderNumber: 3, 
                    name: 'Ефективност на мерките за превенция',
                    criteriaId: criteriaId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();

        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 5, 
                    name: 'Целодневната организация на учебния ден в училище е ефективна',
                    indicatorId: indicatorId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 5, 
                    title: 'В каква степен е ефективна целодневната организация на учебния ден?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                    subindicatorId: subindicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'не е приложимо',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: -1
                },
                { 
                    title: 'в  незадоволителна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();


        criteriaId = await connection
            .createQueryBuilder()
            .insert()
            .into(Criterion)
            .values([
                { 
                    orderNumber: 4, 
                    name: 'Степен на удовлетвореност от образователния процес', 
                    questionAreaID: QuestionAreaId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        indicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Indicator)
            .values([
                { 
                    orderNumber: 4, 
                    name: 'Степен на удовлетвореност у учителите от образователния процес',
                    criteriaId: criteriaId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 6, 
                    title: 'В каква степен сте удовлетворен/удовлетворена от образователния процес в детската градина/училището?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'не съм удовлетворен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();


        
        QuestionAreaId = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionArea)
            .values([
                { orderNumber: 1, title: 'ОБЛАСТ „УПРАВЛЕНИЕ НА ИНСТИТУЦИЯТА“' },
            ])
            .returning(['id'])
            .execute();

        criteriaId = await connection
            .createQueryBuilder()
            .insert()
            .into(Criterion)
            .values([
                { 
                    orderNumber: 5, 
                    name: 'Устойчиво развитие на детската градина/училището', 
                    questionAreaID: QuestionAreaId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        indicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Indicator)
            .values([
                { 
                    orderNumber: 5, 
                    name: 'Стратегически мениджмънт',
                    criteriaId: criteriaId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();

        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 6, 
                    name: 'Екипът на детската градина/училището е включен в разработването на Стратегията за развитие на институцията и плана за изпълнението',
                    indicatorId: indicatorId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 7, 
                    title: 'В каква степен сте включен/а в разработването на Стратегията за развитие и  в плановете за дейността на детската градина/училището?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                    subindicatorId: subindicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'не бях включен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();

        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 7, 
                    name: 'Екипът на детската градина/училището споделя мисията и визията',
                    indicatorId: indicatorId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 8, 
                    title: 'В каква степен споделяте мисията и визията на детската градина/училището?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                    subindicatorId: subindicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'не споделям',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: 'не съм запознат и/или не  ги разбирам и/или споделям само отделни компоненти',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: 'запознат съм, но не ги споделям напълно',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();


        indicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Indicator)
            .values([
                { 
                    orderNumber: 6, 
                    name: 'Автономия на детската градина/училището',
                    criteriaId: criteriaId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();

        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 8, 
                    name: 'Училището определя профилите, професиите, учебните планове и учебните програми',
                    indicatorId: indicatorId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 9, 
                    title: 'В училището прилага ли се автономията за определяне  на профилите, професиите, учебните планове и програми?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                    subindicatorId: subindicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'не е приложимо ',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: -1
                },
                { 
                    title: 'не',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: 'не съм запознат и/или не  ги разбирам и/или споделям само отделни компоненти',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: 'запознат съм, но не ги споделям напълно',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();

        
        indicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Indicator)
            .values([
                { 
                    orderNumber: 7, 
                    name: 'Оперативен мениджмънт ',
                    criteriaId: criteriaId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();

        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 9, 
                    name: 'Директорът е осигурил ясни и прозрачни процеси на координация и субординация',
                    indicatorId: indicatorId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 10, 
                    title: 'Ясни и прозрачни ли са за Вас процесите на координация и субординация в институцията?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                    subindicatorId: subindicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'не',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();

        
        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 10, 
                    name: 'Осъществява системна контролна дейност от директора и заместник-директорите',
                    indicatorId: indicatorId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 11, 
                    title: 'Осъществява ли се системна контролна дейност от директора и заместник-директорите?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                    subindicatorId: subindicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'не',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'рядко',
                    description: 'веднъж годишно',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'понякога',
                    description: 'два пъти в годината',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'често',
                    description: 'на всеки 1-2 месеца или веднъж месечно',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();


        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 11, 
                    name: 'Наблюдава се подобряване на резултатите чрез контролната дейност',
                    indicatorId: indicatorId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 12, 
                    title: 'В каква степен се наблюдава подобряване на резултатите вследствие на контролната дейност?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                    subindicatorId: subindicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'не се наблюдава подобряване',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();


        indicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Indicator)
            .values([
                { 
                    orderNumber: 8, 
                    name: 'Лидерство в училищната общност',
                    criteriaId: criteriaId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();

        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 12, 
                    name: 'Директорът като лидер обединява и вдъхновява училищната общност',
                    indicatorId: indicatorId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 13, 
                    title: 'Според Вас директорът успява ли да обедини и вдъхнови деца/ученици, учители и родители?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                    subindicatorId: subindicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'не успява',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();

        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 12, 
                    name: 'Директорът създава условия за развитие на ръководни умения и лидерски компетентности сред педагогическите специалисти',
                    indicatorId: indicatorId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 13, 
                    title: 'В каква степен директорът създава условия за развитие на ръководни умения и лидерски компетентности сред педагогическите специалисти?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                    subindicatorId: subindicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'не са създадени условия',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();

        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 14, 
                    name: 'Екипът на детската градина/училището е мотивиран и проявява активност за изпълнение на целите на институцията',
                    indicatorId: indicatorId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 15, 
                    title: 'В каква степен сте мотивиран/а да изпълните целите на институцията?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                    subindicatorId: subindicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'не съм мотивиран/а',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();


        indicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Indicator)
            .values([
                { 
                    orderNumber: 9, 
                    name: 'Екипна работа в детската градина/училището',
                    criteriaId: criteriaId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();

        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 15, 
                    name: 'В детската градина/училището се осъществява обмяна на добри практики',
                    indicatorId: indicatorId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 16, 
                    title: 'Обменяте ли добри практики с други педагогически специалисти?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                    subindicatorId: subindicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'не',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'рядко',
                    description: 'веднъж годишно',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'понякога',
                    description: 'два пъти годишно',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'често',
                    description: 'на всеки 1-2 месеца или веднъж месечно',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();

        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 16, 
                    name: 'ЕкиЕфективно взаимодействие между педагогическите специалисти и непедагогическия персонал',
                    indicatorId: indicatorId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 17, 
                    title: 'В каква степен е ефективно взаимодействието между педагогическите специалисти и непедагогическия персонал във Вашата институция?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                    subindicatorId: subindicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'не е ефективно',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();


        
        criteriaId = await connection
            .createQueryBuilder()
            .insert()
            .into(Criterion)
            .values([
                { 
                    orderNumber: 6, 
                    name: 'Ефективно управление на ресурсите', 
                    questionAreaID: QuestionAreaId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();


        indicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Indicator)
            .values([
                { 
                    orderNumber: 10, 
                    name: 'Целесъобразно управление на финансовите ресурси за развитие на детската градина/училището',
                    criteriaId: criteriaId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();

        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 17, 
                    name: 'Планираните и изразходваните годишни средства за квалификация на педагогическите специалисти осигуряват изпълнение на заложените цели и  установените потребности',
                    indicatorId: indicatorId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 18, 
                    title: 'Средствата за квалификация осигуряват ли изпълнение на заложените цели и  установените потребности?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                    subindicatorId: subindicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'не',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();


        indicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Indicator)
            .values([
                { 
                    orderNumber: 11, 
                    name: 'Прозрачно управление на бюджета',
                    criteriaId: criteriaId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();

        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 18, 
                    name: 'Педагогическите специалисти и непедагогическият персонал са запознати с бюджета на детската градина/училището, както и с отчетите за неговото изпълнение',
                    indicatorId: indicatorId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 19, 
                    title: 'Запознати ли сте с бюджета на институцията, както и с отчетите за неговото изпълнение?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                    subindicatorId: subindicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'не съм запознат/а',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();

        
        indicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Indicator)
            .values([
                { 
                    orderNumber: 12, 
                    name: 'Привличане, мотивиране и задържане на педагогически специалисти в детската градина/училището ',
                    criteriaId: criteriaId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();

        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 19, 
                    name: 'Ясни и прозрачни ли са за Вас процедурите за назначаване и освобождаване на педагогическите специалисти?',
                    indicatorId: indicatorId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 20, 
                    title: 'Ясни и прозрачни ли са за Вас процедурите за назначаване и освобождаване на педагогическите специалисти?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                    subindicatorId: subindicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'не',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();


        
        
        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 20, 
                    name: 'Директорът създава условия за подкрепа на педагогическите специалисти в професионалното им развитие',
                    indicatorId: indicatorId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 21, 
                    title: 'Директорът подкрепя ли Ви за професионалното Ви развитие?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                    subindicatorId: subindicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'не създава условия',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();

        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 21, 
                    name: 'Разработени са показатели за оценяване на резултатите от труда на педагогическите специалисти, съобразени със спецификата на детската градина/училището',
                    indicatorId: indicatorId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 22, 
                    title: 'Разработени ли са показатели (съобразени със спецификата на детската градина/училището) за оценяване на резултатите от труда Ви?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                    subindicatorId: subindicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'не',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();


        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 22, 
                    name: 'Педагогическите специалисти се поощряват  с морални и материални награди',
                    indicatorId: indicatorId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 23, 
                    title: 'Поощрявани ли сте с морални и материални награди?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                    subindicatorId: subindicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'не',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();


        indicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Indicator)
            .values([
                { 
                    orderNumber: 13, 
                    name: 'Привличане, мотивиране и задържане на педагогически специалисти в детската градина/училището ',
                    criteriaId: criteriaId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();

        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 23, 
                    name: 'Квалификацията съответства на политиките и приоритетите, определени в Стратегията на детската градина/училището и на установените потребности за професионално развитие на педагогическите специалисти',
                    indicatorId: indicatorId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 24, 
                    title: 'В каква степен квалификацията съответства на политиките и приоритетите на детската градина/училището и на Вашите потребности за професионално развитие?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                    subindicatorId: subindicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'не съответства',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();


        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 24, 
                    name: 'Педагогическите специалисти имат възможност за участие в международни и национални програми и проекти за професионално развитие',
                    indicatorId: indicatorId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 25, 
                    title: 'Имате ли възможност за участие в международни и национални програми и проекти за професионално развитие?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                    subindicatorId: subindicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'не',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();


        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 25, 
                    name: 'Планирана е и е реализирана квалификация на педагогическите специалисти, насочена към прилагане на ИКТ в образователния процес',
                    indicatorId: indicatorId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 26, 
                    title: 'Участвали ли сте в квалификация, насочена към прилагане на ИКТ в образователния процес?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                    subindicatorId: subindicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'не',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: 'участвал/а съм еднократно',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: 'участвам понякога',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: 'участвам винаги, когато се предостави възможност',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();


        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 26, 
                    name: 'Вътрешноинституционалната квалификация на педагогическите специалисти през последната учебна година е допринесла за професионалното им развитие ',
                    indicatorId: indicatorId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 27, 
                    title: 'В каква степен вътрешнонституционалната квалификация през последната учебна година е допринесла за професионалното Ви развитие?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                    subindicatorId: subindicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'в незадоволителна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();



        criteriaId = await connection
            .createQueryBuilder()
            .insert()
            .into(Criterion)
            .values([
                { 
                    orderNumber: 7, 
                    name: 'Управление и развитие на  физическата среда', 
                    questionAreaID: QuestionAreaId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();

        indicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Indicator)
            .values([
                { 
                    orderNumber: 14, 
                    name: 'Въвеждане на информационно- технологичните ресурси в цялостната дейност на детската градина/училището',
                    criteriaId: criteriaId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();

        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 27, 
                    name: 'ИКТ се използват целесъобразно в административната дейност на детската градина/училището',
                    indicatorId: indicatorId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 28, 
                    title: 'В каква степен целесъобразно използвате ИКТ в административната си дейност?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                    subindicatorId: subindicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'в незадоволителна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();


        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 28, 
                    name: 'Осигурена е възможност за комуникация на екипа в институцията чрез технологиите',
                    indicatorId: indicatorId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 29, 
                    title: 'В каква степен комуникация на екипа в институцията се осъществява чрез технологиите?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                    subindicatorId: subindicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'в незадоволителна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();



        criteriaId = await connection
            .createQueryBuilder()
            .insert()
            .into(Criterion)
            .values([
                { 
                    orderNumber: 8, 
                    name: 'Развитие на институционалната култура в детската градина/училището', 
                    questionAreaID: QuestionAreaId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();

        indicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Indicator)
            .values([
                { 
                    orderNumber: 15, 
                    name: 'Изграждане на позитивна среда в детската градина/училището',
                    criteriaId: criteriaId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();

        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 29, 
                    name: 'В институцията се прилага Етичен кодекс на училищната общност',
                    indicatorId: indicatorId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 30, 
                    title: 'В каква степен прилагането на Етичния кодекс води до изграждане на позитивна среда в детската градина/училището?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                    subindicatorId: subindicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'не се прилага Етичен кодекс на училищната общност',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();

        
        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 30, 
                    name: 'Училищният екип има формирани умения за управление и справяне с конфликти',
                    indicatorId: indicatorId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 31, 
                    title: 'В каква степен имате формирани умения за управление и справяне с конфликти?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                    subindicatorId: subindicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'в незадоволителна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();


        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 31, 
                    name: 'Прилага се система от дежурства с цел поддържане на сигурна среда',
                    indicatorId: indicatorId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 32, 
                    title: 'Системата за дежурства във Вашата институция осигурява ли сигурна среда?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                    subindicatorId: subindicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'не осигурява сигурна среда',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();


        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 32, 
                    name: 'Наложените санкции на учениците са ефективни',
                    indicatorId: indicatorId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 33, 
                    title: 'В каква степен наложените санкции на учениците водят до положителна промяна?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                    subindicatorId: subindicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'не водят до положителна промяна',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();


        indicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Indicator)
            .values([
                { 
                    orderNumber: 16, 
                    name: 'Училищни политики за развиване на социални и граждански компетентности',
                    criteriaId: criteriaId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();

        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 33, 
                    name: 'Утвърдена е институционална политика, насочена към изграждане и поддържане на национални и колективни ценности, включително в интеркултурна среда',
                    indicatorId: indicatorId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 34, 
                    title: 'Прилагате ли институционална политика, насочена към изграждане и поддържане на национални и колективни ценности, включително и в интеркултурна среда?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                    subindicatorId: subindicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'не е налична такава политика',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();


        indicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Indicator)
            .values([
                { 
                    orderNumber: 17, 
                    name: 'Ефективност на системата за интервенция и подкрепа при прояви на тормоз и насилие',
                    criteriaId: criteriaId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();

        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 34, 
                    name: 'Създадена е система за интервенция и подкрепа при прояви на тормоз и насилие (разработени и разяснени съществуващи правила и процедури във връзка с всяка една проява на насилие и тормоз, вкл. по отношение на идентифициране и сигнализиране)',
                    indicatorId: indicatorId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 35, 
                    title: 'В каква степен системата за интервенция и подкрепа при прояви на тормоз и насилие е ефективна?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                    subindicatorId: subindicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'в незадоволителна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();


        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 35, 
                    name: 'Осъществяват се съвместни действия с външни специалисти на ниво детска градина/училище при прояви на тормоз и насилие',
                    indicatorId: indicatorId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 36, 
                    title: 'Във Вашата институция осъществяват ли се съвместни дейности с външни специалисти във връзка с реализиране на мерки по интервенция и подкрепа при прояви на тормоз и насилие?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                    subindicatorId: subindicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'не',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();


        criteriaId = await connection
            .createQueryBuilder()
            .insert()
            .into(Criterion)
            .values([
                { 
                    orderNumber: 9, 
                    name: 'Управлениe на партньорства', 
                    questionAreaID: QuestionAreaId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();

        indicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Indicator)
            .values([
                { 
                    orderNumber: 18, 
                    name: 'Проактивност на директора',
                    criteriaId: criteriaId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();

        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 36, 
                    name: 'Установените партньорства се развиват устойчиво',
                    indicatorId: indicatorId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 37, 
                    title: 'В каква степен установените партньорства се развиват устойчиво?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                    subindicatorId: subindicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'не се развиват устойчиво',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();


        indicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Indicator)
            .values([
                { 
                    orderNumber: 19, 
                    name: 'Ефективност на взаимодействието със заинтересованите страни',
                    criteriaId: criteriaId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();

        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 37, 
                    name: 'Създадени са условия за конструктивен диалог в детската градина/училището с организациите на работниците и служителите.',
                    indicatorId: indicatorId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();
        
        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 38, 
                    title: 'Във Вашата институция създадени ли са условия за конструктивен диалог с организациите на работниците и служителите?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                    subindicatorId: subindicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'не са създадени',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();



        criteriaId = await connection
            .createQueryBuilder()
            .insert()
            .into(Criterion)
            .values([
                { 
                    orderNumber: 10, 
                    name: 'Степен на удовлетвореност от управлението на институцията ', 
                    questionAreaID: QuestionAreaId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();

        indicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Indicator)
            .values([
                { 
                    orderNumber: 20, 
                    name: 'Степен на удовлетвореност у педагогическите специалисти от управлението на институцията',
                    criteriaId: criteriaId.identifiers[0].id
                },
            ])
            .returning(['id'])
            .execute();

        questionId = await connection
            .createQueryBuilder()
            .insert()
            .into(Question)
            .values([
                { 
                    orderNumber: 39, 
                    title: 'В каква степен сте удовлетворени от управлението на детската градина/училището?',
                    questionAreaId: QuestionAreaId.identifiers[0].id,
                    criteriaId: criteriaId.identifiers[0].id,
                    indicatorId: indicatorId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();
        
        answers = await connection
            .createQueryBuilder()
            .insert()
            .into(Choice)
            .values([
                { 
                    title: 'не съм удовлетворен/а',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 4
                },
            ])
            .returning(['id'])
            .execute();
        
        questionaire_question = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionaireQuestion)
            .values([
                { 
                    questionaireId: QuestionaireID.identifiers[0].id,
                    questionId: questionId.identifiers[0].id,
                },
            ])
            .returning(['id'])
            .execute();

    }


}