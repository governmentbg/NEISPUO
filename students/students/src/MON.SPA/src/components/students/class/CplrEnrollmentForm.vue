<template>
  <div>
    <v-form
      :ref="'form' + _uid"
      :disabled="disabled"
    >
      <student-class-basic-data
        v-model="model"
        class="my-2"
        :min-school-year="minSchoolYear"
        :max-school-year="maxSchoolYear"
        :institution-id="userInstitutionId"
        additional-enrollment
        cplr-enrollment
      />

      <student-class-supportive-environment
        v-model="model"
        class="my-2"
      />
    </v-form>
  </div>
</template>

<script>
import StudentClassBasicData from '@/components/students/class/StudentClassBasicData.vue';
import StudentClassSupportiveEnvironment from '@/components/students/class/StudentClassSupportiveEnvironment.vue';
import { StudentClass } from '@/models/studentClass/studentClass';
import { mapGetters } from "vuex";

export default {
  name: "CplrEnrollmentFormEditor",
  components: {
    StudentClassBasicData,
    StudentClassSupportiveEnvironment
  },
  props: {
    document: {
      type: Object,
      default: null,
    },
    disabled: {
      type: Boolean,
      default() {
        return false;
      },
    }
  },
  data() {
    return {
      model: this.document ?? new StudentClass(),
      minSchoolYear: undefined,
      maxSchoolYear: undefined
    };
  },
  computed: {
    ...mapGetters(['userInstitutionId']),
  },
  mounted() {
    this.$api.institution.getCurrentYear(this.userInstitutionId)
      .then(response => {
        const currentYear = Number(response.data);
        if(!isNaN(currentYear)) {
          this.model.schoolYear = currentYear;
          this.minSchoolYear = currentYear;
          this.maxSchoolYear = currentYear + 1;
        }
      });
  },
  methods: {
    validate() {
      const form = this.$refs['form' + this._uid];
      return form ? form.validate() : false;
    }
  }
};
</script>
