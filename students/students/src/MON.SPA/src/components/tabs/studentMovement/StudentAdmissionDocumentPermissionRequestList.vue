<template>
  <div>
    <grid
      ref="studentAdmissionDocumentPermissionRequestGrid"
      url="/api/admissionPermissionRequest/list"
      file-export-name="AdmissionPermissionRequestList"
      :headers="headers"
      :title="hideTitle ? '' : $t('studentAdmissionDocumentPermissionRequest.listTitle')"
      :file-exporter-extensions="['xlsx', 'csv', 'txt']"
      :filter="{ listFilter: listFilter }"
    >
      <template #subtitle>
        <v-row dense>
          <v-radio-group
            v-model="listFilter"
            row
          >
            <v-radio
              :label="$t('studentAdmissionDocumentPermissionRequest.filter.requesting')"
              :value="0"
            />
            <v-radio
              :label="$t('studentAdmissionDocumentPermissionRequest.filter.authorizing')"
              :value="1"
            />
            <v-radio
              :label="$t('studentAdmissionDocumentPermissionRequest.filter.finished')"
              :value="2"
            />
            <v-radio
              :label="$t('studentAdmissionDocumentPermissionRequest.filter.all')"
              :value="3"
            />
          </v-radio-group>
        </v-row>
      </template>
      <template v-slot:[`item.isPermissionGranted`]="{ item }">
        <v-chip
          :color="item.isPermissionGranted === true ? 'success' : 'error'"
          small
        >
          <yes-no :value="item.isPermissionGranted" />
        </v-chip>
      </template>

      <template v-slot:[`item.createDate`]="{ item }">
        {{ item.createDate ? $moment(item.createDate).format(dateFormat): "" }}
      </template>

      <template v-slot:[`item.id`]="{ item }">
        <doc-downloader
          v-for="document in item.documents"
          :key="document.id"
          :value="document"
          small
        />
      </template>

      <template #actions="item">
        <button-group>
          <button-tip
            v-if="hasManagePermission && item.item.isPermissionGranted === false && userInstitutionId && userInstitutionId === item.item.requestingInstitutionId"
            icon
            icon-color="primary"
            icon-name="mdi-pencil"
            iclass=""
            tooltip="buttons.edit"
            bottom
            small
            :to="`/studentAdmissionDocumentPermissionRequest/${item.item.id}/edit`"
          />
          <button-tip
            v-if="hasManagePermission && item.item.isPermissionGranted === false && userInstitutionId && userInstitutionId === item.item.requestingInstitutionId"
            icon
            icon-name="mdi-delete"
            icon-color="error"
            tooltip="buttons.delete"
            bottom
            iclass=""
            small
            :disabled="saving"
            @click="deleteItem(item.item)"
          />
          <button-tip
            v-if="hasManagePermission && item.item.isPermissionGranted === false && userInstitutionId && userInstitutionId === item.item.authorizingInstitutionId"
            icon
            icon-color="success"
            icon-name="mdi-check"
            iclass=""
            tooltip="buttons.confirm"
            bottom
            small
            @click="confirm(item.item)"
          />
          <button-tip
            v-if="item.item.personId"
            icon
            icon-color="primary"
            icon-name="fas fa-info-circle"
            iclass=""
            tooltip="student.details"
            top
            small
            :to="`/student/${item.item.personId}/details`"
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
import Constants from "@/common/constants.js";
import DocDownloader from '@/components/common/DocDownloader.vue';
import { Permissions } from '@/enums/enums';
import { mapGetters } from 'vuex';

export default {
  name: "StudentAdmissionDocumentPermissionRequestList",
  components: {
    Grid,
    DocDownloader
  },
  props: {
    hideTitle: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
      saving: false,
      dateFormat: Constants.DATEPICKER_FORMAT,
      listFilter: 3, // По подразбиране, Всички
      headers: [
        {text: this.$t('studentAdmissionDocumentPermissionRequest.personName'), value: "personName", sortable: true},
        {text: this.$t('studentAdmissionDocumentPermissionRequest.requestingInstitution'), value: "requestingInstitutionAbbreviation", sortable: true},
        {text: this.$t('studentAdmissionDocumentPermissionRequest.requestingInstitutionTown'), value: "requestingInstitutionTown", sortable: true},
        {text: this.$t('studentAdmissionDocumentPermissionRequest.requestingInstitutionMunicipality'), value: "requestingInstitutionMunicipality", sortable: true},
        {text: this.$t('studentAdmissionDocumentPermissionRequest.requestingInstitutionRegion'), value: "requestingInstitutionRegion", sortable: true},
        {text: this.$t('studentAdmissionDocumentPermissionRequest.note'), value: "note", sortable: true},
        {text: this.$t('studentAdmissionDocumentPermissionRequest.authorizingInstitution'), value: "authorizingInstitutionAbbreviation", sortable: true},
        {text: this.$t('studentAdmissionDocumentPermissionRequest.isPermissionGranted'), value: "isPermissionGranted", sortable: true},
        {text: this.$t('studentAdmissionDocumentPermissionRequest.createDate'), value: "createDate", sortable: true},
        {
          text: this.$t("otherDocuments.attachedDocs"),
          value: "id",
          sortable: false,
          filterable: false,
        },
        {text: '', value: 'controls', inFavourOfIdentifier: false, sortable: false, filterable: false, align: 'end'},
      ]
    };
  },
  computed: {
    ...mapGetters(['hasPermission', 'userInstitutionId']),
    hasReadPermission() {
      return this.hasPermission(Permissions.PermissionNameForAdmissionPermissionRequestRead);
    },
    hasManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForAdmissionPermissionRequestManage);
    },
  },
  methods: {
    async deleteItem(item) {
      if(await this.$refs.confirm.open(this.$t('common.delete'), this.$t('common.confirm'))){
        this.saving = true;

        this.$api.studentAdmissionDocumentPermissionRequest
          .delete(item.id)
          .then(() => {
            this.$notifier.success('', this.$t('common.deleteSuccess'));
            this.$refs['studentAdmissionDocumentPermissionRequestGrid'].get();
          })
          .catch(error => {
            this.$notifier.error('', this.$t('common.deleteError'));
            console.log(error.response);
          })
          .finally(() => {
             this.saving = false;
          });
      }
    },
    async confirm(item) {
      if(await this.$refs.confirm.open(this.$t('buttons.confirm'), this.$t('common.confirm'))){
        this.saving = true;
        this.$api.studentAdmissionDocumentPermissionRequest.confirm(item.id)
          .then(() => {
            this.$notifier.success('', this.$t('common.saveSuccess'));
            this.$refs['studentAdmissionDocumentPermissionRequestGrid'].get();
          })
          .catch(error => {
            this.$notifier.error('', this.$t('common.saveError'));
            console.log(error.response);
          })
          .then(() => { this.saving = false; });
      }
    },
  }
};
</script>
