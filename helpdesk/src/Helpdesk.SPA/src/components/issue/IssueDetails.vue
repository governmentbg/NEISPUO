<template>
  <v-card v-if="issue">
    <v-card-title>
      <h5>{{ issue.submitterUsername }}</h5>
    </v-card-title>
    <v-card-subtitle>{{ subtitle }}</v-card-subtitle>
    <v-card-text>
      <v-layout
        row
        class="pl-5 pt-5 pb-7 pr-3"
      >
        <v-flex
          class="textarea"
          v-html="issue.htmlDescription"
        />
      </v-layout>
      <text-field
        v-if="issue.phone"
        :value="issue.phone"
        :label="$t('issue.phone')"
        prepend-icon="mdi-phone"
        outlined
        filled
        readonly
      />
      <v-row dense>
        <v-col>
          <v-alert v-if="selectedCategory && selectedCategory.surveySchema && issue.survey" border="left" colored-border type="info" elevation="2">
            <v-form>
              <v-jsf v-model="issue.surveyObject" :schema="selectedCategoryObject" />
            </v-form>
          </v-alert>
        </v-col>
      </v-row>
      <v-row>
        <v-col>
          <file-manager
            v-model="issue.documents"
            disabled
          />
        </v-col>
      </v-row>
    </v-card-text>
  </v-card>
</template>

<script>
import Constants from "@/common/constants.js";
import FileManager from "@/components/wrappers/FileManager.vue";
import VJsf from '@koumoul/vjsf/lib/VJsf.js';
import '@koumoul/vjsf/lib/VJsf.css';
import '@koumoul/vjsf/lib/deps/third-party.js';

export default {
  name: "IssueDetailsCard",
  components: {
    FileManager,
    VJsf
  },
  props: {
    issue: {
      type: Object,
      default() {
        return undefined;
      },
    },
  },
  data() {
    return {
      dateTimeFormat: `${Constants.DATEPICKER_FORMAT} ${Constants.DISPLAY_TIME_FORMAT}`,
      selectedCategory: null,
      selectedCategoryObject: null
    };
  },

  computed: {
    subtitle() {
      if (!this.issue) return "";

      const modifyDate = this.issue.modifyDate
        ? this.$moment
            .utc(this.issue.modifyDate)
            .local()
            .format(this.dateTimeFormat)
        : "";
      if (modifyDate) return `редактира на ${modifyDate}`;

      const createDate = this.issue.createDate
        ? this.$moment
            .utc(this.issue.createDate)
            .local()
            .format(this.dateTimeFormat)
        : "";
      return `създаде на ${createDate}`;
    },
  },
  mounted() {
    this.$api.lookups
          .getCategory(this.issue.categoryId)
          .then((response) => {
            if (response.data) {
              this.selectedCategory = response.data;
              if (this.selectedCategory.surveySchema){
                this.selectedCategoryObject = JSON.parse(this.selectedCategory.surveySchema);
                this.selectedCategoryObject["readOnly"] = true;
              }
            }
          })
          .catch((error) => {
            console.log(error);
            console.log(error.response);
          });
  },
};
</script>
