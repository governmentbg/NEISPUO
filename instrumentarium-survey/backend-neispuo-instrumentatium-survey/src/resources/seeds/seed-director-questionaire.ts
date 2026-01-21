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

export default class CreateQuestionaireDirector implements Seeder {
    public async run(factory: Factory, connection: Connection): Promise<any> {
        let QuestionaireID = await connection
            .createQueryBuilder()
            .insert()
            .into(Questionaire)
            .values([
                { name: 'Въпросник - директор' },
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
                    name: 'Оценяване на потребностите за превенция на обучителните затруднения (за предучилищно образование) Отнася се за училищата, в които има предучилищна група', 
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
                    name: 'Проведен е скрининг за определяне на риск от възникване на обучителни затруднения за децата на 3 г.-3 год. и 6 месеца или оценка на риска от обучителни затруднения на /5 и 6 - годишни, които нямат скрининг', 
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
                    title: 'Проведен е скрининг за определяне на риск от възникване на обучителни затруднения за децата на 3 г.-3 год. и 6 месеца или е извършена оценка на риска от обучителни затруднения на 5 и 6 - годишни, които нямат скрининг.', 
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
                    title: 'в незадоволителна степен',
                    description: 'Не е проведен скриниг/не е извършена оценка',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: 'Проведен е скриниг/извършена е оценка на малка част от децата/учениците',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: 'Проведен е скриниг/извършена е оценка на повечето деца/ученици',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: 'Проведен е скриниг/извършена е оценка на всички деца/ученици',
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


        indicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Indicator)
            .values([
                { 
                    orderNumber: 2, 
                    name: 'Осигуряване на допълнителна подкрепа',
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
                    name: 'Извършена е оценка на индивидуалните потребности на детето/ученика за осигуряване на допълнителна подкрепа за личностно развитие',
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
                    title: 'Извършена е оценка на индивидуалните потребности на детето/ученика за осигуряване на допълнителна подкрепа за личностно развитие.',
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
                    title: 'не',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'да',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
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
            .execute();


        criteriaId = await connection
            .createQueryBuilder()
            .insert()
            .into(Criterion)
            .values([
                { 
                    orderNumber: 2, 
                    name: 'Работата с преждевременно напусналите и отпадналите образователната система деца/ученици', 
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
                    name: 'Ефективност на мерките за интервенция ',
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
                    orderNumber: 3, 
                    name: 'Повишена е ангажираността на родителите за редовното посещаване на детската градина и училището от детето им',
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
                    title: 'Повишена е ангажираността на родителите за редовното посещаване на детската градина/училището от детето им.',
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
                    title: 'не се наблюдава',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: 'Повишена е ангажираността на малка част от родителите ',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: 'Повишена е ангажираността на голяма част от родителите',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: 'Повишена е ангажираността на повечето/на всички родители',
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
            .execute();

        
        QuestionAreaId = await connection
            .createQueryBuilder()
            .insert()
            .into(QuestionArea)
            .values([
                { orderNumber: 2, title: 'ОБЛАСТ „УПРАВЛЕНИЕ НА ИНСТИТУЦИЯТА“' },
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
                    orderNumber: 4, 
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
                    orderNumber: 4, 
                    name: 'Анализът в Стратегията за развитие на детската градина/училището отразява спецификите на институцията',
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
                    title: 'Анализът в Стратегията за развитие на детската градина/училището отразява спецификите на институцията.',
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
                    description: 'Липсва анализ/анализът не отразява спецификите на институцията',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: 'Анализът отразява част от спецификите на институцията',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: '(Анализът отразява по-голяма част от спецификите на институцията',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: 'Анализът отразява напълно спецификите на институцията',
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
            .execute();


        indicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Indicator)
            .values([
                { 
                    orderNumber: 5, 
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
                    orderNumber: 5, 
                    name: 'Детската градина/училището определя дейността си в правилник в съответствие със спецификата на институцията',
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
                    title: 'В Правилника за дейността на детската градина/училището е отразена спецификата на институцията.',
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
                    description: 'Не е отразена спецификата',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: 'Отразена е част от спецификата на институцията',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: 'Отразена е по-голяма част от спецификата на институцията',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: 'Отразена е напълно спецификата на институцията',
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
            .execute();


        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 6, 
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
                    orderNumber: 6, 
                    title: 'Училището прилага принципа на автономия и определя профилите, професиите, учебните планове и учебните програми.',
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
                    title: 'в незадоволителна степен',
                    description: 'не се прилага принципът на автономия в ниска степен',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: 'Прилага се принципът на автономия за малка част от нормативно определените дейностите',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: 'Прилага се принципът на автономия за повечето, но не за всички нормативно определени дейности',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен ',
                    description: 'Прилага се принципът на автономия за всички нормативно определени дейности',
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
            .execute();


        indicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Indicator)
            .values([
                { 
                    orderNumber: 6, 
                    name: 'Оперативен мениджмънт',
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
                    orderNumber: 7, 
                    name: 'Осъществява се системна контролна дейност и своевременна обратна връзка от директора и заместник-директора/директорите',
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
                    title: 'Осъществява се системна контролна дейност и своевременна обратна връзка от директора и заместник-директора/директорите.',
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
                    description: 'Не се осъществява контролна дейност',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: '(Осъществява се контрол на част от дейностите',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: 'Осъществява се контрол без обратна връзка',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: 'Осъществява се системен контрол със своевременна обратна връзка',
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
            .execute();


        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 8, 
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
                    orderNumber: 8, 
                    title: 'Наблюдава се подобряване на резултатите чрез контролната дейност.',
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
                    title: 'понякога',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в повечето случаи',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'винаги',
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
            .execute();

        indicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Indicator)
            .values([
                { 
                    orderNumber: 7, 
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
                    orderNumber: 9, 
                    name: 'Създадени са условия за действащи структури на ученическо самоуправление(ученически съвет или други форми на ученическо представителство)',
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
                    title: 'Създадени са условия за действащи структури на ученическо самоуправление (ученически съвет или други форми на ученическо представителство).',
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
                    description: 'Не са създадени структури на ученическото самоуправление',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: 'Създадени са условия и структури на ученическото самоуправление без реализирани дейности',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: 'Създадени са условия и структури на ученическото самоуправление и са реализирани малко дейности',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: 'Структурите на ученическото самоуправление развиват активна дейност и реализират инициативи',
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
            .execute();


        indicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Indicator)
            .values([
                { 
                    orderNumber: 8, 
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
                    orderNumber: 10, 
                    name: 'Развити са култура, структури и са създадени условия за професионален диалог и екипна работа между педагогическите специалисти',
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
                    title: 'Развити са култура, структури и са създадени условия за професионален диалог и екипна работа между педагогическите специалисти.',
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
                    title: 'не се наблюдава',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: 'Развити са структури, създадени са условия за професионален диалог, понякога се осъществява екипна работа между педагогическите специалисти',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: 'Развити са структури, води се професионален диалог, често се осъществява екипна работа между педагогическите специалисти',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: 'Развити са култура, структури, води се професионален диалог, много често и активно се осъществява екипна работа между педагогическите специалисти',
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
            .execute();

        
        criteriaId = await connection
            .createQueryBuilder()
            .insert()
            .into(Criterion)
            .values([
                { 
                    orderNumber: 4, 
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
                    orderNumber: 9, 
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
                    orderNumber: 11, 
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
                    orderNumber: 11, 
                    title: 'Планираните и изразходваните годишни средства за квалификация на педагогическите специалисти осигуряват изпълнение на заложените цели и  установените потребности.',
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
                    description: 'Планираните и изразходваните годишни средства за квалификация на педагогическите специалисти не са осигурили изпълнение на заложените цели и на установените потребности',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: 'Планираните и изразходваните годишни средства за квалификация на педагогическите специалисти са осигурили изпълнение на част от заложените цели и на установените потребности',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: 'Планираните и изразходваните годишни средства за квалификация на педагогическите специалисти са осигурили изпълнение на по-голяма част от заложените цели и на установените потребности',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: 'Планираните и изразходваните годишни средства за квалификация на педагогическите специалисти са осигурили изпълнение на заложените цели и на установените потребности',
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
            .execute();

        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 12, 
                    name: 'Общественият съвет е съгласувал предложението на директора за разпределение на средствата от установеното към края на предходната година превишение на постъпленията над плащанията по бюджета на училището/детската градина',
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
                    title: 'Общественият съвет е съгласувал предложението на директора за разпределение на средствата от установеното към края на предходната година превишение на постъпленията над плащанията по бюджета на училището/детската градина.',
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
                    title: 'Не',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'Да',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
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
            .execute();

        indicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Indicator)
            .values([
                { 
                    orderNumber: 10, 
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
                    orderNumber: 13, 
                    name: 'Общественият съвет е дал становище за разпределението на бюджета по дейности и размера на капиталовите разходи, както и за отчета за изпълнението му.',
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
                    title: 'Общественият съвет е дал становище за разпределението на бюджета по дейности и размера на капиталовите разходи, както и за отчета за изпълнението му.',
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
                    title: 'Не',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'Да',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
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
            .execute();

        
        indicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Indicator)
            .values([
                { 
                    orderNumber: 11, 
                    name: 'Привличане, мотивиране и задържане на педагогически специалисти в детската градина/училището',
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
                    orderNumber: 14, 
                    name: 'Прилагат се ясни и прозрачни правила и процедури за назначаване и освобождаване на педагогически специалисти',
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
                    orderNumber: 14, 
                    title: 'Прилагат се ясни и прозрачни правила и процедури за назначаване и освобождаване на педагогически специалисти. ',
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
                    title: 'понякога',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в повечето случаи',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'винаги',
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
            .execute();

        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 15, 
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
                    orderNumber: 15, 
                    title: 'Създадени са условия за подкрепа на педагогическите специалисти в професионалното им развитие.',
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
                    title: 'да',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
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
            .execute();

        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 16, 
                    name: 'Прилага се наставничеството за мотивиране и подкрепа на педагогическите специалисти в професионалното им развитие ',
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
                    title: 'Прилага се наставничеството за мотивиране и подкрепа на педагогическите специалисти в професионалното им развитие.',
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
                    title: 'понякога',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в повечето случаи',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'винаги',
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
            .execute();

        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 17, 
                    name: 'Разпределят се ясни отговорности и се делегират правомощия на педагогическите специалисти за постигане целите на детската градина/училището',
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
                    title: 'Разпределят се ясни отговорности и се делегират правомощия на педагогическите специалисти за постигане целите на детската градина/училището.',
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
                    title: 'частично',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в повечето случаи',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'напълно',
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
            .execute();

        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 18, 
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
                    orderNumber: 18, 
                    title: 'Разработените показатели за оценяване на резултатите от труда на педагогическите специалисти са съобразени със спецификата на детската градина/училището.',
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
                    description: 'Прилага се картата от приложение № 4 на Наредба № 4 от 20 април 2017 г. за нормиране и заплащане на труда без корекции',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: 'Разработени са показатели за оценяване на резултатите от труда на педагогическите специалисти и са съобразени с част от спецификата на институцията',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: 'Разработени са показатели за оценяване на резултатите от труда на педагогическите специалисти и са съобразени с по-голяма част от спецификата на институцията',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: 'Разработени са показатели за оценяване на резултатите от труда на педагогическите специалисти и изцяло са съобразени със спецификата на детската градина/училището',
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
            .execute();


        indicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Indicator)
            .values([
                { 
                    orderNumber: 12, 
                    name: 'Мотивиране и задържане на непедагогически персонал',
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
                    name: 'Прилагат се ясни и прозрачни правила и процедури за назначаване и освобождаване на непедагогическия персонал',
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
                    title: 'Прилагат се ясни и прозрачни правила и процедури за назначаване и освобождаване на непедагогическия персонал.',
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
                    title: 'понякога',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в повечето случаи',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'винаги',
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
            .execute();

        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 20, 
                    name: 'Осигурени са ресурси и подходяща работна среда за ефективно изпълнение на задълженията от непедагогическия персонал съобразно заеманата длъжност',
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
                    title: 'Осигуряват се ресурси и подходяща работна среда за ефективно изпълнение на задълженията от непедагогическия персонал съобразно заеманата длъжност.',
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
                    title: 'понякога',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в повечето случаи',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'винаги',
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
            .execute();

        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 21, 
                    name: 'Планирана и реализирана е квалификация за ефективно изпълнение на задълженията от непедагогическия персонал',
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
                    title: 'Планирана е и е реализирана квалификация за ефективно изпълнение на задълженията от непедагогическия персонал.',
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
                    title: 'в незадоволителнастепен',
                    description: 'Планирана е квалификация за ефективно изпълнение на задълженията, не е реализирана',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: 'Част от планираната квалификация е реализирана',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: 'По-голяма част от планираната квалификация е реализирана',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: 'Планираната квалификация за ефективно изпълнение на задълженията от непедагогическия персонал е реализирана изцяло',
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
            .execute();

        
        indicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Indicator)
            .values([
                { 
                    orderNumber: 13, 
                    name: 'Насоченост на квалификацията на педагогическите специалисти към развитие на професионалните им умения и компетентности и към напредъка на децата/учениците',
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
                    orderNumber: 22, 
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
                    orderNumber: 22, 
                    title: 'Планирана е и е реализирана квалификация на педагогическите специалисти, насочена към прилагане на ИКТ в образователния процес.',
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
                    title: 'в незадоволителнастепен',
                    description: 'Планирана е квалификация на педагогическите специалисти, насочена към прилагане на ИКТ в образователния процес',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: 'Реализирана е част от планираната квалификация на педагогическите специалисти, насочена към прилагане на ИКТ в образователния процес',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: 'Реализирана е планираната квалификация на педагогическите специалисти и ИКТ се прилага в образователния процес от отделни педагогически специалисти',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: 'Реализирана е планираната квалификация на педагогическите специалисти и ИКТ се прилага в образователния процес от повечето/всички педагогически специалисти',
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
            .execute();

        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 23, 
                    name: 'Педагогическите специалисти прилагат придобитите компетентности от квалификационната дейност в пряката си работа',
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
                    title: 'Педагогическите специалисти прилагат придобитите компетентности от квалификационната дейност в пряката си работа.',
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
                    title: 'понякога',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в повечето случаи',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'винаги',
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
            .execute();


        criteriaId = await connection
            .createQueryBuilder()
            .insert()
            .into(Criterion)
            .values([
                { 
                    orderNumber: 5, 
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
                    orderNumber: 14, 
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
                    orderNumber: 24, 
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
                    orderNumber: 24, 
                    title: 'Училищният екип има формирани умения за управление на конфликти и се справя успешно.',
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
                    title: 'не се наблюдава',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'в ниска степен',
                    description: 'Училищният екип има сформирани умения за управление на конфликти и се справя понякога',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в средна степен',
                    description: 'Училищният екип има сформирани умения за управление на конфликти и се справя често',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'във висока степен',
                    description: 'Училищният екип има сформирани умения за управление и справяне с конфликти и ги прилага ефективно',
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
            .execute();

        indicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Indicator)
            .values([
                { 
                    orderNumber: 15, 
                    name: 'Училищни политики за развиване на социални и граждански компетентности ',
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
                    orderNumber: 25, 
                    name: 'В училището са установени демократични практики, свързани с младежко лидерство',
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
                    title: 'В училището са установени демократични практики, свързани с младежко лидерство и се реализират успешно.',
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
                    title: 'понякога',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в повечето случаи',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'винаги',
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
            .execute();

        indicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Indicator)
            .values([
                { 
                    orderNumber: 16, 
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
                    orderNumber: 26, 
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
                    orderNumber: 26, 
                    title: 'Осъществяват се съвместни действия с външни специалисти на ниво детска градина/училище при прояви на тормоз и насилие.',
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
                    title: 'не',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 1
                },
                { 
                    title: 'понякога',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в повечето случаи',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'винаги',
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
            .execute();

        
    
        criteriaId = await connection
            .createQueryBuilder()
            .insert()
            .into(Criterion)
            .values([
                { 
                    orderNumber: 6, 
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
                    orderNumber: 17, 
                    name: 'Взаимодействие с родителите',
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
                    name: 'Родителите са привлечени в дейности на образователната институция',
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
                    title: 'Родителите са привлечени в дейности на образователната институция.',
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
                    title: 'понякога',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в повечето случаи',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'винаги',
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
            .execute();

        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 28, 
                    name: 'Осъществява се партньорство с родителите по превенция на насилието и тормоза',
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
                    title: 'Осъществява се партньорство с родителите по превенция на насилието и тормоза.',
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
                    title: 'понякога',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в повечето случаи',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'винаги',
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
            .execute();

        indicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Indicator)
            .values([
                { 
                    orderNumber: 18, 
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
                    orderNumber: 29, 
                    name: 'Партньорствата допринасят за повишаване качеството на образователния процес',
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
                    title: 'Партньорствата допринасят за повишаване качеството на образователния процес.',
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
                    title: 'понякога',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в повечето случаи',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'винаги',
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
            .execute();

        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 30, 
                    name: 'Контактите със заинтересованите страни създават условия за подкрепа на изявата в областта на науката, културата, изкуството, спорта и други',
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
                    title: 'Контактите със заинтересованите страни създават условия за подкрепа на изявата в областта на науката, културата, изкуството, спорта и други',
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
                    title: 'понякога',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
                },
                { 
                    title: 'в повечето случаи',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 3
                },
                { 
                    title: 'винаги',
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
            .execute();

        subindicatorId = await connection
            .createQueryBuilder()
            .insert()
            .into(Subindicator)
            .values([
                { 
                    orderNumber: 31, 
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
                    orderNumber: 31, 
                    title: 'В детската градина/училището са създадени условия за конструктивен диалог с организациите на работниците и служителите.',
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
                    title: 'Да',
                    description: '',
                    questionId: questionId.identifiers[0].id,
                    weight: 2
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
            .execute();
    }


}