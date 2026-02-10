<template>
  <v-card
    v-if="model"
    class="mb-2"
  >
    <v-form
      :ref="'EditorForm_' + model.uid"
      :disabled="disabled"
    >
      <v-card-actions>
        <v-spacer />
        <button-tip
          v-if="!disabled"
          icon-name="fas fa-times"
          color="error"
          icon-color="error"
          :text="deleteBtnText(model.name || model.uid)"
          outlined
          :tooltip="deleteBtnText(model.name || model.uid)"
          iclass="mx-2"
          left
          small
          @click="$emit('docPartDelete', model.uid)"
        />
      </v-card-actions>
      <v-card-text>
        <v-row dense>
          <v-col
            cols="12"
            md="6"
          >
            <v-text-field
              v-model="model.name"
              :label="$t('basicDocumentPart.name')"
              :rules="[$validator.required()]"
              clearable
              class="required"
              @input="v => $emit('input', model)"
            />
          </v-col>

          <v-col
            cols="12"
            md="6"
          >
            <v-text-field
              v-model="model.description"
              :label="$t('basicDocumentPart.description')"
              clearable
              @input="v => $emit('input', model)"
            />
          </v-col>
          <v-col
            cols="12"
            sm="4"
            md="2"
          >
            <v-text-field
              v-model="model.position"
              type="number"
              :label="$t('basicDocumentPart.position')"
              :rules="[$validator.required(), $validator.min(0), $validator.max(1000)]"
              clearable
              class="required"
            />
          </v-col>
          <v-col
            cols="12"
            sm="4"
            md="2"
          >
            <c-info
              uid="basicDocumentPart.code"
            >
              <c-select
                v-model="model.code"
                :items="basicDocumentPartOptions"
                :label="$t('basicDocumentPart.code')"
                :rules="[$validator.required()]"
                :hint="'Изберете опция от падащото меню'"
                clearable
                class="required"
              />
            </c-info>
          </v-col>
          <v-col
            cols="12"
            sm="4"
            md="2"
          >
            <v-select
              v-model="model.basicClassId"
              :items="basicClassOptions"
              :label="$t('basicDocumentPart.basicClass')"
              item-value="value"
              item-text="text"
              clearable
            />
          </v-col>
          <v-col
            cols="12"
            md="3"
          >
            <v-autocomplete
              v-model="model.subjectTypes"
              :items="subjectTypeOptions"
              :label="$t('basicDocumentPart.basicSubjectType')"
              :placeholder="$t('buttons.search')"
              chips
              small-chips
              deletable-chips
              clearable
              hide-no-data
              multiple
            >
              <template v-slot:item="data">
                <template>
                  <v-list-item-content
                    v-text="data.item.text"
                  />
                  <v-list-item-icon
                    v-if="!data.item.isValid"
                  >
                    <v-chip
                      color="error"
                      small
                      outlined
                    >
                      {{ $t('common.unactive') }}
                    </v-chip>
                  </v-list-item-icon>
                </template>
              </template>
            </v-autocomplete>
          </v-col>
          <v-col
            cols="12"
            md="3"
          >
            <v-autocomplete
              v-model="model.externalEvaluationTypes"
              :items="externalEvaluationTypeOptions"
              :label="$t('basicDocumentPart.externalEvaluationType')"
              :placeholder="$t('buttons.search')"
              item-value="id"
              item-text="name"
              chips
              small-chips
              deletable-chips
              clearable
              hide-no-data
              multiple
            />
          </v-col>
          <v-col
            cols="12"
            sm="4"
            md="2"
          >
            <v-checkbox
              v-model="model.isHorariumHidden"
              color="primary"
              :label="$t('basicDocumentPart.isHorariumHidden')"
            />
          </v-col>
          <v-col
            cols="12"
            sm="4"
            md="2"
          >
            <v-text-field
              v-model="model.totalLines"
              type="number"
              :label="$t('basicDocumentPart.totalLines')"
              :rules="[$validator.min(1), $validator.max(1000)]"
              oninput="if(Number(this.value) < 1) this.value = 1;"
              clearable
            />
          </v-col>
          <v-col
            cols="12"
            sm="4"
            md="2"
          >
            <v-text-field
              v-model="model.printedLines"
              type="number"
              :label="$t('basicDocumentPart.printedLines')"
              :rules="[$validator.min(1), $validator.max(1000)]"
              oninput="if(Number(this.value) < 1) this.value = 1;"
              clearable
            />
          </v-col>
        </v-row>

        <v-divider class="mt-3 mb-3" />

        <button-tip
          v-if="!disabled"
          text="basicDocumentSubject.add"
          color="primary"
          tooltip="basicDocumentSubject.add"
          right
          small
          @click="onDocSubjectAdd"
        />

        <div
          v-if="sortedDocumentSubjects && sortedDocumentSubjects.length > 0"
        >
          <basic-document-subject-editor
            v-for="(subject,index) in sortedDocumentSubjects"
            :key="subject.uid"
            v-model="sortedDocumentSubjects[index]"
            :subject-type-options="filteredSubjectTypeOptions"
            @subject-delete="onDocSubjectDelete"
          />
        </div>
      </v-card-text>
    </v-form>
  </v-card>
</template>

<script>
import BasicDocumentSubjectEditor from '@/components/diplomas/BasicDocumentSubjectEditor.vue';
import { BasicDocumentPartCategory } from '../../enums/enums';

export default {
  name: 'BasicDocumentPartEditor',
  components: {
    BasicDocumentSubjectEditor
  },
  props: {
    value: {
      type: Object,
      required: true
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
      basicClassOptions: [],
      subjectTypeOptions: [],
      externalEvaluationTypeOptions: [],
      basicDocumentPartOptions: BasicDocumentPartCategory,
    };
  },
  computed: {
    sortedDocumentSubjects() {
      if(!this.model || !this.model.basicDocumentSubjects) return [];

      let sortedDocSubjects = this.model.basicDocumentSubjects;
      sortedDocSubjects = sortedDocSubjects.sort((a, b) => { return a.position - b.position; });
      return sortedDocSubjects;
    },
    filteredSubjectTypeOptions() {
      if(!this.subjectTypeOptions) return [];

      if(!this.model || !this.model.subjectTypes) {
        return this.subjectTypeOptions;
      } else {
        return this.subjectTypeOptions.filter(x => this.model.subjectTypes.includes(x.value));
      }
    }
  },
  watch: {
    'model.totalLines': function (val) {
      if(val === null) {
        return this.model.printedLines = null;
      }

      if(Number(val) < Number(this.model.printedLines)) {
        return this.model.printedLines = Number(val);
      }
    },
  },
  mounted() {
    this.loadOptions();
  },
  methods: {
    loadOptions() {
      this.$api.lookups
      .getBasicClassOptions()
      .then((response) => {
        if(response.data) {
          this.basicClassOptions = response.data;
        }
      })
      .catch((error) => {
        console.log(error);
      });

      this.$api.lookups
      .getSubjectTypeOptions({ showAll: true })
      .then((response) => {
        if(response.data) {
          this.subjectTypeOptions = response.data;
        }
      })
      .catch((error) => {
        console.log(error);
      });


      this.$api.lookups
      .getExternalEvaluationTypeOptions()
      .then((response) => {
        if(response.data) {
          this.externalEvaluationTypeOptions = response.data;
        }
      })
      .catch((error) => {
        console.log(error);
      });
    },
    onDocSubjectDelete(uid) {
      const index = this.model.basicDocumentSubjects.findIndex(x => x.uid === uid);
      if(index > -1) {
        this.model.basicDocumentSubjects.splice(index, 1);
      }
    },
    onDocSubjectAdd() {
      if(!this.model.basicDocumentSubjects) {
        this.model.basicDocumentSubjects = [];
      }

      this.model.basicDocumentSubjects.push({
        id: '',
        position: 1,
        subjectCanChange: false,
        subjectTypeId: this.model.subjectTypes && this.model.subjectTypes.length === 1
          ? this.model.subjectTypes[0]
          : null,
        uid: this.$uuid.v4(),
      });
    },
    validate() {
      const form = this.$refs['EditorForm_' + this.model.uid];
      return form ? form.validate() : false;
    },
    deleteBtnText(text) {
      return { key: 'basicDocumentPart.deleteBtnTooltip', value: text };
    }
  }
};
</script>
