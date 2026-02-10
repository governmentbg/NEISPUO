<template>
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
            <v-list-item-subtitle
              v-if="userDetails"
            >
              {{ $t('account.role', { role: userDetails.roleName }) + (userDetails.institutionID ? ` / ${userDetails.institutionID}` : '') }}
            </v-list-item-subtitle>
          </span>
          <span
            v-else
          >
            <v-list-item-title>{{ loggedUserUsername }}</v-list-item-title>
            <v-list-item-subtitle
              v-if="userDetails"
            >
              {{ $t('account.role', { role: userDetails.roleName }) + (userDetails.institutionID ? ` / ${userDetails.institutionID}` : '') }}
            </v-list-item-subtitle>
          </span>
        </v-list-item-content>
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
        v-if="isAuthenticated"
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
</template>

<script>
import { mapGetters } from 'vuex';

export default {
  name: 'ProfileMenu',
  props: {
    showProfileLink: {
      type: Boolean,
      default() {
        return true;
      }
    }
  },
  computed: {
    ...mapGetters(['user','isAuthenticated', 'loggedUserUsername', 'loggedUserFullName', 'loggedUserAvatarText', 'userDetails']),
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
