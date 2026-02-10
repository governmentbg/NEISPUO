<template>
  <div>
    <form-layout
      v-if="massEnrollmentSelectorVisible"
      class="my-2"
      @on-save="onMassEnrollmentSave"
      @on-cancel="onMassEnrollmentCancel"
    >
      <template #title>
        {{ $t("common.enroll") }}
      </template>
      <student-class-enrollent-person-selector
        :ref="'studentClassMassEnrollmentForm' + _uid"
        :value="massEnrollmentSelectedPersons"
        :enrollment-form="massEnrollmentForm"
        :disabled="saving"
      />
    </form-layout>
    <v-card>
      <v-card-title>
        {{ $t('institution.classSubtitle') }}
        <v-spacer />
        <v-text-field
          v-model="search"
          append-icon="mdi-magnify"
          :label="$t('common.search')"
          single-line
          hide-details
        />
      </v-card-title>
      <v-card-text>
        <v-data-table
          v-model="selected"
          :headers="selectedHeaders"
          :items="students"
          item-key="personId"
          :loading="loading"
          :search="search"
          :footer-props="{ itemsPerPageOptions: gridItemsPerPageOptions }"
          hide-default-footer
          disable-pagination
          show-select
          class="elevation-1"
        >
          <template v-slot:top>
            <v-row class="mr-1">
              <v-col cols="6">
                <GridExporter
                  v-if="classGroup"
                  :items="students"
                  :file-extensions="['xlsx', 'csv', 'txt']"
                  :file-name="classGroup.className + ' клас'"
                  :headers="headers"
                />
              </v-col>
              <v-spacer />
              <v-col
                cols="6"
                class="text-right"
              >
                <button-group v-if="hasStudentClassMassEnrolmentManagePermission && hasClassManagePermission">
                  <button-tip
                    v-if="!massEnrollmentSelectorVisible"
                    icon
                    icon-name="fas fa-long-arrow-alt-right"
                    icon-color="success"
                    iclass=""
                    tooltip="common.enroll"
                    bottom
                    small
                    raised
                    @click="massEnrollmentSelectorVisible = true"
                  />
                  <button-tip
                    v-if="lodFinalizationStudents && lodFinalizationStudents.length > 0"
                    icon
                    icon-name="mdi-logout"
                    icon-color="error"
                    iclass=""
                    tooltip="studentClass.unenrollSelected"
                    bottom
                    small
                    raised
                    @click="onMassUnenroll()"
                  />
                </button-group>
                <lod-finalization
                  v-if="hasLodStateManagePermission && classGroup"
                  :school-year="schoolYear"
                  :selected-students="lodFinalizationStudents"
                  :show-generate-lod="true"
                  :lod-file-name="classGroup.className + ' клас'"
                  :is-prev-school-year-class="classGroup.isCurrent !== true"
                  :class-id="classGroup.isCurrent !== true ? classGroup.id : null"
                  class="mx-3"
                  @lodFinalizationEnded="lodFinalizationEnded"
                />
                <button-group>
                  <button-tip
                    v-if="hasClassEnrollmentPermission && classGroup
                      && ((isCplrInstitution && classGroup.classKindId == ClassKind.Other) || (!isCplrInstitution && ([ClassKind.Basic, ClassKind.Cdo, ClassKind.Other].includes(classGroup.classKindId) || classGroup.isNotPresentForm === true)))"
                    icon
                    icon-name="fas fa-sign-in-alt"
                    icon-color="primary"
                    iclass=""
                    small
                    tooltip="common.enroll"
                    bottom
                    raised
                    :to="`/class/${classId}/enroll?schoolYear=${schoolYear}`"
                  />
                  <button-tip
                    v-if="hasClassAbsenceReadPermission && classGroup && classGroup.classKindId === 1"
                    icon
                    icon-name="fa-calendar-times"
                    icon-color="primary"
                    iclass=""
                    small
                    tooltip="absence.absence"
                    bottom
                    raised
                    @click="onShowAbsences()"
                  />
                  <button-tip
                    v-if="hasOresReadPermission && classGroup && classGroup.classKindId === 1"
                    icon
                    icon-name="fas fa-viruses"
                    icon-color="primary"
                    iclass=""
                    small
                    tooltip="menu.ores"
                    bottom
                    raised
                    :to="`/ores?classId=${classId}`"
                  />
                  <!-- Да се махне автоматичното номериране на децата в клас #1048 -->
                  <!-- <button-tip
                    v-if="hasClassManagePermission"
                    icon
                    icon-name="fa-sort-numeric-down"
                    icon-color="primary"
                    iclass=""
                    small
                    tooltip="buttons.autoNumber"
                    bottom
                    raised
                    @click="onAutoNumber()"
                  /> -->
                  <!-- <button-tip
                    v-if="hasClassStudentsReadPermission"
                    icon
                    icon-name="fa-print"
                    icon-color="primary"
                    iclass=""
                    small
                    tooltip="buttons.print"
                    bottom
                    raised
                    @click="onPrint()"
                  /> -->
                </button-group>
              </v-col>
            </v-row>
          </template>

          <template v-slot:[`item.pin`]="{ item }">
            {{ `${item.pin} - ${item.pinType}` }}
          </template>

          <template v-slot:[`item.isLodApproved`]="{ item }">
            <v-chip
              :color="item.isLodApproved === true ? 'success' : 'light'"
              outlined
              small
            >
              <yes-no :value="item.isLodApproved" />
            </v-chip>
          </template>

          <template v-slot:[`item.usernames`]="{ item }">
            <v-chip
              v-for="(username, index) in item.usernamesList"
              :key="index"
              color="light"
              outlined
              small
              class="mr-1"
            >
              {{ username }}
            </v-chip>
          </template>

          <template v-slot:[`item.initialPasswords`]="{ item }">
            <v-chip
              v-for="(password, index) in item.initialPasswordsList"
              :key="index"
              color="light"
              outlined
              small
              class="mr-1"
            >
              {{ password }}
            </v-chip>
          </template>

          <template v-slot:[`item.isLodFinalized`]="{ item }">
            <v-chip
              :color="item.isLodFinalized === true ? 'success' : 'light'"
              outlined
              small
            >
              <yes-no :value="item.isLodFinalized" />
            </v-chip>
          </template>

          <template v-slot:[`item.lodNotFinalizationYearsList`]="{ item }">
            <v-chip
              v-for="(lodFinalizationSchoolYear, index) in item.lodNotFinalizationYearsList"
              :key="index"
              :color="userDetails && userDetails.schoolYearName && userDetails.schoolYearName === lodFinalizationSchoolYear ? 'light' : 'error'"
              outlined
              small
              class="mr-1"
              :to="`/student/${item.personId}/lodFinalizations`"
            >
              {{ lodFinalizationSchoolYear }}
            </v-chip>
          </template>

          <template v-slot:[`item.actions`]="{ item }">
            <button-group>
              <button-tip
                v-if="(classGroup?.isCurrent ?? false)"
                icon
                icon-name="mdi-eye"
                icon-color="primary"
                tooltip="buttons.details"
                bottom
                iclass=""
                small
                :to="`/student/${item.personId}/details`"
              />
              <button-tip
                v-if="item.document && item.document.blobId"
                icon
                icon-name="mdi-file-word"
                icon-color="primary"
                :tooltip="item.document.noteFileName"
                bottom
                iclass=""
                small
                :href="`${item.document.blobServiceUrl}/${item.document.blobId}?t=${item.document.unixTimeSeconds}&h=${item.document.hmac}`"
              />
            </button-group>
          </template>

          <template slot="body.append">
            <tr>
              <th
                class="title text-right"
                colspan="3"
              >
                Общо {{ students.length }} ученици
              </th>
            </tr>
          </template>
        </v-data-table>
        <confirm-dlg ref="confirm" />
      </v-card-text>
      <v-overlay :value="saving">
        <v-progress-circular
          indeterminate
          size="64"
        />
      </v-overlay>
    </v-card>
    <prompt-dlg ref="unenrollmentPrompt">
      <template>
        <date-picker
          v-model="selectedDischargeDate"
          :show-buttons="false"
          :scrollable="false"
          :no-title="true"
          :show-debug-data="false"
          :rules="[$validator.required()]"
          class="required"
        />
        <v-alert
          border="left"
          colored-border
          type="info"
          elevation="2"
        >
          {{ $t('enroll.unenrollmentDischargeDateInfo', { enrollmentDate: '' }) }}
        </v-alert>
      </template>
    </prompt-dlg>
  </div>
</template>

<script>
import GridExporter from '@/components/wrappers/gridExporter';
import LodFinalization from '../lod/LodFinalization.vue';
import StudentClassEnrollentPersonSelector from '@/components/institution/StudentClassEnrollentPersonSelector';
import { Permissions, InstType, ClassKind } from '@/enums/enums';
import { mapGetters } from 'vuex';
import { LodFinalizationStudentModel } from '@/models/lodModels/lodFinalizationStudentModel.js';
import { StudentClass } from '@/models/studentClass/studentClass';
import ButtonGroup from '../wrappers/ButtonGroup.vue';

export default {
  name: 'ClassGroupDetails',
  components: {
    GridExporter,
    LodFinalization,
    StudentClassEnrollentPersonSelector,
    ButtonGroup
  },
  props: {
    classId: {
      type: Number,
      required: true
    },
    schoolYear: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      classGroup: null,
      students: [],
      search: '',
      selected: [],
      loading: false,
      saving: false,
      selectedDischargeDate: null,
      massEnrollmentSelectorVisible: false,
      massEnrollmentSelectedPersons: [],
      massEnrollmentForm: new StudentClass(),
      lodFinalizationStudents: [],
      headers: [
        {
          text: this.$t('student.headers.firstName'),
          value: 'fullName'
        },
        {
          text: this.$t('student.headers.identifier'),
          value: 'pin'
        },
        {
          text: this.$t('student.headers.username'),
          value: 'usernames'
        },
        {
          text: this.$t('student.headers.initialPassword'),
          value: 'initialPasswords'
        },
        {
          text: this.$t('student.headers.lodApproval'),
          value: 'isLodApproved'
        },
        {
          text: this.$t('student.headers.lodFinalization'),
          value: 'isLodFinalized'
        },
        {
          text: this.$t('student.headers.lodNotFinalizationYearsList'),
          value: 'lodNotFinalizationYearsList',
          sortable: false
        },
        {
          text: this.$t('institutionStudents.headers.mainClassName'),
          value: 'mainClassName',
          sortable: false
        },
        { text: '', value: 'actions', sortable: false, align: 'end' },
      ],
      ClassKind
    };
  },
  computed: {
    ...mapGetters(['gridItemsPerPageOptions', 'hasClassPermission', 'turnOnOresModule', 'userDetails', 'userInstitutionId', 'isInstType']),
    isCplrInstitution() {
      return this.isInstType(InstType.CPLR) || this.isInstType(InstType.SOZ);
    },
    hasClassEnrollmentPermission() {
      return (this.classGroup?.isCurrent ?? false) && this.hasClassPermission(Permissions.PermissionNameForStudentToClassEnrollment);
    },
    hasClassAbsenceReadPermission() {
      return (this.classGroup?.isCurrent ?? false) && this.hasClassPermission(Permissions.PermissionNameForClassAbsenceRead);
    },
    hasClassManagePermission() {
      return this.hasClassPermission(Permissions.PermissionNameForClassManage);
    },
    hasStudentClassMassEnrolmentManagePermission() {
      return (this.classGroup?.isCurrent ?? false) &&this.hasClassPermission(Permissions.PermissionNameForStudentClassMassEnrolmentManage);
    },
    hasClassStudentsReadPermission() {
      return this.hasClassPermission(Permissions.PermissionNameForClassStudentsRead);
    },
    hasLodStateManagePermission() {
      return this.hasClassPermission(Permissions.PermissionNameForLodStateManage);
    },
    hasOresReadPermission() {
      return (this.classGroup?.isCurrent ?? false) && this.turnOnOresModule && this.hasClassPermission(Permissions.PermissionNameForOresRead);
    },
    selectedHeaders() {
      if(this.classGroup && this.classGroup.classKindId !== 1) {
        return this.headers;
      }

      return this.headers.filter(x => x.value !== 'mainClassName');
    }
  },
  watch: {
    selected() {
      this.lodFinalizationStudents = this.selected.map(el => {
        return new LodFinalizationStudentModel(el);
      });
    }
  },
  async mounted() {
    if (isNaN(this.schoolYear)) {
      this.schoolYear = Number((await this.$api.institution.getCurrentYear(this.userInstitutionId))?.data);
    }
    this.load();
  },
  methods: {
    load() {
      this.loadClassDetails();
      this.loadClassStudents();
    },
    loadClassDetails() {
      this.$api.classGroup.getById(this.classId)
        .then(response => {
          if (response.data) {
            this.classGroup = response.data;
            if (!this.schoolYear) {
              this.schoolYear == this.classGroup.schoolYear;
            }
          }
        })
        .catch(error => {
          this.$notifier.error('', this.$t('errors.load'));
          console.log(error.response);
        });
    },
    loadClassStudents() {
      this.loading = true;

      this.$api.classGroup.getStudents(this.classId)
        .then(response => {
          if (response.data) {
            this.students = response.data;
          }
        })
        .catch(error => {
          this.$notifier.error('', this.$t('errors.load'));
          console.log(error.response);
        })
        .finally(() => {
          this.loading = false;
        });
    },
    onShowAbsences() {
      this.$router.push({ name: 'AbsenceClass', params: { id: this.classId }, query: { schoolYear: this.schoolYear } });
    },
    async onAutoNumber() {
      if (await this.$refs.confirm.open('Автоматична номерация', this.$t('common.confirm'))) {
        await this.$api.institution.autoNumber(this.classId);
        this.$notifier.success('', this.$t('common.saveSuccess'));
        this.load();
      }
    },
    onPrint() {
      window.print();
    },
    lodFinalizationEnded() {
      this.selected = [];
      this.load();
    },
    lodFinalizationLoading(value) {
      this.loading = value;
    },
    async onMassUnenroll() {
      if (!this.lodFinalizationStudents) {
        return this.$notifier.error('', this.$t('addCurriculum.missingSelection'));
      }

      if (await this.$refs.unenrollmentPrompt.open('', this.$t('documents.dischargeDateLabel'))) {
        if (!this.selectedDischargeDate) {
          return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
        }

        this.saving = true;
        this.$api.studentClass.unenrollSelected({
          classId: this.classId,
          dischargeDate: this.$helper.parseDateToIso(this.selectedDischargeDate, ''),
          selectedStudents: this.lodFinalizationStudents.map((item) => { return item.personId; })
        })
          .then(() => {
            this.$notifier.success(this.$t('studentClass.unenrollSelected'), this.$t('common.saveSuccess'));
            this.loadClassStudents();
            this.selected = [];
          })
          .catch(error => {
            this.$notifier.error(this.$t('studentClass.unenrollSelected'), this.$t('common.saveError'));
            console.log(error.response);
          })
          .finally(() => {
            this.saving = false;
            this.selectedDischargeDate = null;
          });
      } else {
        this.selectedDischargeDate = null;
      }
    },
    async onMassEnrollmentSave() {
      if (!this.massEnrollmentSelectedPersons || this.massEnrollmentSelectedPersons.length === 0) {
        return this.$notifier.error('', this.$t('addCurriculum.missingSelection'));
      }

      const form = this.$refs['studentClassMassEnrollmentForm' + this._uid];
      const isValid = form.validate();

      if (!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      }

      if (await this.$refs.confirm.open(this.$t('common.enroll'), this.$t('common.confirm'))) {
        this.saving = true;
        this.$api.studentClass.enrollSelected({
          classId: this.classId,
          selectedStudents: this.massEnrollmentSelectedPersons.map((item) => { return item.personId; }),
          enrollmentDate: this.$helper.parseDateToIso(this.massEnrollmentForm.enrollmentDate, '')
        })
          .then(() => {
            this.$notifier.success(this.$t('common.enroll'), this.$t('common.saveSuccess'));
            this.onMassEnrollmentCancel();
            this.loadClassStudents();
          })
          .catch(error => {
            this.$notifier.error(this.$t('common.enroll'), this.$t('common.saveError'));
            console.log(error.response);
          })
          .finally(() => {
            this.saving = false;
          });
      }
    },
    onMassEnrollmentCancel() {
      this.massEnrollmentSelectorVisible = false;
      this.massEnrollmentForm = new StudentClass();
      this.$helper.clearArray(this.massEnrollmentSelectedPersons);
    }
  }
};
</script>
