<template>
  <div>
    <!-- {{ note }} -->

    <v-form
      :ref="'form' + _uid"
      :disabled="disabled"
    >
      <v-row>
        <v-col
          cols="12"
          sm="6"
          md="3"
          lg="2"
        >
          <c-info uid="notes.issueDate">
            <date-picker
              id="issueDate"
              ref="issueDate"
              v-model="model.issueDate"
              :show-buttons="false"
              :scrollable="false"
              :no-title="true"
              :show-debug-data="false"
              :label="$t('notes.issueDate')"
              :rules="[$validator.required()]"
              class="required"
            />
          </c-info>
        </v-col>
        <v-col
          cols="12"
          sm="6"
          md="3"
          lg="2"
        >
          <c-info uid="notes.schoolYear">
            <school-year-selector
              v-model="model.schoolYear"
              :label="$t('notes.schoolYear')"
              :min-year="minSchoolYear"
              :max-year="maxSchoolYear"
              :rules="[$validator.required()]"
              class="required"
            />
          </c-info>
        </v-col>
        <v-col
          cols="12"
          md="6"
          lg="8"
        >
          <c-info uid="notes.title">
            <v-text-field
              v-model="model.title"
              :label="$t('notes.noteTitle')"
              :rules="[$validator.required()]"
              class="required"
              required
            />
          </c-info>
        </v-col>

        <v-col
          cols="12"
        >
          <c-info uid="notes.content">
            <v-textarea
              v-model="model.content"
              :label="$t('notes.content')"
              :rules="[$validator.required()]"
              class="required"
              prepend-icon="mdi-comment"
              outlined
              clearable
            />
          </c-info>
        </v-col>
      </v-row>
    </v-form>
    <confirm-dlg ref="confirm" />
  </div>
</template>

<script>
import { mapGetters } from "vuex";
import { StudentNoteModel } from "@/models/studentNoteModel.js";
import SchoolYearSelector from "@/components/common/SchoolYearSelector";

export default {
  name: "NoteForm",
  components: {
    SchoolYearSelector,
  },
  props: {
    note: {
      type: Object,
      default: null,
    },
    disabled: {
      type: Boolean,
      default() {
        return false;
      },
    },
    schoolYearRequired: {
      type: Boolean,
      default: false,
    },
    minSchoolYear: {
      type: [Number, String],
      default() {
        return undefined;
      },
    },
    maxSchoolYear: {
      type: [Number, String],
      default() {
        return undefined;
      },
    },
  },
  data() {
    return {
      model: this.note ?? new StudentNoteModel(),
    };
  },
  computed: {
    ...mapGetters(["studentFinalizedLods"]),
  },
  methods: {
    validate() {
      const form = this.$refs["form" + this._uid];
      return form ? form.validate() : false;
    },
  },
};
</script>
