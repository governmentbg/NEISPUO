<template>
  <div>
    <v-toolbar
      flat
      class="pt-2"
    >
      <c-select
        v-model="selectedSchoolYear"
        :items="schoolYearsOptions"
        :label="$t('lod.evaluations.schoolYearSelect')"
        dense
        outlined
      />
      <c-select
        v-model="selectedStudentClass"
        :items="studentClassOptions"
        :label="$t('lod.evaluations.studentClassSelect')"
        return-object
        dense
        outlined
        class="ml-2"
        @input="v => $emit('input', v)"
      >
        <template #append-outer>
          <v-btn
            v-if="selectedStudentClass"
            icon
            small
            @click="selectedOpen = true"
          >
            <v-icon>mdi-eye</v-icon>
          </v-btn>
        </template>
      </c-select>
      <v-spacer />
      <slot name="append" />
    </v-toolbar>
    <v-dialog
      v-model="selectedOpen"
      :close-on-content-click="false"
      offset-x
    >
      <v-card
        v-if="selectedStudentClass"
        color="grey lighten-4"
        min-width="350px"
        flat
      >
        <v-toolbar
          color="primary"
          dark
        >
          <v-toolbar-title>
            {{ selectedStudentClass.details.classGroup.className }}
          </v-toolbar-title>
          <v-spacer />
          <v-btn
            icon
            @click="selectedOpen = false"
          >
            <v-icon>mdi-close</v-icon>
          </v-btn>
        </v-toolbar>
        <v-card-text class="pt-5">
          <student-class-details
            :value="selectedStudentClass.details"
            expanded
          />
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn
            text
            color="secondary"
            @click="selectedOpen = false"
          >
            <v-icon left>
              fas fa-times
            </v-icon>
            {{ $t('buttons.cancel') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script>
import StudentClassDetails from '@/components/students/class/StudentClassDetails.vue';


export default {
  name: 'SchoolYearAndClassSelector',
  components: {
    StudentClassDetails
  },
  props: {
    value: undefined,
    personId: {
      type: Number,
      required: true
    },
  },
  data() {
    return {
      schoolYearsOptions: [],
      selectedSchoolYear: null,
      studentClassOptions: [],
      selectedStudentClass: this.value,
      selectedOpen: false
    };
  },
  watch: {
    selectedSchoolYear(val) {
      this.loadStudentClasses(val);
    },
  },
  async mounted() {
    this.selectedSchoolYear = (await this.$api.institution.getCurrentYear(this.userInstitutionId))?.data;
    this.loadOptions();
  },
  methods: {
    loadOptions() {
      this.$api.lookups.getSchoolYearsForPerson(this.personId)
        .then((response) => {
          if(response.data) {
            this.schoolYearsOptions = response.data.sort((a, b) => { return b.value - a.value; });
          }
        })
        .catch((error) => {
          console.log(error.response);
        });
    },
    async loadStudentClasses(schoolYear) {
      if(!schoolYear) {
        this.studentClassOptions = [];
        return;
      }

      this.studentClassOptions = (await this.$api.studentClass.getMainForPersonAndLoggedInstitution(this.personId, schoolYear))?.data.map(x => {
        return { id: x.id, text: x.classGroup.className, details: x};
      });
    }
  }
};
</script>
