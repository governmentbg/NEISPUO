<template>
  <v-card>
    <v-card-title
      v-if="value.errors && value.errors.length > 0"
      class="pb-0"
    >
      <v-list
        dense
      >
        <v-list-item
          v-for="(error, index) in value.errors"
          :key="index"
        >
          <v-list-item-content class="error--text">
            {{ error.message }}
          </v-list-item-content>
        </v-list-item>
      </v-list>
    </v-card-title>
    <v-card-title>
      <v-row dense>
        <v-radio-group
          v-model="errorsFilter"
          row
          class="mt-0"
        >
          <v-radio
            :label="$t('lod.assessments.filter.valid')"
            :value="0"
          />
          <v-radio
            :label="$t('lod.assessments.filter.invalid')"
            :value="1"
          />
          <v-radio
            :label="$t('lod.assessments.filter.all')"
            :value="2"
          />
        </v-radio-group>
        <v-spacer />
        <button-group>
          <v-btn
            v-if="hasSubjectErrors"
            small
            color="primary"
            bottom
            @click="fixSubjectErrors"
          >
            Корекция на грешки за предмет
          </v-btn>
          <v-btn
            v-if="selectedItems && selectedItems.length > 0"
            small
            color="error"
            bottom
            @click="removeSelected"
          >
            Премахване на избраните
          </v-btn>
          <v-btn
            small
            color="success"
            bottom
            @click="exportFile"
          >
            Експортиране на файл
          </v-btn>
        </button-group>
        <v-spacer />
        <button-group>
          <v-btn
            small
            color="light"
            bottom
            @click="expandAll"
          >
            Покажи всички грешки
          </v-btn>
          <v-btn
            small
            color="light"
            @click="collapseAll"
          >
            Скрий всички грешки
          </v-btn>
        </button-group>
      </v-row>
    </v-card-title>
    <v-card-text>
      <v-data-table
        v-if="items"
        :ref="'dTable_' + _uid"
        v-model="selectedItems"
        :items="items"
        :headers="headers"
        show-select
        show-expand
        selectable-key="hasErrors"
        :expanded.sync="expandedItems"
        :footer-props="{itemsPerPageOptions: gridItemsPerPageOptions, showCurrentPage: true, showFirstLastPage: true}"
        item-key="uid"
      >
        <template v-slot:[`expanded-item`]="{ item }">
          <td
            :colspan="headers.length + 2"
            class="px-0"
          >
            <v-list
              v-if="item.hasErrors === true"
              dense
            >
              <v-list-item
                v-for="(err, index) in item.errors"
                :key="index"
                dense
              >
                <v-list-item-content>
                  {{ err.message }}
                </v-list-item-content>
              </v-list-item>
            </v-list>
          </td>
        </template>
        <template v-slot:[`item.data-table-expand`]="{ item, isExpanded, expand }">
          <td
            class="text-center px-0"
            style="width: 60px;"
          >
            <v-icon
              v-if="item.hasErrors"
              color="error"
            >
              mdi-alert-circle
            </v-icon>
            <v-icon
              v-else
              color="success"
            >
              mdi-check-circle
            </v-icon>
            <v-btn
              v-if="item.hasErrors"
              small
              icon
              @click="expand(!isExpanded)"
            >
              <v-icon v-if="isExpanded">
                mdi-chevron-up
              </v-icon>
              <v-icon v-else>
                mdi-chevron-down
              </v-icon>
            </v-btn>
          </td>
        </template>
      </v-data-table>
    </v-card-text>
    <v-dialog
      v-model="subjectErrorFixDialog"
      transition="dialog-bottom-transition"
    >
      <v-toolbar
        color="primary"
      >
        <v-btn
          icon
          dark
          @click="subjectErrorFixDialog = false"
        >
          <v-icon>mdi-close</v-icon>
        </v-btn>
        <v-toolbar-title class="white--text">
          Корекция на грешкте за предмет
        </v-toolbar-title>
        <v-spacer />
      </v-toolbar>
      <v-card>
        <v-card-text>
          <v-simple-table>
            <template>
              <thead>
                <tr>
                  <th>
                    Наименование на предмет от файла
                  </th>
                  <th>
                    Наименование на ПП от файла
                  </th>
                  <th>
                    Избор на предмет
                  </th>
                  <th />
                </tr>
              </thead>
              <tbody>
                <tr
                  v-for="(item, index) in subjectErrorFixModels"
                  :key="index"
                >
                  <td>{{ item.subjectName }}</td>
                  <td>{{ item.profSubjectDisplayText }}</td>
                  <td>
                    <c-autocomplete
                      v-model="item.newSubject"
                      api="/api/lookups/GetSubjectOptions"
                      :placeholder="$t('buttons.search')"
                      defer-options-loading
                      hide-no-data
                      hide-selected
                      remove-items-on-clear
                      persistent-placeholder
                      dense
                      hide-details
                      append-icon=""
                      clerable
                      return-object
                    >
                      <template v-slot:item="data">
                        <v-list-item-content
                          v-text="data.item.text"
                        />
                        <v-list-item-icon>
                          <v-chip
                            color="light"
                            small
                            outlined
                          >
                            {{ data.item.value }}
                          </v-chip>
                        </v-list-item-icon>
                      </template>
                      <template v-slot:selection="data">
                        <span style="font-size: 0.8em;">{{ data.item.text }}</span>
                        <v-chip
                          color="light"
                          x-small
                          outlined
                        >
                          {{ data.item.value }}
                        </v-chip>
                      </template>
                    </c-autocomplete>
                  </td>
                  <td class="text-center">
                    <v-btn
                      v-if="item.newSubject"
                      small
                      color="primary"
                      bottom
                      @click="replaceSubject(item)"
                    >
                      Подмени
                    </v-btn>
                  </td>
                </tr>
              </tbody>
            </template>
          </v-simple-table>
        </v-card-text>
      </v-card>
    </v-dialog>
  </v-card>
</template>

<script>
import { mapGetters } from 'vuex';
import CAutocomplete from '@/components/wrappers/CustomAutocomplete.vue';
import groupBy from 'lodash.groupby';

export default {
  name: 'LodAssessmentImportValidationDetails',
  components: { CAutocomplete },
  props: {
    value: {
      type: Object,
      default() {
        return undefined;
      }
    }
  },
  data() {
    return {
      selectedItems: [],
      expandedItems: [],
      errorsFilter: 2, // 0 - Валидни, 1 - Невалидни, 2 - Всички,
      subjectErrorFixDialog: false,
      subjectErrorFixModels: [],
      headers: [
        {
          text: 'InstitutionId',
          value: 'institutionId',
        },
        {
          text: 'SchoolYear',
          value: 'schoolYear',
        },
        {
          text: 'PersonalId',
          value: 'personalId',
        },
        {
          text: 'PersonalIdType',
          value: 'personalIdType',
        },
        {
          text: 'CurriculumId',
          value: 'curriculumId',
        },
        {
          text: 'GradeType',
          value: 'gradeType',
        },
        {
          text: 'GradeCode',
          value: 'gradeCode',
        },
        {
          text: 'SubjectId',
          value: 'subjectId',
        },
        {
          text: 'ProfSubjectId',
          value: 'profSubjectDisplayText',
        },
        {
          text: 'SubjectTypeId',
          value: 'subjectTypeId',
        },
        {
          text: 'SubjectName',
          value: 'subjectName',
        },
        {
          text: 'ProfSubjectDecimalGrade',
          value: 'profSubjectDecimalGrade',
        },
        {
          text: 'BasicClassId',
          value: 'basicClassId',
        },
      ]
    };
  },
  computed: {
    ...mapGetters(['gridItemsPerPageOptions']),
    items() {
      if(!this.value || !this.value.models) return null;

      switch (this.errorsFilter) {
        case 0:
          return this.value.models.filter(x => x.hasErrors === false);
        case 1:
          return this.value.models.filter(x => x.hasErrors === true);
        default:
          return this.value.models;
      }
    },
    hasSubjectErrors() {
      return this.value && this.value.models.some(x => x.hasErrors === true && x.errors.some(e => e.id === 'SubjectError'));
    }
  },
  methods:{
    expandAll() {
      this.expandedItems = this.value.models.filter(x => x.hasErrors === true);
    },
    collapseAll() {
      this.expandedItems = [];
    },
    fixSubjectErrors() {
      this.$helper.clearArray(this.subjectErrorFixModels);
      const errors = this.value.models.filter(x => x.hasErrors === true && x.errors.some(e => e.id === 'SubjectError'));
      const groups = groupBy(errors, item => item.profSubjectId ? `"${item.subjectName}|${item.profSubjectId}"` : `${item.subjectName}`);
      for(const key in groups) {
        const firstItem = groups[key][0];
        this.subjectErrorFixModels.push({
          subjectName: firstItem?.subjectName,
          newSubject: null,
          profSubjectId: firstItem?.profSubjectId,
          profSubjectDisplayText: firstItem?.profSubjectDisplayText,
          items: groups[key],
        });
      }
      this.subjectErrorFixDialog = true;
    },
    replaceSubject(model) {
      if(!model.items || !model.newSubject?.value) return;

      model.items.forEach(x => {
        x.subjectId = model.newSubject.value;
        x.subjectName = model.newSubject.text;
        const errorsCount = x.errors.filter(e => e.id === 'SubjectError').length;
        if(errorsCount > 0) {
          for (let i = 0; i < errorsCount; i++) {
            const errorIndex = x.errors.findIndex(e => e.id === 'SubjectError');
            x.errors.splice(errorIndex, 1);
          }
        }

        x.hasErrors = !!x.errors?.length;
      });

      this.recalcHasErrors();
    },
    removeSelected() {
      if(!this.selectedItems) return;

      this.selectedItems.forEach(x => {
        const index = this.value.models.findIndex(m => m.uid === x.uid);
        if(index >= 0 ) {
          this.value.models.splice(index, 1);
        }
      });

      this.$helper.clearArray(this.selectedItems);
      this.recalcHasErrors();
    },
    exportFile() {
      this.$api.lodAssessment.exportFile(this.value)
        .then((response) => {
          const url = window.URL.createObjectURL(new Blob([response.data]));
          const link = document.createElement('a');
          link.href = url;
          link.setAttribute('download', `${this.value.models[0].institutionId}_${this.value.models[0].schoolYear}_replaced.txt`);
          document.body.appendChild(link);
          link.click();
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('common.exportError'));
          console.log(error);
        })
        .finally(() => {
          this.loading = false;
        });
    },
    recalcHasErrors() {
      this.value.hasErrors = this.value.models.some(x => x.hasErrors === true);
    }
  }
};
</script>
