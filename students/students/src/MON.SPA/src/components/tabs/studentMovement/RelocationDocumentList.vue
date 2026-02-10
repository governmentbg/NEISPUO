<template>
  <v-card>
    <!-- {{relocationDocuments}} -->
    <v-card-title>
      {{ getTranslation("relocationDocumentListTitle") }}
    </v-card-title>
    <v-card-text class="pa-0">
      <v-alert
        v-if="isStudentInCurrentInstitution === false"
        outlined
        type="info"
        prominent
        dense
      >
        <h4>{{ $t('relocationDocument.isNotStudentInInstitutionError') }}</h4>
      </v-alert>
      <v-data-table
        ref="relocationDocumentListTable"
        :headers="headers"
        :items="relocationDocuments"
        :search="search"
        :loading="loading"
        :footer-props="{itemsPerPageOptions: gridItemsPerPageOptions}"
        class="elevation-1"
      >
        <template v-slot:top>
          <v-toolbar
            flat
          >
            <v-toolbar-title>
              <GridExporter
                :items="relocationDocuments"
                :file-extensions="['xlsx', 'csv', 'txt']"
                :file-name="$t('documents.relocationDocuments')"
                :headers="headers"
              />
            </v-toolbar-title>
            <v-spacer />
            <v-text-field
              v-model="search"
              append-icon="mdi-magnify"
              :label="$t('common.search')"
              single-line
              hide-details
            />
          </v-toolbar>
          <v-dialog
            v-model="printDialog"
            fullscreen
            hide-overlay
            persistent
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
        </template>

        <template v-slot:[`item.id`]="{ item }">
          <doc-downloader
            v-for="doc in item.documents"
            :key="doc.id"
            :value="doc"
            small
          />
        </template>

        <template v-slot:[`item.ruoOrderNumber`]="{ item }">
          {{ item.ruoOrderNumber ? `№ ${item.ruoOrderNumber}` : '' }}
        </template>

        <template v-slot:[`item.admissionDocumentsInstitutionsUsed`]="{ item }">
          <span
            v-if="item.admissionDocumentModels.length > 0"
          >
            {{ getAdmissionDocumentsInstitutionsNames(item.admissionDocumentModels) }}
          </span>
        </template>

        <template v-slot:[`item.noteDate`]="{ item }">
          {{ item.noteDate ? $moment(item.noteDate).format(dateFormat) : '' }}
        </template>

        <template v-slot:[`item.ruoOrderDate`]="{ item }">
          {{ item.ruoOrderDate ? $moment(item.ruoOrderDate).format(dateFormat) : '' }}
        </template>

        <template v-slot:[`item.statusName`]="{ item }">
          <v-chip
            :class="item.status === 2 ? 'success': 'light'"
            small
          >
            {{ item.statusName }}
          </v-chip>
        </template>

        <template v-slot:[`item.actions`]="{ item }">
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
              :to="`/student/${personId}/relocationDocument/${item.id}/details`"
            />

            <button-tip
              v-if="hasUpdatePermission && item.status != 2 && item.canBeModified"
              icon
              icon-name="mdi-pencil"
              icon-color="primary"
              tooltip="buttons.edit"
              bottom
              iclass=""
              small
              :disabled="saving"
              :to="`/student/${personId}/relocationDocument/${item.id}/edit`"
            />
            <!-- <button-tip
              v-if="hasUpdatePermission && item.status != 2 && item.canBeModified"
              icon
              icon-name="mdi-check"
              icon-color="success"
              tooltip="buttons.confirm"
              bottom
              iclass=""
              small
              :disabled="saving"
              @click="documentConfirm(item)"
            /> -->
            <button-tip
              v-if="hasDeletePermission && item.status !== 2 && item.canBeModified"
              icon
              icon-name="mdi-delete"
              icon-color="error"
              tooltip="buttons.delete"
              bottom
              iclass=""
              small
              :disabled="saving"
              @click="deleteItem(item)"
            />

            <button-tip
              v-if="hasReadPermission && item.reportPath"
              icon
              icon-name="mdi-printer"
              icon-color="primary"
              tooltip="buttons.print"
              bottom
              iclass=""
              small
              :disabled="saving"
              @click="print(item)"
            />

            <button-tip
              v-if="hasEvaluationsPermission && item.canAddEvaluations"
              icon
              icon-name="mdi-clipboard-list-outline"
              icon-color="primary"
              tooltip="student.menu.evaluations"
              bottom
              iclass=""
              small
              @click="showAssessmentsDialog(item)"
            />
          </button-group>
        </template>

        <template v-slot:[`footer.prepend`]>
          <button-group>
            <v-btn
              v-if="hasCreatePermission"
              color="primary"
              small
              :to="`/student/${personId}/relocationDocument/create`"
            >
              {{ $t('buttons.newRecord') }}
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
      <confirm-dlg ref="confirm" />
    </v-card-text>
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
    <v-dialog
      v-model="assessmentsDialog"
      persistent
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
            @click.stop="closeAssessmentsDialog"
          >
            <v-icon>mdi-close</v-icon>
          </v-btn>
          <v-toolbar-title>{{ $t('documents.relocationDocumentListTitle') }} / {{ $t('student.menu.evaluations') }}</v-toolbar-title>
          <v-spacer />
        </v-toolbar>
        <v-card-text
          v-if="docId"
        >
          <relocation-document-assessments
            :doc-id="docId"
            :person-id="personId"
          />
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <button-tip
            color="primary"
            icon-name="mdi-close"
            text="buttons.close"
            tooltip="buttons.close"
            left
            @click="closeAssessmentsDialog"
          />
        </v-card-actions>
      </v-card>
    </v-dialog>
  </v-card>
</template>

<script>
import PrintComponent from "@/components/print/PrintComponent.vue";
import GridExporter from "@/components/wrappers/gridExporter";
import DocDownloader from '@/components/common/DocDownloader.vue';
import Constants from "@/common/constants.js";
import { Permissions } from '@/enums/enums';
import { mapGetters } from 'vuex';
import RelocationDocumentAssessments from '@/components/lod/relocationDocument/RelocationDocumentAssessments.vue';
import { UserRole } from '@/enums/enums';

export default {
  name: "RelocationDocumentList",
  components: {
    GridExporter,
    DocDownloader,
    PrintComponent,
    RelocationDocumentAssessments
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
      search: '',
      loading: false,
      saving: false,
      relocationDocuments: [],
      dateFormat: Constants.DATEPICKER_FORMAT,
      printDialog: false,
      printId: null,
      reportName: '',
      docId: undefined,
      assessmentsDialog: false,
      isStudentInCurrentInstitution: undefined,
      headers: [
        {
          text: this.getTranslation('noteNumberLabel'),
          value: 'noteNumber',
        },
        {
          text: this.getTranslation('datePickerLabel'),
          value: 'noteDate',
        },
        {
          text: this.getTranslation('statusLabel'),
          value: 'statusName',
        },
        {
          text: this.$t('relocationDocument.schoolYear'),
          value: 'schoolYearName',
        },
        {
          text: this.$t('relocationDocument.institutionCode'),
          value: 'institutionId',
        },
        {
          text: this.getTranslation('recipientInstitution'),
          value: 'institutionName',
        },
        {
          text: this.$t('relocationDocument.currentStudentClass'),
          value: 'currentStudentClassName',
        },
        {
          text: this.$t('relocationDocument.institutionCode'),
          value: 'sendingInstitutionId',
        },
        {
          text: this.$t('relocationDocument.gridSendingInstitution'),
          value: 'sendingInstitution',
        },
        {
          text: this.getTranslation('relocationReasonTypeLabel'),
          value: 'relocationReasonType',
        },
        {
          text: this.getTranslation('ruoOrderNumber'),
          value: 'ruoOrderNumber',
        },
        {
          text: this.getTranslation('ruoOrderDate'),
          value: 'ruoOrderDate',
        },
        {
          text: this.getTranslation('filesListLabel'),
          value: 'id',
        },
        {
          text: this.getTranslation('admissionDocumentsInstitutionsUsed'),
          value: 'admissionDocumentsInstitutionsUsed',
          sortable: false,
          filterable: false,
        },
        {
          text: this.getTranslation('actionsHeader'),
          value: 'actions',
          sortable: false,
          align: 'end'
        }
      ]
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission', 'isInRole', 'gridItemsPerPageOptions']),
    hasReadPermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentRelocationDocumentRead);
    },
    hasCreatePermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentRelocationDocumentCreate);
    },
    hasUpdatePermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentRelocationDocumentUpdate);
    },
    hasDeletePermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentRelocationDocumentDelete);
    },
    isSchoolDirector() {
      return this.isInRole(UserRole.School);
    },
    hasEvaluationsPermission() {
      return this.hasCreatePermission || this.hasUpdatePermission;
    }
  },
  mounted() {
    this.load();
    this.isStudentInCurrentInstitutionCheck(this.personId);
  },
  methods: {
    load() {
      this.loading = true;

      this.$api.relocationDocument
        .getByPersonId(this.personId)
        .then((response) => {
          if(response.data) {
            this.relocationDocuments = response.data;
          }
        })
        .catch((error) => {
          this.$notifier.error('', this.getTranslation('relocationDocumentsLoadErrorMessage'));
          console.log(error.response);
        })
        .then(() => { this.loading = false; });
    },
    getAdmissionDocumentsInstitutionsNames(admissionDocumentModels) {
      const names = admissionDocumentModels.map(el => {
        return el.institutionName;
      });

      return names.join(' | ');
    },
    getTranslation(text, params) {
      return this.$t(`documents.${text}`, params);
    },
    closePrintDialog() {
      this.printDialog = false;
    },
    print(item){
      this.printId = item.id.toString();
      this.reportName = item.reportPath;
      if(!item.reportPath) {
        return this.$notifier.error('', 'Unknown report name');
      }

      this.printDialog = true;
      // var vm = this;
      // console.log(item);
      // switch (item.currentStudentClassBasicClassId){
      //   // Яслени и градински групи
      //   case -1:
      //   case -3:
      //   case -4:
      //   case -5:
      //   case -6: vm.reportName = 'Student\\RelocationDocument';
      //     break;
      //   case 3:
      //   case 5:
      //   case 1:
      //   case 4:
      //   case 8:
      //   case 9:
      //   case 10:
      //   case 11:
      //   case 12: vm.reportName = 'Student\\ThirdAndFifthClass\\ThirdAndFifthClassRelocationDocument';
      //   break;
      //   case 6: vm.reportName = 'Student\\SixthClass\\SixthClassRelocationDocument';
      //     break;
      //   case 7: vm.reportName = 'Student\\SeventhClass\\SeventhClassRelocationDocument';
      //     break;
      //   //default: vm.reportName = 'Student\\ThirdAndFifthClass\\ThirdAndFifthClassRelocationDocument';
      // }
    },
    async deleteItem(item) {
      if(await this.$refs.confirm.open(this.$t('common.delete'), this.$t('common.confirm'))){
      this.saving = true;

      this.$api.relocationDocument
        .delete(item.id)
        .then(() => {
          this.$studentEventBus.$emit('studentMovementUpdate', this.personId);
          this.load();
        })
        .catch(error => {
          this.$notifier.error('', this.getTranslation('deleteRelocationDocumentErrorMessage'));
          console.log(error.response.data.message);
        })
        .finally(() => {
          this.saving = false;
        });
      }
    },
    showAssessmentsDialog(item) {
      this.docId = item.id;
      this.assessmentsDialog = true;
    },
    closeAssessmentsDialog() {
      this.docId = 0;
      this.assessmentsDialog = false;
    },
    async documentConfirm(item) {
      if(await this.$refs.confirm.open(this.$t('buttons.confirm'), this.$t('common.confirm'))){
        this.saving = true;
        this.$api.relocationDocument.confirm(item.id)
          .then(() => {
            this.$studentEventBus.$emit('studentMovementUpdate', this.personId);
            this.load();
            this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          })
          .catch(error => {
            console.log(error.response);
            this.$notifier.error(this.$t('common.saveError'), error.response?.data?.message ?? this.$t('documents.editRelocationDocumentErrorMessage', 5000));
          })
          .then(() => { this.saving = false; });
      }
    },
    isStudentInCurrentInstitutionCheck(personId) {
      this.$api.student.isStudentInCurrentInstitution(personId)
      .then((response) => {
        if(response) {
          this.isStudentInCurrentInstitution = response.data;
        }
      })
      .catch((error) => {
        console.log(error.response);
      });
    }
  }
};
</script>
