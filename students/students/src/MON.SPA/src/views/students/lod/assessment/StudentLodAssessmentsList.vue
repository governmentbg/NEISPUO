<!-- ЛОД/Оценки представлява агрегиран изглед на срочните и годишните оценки от днечника, приравняване, признаване и самостоятелна форма на обучение. -->
<!-- За самостоятелната форма на обучение следва да има интерфейс за управление в ЛОД-а на иченика, тъй като си нямат дневник. -->
<template>
  <div>
    <grid
      :ref="'studentLodAssessmentsGrid' + _uid"
      url="/api/lodAssessment/list"
      file-export-name="Списък с импортирани файлове"
      :headers="_headers"
      :title="$t('lod.assessments.listTitle')"
      :file-exporter-extensions="['xlsx', 'csv', 'txt']"
      :filter="{ personId: personId }"
      multi-sort
    >
      <template #footerPrepend>
        <v-btn
          v-if="hasManagePermission"
          small
          color="primary"
          :to="`/student/${personId}/lod/assessment/create`"
        >
          {{ $t('buttons.newRecord') }}
        </v-btn>
      </template>

      <template v-slot:[`item.uniqueCategories`]="{ item }">
        <div v-if="item.uniqueCategories">
          <v-chip
            v-for="(category, index) in item.uniqueCategories"
            :key="index"
            color="light"
            small
          >
            {{ category }}
          </v-chip>
        </div>
      </template>
      <template v-slot:[`item.isSelfEduForm`]="{ item }">
        <v-chip
          :color="item.isSelfEduForm ? 'secondary' : 'light'"
          small
        >
          {{ item.isSelfEduForm | yesNo }}
        </v-chip>
      </template>
      <template v-slot:[`item.controls`]="{ item }">
        <button-group>
          <button-tip
            icon
            icon-name="mdi-eye"
            iclass=""
            tooltip="buttons.details"
            bottom
            small
            icon-color="primary"
            @click="onAssessmentClick(item)"
          />
          <button-tip
            v-if="hasManagePermission && item.lodAssessmentsCount > 0"
            icon
            icon-name="mdi-pencil"
            icon-color="primary"
            iclass=""
            tooltip="buttons.edit"
            bottom
            small
            :disabled="deleting || isLodFinalized(item.schoolYear)"
            :lod-finalized="isLodFinalized(item.schoolYear)"
            :to="`/student/${personId}/lod/assessment/edit?schoolYear=${item.schoolYear}&basicClassId=${item.basicClassId}&isSelfEduForm=${item.isSelfEduForm}`"
          />
          <button-tip
            v-if="hasManagePermission && item.lodAssessmentsCount > 0"
            icon
            icon-name="mdi-delete"
            icon-color="error"
            tooltip="buttons.delete"
            bottom
            iclass=""
            small
            :disabled="deleting || isLodFinalized(item.schoolYear)"
            :lod-finalized="isLodFinalized(item.schoolYear)"
            @click="deleteItem(item)"
          />
        </button-group>
      </template>
    </grid>
    <v-overlay :value="deleting">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
    <confirm-dlg ref="confirm" />
  </div>
</template>

<script>
import Grid from "@/components/wrappers/grid.vue";
import { mapGetters } from 'vuex';
import { Permissions, UserRole } from '@/enums/enums';
import store from '@/store/index';

export default {
  name: 'StudentLodAssessmentsList',
  components: { Grid },
  props: {
    personId: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      deleting: false,
      headers: [
        {
          text: this.$t('lod.assessments.headers.personName'),
          value: "fullName",
          show: !this.personId || store.getters.isInRole(UserRole.Consortium) || store.getters.mode !== 'prod'
        },
        {
          text: this.$t('lod.assessments.headers.schoolYear'),
          value: "schooYearName",
        },
        {
          text: this.$t('lod.assessments.headers.basicClass'),
          value: "basicClassName",
        },
        {
          text: this.$t('lod.assessments.headers.category'),
          value: "uniqueCategories",
          sortable: false
        },
        {
          text: this.$t('lod.assessments.headers.isSelfEduForm'),
          value: "isSelfEduForm",
          show: store.getters.isInRole(UserRole.Consortium) || store.getters.mode !== 'prod'
        },
        { text: '', value: 'controls', sortable: false, filterable: false, align: 'end' },
      ]
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission', 'isInRole', 'isLodFinalized', 'userInstitutionId']),
    hasManagePermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentEvaluationManage);
    },
    _headers () {
      return this.headers.filter(x => x.show !== false);
    }
  },
  mounted() {
    if (!this.hasStudentPermission(Permissions.PermissionNameForStudentEvaluationRead)) {
      return this.$router.push('/errors/AccessDenied');
    }
  },
  methods: {
    gridReload() {
      const grid = this.$refs['studentLodAssessmentsGrid' + this._uid];
      if(grid) {
        grid.get();
      }
    },
    onAssessmentClick(item) {
      this.$router.push({
        name: 'StudentLodAssessment',
        query: {
          basicClassId: item.basicClassId,
          schoolYear: item.schoolYear,
          isSelfEduForm: item.isSelfEduForm
        },
        params: {
          id: this.personId,
          assessmentDetails: item
        }
      });
    },
    async deleteItem(item) {
        if(await this.$refs.confirm.open(this.$t('common.delete'), this.$t('common.confirm'))){
          this.deleting = true;

          this.$api.lodAssessment.delete({
            personId: item.personId,
            basicClass: item.basicClassId,
            schoolYear: item.schoolYear,
            isSelfEduForm: item.isSelfEduForm,
            institutionId: item.institutionId
          })
            .then(() => {
              this.gridReload();
              this.$notifier.success('', this.$t('common.deleteSuccess'));
            })
            .catch((error) => {
              this.$notifier.error('', this.$t('common.deleteError'));
              console.log(error.response);
            })
            .finally(() => { this.deleting = false; });
        }
      },
  }
};
</script>
