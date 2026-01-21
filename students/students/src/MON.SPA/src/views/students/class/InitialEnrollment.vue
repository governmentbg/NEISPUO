<template>
  <div>
    <v-form
      ref="form"
      class="text-left pt-6"
    >
      <v-card>
        <v-card-title
          v-if="userInfo"
        >
          {{ $t('enroll.title', { institution: userInfo.institution }) }}
        </v-card-title>
        <v-card-text>
          <student-class-basic-data
            v-model="form"
            class="my-2"
            :min-school-year="currentYear"
            :max-school-year="currentYear + 1"
            :institution-id="userInfo ? userInfo.institutionId : undefined"
            :basic-class="currentStudentClass && currentStudentClass.classGroup ? currentStudentClass.classGroup.basicClassId : undefined"
            is-initial-enrollment
          />

          <student-class-dual-form-company-manager
            v-if="hasDualFormCompanyManagePermission"
            v-model="form"
            class="my-2"
          />

          <student-class-additional-data
            v-model="form"
            class="my-2"
            :base-school-type-id="baseSchoolTypeId"
            :inst-type-id="instTypeId"
          />

          <student-class-supportive-environment
            v-model="form"
            class="my-2"
          />
        </v-card-text>

        <v-card-actions class="justify-end">
          <v-btn
            ref="submit"
            raised
            color="primary"
            :disabled="sending"
            @click="onConfirm"
          >
            {{ $t('buttons.save') }}
          </v-btn>

          <v-btn
            raised
            color="error"
            @click="onReset"
          >
            {{ $t('buttons.clear') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-form>
    <v-overlay :value="sending">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </div>
</template>

<script>
import UserService from '@/services/user.service.js';
import StudentClassSupportiveEnvironment from '@/components/students/class/StudentClassSupportiveEnvironment.vue';
import StudentClassAdditionalData from '@/components/students/class/StudentClassAdditionalData.vue';
import StudentClassBasicData from '@/components/students/class/StudentClassBasicData.vue';
import StudentClassDualFormCompanyManager from '@/components/students/class/StudentClassDualFormCompanyManager.vue';

import { StudentClass } from '@/models/studentClass/studentClass';
import { UserInfo } from '@/models/account/userInfo';
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: "InitialEnrollmentInStudentClass",
  components:{
    StudentClassSupportiveEnvironment,
    StudentClassAdditionalData,
    StudentClassBasicData,
    StudentClassDualFormCompanyManager
  },
  props: {
    pid: {
      type: Number,
      required: true
    },
    admissionDocumentId: {
      type: Number,
      default() {
        return null;
      }
    },
    initialEnrollmentPosition: {
      type: Number,
      default() {
        return null;
      }
    }
  },
  data() {
      return {
          form: new StudentClass(),
          classGroupsOptions: [],
          personId: undefined,
          sending: false,
          userInfo: null,
          baseSchoolTypeId: undefined,
          instTypeId: undefined,
          admissionDocument: null,
          currentStudentClass: null,
          currentYear: undefined
      };
  },
  computed: {
    ...mapGetters(['hasStudentPermission', 'dualEduFormId', 'dualClassTypeId']),
    hasDualFormCompanyManagePermission() {
      return [11,12].includes(this.form?.classGroup?.basicClassId)
        && this.form?.classGroup?.classTypeId == this.dualClassTypeId
        && this.form?.studentEduFormId === this.dualEduFormId;
    }
  },
  async mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentToClassEnrollment)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.form.personId = this.pid;
    this.form.initialEnrollmentPosition = this.initialEnrollmentPosition;
    await UserService.getUserInfo().then(async (data) =>
      {
        this.userInfo = new UserInfo(data.data);
        this.baseSchoolTypeId = this.userInfo.baseSchoolTypeId;
        this.instTypeId = this.userInfo.instTypeId;
        await this.getAdditionalDataFromApi();
      }
    );
  },
  methods: {
    onConfirm() {
      const isValid = this.$refs.form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      }

      const payload = {
        personId: this.form.personId,
        schoolYear: this.form.schoolYear,
        hasIndividualStudyPlan: this.form.hasIndividualStudyPlan,
        isHourlyOrganization: this.form.isHourlyOrganization,
        IsFTACOutsourced : this.form.IsFTACOutsourced,
        commuterTypeId: this.form.commuterTypeId,
        repeaterId: this.form.repeaterId,
        classId: this.form.classId,
        hasSupportiveEnvironment: this.form.hasSupportiveEnvironment,
        supportiveEnvironment: this.form.supportiveEnvironment,
        admissionDocumentId: this.form.admissionDocumentId,
        initialEnrollmentPosition: this.form.initialEnrollmentPosition,
        specialEquipment: this.form.specialEquipment,
        isNotForSubmissionToNra: this.form.isNotForSubmissionToNra,
        availableArchitecture: this.form.availableArchitecture,
        buildingAreas: this.form.buildingAreas,
        buildingRooms: this.form.buildingRooms,
        studentSpecialityId: this.form.studentSpecialityId,
        selectedClassTypeId: this.form.selectedClassTypeId,
        studentProfessionId: this.form.studentProfessionId,
        studentEduFormId: this.form.studentEduFormId,
        basicClassId: this.form.classGroup?.basicClassId,
        enrollmentDate: this.$helper.parseDateToIso(this.form.enrollmentDate, ''),
        entryDate: this.$helper.parseDateToIso(this.form.entryDate, '')
      };

      this.sending = true;
      this.$api.studentClass
        .enrollInClass(payload)
        .then(() => {
          this.$studentEventBus.$emit('studentMovementUpdate', payload.personId);
          this.$notifier.success('', this.$t('common.saveSuccess'));
          this.$router.push({ name: 'StudentDetails', params: { id: this.pid } });
          this.resetForm();
        })
        .catch((error) => {
          this.$notifier.error(this.$t('errors.classSave'), error.response.data.message);
          console.log(error.response.data.message);
        })
        .then(() => { this.sending = false; });
    },
    resetForm() {
      this.form = new StudentClass();
    },
    onReset() {
      this.$refs.form.reset();
    },
    async getAdditionalDataFromApi() {
      this.form.admissionDocumentId = this.admissionDocumentId;

      if (this.admissionDocumentId) {
        this.admissionDocument = (await this.$api.admissionDocument.getById(this.admissionDocumentId)).data;

        if (this.admissionDocument?.admissionDate != null) {
          this.form.enrollmentDate = this.admissionDocument?.admissionDate;
        }

        if (this.admissionDocument?.relocationDocument?.currentStudentClass != null){
          this.currentStudentClass = (await this.$api.studentClass.getById(this.admissionDocument?.relocationDocument?.currentStudentClass)).data;
        }
      }

      this.$api.institution.getCurrentYear(this.userInfo.institutionId).then(
        (currentYear) => {
          this.currentYear = currentYear.data;
          this.form.schoolYear = currentYear.data;
        }
      );
    },
  }
};
</script>
