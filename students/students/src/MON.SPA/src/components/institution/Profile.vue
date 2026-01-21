<template>
  <v-container
    fluid
    ma-0
    pa-0
  >
    <v-card
      v-if="institution"
    >
      <v-card-title
        class="py-0"
      >
        <v-col
          cols="10"
          class="pl-0"
        >
          {{ institution.name }}
        </v-col>
        <v-col
          v-if="!isPreview"
          cols="2"
          class="text-right pr-0"
        >
          <button-group class="mx-2">
            <button-tip
              v-if="isDebug"
              color="blue-grey darken-3"
              icon
              icon-color="blue-grey darken-3"
              icon-name="fas fa-shield-alt"
              iclass=""
              small
              tooltip="profile.permissions"
              bottom
              raised
              @click="dialog = true"
            />
          </button-group>
        </v-col>
      </v-card-title>
      <v-card-subtitle>
        {{ `${institution.town}, ${institution.country}` }}
      </v-card-subtitle>
      <v-card-text />
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
          <pre>{{ permissionsForInstitution }}</pre>
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
  </v-container>
</template>

<script>
import AppLoader from '@/components/wrappers/loader.vue';
import { mapGetters } from 'vuex';
import { Permissions } from '@/enums/enums';

export default {
    name: 'InstitutionProfile',
    components: { AppLoader },
    props:{
        institutionId: {
          type: Number,
          default() {
            return null;
          }
        },
        isPreview: {
          type: Boolean,
          default() {
            return false;
          }
        }
    },
    data()
    {
      return {
        institution: undefined,
        dialog: false
      };
    },
    computed: {
      ...mapGetters(['isDebug', 'permissionsForInstitution', 'hasInstitutionPermission', 'turnOnOresModule']),
      hasOresReadPermission() {
        return this.turnOnOresModule && this.hasInstitutionPermission(Permissions.PermissionNameForOresRead);
      },
    },
    mounted(){
      this.load();
    },
    methods:{
      load() {
        this.$api.institution
          .getFullDetails(this.institutionId)
          .then((response) => {
            this.institution = response.data;
          })
          .catch((error) => {
            console.log(error.response);
          });
      }
    }

};
</script>
