## Предпоставки:
- Да имате валиден сертификат.
- Да има училище избрало вас за доставчик на услугите от модул Списък Образец.
	
## Специфични работни процеси:
### Начало на учебната година и създаване на дневник:
- Трябва да има издадена заповед от МОН ("Заповед за определяне на графика на учебното време"), за съответната учебна година, която се издава в края на месец Август.
- Трябва да се създаде настройка на учебната година за всички класове/групи:
		използва се - `/{schoolYear}/{institutionId}/schoolYearDateInfos`

### Създаване на седмично разписание:
- За да може на един клас да се въвеждат данни, то той трябва да има седмично разписание. Има два начина за създаване на седмично разписание:
  - Чрез използване на предварително въведена смяна по време на създаване на седмичното разписание:
		- трябва да се създаде смяна предварително и след това да се използва създадената смяна при създаването на разписание:
			използва се - `/{schoolYear}/{institutionId}/shifts`
			след което се използва създадената смяна при - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/schedules`
  - Дефиниране на смяна по време на създаване на седмичното разписание:
		използва се - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/schedules`
- Разделяне на разписание - след като се създаде разписание и за някой от часовете има въведени оценки, ученически отсъствия, теми, учителски отсъствия, лекторски часове, то те трябва да се премахнат за да може да се редактира разписанието. В повечето случай това е много трудно и за това в тези случаи има създадено разделяне на разписание, чрез което може да отделите остатъка от предиода на разписанието в отделно разписание, за което няма да има въведени данни и може да бъде редактирано. Трябва да се подават само седмиците, които искате да отделите от разписанието и за които все още няма въведени данни. Използва се - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/schedules/{scheduleId}/splitSchedule`:
	
### Създаване на неучебен ден:
- За да може да се създаде неучебен ден/период трябва да са премахнати всички оценки, ученически отсъствия, теми, учителски отсъствия, лекторски часове за периода на неучебните дни:
	използва се - `/{schoolYear}/{institutionId}/offDays`

### Приключване/Премахване на приключване на дневник:
- В края на учебната година, всеки дневник трябва да бъде приключен преди преминаването в новата учебна година. Това става чрез подписване на принтиран от външен достачик дневник, в pdf формат, който директорите на институциите подписват с електронен подпис, след което той трябва да се изпрати на - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/finalize`

- Друг вариант е тези, подписани дневници, да се качат ръчно от директорите в създаде за тази цел UI в NEISPUO.

## Допълнителни бизнес процеси:
### Видове дневници и съответните секции за тях:
- Подготвителна група (ClassBookType = Book_PG):
  - Имат само присътвия/отсъствия - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/attendances`
  - Нямат оценки, както и срочни/годишни оценки - имат само "Резултати"
  - Срещи с родители - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/parentMeetings`
  - Предчулищни резултати - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/pgResults`
  - Бележки - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/notes`
- Дневници 1-ви клас (ClassBookType = Book_I_III):
	- Имат закъснения/неизвинени отсъстивия/извинени отсъствия - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/absences`
	- Имат оценки (качествени), без срочни/годишни по отделните предмети, като за края на учебната година се попълва "Общ годишен успех" - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/grades`- qualitativeGrade
  - Теми - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/topics`
  - Отзиви - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/remarks`
  - Срещи с родители - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/parentMeetings`
  - Индивидуална работа - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/individualWorks`
  - Контролни - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/exams`
  - Подкрепа - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/supports`
  - Общ годишен успех - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/firstGradeResults`
  - Бележки - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/notes`
- Дневници 2-ри, 3-ти клас (ClassBookType = Book_I_III):
	- Имат закъснения/неизвинени отсъстивия/извинени отсъствия - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/absences`
  - Имат оценки (качествени), без срочни по отделните предмети, но имат годишни оценки - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/grades` - qualitativeGrade
  - Теми - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/topics`
  - Отзиви - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/remarks`
  - Срещи с родители - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/parentMeetings`
  - Индивидуална работа - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/individualWorks`
  - Контролни - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/exams`
  - Подкрепа - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/supports`
  - Бележки - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/notes`
- Дневници 4-ти клас (ClassBookType = Book_IV):
	- Имат закъснения/неизвинени отсъстивия/извинени отсъствия - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/absences`
  - Имат оценки (количествени), имат срочни и годишни оценки - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/grades` - decimalGrade
  - Теми - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/topics`
  - Отзиви - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/remarks`
  - Срещи с родители - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/parentMeetings`
  - Индивидуална работа - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/individualWorks`
  - Контролни - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/exams`
  - Санкции - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/sanctions`
  - Подкрепа - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/supports`
  - Бележки - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/notes`
- Дневници 5-ти до 12-ти клас (ClassBookType = Book_V_XII):
	- Имат закъснения/неизвинени отсъстивия/извинени отсъствия - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/absences`
  - Имат оценки (количествени), имат срочни и годишни оценки - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/grades` - decimalGrade
  - Теми - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/topics`
  - Отзиви - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/remarks`
  - Срещи с родители - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/parentMeetings`
  - Индивидуална работа - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/individualWorks`
  - Контролни - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/exams`
  - Санкции - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/sanctions`
  - Подкрепа - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/supports`
  - Резултати - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/gradeResults`
  - Бележки - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/notes`
- ЦДО (ClassBookType = Book_CDO):
  - Имат закъснения/неизвинени отсъстивия/извинени отсъствия - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/absences`
  - Теми - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/topics`
  - Бележки - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/notes`
- ДПЛР (ClassBookType = Book_DPLR):
  - Имат присъствия/отсъствия - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/absences` - като се използват DPLR стойностите
  - Теми - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/topics`
  - Изяви - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/performances`
  - Участие в РЕПЛР - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/replrParticipations`
  - Бележки - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/notes`
- ЦСОП (ClassBookType = Book_CSOP):
  - Имат закъснения/неизвинени отсъстивия/извинени отсъствия - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/absences`
  - Имат оценки (специални оценки), имат срочни и годишни оценки - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/grades` - specialGrade
  - Теми - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/topics`
  - Срещи с родители - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/parentMeetings`
  - Индивидуална работа - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/individualWorks`
  - Подкрепа - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/supports`
  - Бележки - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/notes`

### Ендпойнти за ученик:
- Посочване на ученик, че получава СОП оценки по даден предмет - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/studentSpecialNeedCurriculums`
- Прехвърляне на отсъствия за ученик, дошъл от друга паралелка/институция - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/studentCarriedAbsences`
- Посочване на ученик, че освободен по даден предмет - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/studentGradelessCurriculums`
- Преномериране на ученик от даден клас - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/studentClassNumbers`
- Посочване на дейности по интереси (информативна информация, незадължителна) - `/{schoolYear}/{institutionId}/classBooks/{classBookId}/studentActivities`
