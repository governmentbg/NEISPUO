<template>
  <v-app>
    <v-navigation-drawer
      v-if="appMenu.showMainMenu && isInStudentsModuleRole"
      id="top"
      v-model="drawer"
      class="d-print-none"
      :clipped="clipped"
      enable-resize-watcher
      fixed
      app
    >
      <v-list-item
        class="px-2"
      >
        <v-avatar
          color="primary"
          size="36"
        >
          <span class="white--text">{{ loggedUserAvatarText }}</span>
        </v-avatar>

        <v-list-item-title
          class="wrap-text text-right"
        >
          <span v-if="isAuthenticated"> {{ loggedUserUsername }}</span>
        </v-list-item-title>
      </v-list-item>

      <template v-for="(item, i) in items.filter((x) => x.visible)">
        <v-list-group
          v-if="item.children"
          :key="i"
          v-model="item.expanded"
          no-action
        >
          <template #activator>
            <v-list-item-icon
              v-if="item.icon"
            >
              <v-icon
                :color="item.iconColor"
                v-text="item.icon"
              />
            </v-list-item-icon>
            <v-list-item-content>
              <v-tooltip right>
                <template #activator="{ on }">
                  <v-list-item-title
                    class="wrap-text"
                    v-on="on"
                  >
                    {{ item.title }}
                  </v-list-item-title>
                </template>
                <span>{{ item.title }}</span>
              </v-tooltip>
            </v-list-item-content>
          </template>

          <template v-for="(child, k) in item.children.filter((x) => x.visible)">
            <v-list-group
              v-if="child.children"
              :key="k"
              v-model="child.expanded"
              no-action
              sub-group
            >
              <template #activator>
                <v-list-item-content>
                  <v-tooltip right>
                    <template #activator="{ on }">
                      <v-list-item-title
                        class="wrap-text"
                        v-on="on"
                      >
                        {{ child.title }}
                      </v-list-item-title>
                    </template>
                    <span>{{ child.title }}</span>
                  </v-tooltip>
                </v-list-item-content>
              </template>

              <v-list-item
                v-for="(subchild, s) in child.children.filter((x) => x.visible)"
                :key="s"
                :to="subchild.link"
                link
                class="pl-10"
              >
                <v-tooltip right>
                  <template v-slot:activator="{ on }">
                    <v-list-item-icon
                      v-if="subchild.icon"
                      v-on="on"
                    >
                      <v-icon
                        :color="subchild.iconColor"
                        v-text="subchild.icon"
                      />
                    </v-list-item-icon>
                    <v-list-item-title
                      class="wrap-text"
                      v-on="on"
                    >
                      {{ subchild.title }}
                    </v-list-item-title>
                  </template>
                  <span>{{ subchild.title }}</span>
                </v-tooltip>
              </v-list-item>
            </v-list-group>

            <template v-else>
              <v-list-item
                :key="k"
                :to="child.link"
                link
                class="pl-10"
              >
                <v-tooltip right>
                  <template v-slot:activator="{ on }">
                    <v-list-item-icon
                      v-if="child.icon"
                      v-on="on"
                    >
                      <v-icon
                        :color="child.iconColor"
                        v-text="child.icon"
                      />
                    </v-list-item-icon>
                    <v-list-item-content
                      class="wrap-text"
                      v-on="on"
                    >
                      {{ child.title }}
                    </v-list-item-content>
                  </template>
                  <span>{{ child.title }}</span>
                </v-tooltip>
              </v-list-item>
            </template>
          </template>
        </v-list-group>

        <template v-else>
          <v-divider
            v-if="item.divider"
            :key="i"
          />
          <template v-else>
            <v-list-item
              v-if="item.visible !== false"
              :key="i"
              :href="item.isExternal ? item.link: ''"
              :target="item.isExternal ? '_blank': ''"
              :to="item.isExternal ? '': item.link"
              link
            >
              <v-tooltip right>
                <template v-slot:activator="{ on }">
                  <v-list-item-icon
                    v-if="item.icon"
                    v-on="on"
                  >
                    <v-icon
                      :color="item.iconColor"
                      v-text="item.icon"
                    />
                  </v-list-item-icon>

                  <v-list-item-content v-on="on">
                    <v-list-item-title>
                      {{ item.title }} <v-icon
                        v-if="item.isExternal"
                        right
                        small
                      >
                        fa-external-link-alt
                      </v-icon>
                    </v-list-item-title>
                  </v-list-item-content>
                </template>
                <span>{{ item.title }}</span>
              </v-tooltip>
            </v-list-item>
          </template>
        </template>
      </template>
      <v-chip
        v-if="mode !== 'prod'"
        class="ma-2"
        color="deep-purple accent-4"
        outlined
      >
        <v-icon left>
          mdi-wrench
        </v-icon>
        {{ mode }}
      </v-chip>
    </v-navigation-drawer>
    <v-app-bar
      v-if="appMenu.showMainMenu"
      class="d-print-none"
      :clipped-left="clipped"
      app
      color="stratos"
      dark
    >
      <v-app-bar-nav-icon
        v-if="isInStudentsModuleRole"
        @click.stop="drawer = !drawer"
      />
      <v-img
        class="mx-2"
        src="@/assets/logo.png"
        max-height="30"
        max-width="30"
        contain
      />
      <router-link
        class="homeLink"
        to="/"
      >
        <v-toolbar-title>
          <span class="font-weight-bold tracking-wider">{{ $t('moduleTitle') }}</span> {{ $t('appTitle') }}
        </v-toolbar-title>
      </router-link>

      <v-spacer />

      <v-badge
        v-if="isInStudentsModuleRole"
        :content="getPersonMessagesCount"
        :value="getPersonMessagesCount"
        overlap
      >
        <router-link
          class="notificationsLink"
          to="/notifications/messages"
        >
          <v-icon
            class="fas fa-bell"
            small
          />
        </router-link>
      </v-badge>
      <dashboard-menu />
      <profile-menu />
    </v-app-bar>


    <v-main class="main">
      <v-container fluid>
        <router-view v-if="permissionsLoaded" />
        <v-fab-transition>
          <v-btn
            v-show="showGoToTop"
            v-scroll="onScroll"
            class="md-1 mr-3 elevation-21"
            transition="fab-transition"
            dark
            fab
            button
            fixed
            right
            bottom
            color="primary"
            :title="$t('buttons.scrollToTop')"
            @click="$vuetify.goTo(0, goToOptions)"
          >
            <v-icon dark>
              fa-chevron-up
            </v-icon>
          </v-btn>
        </v-fab-transition>
      </v-container>
    </v-main>
    <v-footer
      app
      padless
      class="smalltext d-print-none"
    >
      <v-col
        class="text-center"
        cols="12"
      >
        <div>{{ $t('appFooter.title') }}</div>
        <div class="font-weight-bold">
          {{ $t('appFooter.subtitle') }}<a
            :href="helpdesk"
            target="_blank"
          >{{ $t('appFooter.helpdesk') }}</a> в. {{ version }}  {{ mode }} {{ gitHash }}
        </div>
      </v-col>
    </v-footer>

    <notifications
      group="top right"
      position="top right"
      width="25%"
    />
    <notifications
      group="bottom right"
      position="bottom right"
      width="25%"
    />
    <notifications
      group="top center"
      position="top center"
      width="25%"
    />
    <notifications
      group="bottom center"
      position="bottom center"
      width="25%"
    />
    <NotificationSnackbar />
    <NotificationModal />

    <v-overlay :value="demandingPermission">
      <v-row
        justify="center"
      >
        <v-progress-circular
          indeterminate
          color="primary"
          size="64"
          width="6"
        />
      </v-row>
      <div
        class="text-center mt-5"
      >
        <h5>{{ $t('common.authCheck') }}</h5>
      </div>
    </v-overlay>
    <service-worker-notifier />
  </v-app>
</template>

<style scoped>
  #app {
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  /* text-align: center; */
  color: #2c3e50;
}
#nav {
  padding: 30px;
}
#nav a {
  font-weight: bold;
  color: #2c3e50;
}
#nav a.router-link-exact-active {
  color: #42b983;
}
.smalltext{
  font-size: 0.7em;
}
.wrap-text {
  white-space: normal !important;
  font-size: 0.929rem !important;
}
.v-list-group__header__prepend-icon {
    margin-right: 20px !important;
}

.homeLink {
    text-decoration: none;
    color: white;
}
</style>

<script>
//import AuthService  from './services/auth.service';
import Vue from 'vue';
import ProfileMenu from '@/ProfileMenu.vue';
import DashboardMenu from '@/views/account/DashboardMenu.vue';
import { menuItems } from '@/menu/menu';
import logger from '@/common/logger';
import { config } from '@/common/config';
import { mapGetters, mapActions } from 'vuex';
import ServiceWorkerNotifier from '@/components/serviceWorker/ServiceWorkerNotifier.vue';
import { CurrencyModel } from '@/models/currency.js';

export default {
  name: 'App',
  components: {
    ProfileMenu,
    DashboardMenu,
    ServiceWorkerNotifier
  },
  data() {
    return {
      drawer: true,
      clipped: true,
      permissionsLoaded: false,
      version: config.version,
      gitHash: config.gitHash,
      helpdesk: config.helpdesk,
      remoteSupport: config.remoteSupport,
      offsetTop:0,
      goToOptions: {
        duration: 500,
        offset: 0,
        easing: 'easeInOutCubic',
      }
    };
  },
  computed: {
    ...mapGetters(['isAuthenticated', 'getPersonMessagesCount', 'loggedUserUsername',
      'appMenu', 'user', 'loggedUserAvatarText', 'permissions',
      'demandingPermission', 'isInStudentsModuleRole', 'mode']),
    showGoToTop () {
      return this.offsetTop > 200;
    },
    items() {
      return menuItems();
    }
  },
  watch: {
    '$route' (to) {
      document.title = to.meta.title || this.$t('moduleName');
    },
    isAuthenticated: function (newValue) {
      if (newValue === true) {
        // При презареждане на страницата или първоначалното зареждане на логнат потребител ще зареди някои неща от localStorage-а в vuex store-а.
        // Така пази state-а при refresh
        this.loadStudentSearchModel();
      }
    },
  },
  async created() {
    logger.info("Application Started");

    try {
      const user = await this.$auth.getUser();

      await this.setUser(user);
      this.permissionsLoaded = true;

      Vue.prototype.startStudentSignalR(user.access_token);

      this.loadContextualInfo();

      this.countMyUnreadMessages();

      this.loadAppConfiguration();

      this.$studentHub.$on('change-person-messages', this.onMessagesCountChange);
      this.$studentHub.$on('student-finalized-lods-reloaded', this.contextualInfromationReloaded);
    } catch (error) {
      this.permissionsLoaded = true;
      console.log(error.response);
      this.$notifier.error('', this.$t('errors.authError'), 5000);
    }
  },
  beforeDestroy() {
    logger.info('beforeDestroy: clearing permissions...');
    this.clearPermissions();
    this.$studentHub.$off('change-person-messages');
    this.$studentHub.$off('student-finalized-lods-reloaded');
    Vue.prototype.stopStudentSignalR();
  },
  methods: {
    ...mapActions(['setUser', 'countMyUnreadMessages', 'updatePersonMessagesCount','clearPermissions','loadStudentSearchModel', 'loadContextualInfo']),
    onScroll (event) {
      this.offsetTop = event.target.scrollingElement.scrollTop;
    },
    login() {
      this.$auth.login();
    },
    logout() {
      this.$auth.logout();
    },
    contextualInfromationReloaded(contextualInformation) {
      this.$store.commit('setContextualInfo', contextualInformation);
    },
    onMessageReceived(msg) {
      this.$notifier.success(msg, '');
		},
    async onMessagesCountChange(count) {
      await this.$store.dispatch('updatePersonMessagesCount', count.msg);
		},
    loadAppConfiguration() {
      this.$api.appConfiguration.getValueByKey('Currency')
      .then(response => {
        this.$store.commit('setCurrency', new CurrencyModel(JSON.parse(response.data)));
      })
      .catch(error => {
        console.log(error.response);
      });

    },
  }
};
</script>

<style lang="scss">
  .fade-enter-active,
  .fade-leave-active {
    transition: opacity 500ms ease-out;
  }
  .fade-enter-from,
  .fade-leave-to {
    opacity: 0;
  }

  .main {
    background-color: rgba(241,241,254,2)
  }
  .vue-notification {
    padding: 10px;
    margin: 0 5px 5px;

    font-size: 16px !important;

    color: #ffffff;
    background: #44A4FC;
    border-left-color: #187FE7;
    border-left-width: 8px !important;
    border-left-style: solid;
    border-radius: 5px;

    // &.warn {
    //     background: #f7e6d3 !important;
    //     border-left-color: #f18704 !important;
    // }

    // &.error {
    //     background: #fadddb !important;
    //     border-left-color: #b91004 !important;
    // }

    // &.success {
    //     background: #d3f5dd !important;
    //     border-left-color: #237e3d !important;
    // }
  }

  .required label::after {
    content: "*";
     color: red;
    }

  .notification-title {
    font-size: 20px !important;
    border-bottom: 2px solid white;
    margin-bottom: 5px;
  }

  .custom-grid-row {

    // &.left {
    //   border-left: 1em solid !important;
    // }

    // &.right {
    //   border-right: 1em solid !important;
    // }
    &.left {
      border-left: 0.7em solid !important;
    }

    &.right {
      border-right: 0.7em solid !important;
    }

    &.top {
      border-top: 0.7em solid !important;
    }

    &.bottom {
      border-bottom: 0.7em solid !important;
    }

    &.border-primary {
      border-color: #002966 !important;
    }

    &.border-secondary {
      border-color: #424242 !important;
    }

    &.border-accent {
      border-color: #7476f7 !important;
    }

    &.border-error {
      border-color:#DF320C!important;
    }

    &.border-info {
      border-color:#2196F3!important;
    }

    &.border-success {
      border-color: #09A57F !important;
    }

    &.border-warning {
      border-color: #FFB400 !important;
    }


    .ghost {
      opacity: 0.5;
      background: #c8ebfb;
    }
  }
  .custom-grid {
    td {
      font-size: 0.8rem !important;
    }

    th {
      font-size: 0.75rem !important;
    }

    td:not(:first-child):not(:last-child) {
      padding-left: 2px !important;
      padding-right: 2px !important
    }

    th:not(:first-child):not(:last-child) {
      padding-left: 2px !important;
      padding-right: 2px !important;
    }

    td:first-child {
      padding-right: 2px !important
    }

    td:last-child {
      padding-left: 2px !important
    }
  }

</style>


