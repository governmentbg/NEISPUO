<template>
  <div>
    <grid
      v-if="hasReadPermission"
      :ref="'docManagementApplicationsGrid' + _uid"
      url="/api/docManagementApplication/list"
      :headers="headers"
      :title="$tc('docManagement.application.title', 2)"
      :expanded.sync="expanded"
      show-expand
      :single-expand="true"
      file-export-name="Списък със заявления за документи с фабрична номерация"
      :file-exporter-extensions="['xlsx', 'csv', 'txt']"
      :filter="{
        institutionId: gridFilters.institutionId,
        schoolYear: gridFilters.schoolYear,
        campaignId: gridFilters.campaignId,
        regionId: gridFilters.regionId,
        municipalityId: gridFilters.municipalityId,
        instType: gridFilters.instType,
        campaignType: gridFilters.campaignType,
      }"
    >
      <template #subtitle>
        <v-row
          dense
          class="mb-1"
        >
          <v-col
            cols="12"
            md="6"
            lg="3"
          >
            <school-year-selector
              v-model="gridFilters.schoolYear"
              show-current-school-year-button
              :show-navigation-buttons="false"
              hide-details
            />
          </v-col>
          <v-col
            cols="12"
            md="6"
            lg="3"
          >
            <c-select
              v-model="gridFilters.campaignId"
              :items="campaignsOptions"
              :label="$t('docManagement.application.headers.campaign')"
              clearable
              hide-details
            />
          </v-col>
          <v-col
            v-if="!userInstitutionId"
            cols="12"
            md="6"
          >
            <autocomplete
              v-model="gridFilters.institutionId"
              :defer-options-loading="false"
              api="/api/lookups/getInstitutionOptions"
              :label="$t('common.institution')"
              clearable
              :filter="{
                regionId: userRegionId
              }"
              hide-details
            />
          </v-col>
          <v-col
            v-if="!userInstitutionId && !userRegionId"
            cols="12"
            md="6"
            lg="3"
          >
            <autocomplete
              v-model="gridFilters.regionId"
              :defer-options-loading="false"
              api="/api/lookups/getDistricts"
              :label="$t('institution.headers.region')"
              clearable
              hide-details
              @change="gridFilters.municipalityId = null"
            />
          </v-col>
          <v-col
            v-if="!userInstitutionId"
            cols="12"
            md="6"
            lg="3"
          >
            <autocomplete
              v-model="gridFilters.municipalityId"
              :defer-options-loading="false"
              api="/api/lookups/getMunicipalities"
              :label="$t('institution.headers.municipality')"
              clearable
              hide-details
              :filter="{
                regionId: userRegionId || gridFilters.regionId
              }"
            />
          </v-col>
          <v-col
            v-if="!userInstitutionId"
            cols="12"
            md="6"
            lg="3"
          >
            <c-select
              v-model="gridFilters.campaignType"
              :items="options3"
              :label="$t('docManagement.application.filter.campaignType')"
              hide-details
            />
          </v-col>
          <v-col
            v-if="!userInstitutionId"
            cols="12"
            md="6"
            lg="3"
          >
            <c-select
              v-model="gridFilters.instType"
              :items="options1"
              :label="$t('docManagement.application.filter.instType')"
              hide-details
            />
          </v-col>
          <v-col
            v-if="!userInstitutionId"
            cols="12"
            md="6"
            lg="3"
          >
            <c-select
              v-model="gridFilters.groupingType"
              :items="options2"
              :label="$t('docManagement.application.filter.groupingType')"
              hide-details
            />
          </v-col>
        </v-row>
      </template>

      <template v-slot:expanded-item="{ item }">
        <td
          v-if="item.basicDocuments && item.basicDocuments.length > 0"
          :colspan="headers.length + 1"
        >
          <v-card class="ma-4">
            <v-card-text>
              <v-simple-table dense>
                <template #default>
                  <thead>
                    <tr>
                      <th>{{ $t('docManagement.application.basicDocument') }}</th>
                      <th>{{ $t('docManagement.application.number') }}</th>
                    </tr>
                  </thead>
                  <tbody>
                    <tr
                      v-for="(doc, index) in item.basicDocuments"
                      :key="index"
                    >
                      <td>{{ doc.basicDocumentName }}</td>
                      <td>{{ doc.number }}</td>
                    </tr>
                  </tbody>
                </template>
              </v-simple-table>
            </v-card-text>
          </v-card>
        </td>
      </template>

      <template
        v-slot:[`item.status`]="{ item }"
      >
        <v-chip
          :color="['Approved', 'Submitted'].includes(item.status) ? 'success' : ['Rejected', 'ReturnedForCorrection'].includes(item.status) ? 'warning' : 'light'"
          small
          label
        >
          {{ item.statusName }}
        </v-chip>
      </template>

      <template
        v-slot:[`item.campaign.name`]="props"
      >
        <span v-if="props.item.isExchangeRequest">
          <v-chip
            color="secondary"
            small
            label
          >
            {{ $tc('docManagement.exchangeRequest.title', 1) }}
          </v-chip>
        </span>
        <span v-else>
          <span>{{ props.item.campaign.name }}</span>
          <span
            v-if="props.item.campaign.labels && props.item.campaign.labels.length"
            class="ml-2"
          >
            <v-chip
              v-for="(label, index) in props.item.campaign.labels"
              :key="index"
              :color="label.value"
              x-small
              class="mr-1"
              label
            >
              {{ label.key }}
            </v-chip>
          </span>
        </span>
      </template>

      <template
        v-slot:[`item.institutionId`]="{ item }"
      >
        {{ `${item.institutionId} - ${item.institutionName}` }}
      </template>

      <template
        v-slot:[`item.requestedInstitutionId`]="{ item }"
      >
        <span v-if="item.requestedInstitutionId">
          {{ `${item.requestedInstitutionId} - ${item.requestedInstitutionName}` }}
        </span>
        <span v-else />
      </template>

      <template
        v-slot:[`item.attachments`]="{ item }"
      >
        <doc-downloader
          v-for="doc in item.attachments"
          :key="doc.id"
          :value="doc"
          small
        />
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
            :to="`/docManagement/application/${item.item.id}/details`"
          />
          <button-tip
            v-if="hasManagePermission && item.item.isEditable"
            icon
            icon-name="mdi-pencil"
            icon-color="primary"
            tooltip="buttons.edit"
            bottom
            iclass=""
            small
            :to="`/docManagement/application/${item.item.id}/edit`"
          />
          <button-tip
            v-if="hasManagePermission && item.item.canBeSubmited"
            icon
            icon-name="mdi-check-bold"
            icon-color="success"
            tooltip="buttons.submit"
            bottom
            iclass=""
            small
            @click="onSubmit(item.item)"
          />
          <button-tip
            v-if="hasManagePermission && item.item.isDeletable"
            icon
            icon-name="mdi-delete"
            icon-color="error"
            tooltip="buttons.delete"
            bottom
            iclass=""
            small
            @click="deleteApplication(item.item)"
          />
          <button-tip
            v-if="hasReportCreatePermission && item.item.isEditable && !item.item.isExchangeRequest"
            icon
            icon-color="primary"
            icon-name="fas fa-file-word"
            iclass=""
            tooltip="docManagement.application.generateReportFile"
            bottom
            small
            @click="generateApplicationReportFile(item.item)"
          />
          <button-tip
            v-if="hasReportCreatePermission && item.item.isEditable && !item.item.isExchangeRequest"
            icon
            icon-color="primary"
            icon-name="fas fa-file-signature"
            iclass=""
            tooltip="docManagement.application.generateSignAttachReportFile"
            bottom
            small
            @click="generateSignAttachApplicationReportFile(item.item)"
          />
          <button-tip
            v-if="hasReportCreatePermission && item.item.isExchangeRequest"
            icon
            icon-color="primary"
            icon-name="fas fa-file-word"
            iclass=""
            tooltip="docManagement.application.generateProtocolFile"
            bottom
            small
            @click="generateProtocolFile(item.item)"
          />
          <button-tip
            v-if="hasReportCreatePermission && item.item.isExchangeRequest && item.item.hasApprovePermission"
            icon
            icon-color="success"
            icon-name="mdi-check"
            iclass=""
            tooltip="buttons.approve"
            bottom
            small
            @click="onApproveApplication(item.item)"
          />
          <button-tip
            v-if="hasReportCreatePermission && item.item.isExchangeRequest && item.item.hasApprovePermission"
            icon
            icon-color="error"
            icon-name="mdi-file-cancel"
            iclass=""
            tooltip="buttons.reject"
            bottom
            small
            @click="onRejectApplication(item.item)"
          />
          <button-tip
            v-if="hasManagePermission && item.item.isReportable"
            icon
            icon-name="mdi-truck-delivery"
            icon-color="success"
            tooltip="docManagement.application.deliveryReportTitle"
            bottom
            iclass=""
            small
            :to="`/docManagement/application/${item.item.id}/delivery`"
          />
        </button-group>
      </template>

      <template
        v-if="!userInstitutionId"
        #topAppend
      >
        <v-row no-gutters>
          <v-spacer />
          <button-tip
            v-if="hasReportCreatePermission && gridFilters.campaignId"
            icon-color="white"
            icon-name="fas fa-file-word"
            iclass="mr-3"
            tooltip="docManagement.application.generateSummaryReportFile"
            :text="$t('docManagement.application.generateSummaryReportFile')"
            bottom
            small
            @click="generateSummaryReportFile()"
          />
        </v-row>
      </template>

      <template #footerPrepend>
        <button-group>
          <v-btn
            v-if="hasManagePermission"
            small
            color="primary"
            @click.stop="onNewExchangeRequest"
          >
            {{ $t("docManagement.exchangeRequest.new") }}
          </v-btn>
        </button-group>
      </template>
    </grid>

    <confirm-dlg ref="confirm" />
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
    <v-dialog
      v-model="dialog"
      min-width="1000"
      persistent
    >
      <form-layout
        @on-save="onNewExchangeRequestSave"
        @on-cancel="onNewExchangeRequestCancel"
      >
        <template #title>
          <h3>{{ $t('docManagement.exchangeRequest.new') }}</h3>
        </template>
        <template
          v-if="newExchangeRequestModel"
          #default
        >
          <v-row>
            <v-col cols="12">
              <autocomplete
                v-model="newExchangeRequestModel.institutionId"
                :defer-options-loading="true"
                api="/api/lookups/getInstitutionOptions"
                :label="$t('common.institution')"
                clearable
                class="required"
                :rules="[$validator.required()]"
              />
            </v-col>
          </v-row>
          <doc-management-application-form
            :ref="'docManagementExchangeApplicationCreateForm' + _uid"
            :value="newExchangeRequestModel"
            is-exchange-request
          />
        </template>
      </form-layout>
    </v-dialog>
    <v-dialog
        v-model="submitDialog"
        persistent
        max-width="1000"
    >
      <v-card>

        <v-card-title>
          {{ $t('buttons.submit') }}
        </v-card-title>

          <v-card-text v-if="submitApplication">
              <v-row>
                <v-col cols="12">
                  <v-text-field
                    :value="submitApplication.programName"
                    :label="$t('docManagement.application.headers.campaign')"
                    readonly />
                </v-col>
                <v-col cols="12">
                  <v-text-field
                    :value="submitApplication.institutionName"
                    :label="$t('common.institution')"
                    readonly />
                  </v-col>
              </v-row>
          </v-card-text>

            <v-card-actions>
                <v-spacer></v-spacer>

                <v-btn
                    color="light"
                    raised
                    @click.stop="onSubmitCancel"
                >
                    {{ $t('buttons.cancel') }}
                </v-btn>
                <v-btn
                    color="success"
                    raised
                    @click.stop="onSubmitConfirm"
                >
                    {{ $t('buttons.submit') }}
                </v-btn>
            </v-card-actions>
        </v-card>
    </v-dialog>
    <doc-management-request-approve-dialog
      ref="approveDialog"
      @saved="onApproveRejectSaved"
    />
    <doc-management-request-approve-dialog
      ref="rejectDialog"
      @saved="onApproveRejectSaved"
    />
    <v-overlay :value="signing">
      <v-row justify="center">
        <v-progress-circular
          :value="signingProgressPercentage"
          color="primary"
          size="128"
          width="13"
        >
          <h2 class="white--text">
            {{ `${signingProgressCount}/${signingItemsCount}` }}
          </h2>
        </v-progress-circular>
      </v-row>

      <h2 class="white--text mt-2">
        {{ selectedItemName }}
      </h2>
    </v-overlay>
  </div>
</template>

<script>
import Grid from "@/components/wrappers/grid.vue";
import Constants from "@/common/constants.js";
import DocDownloader from '@/components/common/DocDownloader.vue';
import SchoolYearSelector from '@/components/common/SchoolYearSelector';
import Autocomplete from '@/components/wrappers/CustomAutocomplete.vue';
import { mapGetters } from "vuex";
import { Permissions } from "@/enums/enums";
import cSelect from "@/components/wrappers/CustomSelectList.vue";
import Helper from "@/components/helper.js";
import { DocManagementApplicationModel } from "@/models/docManagement/docManagementApplicationModel.js";
import DocManagementApplicationForm from "@/components/docManagement/DocManagementApplicationForm.vue";
import DocManagementRequestApproveDialog from '@/components/docManagement/DocManagementReturnForCorrectionDialog.vue';
import { checkForLocalServer, requireMinLocalServerVersion } from '@/utils/sign.utils.js';

export default {
  name: 'DocManagementApplications',
  components: {
    Grid,
    DocDownloader,
    SchoolYearSelector,
    Autocomplete,
    cSelect,
    DocManagementApplicationForm,
    DocManagementRequestApproveDialog
  },
  data() {
    return {
      saving: false,
      dateAndTimeFormat: Constants.DATE_AND_TIME_FORMAT,
      refKey: 'docManagementApplicationsList',
      defaultGridFilters: {
        instType: 1,
        groupingType: 1,
        campaignType: 1,
      },
      campaignsOptions: null,
      expanded: [],
      headers: [
        {
          text: this.$t('docManagement.application.headers.id'),
          value: "id",
        },
        {
          text: this.$t('docManagement.application.headers.campaign'),
          value: "campaign.name",
        },
        {
          text: this.$t('docManagement.application.headers.schoolYear'),
          value: "schoolYearName",
        },
        {
          text: this.$t('docManagement.application.headers.institution'),
          value: "institutionId",
        },
        {
          text: this.$t('docManagement.application.headers.requestedInstitution'),
          value: "requestedInstitutionId",
        },
        {
          text: this.$t('docManagement.application.headers.status'),
          value: "status",
        },
        {
          text: '',
          value: "attachments",
          filterable: false,
          sortable: false,
        },
        {text: '', value: "controls", filterable: false, sortable: false, align: 'end'},
      ],
      options1: [
        { text: 'Всички', value: 1 },
        { text: 'С делегиран бюджет', value: 2 },
        { text: 'Без делегиран бюджет', value: 3 },
      ],
      options3: [
        { text: 'Всички', value: 1 },
        { text: 'Основни кампании', value: 2 },
        { text: 'Допълнителни кампании', value: 3 },
      ],
      newExchangeRequestModel: null,
      dialog: false,
      submitDialog: false,
      submitApplication: null,
      signing: false,
      signingItemsCount: 0,
      signingProgressCount: 0,
      selectedItemName: '',
    };
  },
  computed: {
    ...mapGetters(["hasPermission", 'userInstitutionId', 'userRegionId']),
    hasReadPermission() {
      return this.hasPermission(Permissions.PermissionNameForDocManagementApplicationRead);
    },
    hasManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForDocManagementApplicationManage);
    },
    hasReportCreatePermission() {
      return this.hasPermission(Permissions.PermissionNameForDocManagementReportCreate);
    },
    gridFilters: {
      get () {
        if (this.refKey in this.$store.state.gridFilters) {
          return this.$store.state.gridFilters[this.refKey] || {};
        }
        else {
          return this.defaultGridFilters;
        }
      },
      set (value) {
        if (this.refKey in this.$store.state.gridFilters) {
          this.$store.commit('updateGridFilter', { options: value, refKey: this.refKey });
        }
        else {
          return this.defaultGridFilters = value;
        }
      }
    },
    options2() {
      const baseOptions = [
        { text: 'Без групиране', value: 1, hidden: false},
        { text: 'Групиране по регион', value: 2, hidden: this.userRegionId },
        { text: 'Групиране по община', value: 3, hidden: false },
      ];
      return baseOptions.filter(x => !x.hidden);
    },
    signingProgressPercentage() {
      return (this.signingProgressCount / this.signingItemsCount) * 100;
    }
  },
  watch: {
    'newExchangeRequestModel.institutionId'(newVal) {
      if (!newVal) {
        this.$helper.clearArray(this.newExchangeRequestModel.attachments);
        this.$helper.clearArray(this.newExchangeRequestModel.basicDocuments);
      } else {
        this.saving = true;
        this.$api.docManagementExchange.getFreeForExchange(newVal)
        .then(response => {
          const basicDocuments = response?.data || [];
          this.newExchangeRequestModel.basicDocuments = basicDocuments.map(x => {
            return {
              basicDocumentId: x.basicDocumentId,
              basicDocumentName: x.basicDocumentName,
              freeDocCount: x.freeDocCount,
              number: null
            };
          });
        })
        .finally(() => {
          this.saving = false;
        });
      }
    }
  },
  mounted() {
    if (!this.hasReadPermission) {
      return this.$router.push("/errors/AccessDenied");
    }

    this.getCampaignsOptions();
  },
  methods: {
    getCampaignsOptions() {
      this.$api.docManagementCampaign.getDropdownOptions()
      .then(response => {
        this.campaignsOptions = response?.data;
      })
      .catch(error => {
        console.log(error.response);
      });
    },
    gridReload() {
      const grid = this.$refs['docManagementApplicationsGrid' + this._uid];
      if(grid) {
        grid.get();
      }
    },
    async deleteApplication(item) {
      if(await this.$refs.confirm.open(this.$t('buttons.delete'), this.$t('common.confirm'))){
        this.saving = true;

        const apiEndpoint = item.isExchangeRequest
          ? this.$api.docManagementExchange.deleteRequest(item.id)
          : this.$api.docManagementApplication.delete(item.id);

        apiEndpoint
        .then(() => {
            this.$notifier.success('', this.$t('common.deleteSuccess'), 5000);
            this.gridReload();
        })
        .catch((error) => {
            this.$notifier.error('',this.$t("common.deleteError"), 5000);
            console.log(error.response);
        })
        .finally(() => { this.saving = false; });
      }
    },
    async generateApplicationReportFile(item) {
      this.saving = true;
      await this.$api.docManagementApplication.generateApplicationReport({ applicationId: item.id })
        .then((response) => {
          const disposition = response.headers["content-disposition"];
          let fileName = Helper.extractFileNameFromDisposition(disposition) || `Заявка за ЗУД_${item.institutionId}.docx`;

          const blob = new Blob([response.data]);
          const url = window.URL.createObjectURL(blob);
          const link = document.createElement('a');
          link.href = url;
          link.setAttribute('download', fileName);
          document.body.appendChild(link);
          link.click();
          setTimeout(() => URL.revokeObjectURL(url), 0);
        })
        .finally(() => { this.saving = false; });
    },
     async generateSignAttachApplicationReportFile(item) {
      this.signingProgressCount = 0;
      this.signingItemsCount = 0;
      this.selectedItemName = '';

      const hasLocalServerError = await checkForLocalServer();
      if (hasLocalServerError) {
          this.$notifier.modalError(
          this.$t('menu.localServer'),
          this.$t('errors.scan.localServerError')
        );
        return;
      }

      const needUpgrade = await requireMinLocalServerVersion("1.0.8");
      if (needUpgrade) {
        this.$notifier.modalError(
          this.$t('menu.localServer'),
          this.$t('errors.upgradeLocalServer')
        );
        return;
      }

      this.signingItemsCount = 1;

      try {
        this.signing = true;
        let thumbprint = null;
        const errors = [];

        const itemTitle = `${item.campaign?.name ?? ''} - ${item.institutionName ?? ''} - ${String(item.institutionId ?? '')} - ${(item.schoolYearName ?? '').replace('/', '-')}`;
        const signOptions = {
          name: item.campaign?.name ?? '',
          title: itemTitle,
          email: item.creator,
        };

        this.signingProgressCount += 1;
        this.selectedItemName = itemTitle;

        var fileResponse = await this.$api.docManagementApplication.generateApplicationReport({ applicationId: item.id });
        if(!fileResponse || !fileResponse.data) {
          return;
        }

        const disposition = fileResponse.headers["content-disposition"];
        let fileName = Helper.extractFileNameFromDisposition(disposition) || `Заявка за ЗУД_${item.institutionId}.docx`;
        const applicatonReportFileBlob = new Blob([fileResponse.data]);
        if (!applicatonReportFileBlob) return;

        const docxBase64 = await this.$helper.blobToBase64(applicatonReportFileBlob);
        if (!docxBase64) return;

        let response = await this.$api.certificate.signDocxThumbprint(docxBase64, thumbprint, signOptions);

        let signedDocument = null;
        if (response && response.isError == false) {
          let signature = response.contents; // base64
          signedDocument = {name: fileName, input: new Blob([new Uint8Array(this.$helper.base64ToByteArray(signature))])};
          thumbprint = response.thumbprint;
          if (
            (thumbprint &&
              thumbprint.toUpperCase() ==
              '6C943294128E6D5455F7CA6B0CE9E4A1F179BFFB') ||
            signature.toUpperCase().indexOf('KONTRAX') > 0
          ) {
            this.$notifier.modalError(
              this.$t('localServer.demoMode'),
              this.$t('localServer.demoModeError')
            );
            this.signing = false;
            return;
          }

          const payload = {
              id: item.id,
              attachments: [{
                noteFileName: signedDocument.name,
                noteFileType: 'application/vnd.openxmlformats-officedocument.wordprocessingml.document',
                noteContents: Helper.base64ToByteArray(signature)
              }]
            };

            try {
              await this.$api.docManagementApplication.attachApplicationReport(payload);
            } catch (error) {
              const { message } = this.$helper.parseError(error);
              if (message) errors.push(`${item.fullName} : ${message}`);
            }
        } else {
          // None = 0,
          // CertificateNotFound = 1,
          // SigningError = 2,
          // OperationCanceled = 100
          if (response.messageCode === 1) {
            throw new Error(response.message);
          } else {
            errors.push(`${item.fullName} : ${response.response}`);
          }
        }


        if (errors.length > 0) {
          this.$notifier.modalError(this.$t('errors.studentLodFinalization'), errors);
        } else {
          this.gridReload();

          const blob = signedDocument.input;
          const url = window.URL.createObjectURL(blob);
          const link = document.createElement('a');
          link.href = url;
          link.setAttribute('download', signedDocument.name);
          document.body.appendChild(link);
          link.click();
          setTimeout(() => URL.revokeObjectURL(url), 0);
        }
      } catch (error) {
        const { message } = this.$helper.parseError(error);
        this.$notifier.error('', message ?? this.$t('common.signError'));
      } finally {
        this.signing = false;
        this.signingProgressCount = 0;
        this.signingItemsCount = 0;
        this.selectedItemName = '';
      }

    },
    async generateProtocolFile(item) {
      this.saving = true;
      await this.$api.docManagementExchange.generateProtocol({ applicationId: item.id })
        .then((response) => {
          const disposition = response.headers["content-disposition"];
          let fileName = Helper.extractFileNameFromDisposition(disposition) || `Заявка за ЗУД_${item.institutionId}.docx`;

          const blob = new Blob([response.data]);
          const url = window.URL.createObjectURL(blob);
          const link = document.createElement('a');
          link.href = url;
          link.setAttribute('download', fileName);
          document.body.appendChild(link);
          link.click();
          setTimeout(() => URL.revokeObjectURL(url), 0);
        })
        .finally(() => { this.saving = false; });
    },
    async generateSummaryReportFile() {
      this.saving = true;
      await this.$api.docManagementCampaign.generateReport({ campaignId: this.gridFilters.campaignId, regionId: this.gridFilters.regionId, municipalityId: this.gridFilters.municipalityId, instType: this.gridFilters.instType, groupingType: this.gridFilters.groupingType, campaignType: this.gridFilters.campaignType  })
        .then((response) => {
          const headers = response.headers;
          const disposition = headers["content-disposition"];

          // Generate filename with timestamp fallback since content-disposition is not exposed
          let fileName = Helper.extractFileNameFromDisposition(disposition);

          if (!fileName) {
            const now = new Date();
            const timestamp = now.toISOString().replace(/[:.]/g, '-').slice(0, -5);
            fileName = `ZUD_Reports_${timestamp}.zip`;
          }

          // Fallback: inspect content-type for extension
          const contentType =  headers['content-type'];
          if (!/\.zip$/i.test(fileName) && contentType && /zip/i.test(contentType)) {
            fileName = fileName.replace(/\.[^.]+$/,'') + '.zip';
          }

          const blob = new Blob([response.data]);
          const url = window.URL.createObjectURL(blob);
          const link = document.createElement('a');
          link.href = url;
          link.setAttribute('download', fileName);
          document.body.appendChild(link);
          link.click();
          setTimeout(() => URL.revokeObjectURL(url), 0);
        })
        .finally(() => { this.saving = false; });
    },
    onNewExchangeRequest() {
      this.newExchangeRequestModel = new DocManagementApplicationModel();
      this.dialog = true;
    },
    onNewExchangeRequestCancel() {
      this.newExchangeRequestModel = null;
      this.dialog = false;
    },
    onNewExchangeRequestSave() {
      const form = this.$refs['docManagementExchangeApplicationCreateForm' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'));
      }

      this.saving = true;
      this.$api.docManagementExchange.createRequest(this.newExchangeRequestModel)
      .then(() => {
        this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
        this.onNewExchangeRequestCancel();
        this.gridReload();
      })
      .catch(() => {
        this.$notifier.error('',this.$t("common.saveError"), 5000);
      })
      .then(() => { this.saving = false;});
    },
    onSubmit(item) {
      this.submitApplication = {
        applicationId: item.id,
        programName: item.campaign.name,
        institutionCode: item.institutionCode,
        institutionName: item.institutionName,
        description: ''
      };
      this.submitDialog = true;
    },
    onSubmitCancel() {
      this.submitDialog = false;
      this.submitApplication = null;
    },
    onSubmitConfirm() {
      this.saving = true;
      this.$api.docManagementApplication.submit(this.submitApplication)
      .then(() => {
        this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
        this.submitDialog = false;
        this.submitApplication = null;
        this.gridReload();
      })
      .catch(() => {
        this.$notifier.error('',this.$t("common.saveError"), 5000);
      })
      .then(() => { this.saving = false;});
    },
    async onApproveApplication(item) {
      this.$refs.approveDialog.open(item.id, null, 'approve', this.$t('buttons.approve'));

    },
    async onRejectApplication(item) {
      this.$refs.rejectDialog.open(item.id, null, 'reject', this.$t('buttons.reject'));
    },
     onApproveRejectSaved() {
      this.gridReload();
    },
  }
};
</script>
