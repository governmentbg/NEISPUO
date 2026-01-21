<template>
  <div>
    <v-card v-if="userDetails">
      <v-card-title>
        <v-icon left>
          mdi-account-circle-outline
        </v-icon>
        {{ user.profile.sub }}
      </v-card-title>
      <v-card-subtitle>
        {{ $t("account.role", { role: userInfo.roleName }) }}
      </v-card-subtitle>
      <v-card-text>
        <v-row dense>
          <v-col
            v-if="userInfo.institution"
            cols="12"
          >
            <v-icon left>
              mdi-town-hall
            </v-icon>{{
              `${$t("account.institution", {
                institution: userInfo.institution,
              })}${userInfo.instType ? ` / ${userInfo.instType}` : ""}`
            }}
          </v-col>
          <v-col
            v-if="userInfo.address"
            cols="12"
          >
            <v-icon left>
              mdi-map-marker-outline
            </v-icon>{{ $t("account.address", { address: userInfo.address }) }}
          </v-col>
          <v-col
            v-if="userInfo.isLeadTeacher"
            cols="12"
          >
            <v-icon left>
              mdi-account-star-outline
            </v-icon>
            {{ $t("common.leadTeacher") }}
            <v-chip
              v-for="classGroup in userInfo.leadTeacherClassGroups"
              :key="classGroup.id"
            >
              {{ classGroup.name }}
            </v-chip>
          </v-col>
        </v-row>
        <v-expansion-panels
          v-if="extended"
          class="pt-2"
        >
          <v-expansion-panel>
            <v-expansion-panel-header>
              {{ $t("profile.userData") }}
            </v-expansion-panel-header>
            <v-expansion-panel-content>
              <button-group>
                <button-tip
                  small
                  color="info"
                  icon-name="fas fa-sync"
                  text="buttons.renewToken"
                  @click="onRenewToken()"
                />
              </button-group>
              <pre>{{ userJson }}</pre>
            </v-expansion-panel-content>
          </v-expansion-panel>
          <v-expansion-panel
            v-if="hasAppSettingsManagePermission"
          >
            <v-expansion-panel-header>
              {{ $t("profile.appSettings") }}
            </v-expansion-panel-header>
            <v-expansion-panel-content>
              <button-group>
                <button-tip
                  small
                  color="info"
                  icon-name="fas fa-chalkboard-teacher"
                  text="leadTeacher.title"
                  to="/leadTeacher/list"
                />
              </button-group>
              <absence-import-type-setting
                v-if="hasAbsenceManagePermission"
              />
            </v-expansion-panel-content>
          </v-expansion-panel>
          <v-expansion-panel>
            <v-expansion-panel-header>
              {{ $t("profile.permissions") }}
            </v-expansion-panel-header>
            <v-expansion-panel-content>
              <pre>{{ permissionsJson }}</pre>
            </v-expansion-panel-content>
          </v-expansion-panel>
          <v-expansion-panel>
            <v-expansion-panel-header>
              {{ $t("profile.apiErrors") }}
            </v-expansion-panel-header>
            <v-expansion-panel-content
              v-if="apiErrors"
            >
              <api-error-details
                v-for="(error, index) in apiErrors"
                :key="index"
                :value="error"
                class="my-1"
              />
            </v-expansion-panel-content>
          </v-expansion-panel>
        </v-expansion-panels>
      </v-card-text>
    </v-card>
  </div>
</template>

<script>
import AbsenceImportTypeSetting from '@/components/admin/appSettings/AbsenceImportTypeSetting.vue';
import ApiErrorDetails from '@/components/admin/ApiErrorDetails.vue';
import { UserInfo } from '@/models/account/userInfo';
import { Permissions } from '@/enums/enums';
import { mapGetters } from 'vuex';
import ButtonGroup from '../wrappers/ButtonGroup.vue';

export default {
  name: 'AccountProfile',
  components: {
    AbsenceImportTypeSetting,
    ApiErrorDetails,
    ButtonGroup
  },
  props: {
    extended: {
      type: Boolean,
      required: false,
      default: true,
    },
  },
  computed: {
    ...mapGetters(['user', 'permissions', 'userDetails', 'apiErrors', 'hasPermission', 'userInstitutionId']),
    hasAppSettingsManagePermission() {
      return this.userInstitutionId && this.hasPermission(Permissions.PermissionNameForTenantAppSettingsManage);
    },
    hasAbsenceManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForStudentAbsenceManage);
    },
    permissionsJson() {
      return this.permissions ? JSON.stringify(this.permissions, null, 2) : "";
    },
    userJson() {
      return this.user ? JSON.stringify(this.user, null, 2) : "";
    },
    userInfo() {
      return new UserInfo(this.userDetails);
    },
  },
  methods:{
    onRenewToken(){
      this.$auth.login();
    }
  }
};
</script>
