<template>
  <div>
    <!-- {{ value }} -->
    <div v-if="disabled || (value && value.gradeSource && value.gradeSource !== 'ЛОД')">
      <v-tooltip
        v-if="value && value.gradeText"
        bottom
      >
        <template v-slot:activator="{ on: tooltip }">
          <v-chip
            color="light"
            small
            label
            outlined
            class="ma-1"
            :close="!disabled && value.gradeSource === 'Изчислена'"
            @click:close="onGradeChipClose"
            v-on="{ ...tooltip}"
          >
            {{ (isProfSubject && (value.decimalGrade != null)) ? value.decimalGrade.toFixed(2) : value.gradeText }}
          </v-chip>
        </template>
        <span>{{ $t('lod.assessments.subjectGradeSource', {source: `${value.gradeSource}${value.classBookName ? ` / ${value.classBookName}` : ''}`}) }}</span>
      </v-tooltip>
    </div>
    <div
      v-else
      style="min-width: 70px;"
    >
      <c-text-field
        v-if="isProfSubject"
        v-model.number="value.decimalGrade"
        dense
        clearable
        :rules="[$validator.decimalNumber({integerNumbersCount: 1, realNumbersCount: 2}), $validator.min(2), $validator.max(6)]"
        @input="v => convertDecimalGradeToGradeNom(v)"
      />
      <v-autocomplete
        v-else
        v-model="value.gradeId"
        :items="gradeOptions"
        dense
        single-line
        append-icon=""
        hide-details
        clearable
        @input="v => convertGradeNomToDecimalGrade(v)"
      >
        <template v-slot:item="data">
          <v-list-item-content
            v-text="data.item.text"
          />
          <v-list-item-icon
            v-if="data.item.gradeTypeId !== 1"
          >
            <v-chip
              color="light"
              small
              outlined
            >
              {{ data.item.gradeTypeName }}
            </v-chip>
          </v-list-item-icon>
        </template>
      </v-autocomplete>
    </div>
  </div>
</template>

<script>

export default {
  name: 'LodAssessmentViewer',
  props: {
    value: {
      type: Object,
      default() {
        return null;
      }
    },
    disabled: {
      type: Boolean,
      default() {
        return true;
      }
    },
    gradeOptions: {
      type: Array,
      default() {
        return null;
      }
    },
    isProfSubject: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  methods: {
    convertDecimalGradeToGradeNom(decimalGrade) {
      if(!this.value) return;

      if(!decimalGrade || decimalGrade < 2 || decimalGrade > 6) {
        this.value.gradeId = null;
      } else {
        this.value.gradeId = Math.round(decimalGrade);
      }

    },
    convertGradeNomToDecimalGrade(gradeNom) {
      if(!this.value || gradeNom.gradeTypeId !== 1) return;

      if(!gradeNom || gradeNom < 2 || gradeNom > 6) {
        this.value.decimalGrade = null;
      } else {
        this.value.decimalGrade = parseFloat(gradeNom.toFixed(2));
      }
    },
    onGradeChipClose() {
      if(!this.value) return;

      this.value.gradeId = null;
      this.value.gradeText = null;
      this.value.gradeSource = null;
      this.value.gradeCategoryId = null;
      this.value.decimalGrade = null;
      this.value.printGradeText = null;
    }
  }
};
</script>
