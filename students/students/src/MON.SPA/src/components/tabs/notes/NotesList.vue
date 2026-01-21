<template>
  <div>
    <grid
      :ref="'notesListGrid' + _uid"
      url="/api/note/list"
      :title="$t('notes.title')"
      :headers="headers"
      :file-exporter-extensions="['xlsx', 'csv', 'txt']"
      :filter="{ personId: personId }"
    >
      <template v-slot:[`item.issueDate`]="{ item }">
        {{ item.issueDate ? $moment(item.issueDate).format(dateFormat) : "" }}
      </template>

      <template #actions="item">
        <button-group>
          <button-tip
            v-if="hasReadPermission"
            icon
            icon-name="mdi-eye"
            icon-color="primary"
            tooltip="buttons.review"
            bottom
            iclass=""
            small
            :disabled="saving"
            :to="`/student/${personId}/note/${item.item.noteId}/details`"
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
            :disabled="saving || item.item.isLodFinalized"
            :lod-finalized="item.item.isLodFinalized"
            :to="`/student/${personId}/note/${item.item.noteId}/edit`"
          />
          <button-tip
            v-if="hasManagePermission"
            icon
            icon-name="mdi-delete"
            icon-color="error"
            tooltip="buttons.delete"
            bottom
            iclass=""
            small
            :disabled="saving || item.item.isLodFinalized"
            :lod-finalized="item.item.isLodFinalized"
            @click="deleteItem(item.item.noteId)"
          />
          <button-tip
            v-if="hasReadPermission"
            icon
            icon-name="mdi-printer"
            icon-color="primary"
            tooltip="buttons.print"
            bottom
            iclass=""
            small
            :disabled="saving"
            @click="print(item.item)"
          />
        </button-group>
      </template>

      <template #footerPrepend>
        <v-btn
          v-if="hasManagePermission"
          small
          color="primary"
          :to="`/student/${personId}/note/create`"
        >
          {{ $t("buttons.newRecord") }}
        </v-btn>
      </template>
    </grid>
    <v-dialog
      v-model="printDialog"
      fullscreen
      hide-overlay
      transition="dialog-bottom-transition"
    >
      <v-card>
        <v-toolbar
          dark
          color="primary"
        >
          <v-btn
            icon
            dark
            @click="closePrintDialog()"
          >
            <v-icon>mdi-close</v-icon>
          </v-btn>
          <v-toolbar-title>Печат</v-toolbar-title>
          <v-spacer />
        </v-toolbar>
        <v-card-text>
          <print-component
            v-if="printDialog == true"
            :id="printId"
            :report-name="reportName"
          />
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <button-tip
            color="warning"
            icon-name="fas fa-times"
            text="buttons.close"
            raised
            @click="closePrintDialog()"
          />
        </v-card-actions>
      </v-card>
    </v-dialog>
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
import { mapGetters } from 'vuex';
import { Permissions } from '@/enums/enums';
import PrintComponent from '@/components/print/PrintComponent.vue';
import Constants from "@/common/constants.js";

export default {
  name: 'NotesList',
  components: {
    Grid,
    PrintComponent,
  },
  props: {
    personId: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      saving: false,
      printId: null,
      printDialog: false,
      dateFormat: Constants.DATEPICKER_FORMAT,
      headers: [
        {
          text: this.$t('notes.noteTitle'),
          value: 'title',
        },
        {
          text: this.$t('notes.issueDate'),
          value: 'issueDate',
        },
        {
          text: this.$t('notes.schoolYear'),
          value: 'schoolYearName',
        },
        {
          text: this.$t('notes.institution'),
          value: 'institutionName',
        },
        {
          text: '',
          value: 'controls',
          filterable: false,
          sortable: false,
          align: 'end',
        },
      ],
    };
  },
  computed: {
    ...mapGetters([
      'gridItemsPerPageOptions',
      'hasStudentPermission',
    ]),
    hasReadPermission() {
      return this.hasStudentPermission(
        Permissions.PermissionNameForStudentNoteRead
      );
    },
    hasManagePermission() {
      return this.hasStudentPermission(
        Permissions.PermissionNameForStudentNoteManage
      );
    },
  },
  mounted() { },
  methods: {
    refresh() {
      const grid = this.$refs['notesListGrid' + this._uid];
      if (grid) {
        grid.get();
      }
    },
    async deleteItem(noteId) {
      if (await this.$refs.confirm.open(this.$t('common.delete'),this.$t('common.confirm')))
      {
        this.saving = true;

        this.$api.note
          .delete(noteId)
          .then(() => {
            this.$notifier.success('', this.$t('common.deleteSuccess'), 5000);
            this.refresh();
          })
          .catch((error) => {
            this.$notifier.error('', this.$t('errors.deleteError'));
            console.log(error.response);
          })
          .then(() => {
            this.saving = false;
          });
      }
    },
    closePrintDialog() {
      this.printDialog = false;
    },
    print(item) {
      this.printId = item.noteId.toString();
      console.log(this.printId);
      this.printDialog = true;
      var vm = this;
      vm.reportName = 'Student\\Note';
    },
  },
};
</script>

