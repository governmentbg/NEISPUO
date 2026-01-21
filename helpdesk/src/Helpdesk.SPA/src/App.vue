<template>
  <v-app>
    <v-app-bar
      class="d-print-none"
      app
      color="stratos"
      dark
    >
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
          <span class="font-weight-bold tracking-wider">{{ $t('moduleTitle') }}</span> <span class="hidden-sm-and-down">{{ $t('appTitle') }}</span>
        </v-toolbar-title>
      </router-link>

      <v-spacer />
      <button-group>
        <button-base
          v-if="hasCategoryStatsPermissions"
          class="homeLink"
          to="/stats/category"
          color="success"
          small
        >
          <v-icon left>
            fa-chart-bar
          </v-icon>
          <span class="hidden-md-and-up"> {{ $t('common.CategoryStatsAbbr') }}</span>
          <span class="hidden-sm-and-down"> {{ $t('common.CategoryStats') }}</span>
        </button-base>
        <button-base
          class="homeLink"
          to="/questions"
          color="success"
          small
        >
          <v-icon left>
            fa-question-circle
          </v-icon>
          <span class="hidden-md-and-up"> {{ $t('common.FAQAbbr') }}</span>
          <span class="hidden-sm-and-down"> {{ $t('common.FAQ') }}</span>
        </button-base>
      </button-group>
      <dashboard-menu />
      <profile-menu />
    </v-app-bar>

    <v-main class="main">
      <v-container fluid>
        <router-view
          v-slot="{ Component }"
        >
          <transition
            name="fade"
            mode="out-in"
          >
            <component :is="Component" />
          </transition>
        </router-view>
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
        <div>
          {{ $t('appFooter.phoneInfo') }}
          <v-icon x-small>
            fa-phone-alt
          </v-icon> {{ $t('appFooter.phone') }}
          <v-icon
            size="16"
            class="mx-2"
          >
            mdi-circle-small
          </v-icon>
          {{ $t('appFooter.title') }}
        </div>
        <div class="font-weight-bold">
          {{ version }} {{ gitHash }}
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
import ProfileMenu from '@/ProfileMenu.vue';
import DashboardMenu from '@/views/account/DashboardMenu.vue';
import { config } from '@/common/config';
import { mapGetters, mapActions } from 'vuex';
import { UserRole } from '@/enums/enums';

export default {
  name: 'App',
  components: {
    ProfileMenu,
    DashboardMenu
  },
  data() {
    return {
      version: config.version,
      gitHash: config.gitHash,
      offsetTop:0,
      goToOptions: {
        duration: 1000,
        offset: 0,
        easing: 'easeInOutCubic',
      }
    };
  },
  computed: {
    ...mapGetters(['isAuthenticated', 'getPersonMessagesCount', 'loggedUserUsername', 'appMenu', 'user', 'loggedUserAvatarText','isInRole']),

     hasCategoryStatsPermissions() {
      return this.isInRole(UserRole.Consortium);

    },
  },
  watch: {
    '$route' (to) {
      document.title = to.meta.title || this.$t('moduleName');
    }
  },
  mounted() {
    this.$auth.getUser()
    .then((user) => {
      if(user) {
        this.setUser(user);
      }
    })
    .catch((error) => {
      console.log(error.response);
      this.$notifier.error('', this.$t('errors.authError'), 5000);
    });
  },

  methods: {
    ...mapActions(['setUser']),
    onScroll (event) {
      this.offsetTop = event.target.scrollingElement.scrollTop;
    },
    login() {
      this.$auth.login();
    },
    logout() {
      this.$auth.logout();
    },
  }
};
</script>

<style lang="scss">
  html {
    scroll-behavior: smooth;
  }

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
</style>
