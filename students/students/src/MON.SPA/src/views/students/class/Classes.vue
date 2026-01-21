<template>
  <div>
    <student-profile
      v-if="showProfile"
      :pid="parseInt(id)"
      :show-classes-button="false"
    />
    <v-card>
      <v-card-subtitle
        class="pt-1"
      >
        {{ $t('student.classes.positionTypeFilterTitle') }}
      </v-card-subtitle>
      <v-card-text>
        <v-row>
          <v-col
            class="pt-0 pb-0"
          >
            <v-checkbox
              v-model="selectedPositionTypes"
              class="positionType"
              :label="$t('student.classes.positionTypeStudent')"
              :value="Positions.Student"
            />
          </v-col>
          <v-col
            class="pt-0 pb-0"
          >
            <v-checkbox
              v-model="selectedPositionTypes"
              class="positionType"
              :label="$t('student.classes.positionTypeStudentOtherInstitution')"
              :value="Positions.StudentOtherInstitution"
            />
          </v-col>
          <v-col
            class="pt-0 pb-0"
          >
            <v-checkbox
              v-model="selectedPositionTypes"
              class="positionType"
              :label="$t('student.classes.positionTypeStudentPersonalSupport')"
              :value="Positions.StudentPersonalSupport"
            />
          </v-col>
          <v-col
            class="pt-0 pb-0"
          >
            <v-checkbox
              v-model="selectedPositionTypes"
              class="positionType"
              :label="$t('student.classes.positionTypeStudentSpecialEducationSupport')"
              :value="Positions.StudentSpecialEducationSupport"
            />
          </v-col>
          <v-col
            class="pt-0 pb-0"
          >
            <v-checkbox
              v-model="selectedPositionTypes"
              class="positionType"
              :label="$t('student.classes.positionTypeProfessionalEducation')"
              :value="Positions.ProfessionalEducation"
            />
          </v-col>
        </v-row>
      </v-card-text>
    </v-card>
    <student-classes-time-line
      :pid="pid"
      :positions="selectedPositionTypes"
    />
  </div>
</template>

<script>
import StudentProfile from '@/components/students/Profile.vue';
import StudentClassesTimeLine from '@/components/students/class/StudentClassesTimeLine.vue';
import { Positions, Permissions, InstType } from '@/enums/enums';
import { mapGetters } from 'vuex';

export default {
  name: 'StudentClasses',
  components: {
    StudentProfile,
    StudentClassesTimeLine
  },
  props: {
    pid: {
      type: Number,
      default() {
        return null;
      }
    },
    showProfile: {
      type: Boolean,
      default() {
        return true;
      },
    },
  },
  data() {
    return {
      Positions,
      selectedPositionTypes: []
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission', 'isInstType']),
  },
  mounted() {
    if(this.isInstType(InstType.School) || this.isInstType(InstType.Kindergarten)) {
      this.selectedPositionTypes.push(Positions.Student);
      this.selectedPositionTypes.push(Positions.StudentSpecialEducationSupport);
      this.selectedPositionTypes.push(Positions.ProfessionalEducation);
      return;
    }

    if(this.isInstType(InstType.CSOP)) {
      this.selectedPositionTypes.push(Positions.StudentOtherInstitution);
      return;
    }

    if(this.isInstType(InstType.CPLR) || this.isInstType(InstType.SOZ)) {
      this.selectedPositionTypes.push(Positions.StudentPersonalSupport);
      return;
    }

    this.selectedPositionTypes.push(Positions.Student);
    this.selectedPositionTypes.push(Positions.StudentPersonalSupport);
  },
  beforeMount() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentClassRead)) {
      return this.$router.push('/errors/AccessDenied');
    }
  }
};
</script>
<style>
.positionType {
  margin: 0;
  padding: 0;
}
.positionType .v-input__slot {
  margin: 0;
}
</style>
