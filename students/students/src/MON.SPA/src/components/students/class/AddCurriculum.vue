<template>
  <div>
    <div
      v-if="loading"
    >
      <v-progress-linear
        v-if="loading"
        indeterminate
        color="primary"
      />
    </div>

    <v-card
      v-else
      class="elevation-2"
    >
      <v-card-title>
        {{ $t('addCurriculum.title') }}
        <v-spacer />
        <v-text-field
          v-model="search"
          append-icon="mdi-magnify"
          :label="$t('common.search')"
          single-line
          hide-details
        />
      </v-card-title>
      <v-card-text>
        <v-data-table
          v-model="selected"
          :headers="headers"
          :items="curriculumClasses"
          item-key="curriculumId"
          class="custom-grid"
          :loading="loading"
          :search="search"
          :items-per-page="100"
          :footer-props="{itemsPerPageOptions: gridItemsPerPageOptions}"
          :item-class="itemRowBackground"
          show-select
        >
          <template v-slot:top>
            <v-toolbar
              flat
            >
              <v-row dense>
                <v-col
                  cols="12"
                  md="6"
                >
                  <GridExporter
                    v-if="curriculumClasses.length > 0"
                    :items="curriculumClasses"
                    :file-extensions="['xlsx', 'csv', 'txt']"
                    :file-name="$t('addCurriculum.fileName')"
                    :headers="headers"
                  />
                </v-col>
                <v-col
                  cols="12"
                  md="6"
                >
                  <v-container
                    class="px-0"
                    fluid
                  >
                    <v-radio-group
                      v-model="statusFilter"
                      row
                    >
                      <v-spacer />
                      <v-radio
                        :label="$t('addCurriculum.filter.active')"
                        :value="0"
                      />
                      <v-radio
                        :label="$t('addCurriculum.filter.inactive')"
                        :value="1"
                      />
                      <v-radio
                        :label="$t('addCurriculum.filter.all')"
                        :value="2"
                      />
                    </v-radio-group>
                  </v-container>
                </v-col>
              </v-row>
            </v-toolbar>
          </template>
          <template
            v-slot:[`item.isIndividualLesson`]="{ item }"
          >
            <div>
              <yes-no
                :value="item.isIndividualLesson ? true : false"
                :class="item.isIndividualLesson === true ? 'success--text': ''"
              />
            </div>
          </template>
          <template
            v-slot:[`item.isIndividualCurriculum`]="{ item }"
          >
            <div>
              <yes-no
                :value="item.isIndividualCurriculum ? true : false"
                :class="item.isIndividualCurriculum === true ? 'success--text': ''"
              />
            </div>
          </template>
          <template
            v-slot:[`item.isCurriculumIncluded`]="{ item }"
          >
            <v-chip
              :color="item.isCurriculumIncluded ? 'success' : 'light'"
              small
            >
              <yes-no
                :value="item.isCurriculumIncluded"
              />
            </v-chip>
          </template>
          <template
            v-slot:[`item.isAllStudents`]="{ item }"
          >
            <div>
              <yes-no
                :value="item.isAllStudents ? true : false"
                :class="item.isAllStudents === true? 'success--text': ''"
              />
            </div>
          </template>
          <template
            v-slot:[`item.isWholeClass`]="{ item }"
          >
            <div>
              <yes-no
                v-if="item.isWholeClass"
                :value="true"
              />
              <span
                v-else
              >
                {{ item.curriculumGroupNum }} {{ $t('addCurriculum.group') }}
              </span>
            </div>
          </template>
          <template
            v-slot:[`item.isValid`]="{ item }"
          >
            <v-chip
              :color="item.isValid ? 'light' : 'error'"
              small
            >
              <yes-no
                :value="item.isValid"
              />
            </v-chip>
          </template>
          <template
            v-slot:[`item.weeksFirstTerm`]="{ item }"
          >
            <v-tooltip
              v-if="!!item.weeksFirstTerm || !!item.curriculumWeeksFirstTerm"
              bottom
            >
              <template #activator="{ on }">
                <v-chip
                  :color="item.weeksFirstTerm > 0 ? 'success' : 'light'"
                  small
                  v-on="on"
                >
                  {{ item.weeksFirstTerm > 0 ? item.weeksFirstTerm : item.curriculumWeeksFirstTerm }}
                </v-chip>
              </template>
              <span>{{ item.weeksFirstTerm > 0 ? $t('addCurriculum.curriculumStudentSource') : $t('addCurriculum.curriculumSource') }}</span>
            </v-tooltip>
          </template>
          <template
            v-slot:[`item.hoursWeeklyFirstTerm`]="{ item }"
          >
            <v-tooltip
              v-if="!!item.hoursWeeklyFirstTerm || !!item.curriculumHoursWeeklyFirstTerm"
              bottom
            >
              <template #activator="{ on }">
                <v-chip
                  :color="item.hoursWeeklyFirstTerm > 0 ? 'success' : 'light'"
                  small
                  v-on="on"
                >
                  {{ item.hoursWeeklyFirstTerm > 0 ? item.hoursWeeklyFirstTerm : item.curriculumHoursWeeklyFirstTerm }}
                </v-chip>
              </template>
              <span>{{ item.hoursWeeklyFirstTerm > 0 ? $t('addCurriculum.curriculumStudentSource') : $t('addCurriculum.curriculumSource') }}</span>
            </v-tooltip>
          </template>
          <template
            v-slot:[`item.weeksSecondTerm`]="{ item }"
          >
            <v-tooltip
              v-if="!!item.weeksSecondTerm || !!item.curriculumWeeksSecondTerm"
              bottom
            >
              <template #activator="{ on }">
                <v-chip
                  :color="item.weeksSecondTerm > 0 ? 'success' : 'light'"
                  small
                  v-on="on"
                >
                  {{ item.weeksSecondTerm > 0 ? item.weeksSecondTerm : item.curriculumWeeksSecondTerm }}
                </v-chip>
              </template>
              <span>{{ item.weeksSecondTerm > 0 ? $t('addCurriculum.curriculumStudentSource') : $t('addCurriculum.curriculumSource') }}</span>
            </v-tooltip>
          </template>

          <template
            v-slot:[`item.hoursWeeklySecondTerm`]="{ item }"
          >
            <v-tooltip
              v-if="!!item.hoursWeeklySecondTerm || !!item.curriculumHoursWeeklySecondTerm"
              bottom
            >
              <template #activator="{ on }">
                <v-chip
                  :color="item.hoursWeeklySecondTerm > 0 ? 'success' : 'light'"
                  small
                  v-on="on"
                >
                  {{ item.hoursWeeklySecondTerm > 0 ? item.hoursWeeklySecondTerm : item.curriculumHoursWeeklySecondTerm }}
                </v-chip>
              </template>
              <span>{{ item.hoursWeeklySecondTerm > 0 ? $t('addCurriculum.curriculumStudentSource') : $t('addCurriculum.curriculumSource') }}</span>
            </v-tooltip>
          </template>
          <template v-slot:[`item.actions`]="{ item }">
            <button-group>
              <template>
                <button-tip
                  v-if=" !!item.curriculumStudentId && item.isCurriculumIncluded === true && item.isProfSubject !== true && item.isIndividualCurriculum !== true && hasStudentCurriculumManagePermission"
                  icon
                  icon-name="mdi-pencil"
                  icon-color="primary"
                  tooltip="buttons.edit"
                  iclass=""
                  small
                  bottom
                  @click="editItem(item)"
                />
              </template>
            </button-group>
          </template>
          <template v-slot:[`footer.prepend`]>
            <button-group>
              <v-btn
                v-if="hasStudentCurriculumManagePermission && selected && selected.length > 0"
                small
                color="success"
                @click="onAddCurriculum()"
              >
                <v-icon
                  left
                  color="white"
                >
                  mdi-plus
                </v-icon>
                {{ $t('addCurriculum.approvеBtnTooltip') }}
              </v-btn>
              <v-btn
                v-if="hasStudentCurriculumManagePermission && selected && selected.length > 0"
                small
                color="error"
                @click="onRemoveCurriculum()"
              >
                <v-icon
                  left
                  color="white"
                >
                  mdi-delete
                </v-icon>
                {{ $t('addCurriculum.disapproveBtnTooltip') }}
              </v-btn>
              <v-btn
                small
                color="secondary"
                outlined
                @click="load"
              >
                {{ $t('buttons.reload') }}
              </v-btn>
            </button-group>
          </template>
        </v-data-table>
      </v-card-text>
      <v-card-actions>
        <v-spacer />
        <v-btn
          raised
          color="primary"
          @click.stop="backClick"
        >
          <v-icon left>
            fas fa-chevron-left
          </v-icon>
          {{ $t('buttons.back') }}
        </v-btn>
      </v-card-actions>
    </v-card>
    <v-dialog
      v-model="dialog"
      max-width="800px"
      persistent
    >
      <form-layout
        skip-cancel-prompt
        @on-save="onCurricuumStudentSave"
        @on-cancel="dialog = false"
      >
        <template #title>
          УП на ученик
        </template>
        <template #subtitle>
          {{ `${editedItem.subjectName} / ${editedItem.subjectTypeName} / ${editedItem.curriculumPartName}` }}
        </template>
        <template>
          <!-- {{ editedItem }} -->
          <v-form
            :ref="'curriculumStudentForm' + _uid"
          >
            <v-row dense>
              <v-col
                cols="12"
                md="6"
              >
                <v-text-field
                  v-model="editedItem.weeksFirstTerm"
                  type="number"
                  step="1"
                  :label="$t('addCurriculum.weeksFirstTerm')"
                  :rules="[$validator.min(1), $validator.max(32767), $validator.numbers()]"
                  clearable
                  outlined
                  persistent-placeholder
                />
              </v-col>
              <v-col
                cols="12"
                md="6"
              >
                <v-text-field
                  v-model="editedItem.hoursWeeklyFirstTerm"
                  type="number"
                  :label="$t('addCurriculum.hoursWeeklyFirstTerm')"
                  :rules="[$validator.min(0)]"
                  clearable
                  outlined
                  persistent-placeholder
                />
              </v-col>
              <v-col
                cols="12"
                md="6"
              >
                <v-text-field
                  v-model="editedItem.weeksSecondTerm"
                  type="number"
                  :label="$t('addCurriculum.weeksSecondTerm')"
                  :rules="[$validator.min(1), $validator.max(32767), $validator.numbers()]"
                  clearable
                  outlined
                  persistent-placeholder
                />
              </v-col>
              <v-col
                cols="12"
                md="6"
              >
                <v-text-field
                  v-model="editedItem.hoursWeeklySecondTerm"
                  type="number"
                  :label="$t('addCurriculum.hoursWeeklySecondTerm')"
                  :rules="[$validator.min(0)]"
                  clearable
                  outlined
                  persistent-placeholder
                />
              </v-col>
            </v-row>
          </v-form>
        </template>
      </form-layout>
    </v-dialog>
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </div>
</template>

<script>
import GridExporter from "@/components/wrappers/gridExporter";
import { mapGetters } from 'vuex';
import { Permissions } from '@/enums/enums';

export default {
  name: "AddCurriculum",
  components: {
    GridExporter
  },
  props: {
    personId: {
      type: Number,
      required: true,
    },
    classId: {
      type: Number,
      required: true,
    },
    schoolYear: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      loading: true,
      saving: false,
      curriculumClasses: [],
      curriculumStudentIds: [],
      search: '',
      statusFilter: 0,
      selected: [],
      headers: [
        // {
        //   value: 'bc',
        //   text: '#',
        // },
        {
          text: this.$t('addCurriculum.headers.isCurriculumIncluded'),
          value: 'isCurriculumIncluded'
        },
        {
          text: this.$t('addCurriculum.headers.subjectName'),
          value: 'subjectName'
        },
        {
          text: this.$t('addCurriculum.headers.flsubject'),
          value: 'flsubjectName'
        },
        {
          text: this.$t('addCurriculum.headers.subjectTypeName'),
          value: 'subjectTypeName'
        },
        {
          text: this.$t('addCurriculum.headers.section'),
          value: 'curriculumPartName'
        },
        // {
        //   text: this.$t('addCurriculum.headers.sectionDescription'),
        //   value: 'curriculumPartDescription'
        // },
        {
          text: this.$t('addCurriculum.headers.isIndividualLesson'),
          value: 'isIndividualLesson'
        },
        {
          text: this.$t('addCurriculum.headers.isIndividualCurriculum'),
          value: 'isIndividualCurriculum'
        },
        // {
        //   text: this.$t('addCurriculum.headers.isAllStudents'),
        //   value: 'isAllStudents'
        // },
        {
          text: this.$t('addCurriculum.headers.isWholeClass'),
          value: 'isWholeClass'
        },
        // {
        //   text: this.$t('addCurriculum.headers.isValid'),
        //   value: 'isValid'
        // },
        {
          text: this.$t('addCurriculum.weeksFirstTerm'),
          value: 'weeksFirstTerm'
        },
        {
          text: this.$t('addCurriculum.hoursWeeklyFirstTerm'),
          value: 'hoursWeeklyFirstTerm'
        },
        {
          text: this.$t('addCurriculum.weeksSecondTerm'),
          value: 'weeksSecondTerm'
        },
        {
          text: this.$t('addCurriculum.hoursWeeklySecondTerm'),
          value: 'hoursWeeklySecondTerm'
        },
        {
          text: this.$t('addCurriculum.totalTermHours'),
          value: 'computedTotalTermHours',
        },
        { text: '', value: 'actions', sortable: false, align: 'end' }
      ],
      dialog: false,
      editedItem: {
        personId: null,
        curriculumStudentId: null,
        weeksFirstTerm: null,
        hoursWeeklyFirstTerm: null,
        weeksSecondTerm: null,
        hoursWeeklySecondTerm: null,
      },
      defaultItem: {
        personId: null,
        curriculumStudentId: null,
        weeksFirstTerm: null,
        hoursWeeklyFirstTerm: null,
        weeksSecondTerm: null,
        hoursWeeklySecondTerm: null,
      },
    };
  },
  computed: {
    ...mapGetters(['gridItemsPerPageOptions', 'hasStudentPermission']),
    hasStudentCurriculumManagePermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentCurriculumManage);
    }
  },
  watch: {
    statusFilter() {
      this.load();
    },
    dialog (val) {
        val || this.close();
      },
  },
  mounted() {
    this.load();
  },
  methods: {
    load() {
      this.$api.curriculum.getForStudentClass(this.classId, this.statusFilter) // this.classId всъщност е StudentClass.Id
        .then((response) => {
          if (response.data) {
            this.curriculumClasses = response.data;
          }
        })
        .catch((error) => {
          console.log(error.response);
        })
        .then(() => {
          this.loading = false;
        });
    },
    onAddCurriculum() {
      const invalid = this.selected.filter(x => x.isValid === false || x.isProfSubject === true);
      if(invalid?.length > 0) {
        const errors = invalid.map(x => `${x.subjectName} - ${x.subjectTypeName}${x.isValid === false ?  ` - ${this.$t('common.unactive')}` : ''}`);
        return this.$notifier.modalWarn(`${this.$t('errors.invalidSelection')} ${this.$t('errors.curriculumInvalidSubjectAdd')}`, errors);
      }

      const model = {
        personId: this.personId,
        curriculumIds: this.selected.map(el => el.curriculumId),
        schoolYear: this.schoolYear,
        studentClassId: this.classId
      };
      if (!model.curriculumIds || model.curriculumIds.length === 0) {
        return this.$notifier.error('', this.$t('addCurriculum.missingSelection'));
      }

      this.saving = true;
      this.$api.curriculum.addForStudentClass(model)
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          this.selected = [];
          this.load();
        })
        .catch((error) => {
          const {message, errors, clientNotificationLevel} = this.$helper.parseError(error.response);
          this.$notifier.modal(message, errors, clientNotificationLevel);
        })
        .then(() => {
          this.saving = false;
        });
    },
    onRemoveCurriculum() {
      const model = {
        personId: this.personId,
        curriculumIds: this.selected.map(el => el.curriculumId),
        schoolYear: this.schoolYear,
        studentClassId: this.classId
      };

      if (!model.curriculumIds || model.curriculumIds.length === 0) {
        return this.$notifier.error('', this.$t('addCurriculum.missingSelection'));
      }

      this.saving = true;
      this.$api.curriculum.removeForStudentClass(model)
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          this.selected = [];
          this.load();
        })
        .catch((error) => {
          const {message, errors, clientNotificationLevel} = this.$helper.parseError(error.response);
          this.$notifier.modal(message, errors, clientNotificationLevel);
        })
        .then(() => {
          this.saving = false;
        });
    },
    itemRowBackground(item) {
      return item.isModule ? 'custom-grid-row left border-info' : '';
    },
    backClick() {
      this.$router.go(-1);
    },
    editItem (item) {
      this.editedItem = Object.assign({}, item);
      this.editedItem.personId = this.personId;
      this.dialog = true;
    },
    onCurricuumStudentSave() {
      const form = this.$refs['curriculumStudentForm' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      }

      this.saving = true;
      this.$api.curriculum.editCurriculumStudent(this.editedItem)
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          this.load();
        })
        .catch((error) => {
          const {message, errors, clientNotificationLevel} = this.$helper.parseError(error.response);
          this.$notifier.modal(message, errors, clientNotificationLevel);
        })
        .finally(() => {
          this.close();
          this.saving = false;
        });
    },
    close () {
        this.dialog = false;
        this.$nextTick(() => {
          this.editedItem = Object.assign({}, this.defaultItem);
        });
      },
  },


};
</script>

<style>
.v-data-table__checkbox.v-simple-checkbox.v-simple-checkbox--disabled {
  display: none;
}
.custom-grid table {
  border-collapse: collapse !important;
}
</style>
