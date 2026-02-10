
<template>
  <div>
    <grid
      :ref="'studentsForDischargeGrid' + _uid"
      url="/api/admin/GetStudentsForDischarge"
      :headers="headers"
      :title="''"
      :file-export-name="$t('dashboards.studentsForDischargeList')"
      :file-exporter-extensions="['xlsx', 'csv', 'txt']"
      item-key="uid"
    >
      <template v-slot:[`item.pin`]="{ item }">
        {{ `${item.pin} - ${item.pinType}` }}
      </template>
      <template v-slot:[`item.controls`]="{ item }">
        <button-group>
          <button-tip
            v-if="item.showStudentLodLinkBtn"
            icon
            icon-color="primary"
            iclass=""
            icon-name="mdi-eye"
            small
            tooltip="student.details"
            left
            :to="`/student/${item.personId}/details`"
          />
          <button-tip
            v-if="item.showRelocationDocumentEditBtn"
            icon
            icon-name="mdi-pencil"
            icon-color="primary"
            tooltip="buttons.edit"
            bottom
            iclass=""
            small
            :to="`/student/${item.personId}/relocationDocument/${item.relocationDocumentId}/edit`"
          />
          <button-tip
            v-if="item.showRelocationDocumentConfirmBtn"
            icon
            icon-name="mdi-check"
            icon-color="success"
            tooltip="buttons.confirm"
            bottom
            iclass=""
            small
            @click="onConfirmRelocationDocument(item)"
          />
          <button-tip
            v-if="item.showRelocationDocumentDeleteBtn"
            icon
            icon-name="mdi-delete"
            icon-color="error"
            tooltip="buttons.delete"
            bottom
            iclass=""
            small
            @click="onDeleteRelocationDocument(item)"
          />

          <button-tip
            v-if="item.showDischargeDocumentEditBtn"
            icon
            icon-name="mdi-pencil"
            icon-color="primary"
            tooltip="buttons.edit"
            bottom
            iclass=""
            small
            :to="`/student/${item.personId}/dischargeDocument/${item.dischargeDocumentId}/edit`"
          />
          <button-tip
            v-if="item.showDischargeDocumentConfirmBtn"
            icon
            icon-name="mdi-check"
            icon-color="success"
            tooltip="buttons.confirm"
            bottom
            iclass=""
            small
            @click="onConfirmDischargeDocument(item)"
          />
          <button-tip
            v-if="item.showDischargenDocumentDeleteBtn"
            icon
            icon-name="mdi-delete"
            icon-color="error"
            tooltip="buttons.delete"
            bottom
            iclass=""
            small
            @click="onDeleteDischargeDocument(item)"
          />
          <button-tip
            v-if="item.showRelocationDocumentCreateBtn"
            icon
            icon-color="primary"
            iclass=""
            icon-name="fas fa-exchange-alt"
            tooltip="documents.relocationDocumentCreateTitle"
            small
            bottom
            :to="`/student/${item.personId}/relocationDocument/create?hostingInstitution=${item.newInstitutionId}`"
          />
        </button-group>
      </template>
    </grid>
    <v-overlay :value="saving">
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
import ButtonGroup from '../../wrappers/ButtonGroup.vue';
import { mapGetters } from "vuex";

export default {
  name: 'StudentsForDischarge',
  components: {
    ButtonGroup,
    Grid
  },
  data() {
    return{
      saving: false,
      items: [],
      headers: [
        {
          text: this.$t("dashboards.headers.studentNames"),
          value: "fullName",
        },
        // {
        //   text: this.$t("studentTabs.gender"),
        //   value: "gender",
        // },
        // {
        //   text: this.$t("createStudent.pinType"),
        //   value: "pinType",
        // },
        {
          text: this.$t("student.headers.identifier"),
          value: "pin",
        },
        {
          text: this.$t("documents.classNumber"),
          value: "oldClassName",
        },
        {
          text: this.$t("admissionDocument.institutionCode"),
          value: "oldInstitutionId",
        },
        {
          text: this.$t("admissionDocument.sendingInstitution"),
          value: "oldInstitution",
        },
        {
          text: this.$t("admissionDocument.institutionCode"),
          value: "newInstitutionId",
        },
        {
          text: this.$t("admissionDocument.hostInstitution"),
          value: "newInstitution",
        },
        {
          text: '',
          value: "selectionType",
        },
        { text: '', value: 'controls', sortable: false, align: 'end' }
      ],
    };
  },
  computed:{
    ...mapGetters(['gridItemsPerPageOptions']),
  },
  mounted() {
  },
  methods: {
    refresh() {
      this.$refs['studentsForDischargeGrid' + this._uid].get();
    },
    async onConfirmRelocationDocument(item) {
      if(await this.$refs.confirm.open(this.$t('buttons.confirm'), this.$t('common.confirm'))) {
        this.saving = true;

        this.$api.relocationDocument.confirm(item.relocationDocumentId)
          .then(() => {
            this.$studentEventBus.$emit('studentMovementUpdate', item.personId);
            this.$notifier.success('', this.$t('common.saveSuccess'));
            this.refresh();
          })
          .catch(() => {
            this.$notifier.error('', this.$t('common.saveError'));
          })
          .then(() => { this.saving = false; });
      }
    },
    async onConfirmDischargeDocument(item) {
      if(await this.$refs.confirm.open(this.$t('buttons.confirm'), this.$t('common.confirm'))) {
        this.saving = true;

        this.$api.dischargeDocument.confirm(item.dischargeDocumentId)
          .then(() => {
            this.$studentEventBus.$emit('studentMovementUpdate', item.personId);
            this.$notifier.success('', this.$t('common.saveSuccess'));
            this.refresh();
          })
          .catch(() => {
            this.$notifier.error('', this.$t('common.saveError'));
          })
          .then(() => { this.saving = false; });
      }
    },
    async onDeleteRelocationDocument(item) {
      if(await this.$refs.confirm.open(this.$t('common.delete'), this.$t('common.confirm'))){
        this.saving = true;

        this.$api.relocationDocument.delete(item.relocationDocumentId)
          .then(() => {
            this.$studentEventBus.$emit('studentMovementUpdate', this.personId);
            this.$notifier.success('', this.$t('common.deleteSuccess'));
            this.refresh();
          })
          .catch(() => {
            this.$notifier.error('', this.$t('common.deleteError'));
          })
          .then(() => { this.saving = false; });
      }
    },
    async onDeleteDischargeDocument(item) {
      if(await this.$refs.confirm.open(this.$t('common.delete'), this.$t('common.confirm'))){
        this.saving = true;

        this.$api.dischargeDocument.delete(item.dischargeDocumentId)
          .then(() => {
            this.$studentEventBus.$emit('studentMovementUpdate', this.personId);
            this.$notifier.success('', this.$t('common.deleteSuccess'));
            this.refresh();
          })
          .catch(() => {
            this.$notifier.error('', this.$t('common.deleteError'));
          })
          .then(() => { this.saving = false; });
      }
    }
  }
};
</script>
