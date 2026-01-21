<template>
  <div>
    <v-card>
      <v-card-title>
        {{ $t('studentClassChange.currentStudentClass') }}
      </v-card-title>
      <v-card-text
        v-if="selectedStudentClass"
      >
        <student-class-details
          :value="selectedStudentClass"
        />
      </v-card-text>
    </v-card>

    <v-divider
      class="mt-2"
    />

    <form-layout
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        {{ $t('studentClassChange.newStudentClass') }}
      </template>

      <template #default>
        <v-form
          :ref="'form' + _uid"
        >
          <student-class-basic-data
            v-if="selectedStudentClass"
            v-model="model"
            class="my-2"
            :min-school-year="selectedStudentClass.schoolYear"
            :max-school-year="selectedStudentClass.schoolYear + 1"
            :min-enrollment-date="selectedStudentClass.enrollmentDate"
            :institution-id="selectedStudentClass.classGroup ? selectedStudentClass.classGroup.institutionId : undefined"
            :basic-class="undefined"
            :min-basic-class="selectedStudentClass.classGroup ? selectedStudentClass.classGroup.basicClassId : undefined"
            is-student-class-cahange
          />

          <student-class-dual-form-company-manager
            v-if="hasDualFormCompanyManagePermission"
            v-model="model"
            class="my-2"
          />

          <student-class-additional-data
            v-model="model"
            class="my-2"
          />

          <student-class-supportive-environment
            :key="supportiveEnvironmentComponentKey"
            v-model="model"
            class="my-2"
          />
        </v-form>
      </template>
    </form-layout>

    <v-overlay :value="sending">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </div>
</template>

<script>
import StudentClassDetails from '@/components/students/class/StudentClassDetails.vue';
import StudentClassBasicData from '@/components/students/class/StudentClassBasicData.vue';
import StudentClassSupportiveEnvironment from '@/components/students/class/StudentClassSupportiveEnvironment.vue';
import StudentClassAdditionalData from '@/components/students/class/StudentClassAdditionalData.vue';
import StudentClassDualFormCompanyManager from '@/components/students/class/StudentClassDualFormCompanyManager.vue';

import { StudentClassEnrollModel } from '@/models/studentClass/studentClassEnrollModel';
import { StudentClassDualFormCompanyModel } from '@/models/studentClass/studentClassDualFormCompanyModel';

import { Permissions } from '@/enums/enums';
import { mapGetters } from 'vuex';

export default {
  name: 'StudentClassChange',
  components: {
    StudentClassDetails,
    StudentClassSupportiveEnvironment,
    StudentClassAdditionalData,
    StudentClassBasicData,
    StudentClassDualFormCompanyManager
  },
  props: {
    pid: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      model: new StudentClassEnrollModel(),
      sending: false,
      supportiveEnvironmentComponentKey: 0
    };
  },
  computed: {
    ...mapGetters(['selectedStudentClass', 'hasStudentPermission', 'dualEduFormId', 'dualClassTypeId']),
    hasDualFormCompanyManagePermission() {
      return [11,12].includes(this.model?.classGroup?.basicClassId)
        && this.model?.classGroup?.classTypeId == this.dualClassTypeId
        && this.model?.studentEduFormId === this.dualEduFormId;
    }
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentToClassEnrollment)) {
      return this.$router.push('/errors/AccessDenied');
    }

    if(!this.selectedStudentClass) {
      this.$notifier.error('', this.$t('errors.currentStudentClassMissing'), 5000);
      return this.$router.go(-1);
    }

    if(this.pid !== this.selectedStudentClass.personId) {
      console.log('Разминаване в pid на текущата паралелка и избрания ученик.');
      this.$notifier.error('', this.$t('errors.accessDenied'), 5000);
      return this.$router.go(-1);
    }
    this.model.basicClassId = this.selectedStudentClass.basicClassId;
    this.model.classId = this.selectedStudentClass.classId;
    this.model.studentEduFormId = this.selectedStudentClass.studentEduFormId;
    this.model.studentSpecialityId = this.selectedStudentClass.studentSpecialityId;
    this.model.studentProfessionId = this.selectedStudentClass.studentProfessionId;
    this.model.classGroup = this.selectedStudentClass.classGroup;
    this.model.schoolYear = this.selectedStudentClass.schoolYear;
    this.model.commuterTypeId = this.selectedStudentClass.commuterTypeId;
    this.model.repeaterId = this.selectedStudentClass.repeaterId;
    this.model.internationalProtectionStatus = this.selectedStudentClass.internationalProtectionStatus;
    this.model.hasIndividualStudyPlan = this.selectedStudentClass.hasIndividualStudyPlan;
    this.model.isNotForSubmissionToNra = this.selectedStudentClass.isNotForSubmissionToNra;
    this.model.isHourlyOrganization = this.selectedStudentClass.isHourlyOrganization;
    this.model.hasSupportiveEnvironment = this.selectedStudentClass.hasSupportiveEnvironment;
    this.model.supportiveEnvironment = this.selectedStudentClass.supportiveEnvironment;
    this.model.currentStudentClassId = this.selectedStudentClass.id;
    this.model.personId = this.selectedStudentClass.personId;
    this.model.specialEquipment = this.selectedStudentClass.specialEquipment;
    this.model.availableArchitecture = this.selectedStudentClass.availableArchitecture;
    this.model.buildingAreas = this.selectedStudentClass.buildingAreas;
    this.model.buildingRooms = this.selectedStudentClass.buildingRooms;
    this.model.oresTypeId = this.selectedStudentClass.oresTypeId;
    this.model.selectedClassTypeId = this.selectedStudentClass.selectedClassTypeId;
    this.supportiveEnvironmentComponentKey++;

    this.loadStudentClassDualFormCompanies(this.selectedStudentClass.id);

  },
  methods: {
    onSave() {
      const form = this.$refs['form' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      }

      this.model.enrollmentDate = this.$helper.parseDateToIso(this.model.enrollmentDate ?? '');
      this.model.entryDate = this.$helper.parseDateToIso(this.model.entryDate ?? '');
      this.model.dischargeDate = this.$helper.parseDateToIso(this.model.dischargeDate ?? '');

      if(this.model.dualFormCompanies?.length > 0) {
        this.model.dualFormCompanies.forEach(x => {
          x.startDate = this.$helper.parseDateToIso(x.startDate ?? '');
          x.endDate = this.$helper.parseDateToIso(x.endDate ?? '');
        });
      }

      this.sending = true;
      this.$api.studentClass
        .changeClassInInstitution(this.model)
        .then(() => {
          this.$studentEventBus.$emit('studentMovementUpdate', this.model.personId);
          this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          this.$router.go(-1);
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.classSave'), 5000);
          console.log(error.response.data.message);
        })
        .then(() => { this.sending = false; });
    },
    onCancel() {
      this.$router.go(-1);
    },
    loadStudentClassDualFormCompanies(studentClassId) {
      this.$api.studentClass
        .getDualFormCompanies(studentClassId)
        .then((response) => {
          if(response.data) {
            this.model.dualFormCompanies = response.data.map(x => new StudentClassDualFormCompanyModel(x, this.$moment));
            if(this.model.dualFormCompanies?.length > 0) {
              this.model.dualFormCompanies.forEach(x => {
                x.id = null;
              });
            }
          }
        })
        .catch((error) => {
          console.log(error.response);
        });
    }
  }
};
</script>
