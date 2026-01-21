<template>
  <v-card>
    <v-card-title class="pb-0">
      <v-spacer />
      <v-btn
        raised
        color="primary"
        @click.stop="backClick"
      >
        <v-icon left>
          fas fa-chevron-left
        </v-icon>
        {{ $t('buttons.back') }}
      </v-btn>
    </v-card-title>
    <v-card-text>
      <v-tabs
        v-model="tab"
      >
        <v-tab key="editor">
          <h5>
            {{ isValidationDocument ? $t("validationDocument.documentsList") : $t("diplomas.documentsList") }}
          </h5>
        </v-tab>
        <v-tab
          v-if="details"
          key="details"
        >
          <h5>
            {{ $t('buttons.details') }}
          </h5>
        </v-tab>
      </v-tabs>
      <v-tabs-items
        v-model="tab"
      >
        <v-tab-item
          key="editor"
        >
          <diploma-documents
            :diploma-id="id"
            :is-validation-document="isValidationDocument"
          />
        </v-tab-item>
        <v-tab-item
          v-if="details"
          key="details"
        >
          <vue-json-pretty
            path="res"
            :data="details"
            show-length
          />
        </v-tab-item>
      </v-tabs-items>
    </v-card-text>
  </v-card>
</template>

<script>
import DiplomaDocuments from "@/components/tabs/diplomas/DiplomaDocuments.vue";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";
import VueJsonPretty from "vue-json-pretty";
import "vue-json-pretty/lib/styles.css";

export default {
  name: 'DiplomaImagesView',
  components: {
    DiplomaDocuments,
    VueJsonPretty
  },
  props: {
    id: {
      type: Number,
      required: true
    },
    isValidationDocument: {
      type: Boolean,
      required: true
    },
    details: {
      type: Object,
      default() {
        return null;
      }
    }
  },
  data() {
    return {
      tab: null,
    };
  },
  computed: {
    ...mapGetters(['hasPermission'])
  },
  mounted() {
    if(!this.hasPermission(Permissions.PermissionNameForInstitutionDiplomaRead)
      && !this.hasPermission(Permissions.PermissionNameForAdminDiplomaRead)
      && !this.hasPermission(Permissions.PermissionNameForMonHrDiplomaRead)) {
      return this.$router.push('/errors/AccessDenied');
    }
  },
  methods: {
    backClick() {
      this.$router.go(-1);
    }
  }
};
</script>
