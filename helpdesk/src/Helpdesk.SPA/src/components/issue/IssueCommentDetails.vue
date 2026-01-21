<template>
  <v-card
    v-if="comment"
    outlined
    light
  >
    <v-card-title><h5>{{ title }}</h5></v-card-title>
    <v-card-subtitle>{{ subtitle }}</v-card-subtitle>
    <v-card-text>
      <!-- eslint-disable vue/no-v-html -->
      <div
        class="textarea"
        v-html="comment.comment"
      />
      <!-- eslint-enable -->
      <v-row v-if="comment.documents && comment.documents.length > 0">
        <v-col>
          <file-manager
            v-model="comment.documents"
            disabled
          />
        </v-col>
      </v-row>
    </v-card-text>
  </v-card>
</template>

<script>
import Constants from "@/common/constants.js";
import FileManager from '@/components/wrappers/FileManager.vue';

export default {
  name: 'IssueCommentDetails',
  components: {
    FileManager
  },
  props: {
    comment: {
      type: Object,
      default() {
        return undefined;
      }
    }
  },
  data() {
    return {
      dateTimeFormat: `${Constants.DATEPICKER_FORMAT} ${Constants.DISPLAY_TIME_FORMAT}`
    };
  },
  computed: {
    title() {
      if(!this.comment) return '';

      return this.comment.modifierUsername ?? this.comment.creatorUsername;
    },
    subtitle() {
      if(!this.comment) return '';

      const modifyDate = this.comment.modifyDate ? this.$moment.utc(this.comment.modifyDate).local().format(this.dateTimeFormat) : '';
      if(modifyDate) return `редактира на ${modifyDate}`;

      const createDate = this.comment.createDate ? this.$moment.utc(this.comment.createDate).local().format(this.dateTimeFormat) : '';
      return `коментира на ${createDate}`;
    }
  }
};
</script>
