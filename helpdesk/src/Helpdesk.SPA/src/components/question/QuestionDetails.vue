<template>
  <v-card
    v-if="question"
  >
    <v-card-title><h5>{{ question.question }}</h5></v-card-title>
    <v-card-subtitle>{{ subtitle }}</v-card-subtitle>
    <v-card-text>
      <v-alert
        border="left"
        colored-border
        color="deep-purple accent-4"
        elevation="2"
      >
        <!-- eslint-disable-next-line vue/no-v-html -->
        <div v-html="compiledMarkdown" />
      </v-alert>
    </v-card-text>
  </v-card>
</template>

<script>
import Constants from "@/common/constants.js";
import * as marked from 'marked';
//const marked = require(‘marked’);

export default {
  name: 'QuestionDetailsCard',
  props: {
    question: {
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
    compiledMarkdown: function() {
       return marked.marked(this.question.answer, { sanitize: true });
     },
    subtitle() {
       if(!this.question) return '';

       const modifyDate = this.question.modifyDate ? this.$moment.utc(this.question.modifyDate).local().format(this.dateTimeFormat) : '';
       if(modifyDate) return `редактиран на ${modifyDate}`;

       const createDate = this.question.createDate ? this.$moment.utc(this.question.createDate).local().format(this.dateTimeFormat) : '';
       return `създаден на ${createDate}`;
    }
  }
};
</script>
