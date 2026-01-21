<template>
  <div>
    <grid
      :ref="'preSchoolEvaluationsGrid' + _uid"
      url="/api/preSchoolEvaluation/list"
      :title="$t('preSchool.title')"
      :headers="headers"
      :filter="{ personId: personId }"
      ref-key="preSchoolEvaluationsList"
      :items-per-page="-1"
      group-by="basicClass"
      show-group-by
    >
      <template v-slot:[`group.header`]="{ group, items, isOpen, toggle }">
        <th :colspan="headers.length">
          <v-row dense>
            <v-icon
              @click="toggle"
            >
              {{ isOpen ? 'mdi-chevron-up' : 'mdi-chevron-down' }}
            </v-icon>
            <span class="text-h6 ml-3">{{ group }}{{ ` - ${items.length} направления` }}</span>
            <v-spacer />
            <v-btn
              v-if="hasManagePermission"
              color="primary"
              small
              @click="importFromSchoolBook(personId,items[0].basicClassId, items[0].schoolYear)"
            >
              <v-icon left>
                mdi-import
              </v-icon>
              {{ $t('preSchool.importFromSchoolBooks') }}
            </v-btn>
          </v-row>
        </th>
      </template>

      <template v-slot:[`item.enteredStartOfYearEvaluation`]="{ item }">
        <v-chip
          :color="item.enteredStartOfYearEvaluation === true ? 'success' : 'error'"
          outlined
          small
        >
          {{ item.enteredStartOfYearEvaluation ? 'Въведена' : 'Не е въведена' }}
        </v-chip>
      </template>

      <template v-slot:[`item.enteredEndOfYearEvaluation`]="{ item }">
        <v-chip
          :color="item.enteredEndOfYearEvaluation === true ? 'success' : 'error'"
          outlined
          small
        >
          {{ item.enteredEndOfYearEvaluation ? 'Въведена' : 'Не е въведена' }}
        </v-chip>
      </template>

      <template v-slot:[`item.controls`]="{ item }">
        <button-group>
          <button-tip
            v-if="hasManagePermission"
            icon
            icon-name="mdi-eye"
            icon-color="primary"
            tooltip="buttons.review"
            bottom
            iclass=""
            small
            @click="onReviewClick(item)"
          />
          <button-tip
            v-if="hasManagePermission"
            icon
            icon-name="mdi-pencil"
            icon-color="primary"
            tooltip="buttons.edit"
            bottom
            iclass=""
            small
            @click="onEditClick(item)"
          />
          <!-- <button-tip
            v-if="hasManagePermission"
            icon
            icon-name="mdi-eye"
            icon-color="primary"
            tooltip="buttons.review"
            bottom
            iclass=""
            small
            :to="`/student/${personId}/preSchoolEvaluation/${item.id}/details`"
          />
          <button-tip
            v-if="hasManagePermission"
            icon
            icon-name="mdi-pencil"
            icon-color="primary"
            tooltip="buttons.edit"
            bottom
            iclass=""
            small
            :to="`/student/${personId}/preSchoolEvaluation/${item.id}/edit`"
          /> -->
          <button-tip
            v-if="hasManagePermission"
            icon
            icon-name="mdi-delete"
            icon-color="error"
            tooltip="buttons.delete"
            bottom
            iclass=""
            small
            @click="onDeleteClick(item.id)"
          />
        </button-group>
      </template>

      <template #footerPrepend>
        <v-btn
          v-if="hasManagePermission && basicClasses.includes(-4)"
          color="primary"
          small
          @click="onAddForBasicClass(-4)"
        >
          <v-icon
            left
            color="white"
          >
            mdi-plus
          </v-icon>
          {{ $t('preSchoolEvaluation.firstGroupAddButton') }}
        </v-btn>
        <v-btn
          v-if="hasManagePermission && basicClasses.includes(-3)"
          color="primary"
          small
          @click="onAddForBasicClass(-3)"
        >
          <v-icon
            left
            color="white"
          >
            mdi-plus
          </v-icon>
          {{ $t('preSchoolEvaluation.secondGroupAddButton') }}
        </v-btn>
        <v-btn
          v-if="hasManagePermission && basicClasses.includes(-6)"
          color="primary"
          small
          @click="onAddForBasicClass(-6)"
        >
          <v-icon
            left
            color="white"
          >
            mdi-plus
          </v-icon>
          {{ $t('preSchoolEvaluation.thirdGroupAddButton') }}
        </v-btn>
        <v-btn
          v-if="hasManagePermission && basicClasses.includes(-1)"
          color="primary"
          small
          @click="onAddForBasicClass(-1)"
        >
          <v-icon
            left
            color="white"
          >
            mdi-plus
          </v-icon>
          {{ $t('preSchoolEvaluation.fourthGroupAddButton') }}
        </v-btn>
        <v-btn
          v-if="hasManagePermission"
          color="success"
          small
          @click="onAddReadinessForFirstGrade"
        >
          <v-icon
            left
            color="white"
          >
            mdi-plus
          </v-icon>
          {{ $t('preSchoolEvaluation.readinessFirstGradeAddButton') }}
        </v-btn>
      </template>
    </grid>
    <pre-school-readiness
      :ref="'PreSchoolReadinessComponent' + _uid"
      :person-id="personId"
      class="mt-2"
    />
    <confirm-dlg ref="confirm" />
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
    <v-dialog
      v-model="reviewDialog"
    >
      <v-card>
        <v-card-title>
          {{ $t('preSchool.reviewTitle') }}
        </v-card-title>
        <v-card-text>
          <pre-school-evaluation-form
            :value="reviewedItem"
            disabled
          />
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn
            raised
            color="primary"
            @click.stop="reviewDialog = false"
          >
            <v-icon left>
              mdi-close
            </v-icon>
            {{ $t('buttons.close') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
    <v-dialog
      v-model="editDialog"
      persistent
    >
      <v-card>
        <v-card-title>
          {{ $t('preSchool.editTitle') }}
        </v-card-title>
        <v-card-text>
          <pre-school-evaluation-form
            :value="editedItem"
            :disabled="saving"
          />
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn
            raised
            color="primary"
            @click.stop="onSave"
          >
            <v-icon left>
              fas fa-save
            </v-icon>
            {{ $t('buttons.save') }}
          </v-btn>

          <v-btn
            raised
            color="error"
            @click.stop="editDialog = false"
          >
            <v-icon left>
              fas fa-times
            </v-icon>
            {{ $t('buttons.cancel') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script>
import Grid from '@/components/wrappers/grid.vue';
import PreSchoolEvaluationForm from '@/components/preSchoolEval/PreSchoolEvaluationForm';
import PreSchoolReadiness from "@/components/preSchoolEval/PreSchoolReadiness.vue";
import { Permissions } from'@/enums/enums';
import { mapGetters } from 'vuex';
import clonedeep from 'lodash.clonedeep';

export default {
  name: 'PreSchoolEvaluationsListComponent',
  components: {
    Grid,
    PreSchoolEvaluationForm,
    PreSchoolReadiness
  },
  props: {
    personId: {
      type: Number,
      default() {
        return undefined;
      }
    }
  },
  data() {
    return {
      saving: false,
      reviewDialog: false,
      editDialog: false,
      reviewedItem: null,
      editedItem: null,
      personBasicClasses: null,
      headers: [
        {
          text: this.$t('preSchool.headers.basicClass'),
          value: 'basicClass'
        },
        {
          text: this.$t('preSchool.headers.subject'),
          value: 'subject',
          groupable: false,
        },
        {
          text: this.$t('preSchool.headers.schoolYearName'),
          value: 'schoolYearName',
          groupable: false,
        },
        {
          text: this.$t('preSchool.headers.startOfYearEvaluation'),
          value: 'enteredStartOfYearEvaluation',
          groupable: false,
        },
        {
          text: this.$t('preSchool.headers.endOfYearEvaluation'),
          value: 'enteredEndOfYearEvaluation',
          groupable: false,
        },
        {
          text: '',
          value: 'controls',
          filterable: false,
          sortable: false,
          groupable: false,
          align: 'end',
        },
      ]
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission']),
    hasManagePermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentPreSchoolEvaluationManage);
    },
    basicClasses() {
      return this.personBasicClasses && Array.isArray(this.personBasicClasses)
        ? this.personBasicClasses.map((item) => { return item.basicClassId; })
        : [];
    }
  },
  watch: {
    reviewDialog (val) {
      val || this.onCloseReviewDialog();
    },
    editDialog (val) {
      val || this.onClosedEditDialog();
    }
  },
  beforeMount() {
    this.getBasicClasses(this.personId, true);
  },
  methods: {
    refresh() {
      const grid = this.$refs['preSchoolEvaluationsGrid' + this._uid];
      if (grid) {
        grid.get();
      }
    },
    getBasicClasses(personId, forCurrentInstitution) {
      this.$api.studentClass.getPersonBasicClasses(personId, forCurrentInstitution)
        .then((response) => {
          if(response.data) {
            this.personBasicClasses = response.data;
          }
        });
    },
    async onDeleteClick(id) {
      if (await this.$refs.confirm.open(this.$t('buttons.delete'),this.$t('common.confirm'))) {
        this.saving = true;

        this.$api.preSchool.delete(id)
          .then(() => {
            this.$notifier.success('', this.$t('common.deleteSuccess'));
            this.refresh();
          })
          .catch(() => {
            this.$notifier.error('', this.$t('common.deleteError'));
          })
          .then(() => {
            this.saving = false;
          });
      }
    },
    onReviewClick(item) {
      this.reviewedItem = clonedeep(item);
      this.reviewDialog = true;
    },
    onEditClick(item) {
      this.editedItem = clonedeep(item);
      this.editDialog = true;
    },
    onCloseReviewDialog() {
      this.$nextTick(() => {
        this.reviewedItem = null;
      });
    },
    onClosedEditDialog() {
      this.$nextTick(() => {
        this.editedItem = null;
      });
    },
     onSave() {
      this.saving = true;
      this.$api.preSchool.update(this.editedItem)
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          this.editDialog = false;
          this.refresh();
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('common.saveError'), 5000);
          console.log(error);
        })
        .then(() => {
          this.saving = false;
        });
    },
    async onAddForBasicClass(basicClassId) {
      if ( await this.$refs.confirm.open('Добавяне на предмети', this.$t('common.confirm'))) {
        await this.$api.preSchool.createForBaiscClass({
          personId: this.personId,
          basicClassId,
        });

        this.refresh();
      }
    },
    async onAddReadinessForFirstGrade() {
      if (await this.$refs.confirm.open(this.$t('preSchoolEvaluation.readinessFirstGradeAddButton'), this.$t('common.confirm'))) {
        await this.$api.preSchool.createReadinessForFirstGrade({
          personId: this.personId,
        });

        this.$refs['PreSchoolReadinessComponent' + this._uid].load();
      }
    },
    async importFromSchoolBook(personId, basicClassId, schoolYear) {
      if (await this.$refs.confirm.open("Взимане от дневник",this.$t('common.confirm'))) {
        this.saving = true;

        this.$api.preSchool.importFromSchoolBook(personId, basicClassId, schoolYear)
          .then(() => {
            this.$notifier.success('', this.$t('common.saveSuccess'));
            this.refresh();
          })
          .catch(() => {
            this.$notifier.error('', this.$t('common.saveError'));
          })
          .then(() => {
            this.saving = false;
          });
      }
    },
  }
};
</script>
