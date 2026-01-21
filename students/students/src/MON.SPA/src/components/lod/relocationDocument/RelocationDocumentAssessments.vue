<template>
  <div>
    <v-tabs
      v-model="mainTab"
      color="primary"
      centered
    >
      <v-tab
        key="lodAssessments"
      >
        {{ $t('relocationDocument.annualGradesTitle') }}
      </v-tab>
      <v-tab
        key="currentGrades"
      >
        {{ $t('relocationDocument.currentTermGradesTitle') }}
      </v-tab>
      <v-tab
        key="absences"
      >
        {{ $t('relocationDocument.absencesTitle') }}
      </v-tab>
      <v-tabs-items
        v-model="mainTab"
      >
        <v-tab-item
          key="lodAssessments"
        >
          <v-alert
            border="bottom"
            colored-border
            type="info"
            elevation="2"
          >
            Резултати от обучението по учебни предмети от минали години се въвеждат в "ЛОД", раздел "Оценки" и се визуализират тук.
            За да въведете оценки, натиснете
            <v-btn
              color="primary"
              text
              target="_blank"
              :to="`/student/${personId}/lod/assessments`"
            >
              тук.
            </v-btn>
          </v-alert>
          <v-tabs
            v-if="lodAssessmentsList && lodAssessmentsList.length > 0"
            v-model="innerTab"
            color="primary"
          >
            <v-tab
              v-for="(lodAssessment, index) in lodAssessmentsList"
              :key="index"
            >
              {{ `${lodAssessment.basicClassDescription} (${lodAssessment.schooYearName})` }}
            </v-tab>
          </v-tabs>
          <v-tabs-items
            v-model="innerTab"
          >
            <v-tab-item
              v-for="(lodAssessment, index) in lodAssessmentsList"
              :key="index"
            >
              <student-lod-assessment-details
                :person-id="lodAssessment.personId"
                :basic-class-id="lodAssessment.basicClassId"
                :school-year="lodAssessment.schoolYear"
                :is-self-edu-form="lodAssessment.isSelfEduForm"
                hide-back-button
              />
            </v-tab-item>
          </v-tabs-items>
        </v-tab-item>
        <v-tab-item
          key="currentGrades"
        >
          <v-alert
            border="bottom"
            colored-border
            type="info"
            elevation="2"
          >
            Текущи оценки по учебни предмети се въвеждат в "Електронния дневник" и се визуализират тук.
          </v-alert>
          <relocation-document-current-grades
            v-if="docId"
            :doc-id="docId"
          />
        </v-tab-item>
        <v-tab-item
          key="absences"
        >
          <relocation-document-absences
            v-if="docId"
            :doc-id="docId"
          />
        </v-tab-item>
      </v-tabs-items>
    </v-tabs>
    <v-overlay :value="loading">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </div>
</template>


<script>
import StudentLodAssessmentDetails from '@/views/students/lod/assessment/StudentLodAssessmentDetails.vue';
import RelocationDocumentAbsences from '@/components/lod/relocationDocument/RelocationDocumentAbsences.vue';
import RelocationDocumentCurrentGrades from '@/components/lod/relocationDocument/RelocationDocumentCurrentGrades.vue';


export default {
  name: 'RelocationDocumentAssessmentsComponents',
  components: {
    StudentLodAssessmentDetails,
    RelocationDocumentAbsences,
    RelocationDocumentCurrentGrades
  },
  props: {
    docId: {
      type: Number,
      required: true,
    },
    personId: {
      type: Number,
      default() {
        return undefined;
      }
    }
  },
  data() {
    return {
      loading: false,
      mainTab: null,
      innerTab: null,
      lodAssessmentsList: []
    };
  },
  mounted() {
    this.loadLodAssessmentList();
  },
  methods: {
    loadLodAssessmentList() {
      this.loading = true;
      this.$helper.clearArray(this.lodAssessmentsList);
      this.$api.relocationDocument
        .getLodAssessmentsList(this.docId)
        .then((result) => {
          if(result.data) {
            result.data.forEach(el => {
              this.lodAssessmentsList.push(el);
            });
          }
        }).catch((err) => {
          this.$notifier.error('', this.$t('common.loadError'));
          console.log(err.response);
        })
        .finally(() => {
          this.loading = false;
        });
    }
  }
};
</script>
