<template>
  <div>
    <v-menu
      offset-y
    >
      <template v-slot:activator="{ on }">
        <v-avatar
          color="primary"
          size="36"
          class="avatar-pointer"
          style="border:1px solid white !important"
          v-on="on"
        >
          <span class="white--text">{{ loggedUserAvatarText }}</span>
        </v-avatar>
      </template>
      <v-list>
        <v-list-item
          v-if="isAuthenticated"
          :dense="userDetails && !!(userDetails.institution || userDetails.address)"
        >
          <v-list-item-icon>
            <v-avatar
              color="primary"
              size="36"
            >
              <span class="white--text">{{ loggedUserAvatarText }}</span>
            </v-avatar>
          </v-list-item-icon>
          <v-list-item-content>
            <span
              v-if="loggedUserFullName"
            >
              <v-list-item-title>{{ loggedUserFullName }}</v-list-item-title>
              <v-list-item-subtitle>{{ loggedUserUsername }}</v-list-item-subtitle>
            </span>
            <span
              v-else
            >
              <v-list-item-title>{{ loggedUserUsername }}</v-list-item-title>
            </span>
            <v-list-item-subtitle v-if="user && user.profile && user.profile.impersonator">
              {{ $t('account.impersonatedBy') }} {{ user.profile.impersonator }}
            </v-list-item-subtitle>
            <v-list-item-subtitle
              v-if="userDetails"
            >
              {{ $t('account.role', { role: userDetails.roleName }) + (userDetails.institutionID ? ` / ${userDetails.institutionID}` : '') }}
            </v-list-item-subtitle>
            <v-list-item-subtitle
              v-if="userDetails && userDetails.schoolYearName"
            >
              {{ $t('common.schoolYear') +` / ${userDetails.schoolYearName}` }}
            </v-list-item-subtitle>
          </v-list-item-content>
          <v-list-item-action
            v-if="apiErrors && apiErrors.length > 0"
          >
            <v-badge
              bordered
              color="warning"
              :content="apiErrors.length"
              overlap
            >
              <v-btn
                color="warning"
                small
                outlined
                @click.stop="errorsDialog = true"
              >
                {{ $t('common.logs') }}
              </v-btn>
            </v-badge>
          </v-list-item-action>
        </v-list-item>

        <v-list-item
          v-if="isAuthenticated && userDetails && userDetails.institution"
          dense
        >
          <v-list-item-icon>
            <v-icon left>
              mdi-town-hall
            </v-icon>
          </v-list-item-icon>
          <v-list-item-content>
            <v-list-item-title>
              {{ `${$t('account.institution', { institution: userDetails.institution })}${userDetails.instType ? ` / ${userDetails.instType}` : ''}` }}
            </v-list-item-title>
          </v-list-item-content>
        </v-list-item>

        <v-list-item
          v-if="isAuthenticated && userDetails && userDetails.address"
          dense
        >
          <v-list-item-icon>
            <v-icon left>
              mdi-map-marker-outline
            </v-icon>
          </v-list-item-icon>
          <v-list-item-content>
            <v-list-item-title>
              {{ $t('account.address', { address: userDetails.address }) }}
            </v-list-item-title>
          </v-list-item-content>
        </v-list-item>

        <v-list-item
          v-if="isAuthenticated && showProfileLink && isInStudentsModuleRole"
          :dense="userDetails && !!(userDetails.institution || userDetails.address)"
        >
          <v-btn
            block
            color="primary"
            @click.stop="onProfileClick"
          >
            {{ $t('menu.profile') }}
          </v-btn>
        </v-list-item>

        <v-list-item
          v-if="isAuthenticated && hasContextualInfoManagePermission && isInStudentsModuleRole"
          dense
        >
          <v-tooltip
            bottom
          >
            <template v-slot:activator="{ on: contextual }">
              <slot>
                <div
                  v-on="{ ...contextual }"
                >
                  <v-switch
                    v-model="manageContextualInformation"
                    :label="$t('contextualInformation.title')"
                  />
                </div>
              </slot>
            </template>
            <span> {{ $t('contextualInformation.manage') }} </span>
          </v-tooltip>
        </v-list-item>

        <v-list-item
          v-if="isAuthenticated"
          dense
        >
          <v-btn
            block
            plain
            dark
            @click="logout"
          >
            <span
              class="primary--text"
            >
              {{ $t('buttons.logout') }}
            </span>
          </v-btn>
        </v-list-item>
      </v-list>
    </v-menu>
    <v-dialog
      v-model="errorsDialog"
      fullscreen
      hide-overlay
      transition="dialog-bottom-transition"
    >
      <v-card>
        <v-toolbar
          dark
          color="warning"
        >
          <v-btn
            icon
            dark
            @click.stop="errorsDialog = false"
          >
            <v-icon>mdi-close</v-icon>
          </v-btn>
          <v-toolbar-title>{{ $t('common.logs') }}</v-toolbar-title>
          <v-spacer />
        </v-toolbar>
        <v-card-text
          v-if="apiErrors"
        >
          <api-error-details
            v-for="(error, index) in apiErrors"
            :key="index"
            :value="error"
            class="my-1"
          />
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <button-tip
            color="warning"
            icon-name="fas fa-times"
            text="buttons.close"
            @click="errorsDialog = false"
          />
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script>
import ApiErrorDetails from '@/components/admin/ApiErrorDetails.vue';
import { mapGetters } from 'vuex';
import { Permissions } from '@/enums/enums';

export default {
  name: 'ProfileMenu',
  components: {
    ApiErrorDetails
  },
  props: {
    showProfileLink: {
      type: Boolean,
      default() {
        return true;
      }
    }
  },
  data() {
    return {
      errorsDialog: false,
      accessToken: null
    };
  },
  computed: {
    ...mapGetters(['user','isAuthenticated', 'loggedUserUsername',
      'loggedUserFullName', 'loggedUserAvatarText', 'hasPermission',
      'userDetails', 'isInStudentsModuleRole', 'apiErrors']),
    hasContextualInfoManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForContextualInformationManage);
    },
    manageContextualInformation: {
      get() {
        return this.$store.state.manageContextualInformation;
      },
      set(value) {
        this.$store.commit("setManageContextualInformation", value);
      }
    }
  },
  mounted(){
    this.accessToken = this.$helper.parseJwt(this.user?.access_token);
  },
  methods:{
    logout() {
      this.$auth.logout();
    },
    onProfileClick() {
      this.$router.push("/account/profile").catch(()=>{});
    }
  }
};
</script>

<style lang="css" scoped>
.avatar-pointer:hover {
  cursor: pointer;
}
</style>
