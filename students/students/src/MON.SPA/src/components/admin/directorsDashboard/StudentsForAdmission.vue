<template>
  <div>
    <grid
      :ref="'studentsForAdmissionGrid' + _uid"
      url="/api/admin/GetStudentsForAdmission"
      :headers="headers"
      :title="''"
      :file-export-name="$t('dashboards.studentsForAdmissionList')"
      :file-exporter-extensions="['xlsx', 'csv', 'txt']"
      item-key="uid"
    >
      <!-- Груповото записване в паралелка не рабти правилно и за сега се коментира -->
      <!-- <template #topAppend>
        <v-autocomplete
          id="classId"
          ref="studentClass"
          v-model="classId"
          :label="$t('enroll.class')"
          name="classId"
          item-text="text"
          item-value="value"
          :items="classGroupsOptions"
          class="required"
          clearable
        >
          <template v-slot:[`item`]="{ item }">
            <span v-if="!item.disabled">
              {{ item.text }}
            </span>
            <span v-else>
              <v-chip
                class="ma-2"
                color="orange"
                outlined
              >
                {{ item.text }}
              </v-chip>
            </span>
          </template>
        </v-autocomplete>
      </template> -->

      <template v-slot:[`item.pin`]="{ item }">
        {{ `${item.pin} - ${item.pinType}` }}
      </template>

      <template v-slot:[`item.noteDate`]="{ item }">
        {{ item.noteDate ? $moment(item.noteDate).format(dateFormat) : '' }}
      </template>

      <template #actions="item">
        <button-group>
          <button-tip
            :to="`/student/${item.item.personId}/institutionDetails`"
            icon
            icon-color="primary"
            iclass=""
            icon-name="mdi-eye"
            small
            tooltip="student.menu.lod"
            bottom
          />
          <button-tip
            v-if="item.item.canBeEnrolled"
            :to="`/student/${item.item.personId}/class/initialEnrollment?admissionDocumentId=${item.item.admissionDocumentId}`"
            icon
            icon-color="primary"
            iclass=""
            icon-name="fas fa-long-arrow-alt-right"
            small
            tooltip="documents.admissionDocumentCreateTitle"
            bottom
          />
        </button-group>
      </template>
      <!-- Груповото записване в паралелка не рабти правилно и за сега се коментира -->
      <!-- <template #footerPrepend>
        <v-btn
          small
          :disabled="classId == null || $refs['studentsForAdmissionGrid' + _uid] == undefined || $refs['studentsForAdmissionGrid' + _uid].selected.length == 0"
          color="primary"
          @click.stop="admissionSelected"
        >
          {{ $t('dashboards.admissionSelected') }}
        </v-btn>
      </template> -->
    </grid>
    <confirm-dlg ref="confirm" />
  </div>
</template>

<script>
import Grid from '@/components/wrappers/grid.vue';
import UserService from '@/services/user.service.js';
import { UserInfo } from '@/models/account/userInfo';
import Constants from '@/common/constants.js';
import { mapGetters } from 'vuex';

export default {
  name: 'StudentsForAdmission',
  components: {
    Grid
  },
  data() {
    return{
      userInfo: null,
      classGroupsOptions: [],
      dateFormat: Constants.DATEPICKER_FORMAT,
      classId: null
    };
  },
  computed: {
    ...mapGetters(['userInstitutionId']),
    headers() {
      if(this.userInstitutionId) {
        return [
          {
            text: this.$t('dashboards.headers.studentNames'),
            value: 'fullName',
          },
          {
            text: this.$t('student.headers.identifier'),
            value: 'pin',
          },
          {
            text: this.$t('common.position'),
            value: 'positionName',
          },
          {
            text: this.$t('admissionDocument.noteDate'),
            value: 'noteDate',
          },
          { text: '', value: 'controls', sortable: false, align: 'end' }
        ];
      } else {
        return [
          {
            text: this.$t('dashboards.headers.studentNames'),
            value: 'fullName',
          },
          {
            text: this.$t('student.headers.identifier'),
            value: 'pin',
          },
          {
            text: this.$t('common.position'),
            value: 'positionName',
          },
          {
            text: this.$t('admissionDocument.noteDate'),
            value: 'noteDate',
          },
          {
            text: this.$t('common.institutionCode'),
            value: 'institutionId',
          },
          {
            text: this.$t('common.institution'),
            value: 'institutionName',
          },
          {
            text: this.$t('institution.headers.region'),
            value: 'regionName',
          },
          { text: '', value: 'controls', sortable: false, align: 'end' }
        ];
      }
    }
  },
  mounted(){
    this.init();
  },
  methods:{
    async admissionSelected(){
      if(await this.$refs.confirm.open(this.$t('buttons.confirm'), this.$t('dashboards.bulkEnrollmentConfirm'))) {
          const selected = this.$refs['studentsForAdmissionGrid' + this._uid].selected;

          if(!selected || selected.length === 0) {
            return this.$notifier.warn('', this.$t('errors.invalidSelection'));
          }

          selected.forEach(student => {
              student.classId = this.classId;
              student.commuterTypeId = 1;
              student.repeaterId = 1;
              student.schoolYear = this.userInfo.currentSchoolYear;
          });

          this.$api.admin.admissionStudents(selected)
          .then(() => {
            this.$notifier.success('', this.$t('dashboards.bulkEnrollmentSuccess'));
            this.classId = null;
            this.$refs['studentsForAdmissionGrid' + this._uid].selected = [];
          }).catch(error => {
            this.$notifier.error('', this.$t('dashboards.admissionSelectedError'));
            console.log(error.response);
          }).finally(() => {
            this.gridReload();
          });
      }
    },
    loadClassGroupsOptions(institutionId, schoolYear) {
        this.$api.lookups
          .getClassGroupsOptions(
            institutionId,
            schoolYear,
            undefined,
            undefined,
            undefined,
            undefined
          )
          .then((response) => {
            this.classGroupsOptions = response.data.filter(x => !x.text.includes("служебна"));
          })
          .catch((error) => {
            this.$notifier.error("", this.$t("errors.classGroupsOptionsLoad"));
            console.log(error);
          });
    },
    gridReload() {
      const grid = this.$refs['studentsForAdmissionGrid' + this._uid];
      if(grid) {
        grid.get();
      }
    },
    async init(){
          if (this.userInfo == null){
              UserService.getUserInfo().then((data) => {
                  this.userInfo = new UserInfo(data.data);
                  this.loadClassGroupsOptions(this.userInfo.institutionId, this.userInfo.currentSchoolYear);
              });
          }
      },
  }
};
</script>
