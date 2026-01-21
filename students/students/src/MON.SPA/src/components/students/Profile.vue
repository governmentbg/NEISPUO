<template>
  <v-container
    fluid
    ma-0
    pa-0
  >
    <v-card v-if="student">
      <!-- <v-alert
        v-if="student.waitingToBeDischarged && student.waitingToBeDischarged.length"
        outlined
        type="info"
        text
        class="ma-0"
      >
        <div>
          {{ $t('profile.studentIsMovedMessage') }}
        </div>
        <v-card-actions
          class="ma-0 pa-0"
        >
          <v-btn
            text
            color="info"
            to="/administration/directorDashboard"
            class="pl-0"
          >
            <v-icon
              left
              dark
              small
            >
              fas fa-list
            </v-icon>
            {{ $t('buttons.list') }}
          </v-btn>
          <v-btn
            v-for="item in student.waitingToBeDischarged"
            :key="item.studentClassId"
            text
            color="info"
            :to="`/student/${pid}/dischargeDocument/create?studentClassId=${item.studentClassId}`"
          >
            <v-icon
              left
              dark
              small
            >
              fas fa-sign-out-alt
            </v-icon>
            {{ $t('student.movement.discharge') }}
          </v-btn>
        </v-card-actions>
      </v-alert> -->

      <v-card-title
        class="py-0"
      >
        <v-col
          class="pl-0"
        >
          <v-icon
            v-if="hasStudentPersonalDataReadPermission"
            left
          >
            {{ genderIcon }} {{ pid }}
          </v-icon>
          {{ student.fullName }}
        </v-col>
        <v-col
          class="text-right pr-0"
        >
          <button-group class="mx-2">
            <button-tip
              v-if="showDetailsButton"
              color="blue-grey darken-3"
              icon
              icon-color="blue-grey darken-3"
              icon-name="fas fa-info-circle"
              iclass=""
              tooltip="student.details"
              bottom
              small
              :to="`${baseRoute}/${pid}${baseRoute === '/student' ? '' : '&'}`"
            />
            <button-tip
              v-if="hasStudentClassReadPermission && showClassesButton"
              color="blue-grey darken-3"
              icon
              icon-color="blue-grey darken-3"
              icon-name="fas fa-graduation-cap"
              iclass=""
              tooltip="profile.classes"
              bottom
              small
              :to="`${baseRoute}/${pid}/classes`"
            />
            <button-tip
              color="blue-grey darken-3"
              icon
              icon-color="blue-grey darken-3"
              icon-name="fas fa-shield-alt"
              iclass=""
              tooltip="profile.permissions"
              bottom
              small
              @click="dialog = true"
            />
            <!-- <button-tip
              color="blue-grey darken-3"
              icon
              icon-color="blue-grey darken-3"
              icon-name="fas fa-print"
              iclass=""
              tooltip="lod.generatePersonalFile"
              bottom
              small
              @click="generatePersonalFile()"
            /> -->
          </button-group>
          <v-menu
            v-if="hasStudentPersonalDataReadPermission"
            offset-y
          >
            <template v-slot:activator="{ on, attrs }">
              <div class="split-btn">
                <button-group>
                  <v-btn
                    small
                    icon
                    color="primary"
                    dark
                    class="main-btn"
                  >
                    <v-icon
                      left
                      color="blue-grey darken-3"
                    >
                      fa-print
                    </v-icon>
                  </v-btn>
                  <v-btn
                    small
                    icon
                    v-bind="attrs"
                    color="blue-grey darken-3"
                    class="actions-btn"
                    v-on="on"
                  >
                    <v-icon
                      color="blue-grey darken-3"
                    >
                      mdi-menu-down
                    </v-icon>
                  </v-btn>
                </button-group>
              </div>
            </template>
            <v-list dense>
              <v-list-item @click="generatePersonalFile()">
                <v-list-item-title><v-icon name="fas fa-print" /> {{ $t('lod.generatePersonalFile') }}</v-list-item-title>
              </v-list-item>
              <v-list-item @click="generatePersonalFileForStay()">
                <v-list-item-title><v-icon name="fas fa-print" /> {{ $t('lod.generatePersonalFileForStay') }}</v-list-item-title>
              </v-list-item>
            </v-list>
          </v-menu>
          <lod-finalization
            v-if="hasLodStateManagePermission"
            class="mr-1"
            :school-year="schoolYear"
            :selected-students="lodFinalizationStudents"
            @lodFinalizationEnded="lodFinalizationEnded"
          />
        </v-col>
      </v-card-title>
      <v-card-subtitle>{{ cardTitle }}</v-card-subtitle>
      <v-card-text>
        <v-row
          dense
        >
          <class-group-chip
            v-for="item in student.allCurrentClasses"
            :key="item.studentClassId"
            :value="item"
          />
          <v-spacer />
          <class-group-btns
            :pid="pid"
          />
        </v-row>
      </v-card-text>
    </v-card>
    <app-loader
      v-else
    />
    <v-dialog
      v-model="dialog"
    >
      <v-card>
        <v-card-title class="text-h5">
          {{ $t('profile.permissions') }}
        </v-card-title>

        <v-card-text>
          <v-expansion-panels
            v-model="expandablePanelModel"
            multiple
            popout
          >
            <v-expansion-panel>
              <v-expansion-panel-header>
                {{ $t('student.headers.lodFinalization') }}
              </v-expansion-panel-header>
              <v-expansion-panel-content>
                <v-chip
                  v-for="(item, index) in studentFinalizedLods"
                  :key="index"
                  class="ma-2"
                  label
                >
                  {{ item }}
                </v-chip>
              </v-expansion-panel-content>
            </v-expansion-panel>
            <v-expansion-panel>
              <v-expansion-panel-header>
                Права за ученика
              </v-expansion-panel-header>
              <v-expansion-panel-content>
                <pre>{{ permissionsForStudent }}</pre>
              </v-expansion-panel-content>
            </v-expansion-panel>
            <v-expansion-panel>
              <v-expansion-panel-header>
                Права за институцията
              </v-expansion-panel-header>
              <v-expansion-panel-content>
                <pre
                  v-if="permissionsForInstitution && permissionsForInstitution.length > 0"
                >
                  {{ permissionsForInstitution }}
                </pre>
              </v-expansion-panel-content>
            </v-expansion-panel>
          </v-expansion-panels>
        </v-card-text>

        <v-divider />

        <v-card-actions>
          <v-spacer />
          <v-btn
            color="primary"
            text
            @click="dialog = false"
          >
            {{ $t('buttons.close') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
    <confirm-dlg ref="confirm" />
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </v-container>
</template>

<script>
import studentService from '@/services/student.service.js';
import AppLoader from '@/components/wrappers/loader.vue';
import Constants from '@/common/constants';
import LodFinalization from '@/components/lod/LodFinalization.vue';
import ClassGroupChip from '@/components/common/ClassGroupChip.vue';
import ClassGroupBtns from '@/components/common/ClassGroupBtns.vue';
import { StudentSummaryModel } from '@/models/studentSummaryModel';
import { Permissions, UserRole } from '@/enums/enums';
import { LodFinalizationStudentModel } from '@/models/lodModels/lodFinalizationStudentModel.js';
import { mapGetters } from 'vuex';

export default {
    name: 'StudentProfile',
    components: {
      AppLoader,
      LodFinalization,
      ClassGroupChip,
      ClassGroupBtns
    },
    props:{
        pid: {
          type: Number,
          default() {
            return null;
          }
        },
        showDetailsButton: {
          type: Boolean,
          default() {
            return true;
          }
        },
        showClassesButton: {
          type: Boolean,
          default() {
            return true;
          }
        },
        baseRoute: {
          type: String,
          default() {
            return "/student/details"; // Трабва да се премахне когато се преработят детайлите на студен да използват nested routing
          }
        }
    },
    data()
    {
      return {
        student: undefined,
        dialog: false,
        schoolYear: undefined,
        lodFinalizationStudents: undefined,
        expandablePanelModel: [1],
        saving: false,
      };
    },
    computed: {
      ...mapGetters(['egnAnonymization', 'permissionsForStudent',
        'permissionsForInstitution', 'isDebug', 'hasStudentPermission',
        'studentFinalizedLods', 'isInRole']),
      cardTitle() {
        var egn = this.student.pin
          ? this.egnAnonymization ? Constants.AnonymizedEgn : this.student.pin
          : this.student.pin;

          return this.student
        ? `${this.student.pinType} ${egn}  ${this.$t('profile.age')}: ${this.student.age} г.`
        : '';
      },
      genderIcon: function(){
          let icon = '';
          switch(this.student.gender){
              case 'мъж': icon = 'fas fa-mars';
              break;
              case 'жена': icon = 'fas fa-venus';
              break;
          }
          return icon;
      },
      hasStudentClassReadPermission() {
        return this.hasStudentPermission(Permissions.PermissionNameForStudentClassRead);
      },
      hasAzureAccountManagePermission() {
        return this.hasStudentPermission(Permissions.PermissionNameForAzureAccountManage);
      },
      hasLodStateManagePermission() {
        return this.hasStudentPermission(Permissions.PermissionNameForLodStateManage);
      },
      hasStudentPersonalDataReadPermission() {
        return this.hasStudentPermission(Permissions.PermissionNameForStudentPersonalDataRead);
      }
    },
    mounted(){
      this.init();
    },
    created() {
      this.$studentEventBus.$on('studentMovementUpdate', (personId) => this.onStudentMovementUpdate(personId));
      this.$studentHub.$on('student-class-edu-state-change', (personId) => this.onStudentMovementUpdate(personId));
    },
    destroyed() {
      this.$studentEventBus.$off('studentMovementUpdate');
      this.$studentHub.$off('student-class-edu-state-change');
    },
    methods:{
      async init(){
        this.student = undefined;
        let result = await studentService.getSummaryById(this.pid);
        this.student = new StudentSummaryModel(result.data);

        this.schoolYear = this.student.currentClass.schoolYear;
        this.lodFinalizationStudents = [new LodFinalizationStudentModel({
          personId: this.student.personId,
          fullName: this.student.fullName,
          isLodApproved: result.data.isLodApproved,
          isLodFinalized: result.data.isLodFinalized,
        })];
      },
      async generatePersonalFile(){
        try {
          this.saving = true;
          await this.$api.studentLod.generatePersonalFile({ personId: this.student.personId }).then((response) => {
            const url = window.URL.createObjectURL(new Blob([response.data]));
            const link = document.createElement('a');
            link.href = url;
            link.setAttribute('download', `${this.student.fullName} личен картон.docx`);
            document.body.appendChild(link);
            link.click();
          });
        } catch (error) {
          this.$notifier.error("", this.$t("common.loadError"));
          console.log(error.response);
        } finally {
          this.saving = false;
        }
      },
      async generatePersonalFileForStay(){
        try {
          this.saving = true;
          await this.$api.studentLod.generatePersonalFileForStay({ personId: this.student.personId }).then((response) => {
            const url = window.URL.createObjectURL(new Blob([response.data]));
            const link = document.createElement('a');
            link.href = url;
            link.setAttribute('download', `${this.student.fullName} личен картон за престоя.docx`);
            document.body.appendChild(link);
            link.click();
          });
        } catch (error) {
          this.$notifier.error("", this.$t("common.loadError"));
          console.log(error.response);
        } finally {
          this.saving = false;
        }
      },
      onStudentMovementUpdate(personId) {
        if(personId === this.pid) {
          this.init();
        }
      },
      isSchoolDirector() {
        return this.isInRole(UserRole.School);
      },
      lodFinalizationEnded() {
        this.init();
      }
    }
};
</script>

<style>
  .main-btn{
    border-top-right-radius: 0;
    border-bottom-right-radius: 0;
    padding-right: 2px !important;
  }
  .actions-btn{
    border-top-left-radius: 0;
    border-bottom-left-radius: 0;
    padding: 0 !important;
    min-width: 35px !important;
    margin-left: -3.5px;
  }
  .split-btn{
    display: inline-block;
  }
</style>
