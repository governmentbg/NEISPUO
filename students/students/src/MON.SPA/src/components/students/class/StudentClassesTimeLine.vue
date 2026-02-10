<template>
  <div>
    <v-timeline
      v-if="classes.length > 0"
    >
      <v-timeline-item
        v-for="(c, i) in classes"
        :key="i"
        small
      >
        <template v-slot:opposite>
          <span
            class="headline font-weight-bold"
            v-text="c.schoolYearName"
          />
          <div
            class="text--secondary"
            v-text="$moment(c.enrollmentDate).format(dateFormat)"
          />
        </template>

        <v-card
          class="elevation-2"
        >
          <v-card-title
            class="pt-0 pb-0"
          >
            <v-col class="pt-3 pb-0">
              <span
                v-if="c.isCurrent"
              >
                <v-tooltip bottom>
                  <template v-slot:activator="{ on }">
                    <v-chip
                      :color="c.isBasicClassForCurrentInstitution ? 'success' : 'default'"
                      v-on="on"
                    >
                      {{ c.classGroup.className }}
                    </v-chip>
                  </template>
                  <span>
                    {{ $t('documents.currentStudentClass') }}
                  </span>
                </v-tooltip>
              </span>
              <span
                v-else
              >
                {{ c.classGroup.className }}
              </span>
            </v-col>
            <v-col class="text-right">
              <button-tip
                v-if="c.hasHistory && hasStudentClassHistoryReadPermission"
                icon
                icon-name="fas fa-history"
                icon-color="info"
                iclass=""
                tooltip="common.history"
                bottom
                small
                :to="`/student/${pid}/class/${c.id}/history`"
              />
            </v-col>
          </v-card-title>
          <v-card-text
            class="pa-0"
          >
            <student-class-details
              :value="c"
            />
          </v-card-text>
          <v-card-actions
            v-if="c.isCurrent && c.classGroup.institutionId === userInstitutionId"
            class="mt-0 pt-0"
          >
            <v-alert
              v-if="hasStudentClassUpdatePermission && c.hasExternalSoProvider === true"
              border="left"
              colored-border
              type="warning"
              elevation="2"
              class="ma-2"
            >
              {{ $t('common.externalSoProviderActionLimitation') }}
            </v-alert>
            <v-spacer />
            <button-group>
              <!-- Бутонът за редакция е видим и активен, ако имаме права, паралелката е текуща, служебна или не е избран външен доставчик на СО.
              Ако е избран външен доставчик на СО не трябва ClassType-а на паралелката да е ограничен за редакция в AppSettings.ExternalSoProviderClassTypeEnrollmentLimitation -->
              <button-tip
                v-if="hasStudentClassUpdatePermission && c.isCurrent === true"
                icon
                icon-name="mdi-pencil"
                icon-color="success"
                iclass=""
                tooltip="buttons.edit"
                bottom
                small
                :disabled="c.hasExternalSoProvider === true && c.isNotPresentForm === false && c.hasExternalSoProviderClassTypeLimitation == true"
                @click="onEdit(c)"
              />
              <button-tip
                v-if="hasStudentToClassEnrollPermission && c.isCurrent === true && c.isBasicClassForCurrentInstitution === true"
                icon
                icon-name="mdi-swap-horizontal-bold"
                icon-color="success"
                iclass=""
                :tooltip="c.isAdditionalClass === true ? 'enroll.moveToAdditionalClass' : 'enroll.move'"
                bottom
                small
                @click="onMoveToClass(c)"
              />
              <!-- Бутонът за отписване е видим и активен, ако имаме права, паралелката е текуща, неучебна и не е избран външен доставчик на СО.
              Ако е избран външен доставчик на СО не трябва ClassType-а на паралелката да е ограничен за редакция в AppSettings.ExternalSoProviderClassTypeEnrollmentLimitation -->
              <button-tip
                v-if="hasStudentClassUpdatePermission && c.isCurrent === true && c.isAdditionalClass === true"
                icon
                icon-name="mdi-logout"
                icon-color="success"
                iclass=""
                tooltip="common.unenroll"
                bottom
                small
                :disabled="c.hasExternalSoProvider === true && c.hasExternalSoProviderClassTypeLimitation == true"
                @click="onUnenroll(c)"
              />
              <button-tip
                v-if="hasStudentCurriculumReadPermission && c.isCurrent === true && c.canAddCurriculum === true"
                icon
                icon-name="mdi-book-open-blank-variant"
                icon-color="success"
                iclass=""
                tooltip="addCurriculum.addCurriculumBtn"
                bottom
                small
                :disabled="c.hasExternalSoProvider === true"
                @click="onAddCurriculum(c)"
              />
              <button-tip
                v-if="hasStudentClassUpdatePermission && c.canBeDeleted === true"
                icon
                icon-name="mdi-delete"
                icon-color="red"
                iclass=""
                tooltip="common.delete"
                bottom
                small
                @click="onDelete(c)"
              />
              <button-tip
                v-if="hasDataReferencesReadPermission"
                icon
                icon-name="mdi-vector-link"
                icon-color="secondary"
                iclass=""
                tooltip="buttons.dataReferences"
                bottom
                small
                @click="dataReferencesBtnClick(c.id)"
              />
              <button-tip
                v-if="hasStudentClassUpdatePermission && c.canChangePosition === true"
                icon
                icon-name="mdi-undo"
                icon-color="primary"
                iclass=""
                tooltip="studentClass.changePositionBtn"
                bottom
                small
                @click="changePositionBtnClick(c)"
              />
            </button-group>
          </v-card-actions>
          <v-card-actions
            v-else
            class="mt-0 pt-0"
          >
            <v-spacer />
            <button-group>
              <button-tip
                v-if="hasStudentCurriculumReadPermission && c.isCurrent === true && c.canAddCurriculum === true"
                icon
                icon-name="mdi-book-open-blank-variant"
                icon-color="success"
                iclass=""
                tooltip="addCurriculum.addCurriculumBtn"
                bottom
                small
                :disabled="c.hasExternalSoProvider === true"
                @click="onAddCurriculum(c)"
              />
              <button-tip
                v-if="hasStudentClassUpdatePermission && c.canBeDeleted === true"
                icon
                icon-name="mdi-delete"
                icon-color="red"
                iclass=""
                tooltip="common.delete"
                bottom
                small
                @click="onDelete(c)"
              />
              <button-tip
                v-if="hasDataReferencesReadPermission"
                icon
                icon-name="mdi-vector-link"
                icon-color="secondary"
                iclass=""
                tooltip="buttons.dataReferences"
                bottom
                small
                @click="dataReferencesBtnClick(c.id)"
              />
            </button-group>
          </v-card-actions>
        </v-card>
      </v-timeline-item>
    </v-timeline>
    <v-card
      v-else
      class="mt-1"
    >
      <v-card-title class="justify-center">
        <h3 class="headline">
          {{ $t('common.noData') }}
        </h3>
      </v-card-title>
    </v-card>
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
    <v-dialog
      v-model="dataReferencesDialog"
    >
      <v-card>
        <v-toolbar
          color="info"
          outlined
        >
          <v-btn
            icon
            dark
            @click="dataReferencesDialog = false"
          >
            <v-icon>mdi-close</v-icon>
          </v-btn>
          <v-spacer />
          <v-toolbar-items>
            <v-btn
              dark
              text
              @click="dataReferencesDialog = false"
            >
              {{ $t('buttons.close') }}
            </v-btn>
          </v-toolbar-items>
        </v-toolbar>
        <v-card-text>
          <vue-json-pretty
            :data="dataReferences"
            show-length
            show-line
            show-icon
            :deep="3"
          />
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn
            raised
            color="light"
            @click.stop="dataReferencesDialog = false"
          >
            <v-icon left>
              mdi-close
            </v-icon>
            {{ $t('buttons.close') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
    <confirm-dlg ref="confirm" />
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
          {{ $t('enroll.unenrollmentDischargeDateInfo', { enrollmentDate: selectedEnrollmentDate ? $moment(selectedEnrollmentDate).format(dateFormat) : '' }) }}
        </v-alert>
      </template>
    </prompt-dlg>
    <v-dialog
      v-model="changePositionDialog"
    >
      <form-layout
        skip-cancel-prompt
        @on-save="onChangePositionSave"
        @on-cancel="onChangePositionCancel"
      >
        <template #title>
          <h3>{{ $t('studentClass.changePositionBtn') }}</h3>
        </template>
        <template>
          <student-class-details
            :value="changePositionModel?.studentClass"
          />

          <v-form
            v-if="changePositionModel"
            :ref="'changePositionForm' + _uid"
          >
            <v-card>
              <v-card-text>
                <v-row>
                  <v-col>
                    <c-autocomplete
                      v-model="changePositionModel.positionId"
                      api="/api/lookups/GetStudentPositionOptions"
                      :label="$t('common.newPosition')"
                      :rules="[$validator.required()]"
                      class="required"
                      :defer-options-loading="false"
                      hide-details
                      append-icon=""
                      persistent-placeholder
                      readonly
                    />
                  </v-col>
                </v-row>
              </v-card-text>
            </v-card>
          </v-form>
        </template>
      </form-layout>
    </v-dialog>
  </div>
</template>

<script>

import StudentClassDetails from '@/components/students/class/StudentClassDetails.vue';
import { StudentClass } from '@/models/studentClass/studentClass.js';
import { Permissions, ClassKind } from '@/enums/enums';
import { mapGetters, mapActions } from 'vuex';
import Constants from '@/common/constants';
import VueJsonPretty from 'vue-json-pretty';
import 'vue-json-pretty/lib/styles.css';
import { UserInfo } from '@/models/account/userInfo';
import UserService from '@/services/user.service.js';
import CAutocomplete from '@/components/wrappers/CustomAutocomplete.vue';

export default {
  name: 'StudentClassesTimeLine',
  components: {
    StudentClassDetails,
    VueJsonPretty,
    CAutocomplete
  },
  props: {
    pid: {
      type: Number,
      default() {
        return null;
      }
    },
    positions: {
      type: Array,
      default() {
        return [];
      }
    }
  },
  data() {
    return {
      classes: [],
      ClassKind,
      saving: false,
      loading: false,
      dateFormat: Constants.DATEPICKER_FORMAT,
      dataReferencesDialog: false,
      dataReferences: '',
      selectedDischargeDate: null,
      selectedEnrollmentDate: null,
      userInfo: null,
      instTypeId: undefined,
      defaultPositions: [...this.positions],
      mounted: false,
      changePositionDialog: false,
      changePositionModel: null
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission', 'hasInstitutionPermission', 'hasPermission', 'userInstitutionId']),
    hasStudentToClassEnrollPermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentToClassEnrollment);
    },
    hasStudentClassUpdatePermission() {
      return this.hasInstitutionPermission(Permissions.PermissionNameForStudentClassUpdate);
    },
    hasStudentClassHistoryReadPermission() {
      return this.hasInstitutionPermission(Permissions.PermissionNameForStudentClassHistoryRead);
    },
    hasDataReferencesReadPermission() {
      return this.hasPermission(Permissions.PermissionNameDataReferencesRead);
    },
    hasStudentCurriculumReadPermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentCurriculumRead);
    }
  },
  watch: {
    positions() {
      if (this.mounted === false) {
        this.load();
      }
      this.mounted = false;
    },
  },
  async mounted() {
    this.mounted = true;
    await UserService.getUserInfo().then(async (data) =>
      {
        this.userInfo = new UserInfo(data.data);
        this.instTypeId = this.userInfo.instTypeId;
        this.updatePositions();
        this.load();
      });
  },
  methods: {
    ...mapActions(['setSelectedStudentClass']),
    load() {
      const searchModel = {
          personId: this.pid,
          positions: this.mounted ? this.defaultPositions : this.positions
      };

      this.classes.splice(0);

      this.loading = true;

      this.$api.studentClass.getByPersonId(searchModel)
        .then((response) => {
          response.data.forEach((element) => {
            this.classes.push(new StudentClass(element, this.$moment));
          });

          this.$emit("classesDataLoaded");
        })
        .catch((error) => {
          this.showErrorSnackbar = true;
          this.errorMessage = this.$t('errors.studentClassesLoad');
          console.log(error);
        })
        .then(() => {
          this.loading = false;
        });
    },
    onMoveToClass(c) {
      if(!this.pid) {
        this.$notifier.error(this.$t('student.movement.relocation'), 'Missing student pid');
        return;
      }

      this.setSelectedStudentClass(c);

      if(c.classGroup.classKindId === ClassKind.Basic || c.classGroup.isNotPresentForm) {
        this.$router.push({ path: `/student/${this.pid}/class/change` });
      }
      else {
        this.$router.push({ path: `/student/${this.pid}/class/enroll/change?studentClassId=${c.id}&classId=${c.classId}` });
      }
    },
    onEdit(c) {
      if(!this.pid) {
        this.$notifier.error(this.$t('student.movement.relocation'), 'Missing student pid');
        return;
      }

      this.setSelectedStudentClass(c);

      if(c.classGroup.classKindId === ClassKind.Basic || c.classGroup.isNotPresentForm) {
        this.$router.push({ path: `/student/${this.pid}/class/${c.id}/edit` });
      }
      else {
        this.$router.push({ path: `/student/${this.pid}/class/enroll/edit?studentClassId=${c.id}&classId=${c.classId}` });
      }
    },
    async onUnenroll(studentClass) {
      this.selectedEnrollmentDate = studentClass.enrollmentDate;

      if (await this.$refs.unenrollmentPrompt.open('', this.$t('documents.dischargeDateLabel'))) {
        if (!this.selectedDischargeDate) {
          return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
        }

        this.saving = true;
        this.$api.studentClass
          .unenrollFromClass({
            classId: studentClass.id,
            dischargeDate: this.$helper.parseDateToIso(this.selectedDischargeDate, '')
          })
          .then(() => {
            this.$studentEventBus.$emit('studentMovementUpdate', this.pid);
            this.$notifier.success('', this.$t('enroll.unenrollFromClassSuccess'));
            this.load();
          })
          .catch((error) => {
            this.$notifier.error(this.$t('enroll.unenrollFromClassError'), error.response.data.message);
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
    async onDelete(studentClass) {
      if (!await this.$refs.confirm.open(this.$t('buttons.delete'),this.$t('studentClass.deleteMainClassConfirm'), { messageClass: 'confirmDeleteStudentClass' })) {
        return;
      }

      this.saving = true;
      this.$api.studentClass
        .deleteAdditionalClass(studentClass.id)
        .then(() => {
          this.$studentEventBus.$emit('studentMovementUpdate', this.pid);
          this.$notifier.success('', this.$t('enroll.deleteAdditionalClassSuccess'));
          this.load();
        })
        .catch((error) => {
          this.$notifier.modalError(this.$t('enroll.deleteAdditionalClassError'), error.response.data.message);
          console.log(error.response);
        })
        .then(() => {
          this.saving = false;
        });
    },
    canAddCurriculum(studentClass) {
      return studentClass.classGroup.classKindId
        && (studentClass.classGroup.classKindId === ClassKind.Basic || studentClass.classGroup.classKindId === ClassKind.Cdo)
        && !studentClass.classGroup.isNotPresentForm;
    },
    onAddCurriculum(studentClass) {
      this.$router.push({ path: `/student/${this.pid}/class/${studentClass.id}/addCurriculum/${studentClass.schoolYear}` });
    },
    dataReferencesBtnClick(studentClassId) {
      this.saving = true;
      this.dataReferencesDialog = false;
      this.$api.administration
        .getDataReferences('student', 'StudentClass', studentClassId, 100, false)
        .then((response) => {
          if (response.data) {
            this.dataReferences = response.data;
            this.dataReferencesDialog = true;
          }
        })
        .catch((error) => {
          console.log(error.response);
        })
        .finally(() => {
          this.saving = false;
        });
    },
    updatePositions() {
      switch (this.instTypeId) {
        case 1:
        case 2:
          this.defaultPositions = [3, 10];
          break;
        case 3:
        case 5:
          this.defaultPositions = [8];
          break;
        case 4:
          this.defaultPositions = [7];
          break;
        default:
          this.defaultPositions = [...this.positions];
      }
    },
    changePositionBtnClick(studentClass) {
      this.changePositionModel = {
        positionId: studentClass.changeTargetPositionId,
        studentClass: studentClass
      };
      this.changePositionDialog = true;
    },
    onChangePositionCancel() {
      this.changePositionDialog = false;
      this.changePositionModel = null;
    },
    onChangePositionSave() {
      const form = this.$refs['changePositionForm' + this._uid];
      const isValid = form ? form.validate() : false;
      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      }

      this.saving = true;

      this.$api.studentClass
        .changePosition({
          personId: this.changePositionModel.studentClass?.personId,
          positionId: this.changePositionModel.positionId,
          studentClassId: this.changePositionModel.studentClass?.id
        })
        .then(() => {
          this.$studentEventBus.$emit('studentMovementUpdate', this.pid);
          this.$notifier.success('', this.$t('common.saveSuccess'));
          this.load();
          this.changePositionModel = null;
          this.changePositionDialog = false;
        })
        .catch((error) => {
          this.$notifier.error(this.$t('common.saveError'), error.response.data.message);
        })
        .finally(() => {
          this.saving = false;
        });
    }
  }
};
</script>

<style>
  .confirmDeleteStudentClass.v-card__subtitle {
    color: red !important;
  }
</style>

