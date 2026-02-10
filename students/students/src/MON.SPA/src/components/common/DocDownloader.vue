<template>
  <span>
    <v-btn
      v-if="value.blobId"
      :key="value.id"
      :icon="showIcon"
      color="primary"
      link
      text
      v-bind="$attrs"
      :href="`${value.blobServiceUrl}/${value.blobId}?t=${value.unixTimeSeconds}&h=${value.hmac}`"
      v-on="$listeners"
    >
      <slot>
        <v-icon
          v-if="showIcon"
          color="primary"
        >
          fa-download
        </v-icon>
        <span
          v-if="showFileName"
        >
          {{ value.noteFileName }}
        </span>
      </slot>
    </v-btn>
    <v-btn
      v-else
      :icon="showIcon"
      color="primary"
      link
      text
      v-bind="$attrs"
      v-on="$listeners"
      @click="onFileSelected"
    >
      <slot>
        <v-icon
          v-if="showIcon"
          color="primary"
        >
          fa-download
        </v-icon>
        <span
          v-if="showFileName"
        >
          {{ value.noteFileName }}
        </span>
      </slot>
    </v-btn>
  </span>
</template>

<script>
import { DocumentModel } from '@/models/documentModel.js';
import Helper from "@/components/helper.js";

export default {
  name: 'DocDownloader',
  props: {
    value: {
      type: Object,
      required: true,
      default() {
        return new DocumentModel();
      },
    },
    showIcon: {
      type: Boolean,
      default() {
        return false;
      },
    },
    showFileName: {
      type: Boolean,
      default() {
        return true;
      },
    }
  },
  methods: {
    onFileSelected() {
      Helper.onFileSelected(this.value.id, this.value.noteFileName, this.value.noteFileType, this);
    },
  }
};
</script>
