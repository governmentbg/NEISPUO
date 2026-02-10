<template>
  <v-input dense>
    <template name="default">
      <v-container class="contextual-wrapper-default-container">
        <slot name="default" />
      </v-container>
    </template>
    <template
      v-if="text"
      #append
    >
      <v-menu
        offset-y
        offset-x
        :close-on-content-click="closeOnContentClick"
      >
        <template v-slot:activator="{ on, attrs }">
          <v-btn
            icon
            color="info"
            dark
            v-bind="attrs"
            v-on="on"
          >
            <v-icon
              small
            >
              far fa-question-circle
            </v-icon>
          </v-btn>
        </template>
        <v-alert
          border="top"
          colored-border
          color="info"
          elevation=""
          dense
          mb-0
        >
          {{ text }}
        </v-alert>
      </v-menu>
      <button-tip
        v-if="manageContextualInformation && hasContextualInfoManagePermission"
        icon
        icon-name="mdi-cog-outline"
        icon-color="primary"
        iclass="mx-2"
        tooltip="contextualInformation.manage"
        bottom
        @click="onEditContextualInformation"
      />
    </template>
    <contextual-information-modal
      v-model="contextualInfoKey"
    />
  </v-input>
</template>

<script>
import { mapGetters } from 'vuex';
import { Permissions } from "@/enums/enums";
import ContextualInformationModal from '@/components/admin/ContextualInformationModal.vue';

export default {
  name: 'ContextualInformation',
  components: {
    ContextualInformationModal
  },
  props: {
    uid: {
      type: String,
      default() {
        return undefined;
      }
    },
    infoText: {
      type: String,
      default() {
        return undefined;
      }
    }
  },
  data() {
    return {
      closeOnContentClick: true,
      contextualInfoKey: null,
    };
  },
  computed: {
    ...mapGetters(['contextualInformation', 'manageContextualInformation', 'hasPermission']),
    text() {
      if(this.uid) return this.contextualInformation(this.uid);

      return this.infoText ?? '';
    },
    hasContextualInfoManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForContextualInformationManage);
    }
  },
  methods: {
    onEditContextualInformation() {
      this.contextualInfoKey = this.uid;
    }
  }
};
</script>

<style scoped>
  .contextual-wrapper-default-container {
    padding: 0;
  }
</style>
