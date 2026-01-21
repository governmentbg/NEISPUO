<template>
  <v-card
    v-if="model"
    class="mb-2"
  >
    <v-card-title>
      {{ model.name }}
      <v-spacer />
      {{ model.basicSubjectType }}
    </v-card-title>

    <v-card-text>
      <v-form
        ref="DiplomaSectionSubjectsForm"
        :disabled="disabled"
      >
        <v-row
          v-for="(subject, index) in model.subjects"
          :key="subject.uid"
          class="mb-0"
        >
          <v-col
            cols="12"
            md="5"
            sm="12"
            class="pb-0"
          >
            <autocomplete
              :ref="'subject' + subject.uid"
              v-model="subject.subjectDropDown"
              api="/api/lookups/GetSubjectOptions"
              :label="$t('basicDocumentSubject.subjectName')"
              :placeholder="$t('buttons.search')"
              return-object
              hide-no-data
              :page-size="20"
              hide-selected
              clearable
              :rules="[$validator.required()]"
              :disabled="subject.lockedForEdit && !subject.subjectCanChange"
              class="required"
            />
          </v-col>
          <v-col
            cols="10"
            md="1"
            sm="2"
            class="pb-0"
          >
            <v-text-field
              :ref="'position' + subject.uid"
              v-model="subject.position"
              type="number"
              :disabled="subject.lockedForEdit"
              :label="$t('basicDocumentSubject.position')"
              :rules="[$validator.required(), $validator.min(0), $validator.max(100)]"
              class="required"
            />
          </v-col>

          <v-col
            cols="12"
            md="4"
            ms="7"
          >
            <subject-grade-editor
              ref="subjectGradeEditor"
              v-model="model.subjects[index]"
              :subjects-grade-options="subjectsGradeOptions"
              :disabled="disabled"
            />
          </v-col>
          <v-col
            cols="10"
            md="1"
            sm="2"
            class="pb-0"
          >
            <v-text-field
              v-if="!subject.isHorariumHidden"
              :ref="'horarium' + subject.uid"
              v-model="subject.horarium"
              type="number"
              :label="$t('studentEvaluations.horarium')"
            />
          </v-col>
          <v-col
            cols="2"
            md="1"
            sm="1"
            class="pb-0"
          >
            <div
              class="text-end pt-4"
            >
              <button-tip
                v-if="!subject.lockedForEdit"
                icon
                icon-name="mdi-delete"
                icon-color="error"
                tooltip="buttons.delete"
                iclass="mx-2"
                small
                left
                @click="onSubjectDelete(subject.uid)"
              />
            </div>
          </v-col>
        </v-row>
      </v-form>
    </v-card-text>
    <v-card-actions>
      <button-tip
        text="basicDocumentSubject.add"
        color="primary"
        tooltip="basicDocumentSubject.add"
        left
        depressed
        @click="onAddSubject"
      />
    </v-card-actions>
  </v-card>
</template>

<script>

import { DiplomaSubjectModel } from '@/models/diploma/diplomaSubjectModel';
import SubjectGradeEditor from './SubjectGradeEditor.vue';
import Autocomplete from '@/components/wrappers/CustomAutocomplete.vue';

export default {
  name: 'DiplomaSectionSubjectsEditor',
  components: {
    SubjectGradeEditor,
    Autocomplete
  },
  props: {
    value: {
      type: Object,
      required: true
    },
    subjectsGradeOptions: {
      type: Array,
      default() {
        return [];
      }
    },
    disabled: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
      model: this.value,
    };
  },
  methods: {
    onAddSubject() {
      this.model.subjects.push(new DiplomaSubjectModel({
        uid: this.$uuid.v4(),
        isHorariumHidden: this.model.isHorariumHidden
      }));
    },
    onSubjectDelete(uid) {
      const index = this.model.subjects.findIndex((x) => x.uid === uid);
      if (index > -1) {
        this.model.subjects.splice(index, 1);
      }
    },
    validate() {
      return this.$validator.validate(this);
    }
  }
};
</script>
