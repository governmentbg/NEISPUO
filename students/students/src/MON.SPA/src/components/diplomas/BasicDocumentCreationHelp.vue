<template>
  <v-expansion-panels flat>
    <v-expansion-panel>
      <v-expansion-panel-header>
        <v-alert
          dense
          border="left"
          colored-border
          type="info"
          elevation="1"
        >
          {{ $t('menu.help') }}
        </v-alert>
      </v-expansion-panel-header>
      <v-expansion-panel-content>
        <div>
          В базата разчитаме да попълваме данни в няколко важни колони:
        </div>
        <ul>
          <li>Institution - задължителна</li>
          <li>EduForm - задължителна</li>
          <li>Nationality - незадължителна</li>
          <li>Gender - задължителна</li>
          <li>SPPOOProfession - незадължителна</li>
          <li>SPPOOSpeciality - незадължителна</li>
        </ul>
        <br>
        <div>
          Когато добавяме полетата, които ще събират данни за тези колони е важно да съблюдаваме следните особеност:
        </div>
        <ul>
          <li>ИД на полето следва да е точно като името на колоната.</li>
          <li>Тип на полето трябва да 'AutoComplete' с избрана опция 'Връща обект'</li>
        </ul>
        <hr class="mt-2">
      </v-expansion-panel-content>
    </v-expansion-panel>
  </v-expansion-panels>
</template>

<script>
export default {
  name: 'BasicDocumentCreationHelp'
};
</script>
