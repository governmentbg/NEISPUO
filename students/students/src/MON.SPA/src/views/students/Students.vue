<template>
  <div>
    <v-form
      novalidate
    >
      <!-- {{ form }} -->
      <student-search
        v-model="form"
        :loading="sending"
        :disabled="sending"
        @field-enter-click="onSubmit(true)"
      >
        <template #actions>
          <v-btn
            raised
            color="primary"
            :disabled="sending || !isSearchActive"
            @click.stop="onSubmit(true)"
          >
            <font-awesome-icon icon="search" />
            {{ $t('buttons.search') }}
          </v-btn>

          <v-btn
            raised
            color="error"
            :disabled="sending || !isSearchActive"
            @click="onReset"
          >
            <v-icon left>
              fas fa-times
            </v-icon>
            {{ $t('buttons.clear') }}
          </v-btn>
        </template>
      </student-search>
    </v-form>


    <v-card
      v-show="pagedListOfStudents.items.length > 0 || search.length > 0"
      class="mt-2"
    >
      <v-card-title>
        <v-text-field
          ref="searchField"
          v-model="search"
          label="Търси"
          single-line
          clearable
          :hint="$t('common.serachInputHint')"
          persistent-hint
          :disabled="loading"
          @keyup="onKeyUp($event)"
          @click:clear="onSerachClear"
        >
          <template v-slot:prepend-inner>
            <v-tooltip
              bottom
            >
              <template v-slot:activator="{ on }">
                <v-btn
                  icon
                  @click.stop="onSubmit"
                >
                  <v-icon v-on="on">
                    mdi-magnify
                  </v-icon>
                </v-btn>
              </template>
              {{ $t('buttons.search') }}
            </v-tooltip>
          </template>
        </v-text-field>
      </v-card-title>
      <v-data-table
        :headers="headers"
        :items="pagedListOfStudents.items"
        :options.sync="options"
        :server-items-length="pagedListOfStudents.totalCount"
        :loading="loading"
        :footer-props="{itemsPerPageOptions: gridItemsPerPageOptions}"
        item-key="uid"
        class="elevation-1"
        @current-items="trace"
      >
        <template v-slot:[`item.personId`]="{ item }">
          <button-group>
            <button-tip
              icon
              icon-color="primary"
              icon-name="fas fa-info-circle"
              iclass=""
              tooltip="student.details"
              top
              small
              :to="`/student/${item.personId}/details`"
            />
            <!--
            <button-tip
              icon
              icon-color="primary"
              icon-name="fas fa-graduation-cap"
              iclass=""
              tooltip="profile.classes"
              top
              small
              @click="onClasses(item)"
            /> -->
            <button-tip
              v-if="hasAdmissionPermissionRequestManagePermission && item.isStudent && !item.isOwner && (loggedUserInstType === 1 || loggedUserInstType == 2)"
              icon
              icon-color="secondary"
              icon-name="far fa-hand-paper"
              iclass=""
              tooltip="admissionDocument.permissionRequest"
              top
              small
              @click="admissionDocumentPermissionRequest(item)"
            />
            <button-tip
              v-if="isInDevMode && hasDiplomaCreateRequestManagePermission && item.isStudent && !item.isOwner && (loggedUserInstType === 1 || loggedUserInstType == 2)"
              icon
              icon-color="secondary"
              icon-name="fas fa-certificate"
              iclass=""
              tooltip="diplomas.createRequest.createTitle"
              top
              small
              @click="diplomaCreateRequest(item)"
            />
          </button-group>
        </template>
        <template v-slot:[`item.pin`]="{ item }">
          <span>{{ item.pin ? (egnAnonymization ? anonymizedEgn : item.pin) : item.pin }}</span>
        </template>
      </v-data-table>
    </v-card>

    <v-card
      v-if="noDataFound"
      class="mt-2"
    >
      <v-alert
        outlined
        color="primary"
        prominent
      >
        <div class="title">
          {{ $t('common.noDataFound') }}
        </div>
      </v-alert>
    </v-card>
  </div>
</template>

<script>
import StudentSearch from './StudentSearch.vue';
import { StudentSearchModel } from "@/models/studentSearchModel.js";
import { mapGetters, mapActions } from 'vuex';
import { UserRole, Permissions } from '@/enums/enums';
import Constants from "@/common/constants.js";

export default {
    name: "Student",
    components: {
      StudentSearch
    },
    data() {
        return {
          anonymizedEgn: Constants.AnonymizedEgn,
          sending: false,
          loading: true,
          form: new StudentSearchModel(),
          options: {},
          search: '',
          headers: [
            {text: this.$t('student.headers.identifier'), value: "pin", sortable: true},
            {text: this.$t('student.headers.firstName'), value: 'firstName', sortable: true},
            {text: this.$t('student.headers.middleName'), value: 'middleName', sortable: true},
            {text: this.$t('student.headers.lastName'), value: 'lastName', sortable: true},
            {text: this.$t('student.headers.age'), value: 'age', sortable: false},
            {text: this.$t('student.headers.publicEduNumber'), value: 'publicEduNumber', sortable: true},
            {text: this.$t('student.headers.district'), value: 'district', sortable: false},
            {text: this.$t('student.headers.municipality'), value: 'municipality', sortable: false},
            {text: this.$t('student.headers.school'), value: 'school', sortable: false},
            {value: 'personId', sortable: false, filterable: false},
            // {value: 'id', sortable: false, filterable: false}, // EduState Id
          ],
          pagedListOfStudents: {
              totalCount: 0,
              items: [],
          },
          roleMon: UserRole.Mon,
          roleSchool: UserRole.School,
          roleRuo: UserRole.Ruo,
          noDataFound: false,
          mounted: false
        };
    },
    computed: {
      ...mapGetters(['gridItemsPerPageOptions', 'studentSearchModel', 'userSelectedRole', 'isInRole', 'egnAnonymization', 'hasPermission', 'userInstitutionId', 'userDetails', 'mode']),
      isInDevMode() {
        return this.mode !== 'prod';
      },
      userRegionId() {
        if(!this.userSelectedRole) {
          return undefined;
        }

        return this.userSelectedRole.RegionID;
      },
      loggedUserInstType() {
        return this.userDetails?.instTypeId;
      },
      isSearchActive() {
        return true;
          // return (
          //     this.form.pin ||
          //     this.form.firstName ||
          //     this.form.middleName ||
          //     this.form.lastName ||
          //     this.form.publicEduNumber ||
          //     this.form.district ||
          //     this.form.municipality ||
          //     this.form.school
          // );
      },
      hasAdmissionPermissionRequestManagePermission() {
        return this.hasPermission(Permissions.PermissionNameForAdmissionPermissionRequestManage);
      },
      hasDiplomaCreateRequestManagePermission() {
        return this.hasPermission(Permissions.PermissionNameForDiplomaCreateRequestManage);
      }
    },
    watch: {
        options: function () {
          if (this.mounted){
            this.onSubmit();
          }
        },
        // search: function (newValue, oldValue) {
        //   if(newValue &&  newValue.length < 3) return; // стринг с дължина между 1 и 3
        //   if(!newValue && !oldValue) return;
        //   this.onSubmit();
        // },
    },
    async mounted() {
      let doSubmit = false;
      // Включване на последното търсене
      if(this.studentSearchModel) {
        // Loaded from locale storage
        this.form = {...this.studentSearchModel};
        doSubmit = false;
      } else {
        this.form.onlyOwnInstitution = this.isInRole(this.roleSchool);
      }


      if(this.isInRole(this.roleSchool)) {
        const result = await this.getInstitutionById(this.userInstitutionId);
        if(result.data) {
          // this.form.school = result.data.name;
          this.form.school = ''; // В api-то така или иначе ще филтрираме по институцитя на логнатия потребител
          this.form.district = '';
          this.form.municipality = '';
        }
      }

      if(this.isInRole(this.roleRuo)) {
        const result = await this.getRegionById(this.userRegionId);
        if(result.data) {
          // this.form.district = result.data.name;
          this.form.district = ''; // В api-то така или иначе ще филтрираме по regionId на логнатия потребител
          this.form.municipality = '';
        }
      }
      this.mounted = true;

      if(doSubmit) {
        this.onSubmit(true);
      }
    },
    methods: {
      ...mapActions(['setStudentSearchModel', 'claerStudentSearchModel']),
      onSubmit(resetOptions) {
        this.sending = true;
        this.loading = true;
        this.noDataFound = false;
        const opt = this.options;

        if(resetOptions) {
          // Натиснат е бутон ТЪРСЕНЕ. Ресетваме опциите на data-table-а
          this.setStudentSearchModel(this.form);

          this.form.filter = '';
          this.form.pageIndex = 0;
          this.form.pageSize = 10;
          this.form.sortBy = '';
          this.search = '';
        } else {
          this.form.filter = this.search;
          this.form.pageIndex = opt.page - 1;
          this.form.pageSize = opt.itemsPerPage;
          if (opt.sortBy.length > 0 && opt.sortBy[0]) {
            this.form.sortBy =
              opt.sortBy +
              (opt.sortDesc.length > 0 && opt.sortDesc[0] ? " desc" : "");
          }
        }

        this.$api.student
          .getBySearch(this.form)
          .then((response) => {
            if (response.data) {
                this.pagedListOfStudents = response.data;
            }

            if (response.data.totalCount === 0) {
              this.noDataFound = true;
              console.log(this.$t('common.noResults'));
            }
          })
          .catch((error) => {
            this.$notifier.error('', this.$t('errors.studentSearch'));
            console.log(error);
          })
          .then(() => {
            this.sending = false;
            this.loading = false;

            if(!resetOptions) { // Използвано е полето за търсене
              // Todo: да се фокусира полето за търсене;
            }
          });
      },
      onReset() {
          this.form = new StudentSearchModel();
          this.claerStudentSearchModel();
          this.noDataFound = false;
      },
      onClasses(item) {
        this.$router.push({ name: 'StudentClasses', params: { id: item.personId } });
      },
      onKeyUp(event) {
        if(event.keyCode === 27) { // Escape was pressed
          event.preventDefault();
          this.search = '';
          return this.onSubmit();
        }

        if(event.keyCode === 13) { // Enter wa pressed
          event.preventDefault();
          return this.onSubmit();
        }
      },
      onSerachClear() {
        this.search = '';
        this.onSubmit();
      },
      async getInstitutionById(instId) {
        const result = await this.$api.institution.getById(instId);
        return result;
      },
      async getRegionById(regionId) {
        const result = await this.$api.lookups.getRegionById(regionId);
        return result;
      },
      admissionDocumentPermissionRequest(item) {
        const requestingInstitutionId = this.userInstitutionId,
          authorizingInstitutionId = item.institutionID;

        this.$router.push({ name: 'StudentAdmissionDocumentPermissionRequestCreate', query: { personId: item.personId, requestingInstitutionId: requestingInstitutionId, authorizingInstitutionId: authorizingInstitutionId } });
      },
      diplomaCreateRequest(item) {
        const requestingInstitutionId = this.userInstitutionId,
          currentInstitutionId = item.institutionID;

        this.$router.push({ name: 'DiplomaCreateRequestsListCreate', query: { personId: item.personId, requestingInstitutionId: requestingInstitutionId, currentInstitutionId: currentInstitutionId } });
      },
      trace(foundStudents) {
        if (Array.isArray(foundStudents) && foundStudents.length > 0) {
          const data = foundStudents.map(x => {
            return {
              personId: x.id,
              institutionId: x.institutionID,
              instiituttionName: x.school,
              publicEduNumber: x.publicEduNumber,
              isStudent: x.isStudent,
              isOwner: x.isOwner,
              loggedUserInstitutionId: this.userInstitutionId,
              loggedUserInstType: this.loggedUserInstType,
              hasAdmissionPermissionRequestManagePermission: this.hasAdmissionPermissionRequestManagePermission,
              admissionPermissionRequestButtonConditionFunc: "hasAdmissionPermissionRequestManagePermission && item.isStudent && !item.isOwner && (loggedUserInstType === 1 || loggedUserInstType == 2)",
              admissionPermissionRequestButtonCondition: `${this.hasAdmissionPermissionRequestManagePermission} && ${x.isStudent} && ${!x.isOwner} && (${this.loggedUserInstType} === 1 || ${this.loggedUserInstType} == 2)`,
              admissionPermissionRequestButtonConditionEval: this.hasAdmissionPermissionRequestManagePermission && x.isStudent && !x.isOwner && (this.loggedUserInstType === 1 || this.loggedUserInstType === 2)
            };
          });

          this.$api.trace.trace(data);
        }
      }
    },
};
</script>
