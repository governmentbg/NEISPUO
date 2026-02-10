<template>
  <div>
    <v-form :ref="'form' + _uid" :disabled="disabled">
      <v-row dense>
        <v-col cols="12">
          <text-field
            v-model="model.question"
            :label="$t('question.question')"
            clearable
            counter
            maxlength="200"
            :rules="[$validator.required(), $validator.maxLength(2048)]"
            class="required"
          />
        </v-col>
        <v-col cols="12">
          <v-card>
            <v-toolbar dense>
              <v-spacer />
              <markdown-toolbar for="textarea_id">
                <v-btn small>
                  <md-bold>
                    <v-icon small> fas fa-bold </v-icon>
                  </md-bold>
                </v-btn>
                <v-btn small>
                  <md-strikethrough>
                    <v-icon small> fas fa-strikethrough </v-icon>
                  </md-strikethrough>
                </v-btn>
                <v-btn small>
                  <md-header>
                    <v-icon small> fas fa-heading </v-icon>
                  </md-header>
                </v-btn>
                <v-btn small>
                  <md-italic>
                    <v-icon small> fas fa-italic </v-icon>
                  </md-italic>
                </v-btn>
                <v-btn small>
                  <md-quote>
                    <v-icon small> fas fa-quote-left </v-icon>
                  </md-quote>
                </v-btn>
                <v-btn small>
                  <md-code>
                    <v-icon small> fas fa-code </v-icon>
                  </md-code>
                </v-btn>
                <v-btn small>
                  <md-link>
                    <v-icon small> fas fa-link </v-icon>
                  </md-link>
                </v-btn>
                <v-btn small>
                  <md-image>
                    <v-icon small> fas fa-image </v-icon>
                  </md-image>
                </v-btn>
                <v-btn small>
                  <md-unordered-list>
                    <v-icon small> fas fa-list </v-icon>
                  </md-unordered-list>
                </v-btn>
                <v-btn small>
                  <md-ordered-list>
                    <v-icon small> fas fa-list-ol </v-icon>
                  </md-ordered-list>
                </v-btn>
                <!-- <v-btn small>
              <md-task-list>task-list</md-task-list>
            </v-btn>
            <v-btn small>
              <md-mention>mention</md-mention>
            </v-btn>
            <v-btn small>
              <md-ref>ref</md-ref>
            </v-btn> -->
              </markdown-toolbar>
            </v-toolbar>
            <v-card-text>
              <v-textarea
                id="textarea_id"
                v-model="model.answer"
                :label="$t('question.answer')"
                prepend-icon="mdi-text"
                outlined
                clearable
                :rules="[$validator.required()]"
                class="required"
                @drop.prevent="onDrop($event)"
                @dragover="allowDrop($event)"
              />
              <v-input>
                <template name="default">
                  <v-container fluid class="ma-0 pa-0">
                    <v-alert
                      border="left"
                      colored-border
                      color="deep-purple accent-4"
                      elevation="2"
                    >
                      <!-- eslint-disable-next-line vue/no-v-html -->
                      <div v-html="compiledMarkdown" />
                    </v-alert>
                  </v-container>
                </template>
                <template #prepend>
                  <v-icon>mdi-language-markdown</v-icon>
                </template>
              </v-input>
            </v-card-text>
          </v-card>
        </v-col>
        <v-col>
          <v-card>
            <v-card-title><v-icon>fas fa-info-circle</v-icon>Помощна информация</v-card-title>
            <v-card-text>
              <p>
                За да добавите прикачен файл, довлачете файл до мястото, където
                искате да се появи
              </p>
            </v-card-text>
          </v-card>
        </v-col>
      </v-row>
    </v-form>
  </div>
</template>

<script>
import { QuestionModel } from "@/models/questionModel";
import { mapGetters } from "vuex";
import { UserRole } from "@/enums/enums";
import * as marked from "marked";
import "@github/markdown-toolbar-element";

export default {
  name: "QuestionForm",
  props: {
    question: {
      type: Object,
      default: null,
    },
    disabled: {
      type: Boolean,
      default() {
        return false;
      },
    },
  },
  data() {
    return {
      model: this.question ?? new QuestionModel(),
      uploadedFiles: [],
    };
  },
  computed: {
    ...mapGetters(["isInRole"]),
    compiledMarkdown: function () {
      return marked.marked(this.model.answer, { sanitize: true });
    },
    isElevated() {
      return (
        this.isInRole(UserRole.Ruo) ||
        this.isInRole(UserRole.RuoExpert) ||
        this.isInRole(UserRole.Mon) ||
        this.isInRole(UserRole.MonExpert) ||
        this.isInRole(UserRole.Cioo) ||
        this.isInRole(UserRole.Consortium)
      );
    },
    isLevel2SupportGroup() {
      return (
        this.isInRole(UserRole.Mon) ||
        this.isInRole(UserRole.MonExpert) ||
        this.isInRole(UserRole.Cioo) ||
        this.isInRole(UserRole.Consortium)
      );
    },
  },
  methods: {
    validate() {
      const form = this.$refs["form" + this._uid];
      return form ? form.validate() : false;
    },
    async onDrop(e) {
      if (
        e.dataTransfer.files.length != 1 ||
        e.dataTransfer.files[0].size > 5 * 1024 * 1024
      ) {
        this.$notifier.error("", this.$t("errors.singleFileAllowed"), 5000);
      } else {
        let fileName = await this.$api.file.upload(e.dataTransfer.files[0]);
        let serverFileName = `[${e.dataTransfer.files[0].name}](/file/${fileName.data})`;
        this.insertBreakAtPoint(e, serverFileName);
        this.uploadedFiles.push(e.dataTransfer.files[0]);
      }
    },
    insertBreakAtPoint(e, contents) {
      let range;
      let textNode;
      let offset;

      if (document.caretRangeFromPoint) {
        range = document.caretRangeFromPoint(e.clientX, e.clientY);
        textNode = range.startContainer;
        offset = range.startOffset;
      } else if (document.caretPositionFromPoint) {
        range = document.caretPositionFromPoint(e.clientX, e.clientY);
        textNode = range.offsetNode;
        offset = range.offset;
      } else {
        return;
      }
      console.log(range);
      console.log(textNode);
      console.log(offset);

      let tok1 = this.question.answer.substr(0, offset);
      let tok2 = this.question.answer.substr(offset);

      this.question.answer = tok1 + contents + tok2;

      // Only split TEXT_NODEs
      if (textNode && textNode.nodeType == 3) {
        let replacement = textNode.splitText(offset);
        let br = document.createElement("br");
        textNode.parentNode.insertBefore(br, replacement);
      }
    },
    allowDrop(event) {
      event.preventDefault();
    },
  },
};
</script>
