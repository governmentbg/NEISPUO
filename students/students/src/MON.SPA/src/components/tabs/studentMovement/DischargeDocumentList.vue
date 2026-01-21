<template>
  <v-card>
    <v-card-title>
      {{ getTranslation('dischargeDocumentListTitle') }}
    </v-card-title>
    <v-card-text class="pa-0">
      <v-alert
        v-if="isStudentInCurrentInstitution === false"
        outlined
        type="info"
        prominent
        dense
      >
        <h4>{{ $t('dischargeDocument.isNotStudentInInstitutionError') }}</h4>
      </v-alert>
      <v-data-table
        :headers="headers"
        :items="dischargeDocuments"
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
                :items="dischargeDocuments"
                :file-extensions="['xlsx', 'csv', 'txt']"
                :file-name="$t('documents.dischargeDocuments')"
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
        </template>
        <template v-slot:[`item.id`]="{ item }">
          <doc-downloader
            v-for="document in item.documents"
            :key="document.id"
            :value="document"
            small
          />
        </template>
        <template v-slot:[`item.status.text`]="{ item }">
          <v-chip
            :class="item.status && item.status.value === 2 ? 'success': 'light'"
            small
          >
            {{ item.status.text }}
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
              :to="`/student/${personId}/dischargeDocument/${item.id}/details`"
            />
            <button-tip
              v-if="(hasUpdatePermission || hasReadPermission) && userInstitutionId && item.institutionId === userInstitutionId"
              icon
              icon-name="mdi-pencil"
              icon-color="primary"
              tooltip="buttons.edit"
              bottom
              iclass=""
              small
              :disabled="saving"
              :to="`/student/${personId}/dischargeDocument/${item.id}/edit`"
            />
            <!-- <button-tip
              v-if="hasUpdatePermission && item.status.value != 2"
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
              v-if="hasDeletePermission && item.status.value != 2"
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
          </button-group>
        </template>

        <template v-slot:[`footer.prepend`]>
          <button-group>
            <v-btn
              v-if="hasCreatePermission"
              color="primary"
              small
              :to="`/student/${personId}/dischargeDocument/create`"
            >
              {{ $t('dischargeDocument.dischargeFromInstitution') }}
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

    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
    <confirm-dlg ref="confirm" />
  </v-card>
</template>

<script>
import DocDownloader from '@/components/common/DocDownloader.vue';
import GridExporter from "@/components/wrappers/gridExporter";
import { StudentDischargeDocumentModel } from '@/models/studentMovement/studentDischargeDocumentModel.js';
import { mapGetters } from 'vuex';
import { Permissions } from '@/enums/enums';

export default {
  components: {
    GridExporter,
    DocDownloader
  },
  props: {
    education: {
      type: Object,
      default() {
        return null;
      }
    },
    personId: {
      type: Number,
      default() {
        return null;
      },
    }
  },
  data() {
    return {
      search: '',
      loading: false,
      saving: false,
      isStudentInCurrentInstitution: undefined,
      dischargeDocuments: [],
      headers:[
        {
          text: this.getTranslation('noteNumberLabel'),
          value: 'noteNumber'
        },
        {
          text: this.getTranslation('datePickerLabel'),
          value: 'noteDate'
        },
        {
          text: this.getTranslation('dischargeDateLabel'),
          value: 'dischargeDate'
        },
        {
          text: this.getTranslation('statusLabel'),
          value: 'status.text',
        },
        {
          text: this.$t('dischargeDocument.schoolYear'),
          value: 'schoolYearName'
        },
        {
          text: this.$t('dischargeDocument.institution'),
          value: 'institutionName'
        },
        {
          text: this.$t('dischargeDocument.institutionCode'),
          value: 'institutionId'
        },
        {
          text: this.getTranslation('className'),
          value: 'currentStudentClassName',
        },
        {
          text: this.getTranslation('filesListLabel'),
          value: 'id',
          sortable: false,
          filterable: false,
        },
        {
          text: this.getTranslation('actionsHeader'),
          value: 'actions',
          sortable: false,
          align: 'end'
        },
      ],
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission', 'gridItemsPerPageOptions', 'currentStudentSummary', 'isInRole', 'userSelectedRole', 'userInstitutionId']),
    hasReadPermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentDischargeDocumentRead);
    },
    hasCreatePermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentDischargeDocumentCreate);
    },
    hasUpdatePermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentDischargeDocumentUpdate);
    },
    hasDeletePermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentDischargeDocumentDelete);
    },
  },
  mounted() {
    this.load();
    this.isStudentInCurrentInstitutionCheck(this.personId);
  },
  methods: {
    getTranslation(text, params) {
      return this.$t(`documents.${text}`, params);
    },
    load() {
      this.loading = true;
      this.$api.dischargeDocument
        .getListForPerson(this.personId)
        .then((response) => {
          if(response.data) {
            this.dischargeDocuments = response.data.map((el) => new StudentDischargeDocumentModel(el, this.$moment));
          }
        })
        .catch((error) => {
          this.$notifier.error('', this.getTranslation('dischargeDocumentsLoadErrorMessage'));
          console.log(error.response);
        })
        .then(() => { this.loading = false; });
    },
    async deleteItem(item) {
      if(await this.$refs.confirm.open(this.$t('buttons.delete'), this.$t('common.confirm'))){
        this.saving = true;
        this.$api.dischargeDocument.delete(item.id)
          .then(() => {
            this.$studentEventBus.$emit('studentMovementUpdate', this.personId);
            this.load();
            this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          })
          .catch(error => {
            this.$notifier.error('',  this.getTranslation('deleteDischargeDocumentErrorMessage'));
            console.log(error.response);
          })
          .then(() => { this.saving = false; });
      }
    },
    async documentConfirm(item) {
      if(await this.$refs.confirm.open(this.$t('buttons.confirm'), this.$t('common.confirm'))){
        this.saving = true;
        this.$api.dischargeDocument.confirm(item.id)
          .then(() => {
            this.load();
            this.$studentEventBus.$emit('studentMovementUpdate', this.personId);
            this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          })
          .catch(error => {
            console.log(error.response);
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
