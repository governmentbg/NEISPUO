<template>
  <v-app>
    <v-app-bar
      color="hsl(0,0%,100%)"
      app
      prominent
    >
      <router-link
        class="homeLink"
        to="/"
      >
        <v-img
          class="mx-2"
          height="130"
          src="@/assets/logo.png"
          contain
        />
      </router-link>

      <v-toolbar-title class="wrap-text">
        <div class="text-center">
          <span class="text-h4">
            {{ $t('principal') }}
          </span> <hr>
          <span class="text-h6">
            {{ $t('appSubTitle') }}
          </span>
        </div>
      </v-toolbar-title>
      <v-spacer />
      <locale-switch />
    </v-app-bar>

    <v-main class="pt-40">
      <v-container fluid>
        <router-view />
      </v-container>
    </v-main>

    <v-footer
      padless
      class="smalltext"
    >
      <v-col
        class="text-center"
        cols="12"
      >
        <div>{{ $t('appFooter.title') }}</div>
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

<script>
import {config} from '@/common/config';
import LocaleSwitch from '@/components/LocaleSwitch';

export default {
  name: "App",
  components: {LocaleSwitch},
  data: () => ({
    version: config.version,
    gitHash: config.gitHash
  }),
  watch: {
    '$route' (to) {
      document.title = to.meta.title || this.$t('moduleName');
    }
  },
};
</script>

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
}
.v-list-group__header__prepend-icon {
    margin-right: 20px !important;
}

.homeLink {
    text-decoration: none;
    color: white;
}

.line-nav-blue{
    border: solid;
    border-color: #123c70;
    padding: 0px;
    border-width: 2px;
    width: 100%;
}

.line-nav-yellow{
    border: solid;
    border-color: #facb96;
    padding: 0px;
    border-width: 2px;
    width: 100%;
}

#app {
  background: url('assets/background.jpg');
  background-size: 100% auto;
}

</style>
