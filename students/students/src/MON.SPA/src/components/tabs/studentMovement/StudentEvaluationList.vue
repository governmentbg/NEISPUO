<template>
  <v-card>
    <v-data-table
      ref="studentEvaluationListTable"
      :headers="getHeaders()"
      :items="evaluationsWithIndex"
      :items-per-page="5"
      :hide-default-header="true"
      class="elevation-1"
    >
      <template v-slot:top>
        <v-toolbar
          flat
        >
          <v-toolbar-title>
            {{ getTranslation('studentEvaluationListTitle') }}
          </v-toolbar-title>
          <v-spacer />
        </v-toolbar>
      </template>
      <template v-slot:header>
        <thead class="v-data-table-header">
          <tr>
            <th
              rowspan="2"
              class="text-center table-border"
            >
              {{ getTranslation('tableNumberHeader') }}
            </th>
            <th
              rowspan="2"
              class="text-center table-border"
            >
              {{ getTranslation('tableSubjectHeader') }}
            </th>
            <th
              v-if="showTwoYearsAgoEvaluationHeader()"
              rowspan="2"
              class="text-center table-border"
            >
              {{ twoYearsAgoEvaluationHeader }}
            </th>
            <th
              v-if="showOneYearAgoEvaluationHeader()"
              class="text-center table-border"
              rowspan="2"
            >
              {{ oneYearAgoEvaluationHeader }}
            </th>
            <th
              colspan="3"
              class="text-center"
            >
              {{ currentYearEvaluationHeader.toUpperCase() }}
            </th>
          </tr>
          <tr>
            <th class="text-center">
              {{ getTranslation('firstTermEvaluation') }}
            </th>
            <th class="text-center">
              {{ getTranslation('secondTermEvaluation') }}
            </th>
            <th class="text-center">
              {{ getTranslation('annualEvaluation') }}
            </th>
            <th class="text-center">
              {{ getTranslation('actionsHeader') }}
            </th>
          </tr>
        </thead>
      </template>
      <template v-slot:[`item.dropdownModel`]="{ item }">
        <v-edit-dialog
          persistent
          :cancel-text="getTranslation('cancelBtn')"
          :save-text="getTranslation('saveChangesBtn')"
          :return-value.sync="item.dropdownModel"
          large
          @save="save(item, 'dropdownModel')"
          @cancel="cancel"
        >
          <div>{{ item.dropdownModel ? item.dropdownModel.text : '' }}</div>
          <template v-slot:input>
            <combo
              id="subjectOptions"
              ref="subjectOptions"
              v-model="item.dropdownModel"
              api="/api/lookups/GetSubjectOptions"
              :label="getTranslation('subjectName')"
              :placeholder="$t('buttons.search')"
              :return-object="true"
              :defer-options-loading="true"
              :clearable="true"
              :hide-no-data="true"
              :hide-selected="true"
              :disabled="sending || inReviewMode"
              :remove-items-on-clear="true"
              :show-deferred-loading-hint="true"
              :single-line="true"
              :error-messages="getSubjectValidationMessage(item)"
              class="required"
              @blur="subjectBlur(item.index)"
              @change="subjectChange($event, item.index - 1)"
            />
          </template>
        </v-edit-dialog>
      </template>
      <template v-slot:[`item.twoYearsAgoEvaluation`]="{ item }">
        <v-edit-dialog
          :cancel-text="getTranslation('cancelBtn')"
          :save-text="getTranslation('saveChangesBtn')"
          :return-value.sync="item.twoYearsAgoEvaluation"
          persistent
          large
          @save="save(item, 'twoYearsAgoEvaluation')"
          @cancel="cancel"
        >
          <div>{{ item.twoYearsAgoEvaluation }}</div>
          <template v-slot:input>
            <v-text-field
              v-model="item.twoYearsAgoEvaluation"
              :label="getTranslation('evaluationAddEditDialogLabel')"
              :disabled="sending || inReviewMode"
              single-line
              autofocus
            />
          </template>
        </v-edit-dialog>
      </template>
      <template v-slot:[`item.oneYearAgoEvaluation`]="{ item }">
        <v-edit-dialog
          :cancel-text="getTranslation('cancelBtn')"
          :save-text="getTranslation('saveChangesBtn')"
          :return-value.sync="item.oneYearAgoEvaluation"
          persistent
          large
          @save="save(item, 'oneYearAgoEvaluation')"
          @cancel="cancel"
        >
          <div>{{ item.oneYearAgoEvaluation }}</div>
          <template v-slot:input>
            <v-text-field
              v-model="item.oneYearAgoEvaluation"
              :label="getTranslation('evaluationAddEditDialogLabel')"
              :disabled="sending || inReviewMode"
              single-line
              autofocus
            />
          </template>
        </v-edit-dialog>
      </template>
      <template v-slot:[`item.firstTermEvaluation`]="{ item }">
        <v-edit-dialog
          :cancel-text="getTranslation('cancelBtn')"
          :save-text="getTranslation('saveChangesBtn')"
          :return-value.sync="item.firstTermEvaluation"
          persistent
          large
          @save="save(item, 'firstTermEvaluation')"
          @cancel="cancel"
        >
          <div>{{ item.firstTermEvaluation }}</div>
          <template v-slot:input>
            <v-text-field
              v-model="item.firstTermEvaluation"
              :label="getTranslation('evaluationAddEditDialogLabel')"
              :disabled="sending || inReviewMode"
              single-line
              autofocus
            />
          </template>
        </v-edit-dialog>
      </template>
      <template v-slot:[`item.secondTermEvaluation`]="{ item }">
        <v-edit-dialog
          :cancel-text="getTranslation('cancelBtn')"
          :save-text="getTranslation('saveChangesBtn')"
          :return-value.sync="item.secondTermEvaluation"
          persistent
          large
          @save="save(item, 'secondTermEvaluation')"
          @cancel="cancel"
        >
          <div>{{ item.secondTermEvaluation }}</div>
          <template v-slot:input>
            <v-text-field
              v-model="item.secondTermEvaluation"
              :label="getTranslation('evaluationAddEditDialogLabel')"
              :disabled="sending || inReviewMode"
              single-line
              autofocus
            />
          </template>
        </v-edit-dialog>
      </template>
      <template v-slot:[`item.annualEvaluation`]="{ item }">
        <v-edit-dialog
          :cancel-text="getTranslation('cancelBtn')"
          :save-text="getTranslation('saveChangesBtn')"
          :return-value.sync="item.annualEvaluation"
          persistent
          large
          @save="save(item, 'annualEvaluation')"
          @cancel="cancel"
        >
          <div>{{ item.annualEvaluation }}</div>
          <template v-slot:input>
            <v-text-field
              v-model="item.annualEvaluation"
              :label="getTranslation('evaluationAddEditDialogLabel')"
              :disabled="sending || inReviewMode"
              single-line
              autofocus
            />
          </template>
        </v-edit-dialog>
      </template>
      <template v-slot:[`item.actions`]="{ item }">
        <button-tip
          v-if="!inReviewMode"
          icon
          icon-name="mdi-delete"
          icon-color="error"
          tooltip="buttons.delete"
          bottom
          iclass=""
          small
          :disabled="sending"
          @click="deleteItemConfirm(item)"
        />
      </template>
    </v-data-table>
    <confirm-dlg ref="confirm" />
  </v-card>
</template>

<script>
import Constants from '@/common/constants.js';
import Helper from '@/components/helper.js';

import { validationMixin } from 'vuelidate';
import { required  } from 'vuelidate/lib/validators';

export default {
  mixins: [validationMixin],
  props: {
    evaluations: {
      type: Array,
      default() {
        return [];
      }
    },
    sending: {
      type: Boolean,
      default() {
        return false;
      }
    },
    classNumber: {
      type: Number,
      default() {
        return null;
      }
    },
    inReviewMode: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
        editedIndex: -1,
        helper: Helper,
        form: this.evaluations
    };
  },
  validations: {
    form: {
      $each: {
        dropdownModel: {
          required
        }
      }
    }
  },
  computed: {
    evaluationsWithIndex() {  
      return this.evaluations.map(
        (items, index) => ({
          ...items,
          index: index + 1
        }));
    },
    twoYearsAgoEvaluationHeader() {
      return this.classNumber > 2 ?
        this.getTranslation('annualEvaluationLabel', [this.classNumber - 2]) : this.getTranslation('annualEvaluationLabelWithoutParam'); 
    },
    oneYearAgoEvaluationHeader() {
      return this.classNumber > 1 ?
        this.getTranslation('annualEvaluationLabel', [this.classNumber - 1]) : this.getTranslation('annualEvaluationLabelWithoutParam');
    },
    currentYearEvaluationHeader() {
      return this.getTranslation('currentYearEvaluationHeader', [this.classNumber]);
    }
  },
  methods: {
    getTranslation(text, params) {      
      return this.$t(`studentEvaluations.${text}`, params);
    },
    showOneYearAgoEvaluationHeader() {
      return this.classNumber === Constants.SIXTH_GRADE || this.showTwoYearsAgoEvaluationHeader();
    },
    showTwoYearsAgoEvaluationHeader() {
      return this.classNumber === Constants.SEVENTH_GRADE;
    },
    getHeaders() {
      let headers = [
        {
          value: 'index',
          align: 'center'
        },
        {
          value: 'dropdownModel',
          align: 'center'
        },
        this.showTwoYearsAgoEvaluationHeader() ? {
          value: 'twoYearsAgoEvaluation',
          align: 'center'
        } : undefined,
        this.showOneYearAgoEvaluationHeader() ? {
          value: 'oneYearAgoEvaluation',
          align: 'center'
        } : undefined,
        {
          value: 'firstTermEvaluation',
          align: 'center'
        },
        {
          value: 'secondTermEvaluation',
          align: 'center'
        },
        {
          value: 'annualEvaluation',
          align: 'center'
        },
        {
          text: this.getTranslation('actionsHeader'),
          align: 'center',
          value: 'actions', 
          sortable: false
        }
      ];
      
      headers = headers.filter((element) => {
        return element !== undefined;
      });

      return headers;
    },
    subjectBlur(index) {
      const fieldObj = this.$v.form.$each[index - 1];

      if(fieldObj) {
        fieldObj.dropdownModel.$touch();
      }
    },
    subjectChange(value, index) {
      if(typeof value === 'object' && value !== null) {
        this.$refs.subjectOptions.optionsList = [];
        document.getElementsByClassName('v-small-dialog__actions')[0].children[1].disabled = false;
        this.$emit('evaluationSubjectChanged', true);
      }
      else {
        this.$v.form.$each[index].dropdownModel.$touch();
        document.getElementsByClassName('v-small-dialog__actions')[0].children[1].disabled = true;
        this.$emit('evaluationSubjectChanged', false);
      }
    },
    subjectKeyup(event) {
      if(!event) {
        return;
      }

      // to reduce API calls, perform a search only if two characters are typed in the input field
      const value = event.currentTarget.value;
      if(value.length < Constants.SEARCH_INPUT_MIN_LENGTH) {
        return;
      }

      this.getSubjectsFromApi(value);
    },
    getSubjectValidationMessage(item) {
      const fieldObj = this.$v.form.$each[item.index - 1];
      if(fieldObj) {
        const dropdownModelField = this.$v.form.$each[item.index - 1].dropdownModel;
        dropdownModelField.$model = item.dropdownModel;

        return this.helper.getRequiredValidationMessage(dropdownModelField, true);
      }
    },
    async deleteItemConfirm(item) {
        if(await this.$refs.confirm.open(this.$t('buttons.clear'), this.$t('common.confirm'))){
            this.editedIndex = this.evaluationsWithIndex.indexOf(item);
            this.$emit('evaluationValueRemoved', this.editedIndex);
        }
    },
    save(item, propName) {
      if(!this.$v.form.$anyError) {
        this.$emit('evaluationValueChanged', item, propName);
      }
    },
    cancel() {
      document.getElementsByClassName('v-small-dialog__actions')[0].children[1].disabled = false;
      this.$emit('evaluationSubjectChanged', true);
    }
  }
};
</script>

<style>
.table-border {
  border-bottom: thin solid rgba(0, 0, 0, 0.12);
}
</style>