<template>
  <v-card>
    <v-card-text>
      <grid
        :ref="'aspBenefitsGrid' + _uid"
        url="/api/asp/GetImportedBenefitsFiles"
        :title="$t('asp.monthlyBenefitsListTitle')"
        :headers="headers"
        :search="search"
        class="elevation-1"
        file-export-name="Списък с импортирани файлове"
        :file-exporter-extensions="['xlsx', 'csv', 'txt']"
        :show-expand="hasAspImportPermission"
        :expanded.sync="expandedItems"
      >
        <template v-slot:[`item.month`]="{ item }">
          {{ item.month }}
        </template>
        <template v-slot:[`item.isSigned`]="props">
          <div v-if="props.item.recordsCount == 0">
            {{ $t('asp.noRecordsSign') }}
          </div>
          <v-chip
            v-else
            :class="props.item.isSigned === true ? 'success': 'light'"
            small
          >
            <yes-no
              :value="props.item.isSigned"
            />
          </v-chip>
        </template>
        <template v-slot:[`item.isActive`]="props">
          <v-chip
            :class="props.item.isActive === true ? 'success': 'light'"
            small
          >
            <yes-no
              :value="props.item.isActive"
            />
          </v-chip>
        </template>
        <template v-slot:[`item.signedDate`]="{ item }">
          {{ item.signedDate ? $moment(item.signedDate).format(dateAndTimeFormat) : "" }}
        </template>
        <template v-slot:[`item.createdDate`]="{ item }">
          {{ item.createdDate ? $moment(item.createdDate).format(dateAndTimeFormat) : "" }}
        </template>
        <template v-slot:[`item.fromDate`]="{ item }">
          {{ item.fromDate ? $moment(item.fromDate).format(dateFormat) : "" }}
        </template>
        <template v-slot:[`item.toDate`]="{ item }">
          {{ item.toDate ? $moment(item.toDate).format(dateFormat) : "" }}
        </template>
        <template v-slot:[`item.importCompleted`]="{ item }">
          <v-chip
            :class="item.importCompleted === true ? 'success': 'light'"
            small
          >
            <yes-no
              :value="item.importCompleted"
            />
          </v-chip>
        </template>
        <template v-slot:[`item.fileStatusCheck`]="{ item }">
          <v-chip
            v-if="item.fileStatusCheck === 'Грешка'"
            :class="getStatusColor(item.fileStatusCheck)"
            small
            @click="showMessages(item)"
          >
            {{ item.fileStatusCheck }}
          </v-chip>
          <v-chip
            v-else
            :class="getStatusColor(item.fileStatusCheck)"
            small
          >
            {{ item.fileStatusCheck }}
          </v-chip>
        </template>
        <template v-slot:[`item.recordsCount`]="{ item }">
          <v-chip
            v-if="!!item.aspConfirmSessionCount && item.aspConfirmSessionCount != item.recordsCount"
            class="error"
            small
          >
            <v-tooltip
              bottom
            >
              <template v-slot:activator="{ on }">
                <span v-on="on"> {{ item.recordsCount }} </span>
              </template>
              <span>Съществуват необработени записи</span>
            </v-tooltip>
          </v-chip>
          <span v-else>
            {{ item.recordsCount }}
          </span>
        </template>

        <template v-slot:[`expanded-item`]="{ item }">
          <td
            :colspan="headers.length + 100"
            class="py-2"
          >
            <div v-if="!!item.aspConfirmSessionCount && item.aspConfirmSessionCount != item.recordsCount">
              <grid
                :ref="'unprocessedAspConfirmsGrid' + item.id"
                url="/api/asp/unprocessedAspConfirmsList"
                :headers="unprocessedAspConfirmsheaders"
                :filter="{ aspSessionNo: item.aspSessionNo }"
                title=""
                file-export-name="Необработени записи"
                :file-exporter-extensions="['xlsx', 'csv', 'txt']"
              >
                <template v-slot:[`item.pin`]="props">
                  {{ `${props.item.pin} - ${props.item.pinType}` }}
                </template>
              </grid>
            </div>
          </td>
        </template>

        <template v-slot:[`item.actions`]="{ item }">
          <button-group>
            <button-tip
              v-if="item.id && item.recordsCount > 0 && hasAspImportPermission"
              icon
              icon-color="primary"
              iclass=""
              icon-name="mdi-pencil"
              small
              tooltip="buttons.edit"
              bottom
              :to="`/asp/monthlyBenefitsImport/${item.id}/edit`"
            />

            <button-tip
              v-if="item.recordsCount > 0"
              icon
              icon-name="mdi-eye"
              icon-color="primary"
              tooltip="buttons.details"
              bottom
              iclass=""
              small
              @click="showDetails(item)"
            />

            <v-tooltip bottom>
              <template v-slot:activator="{ on: tooltip }">
                <doc-downloader
                  v-if="item.blobId && item.blobId !== null && hasAspImportPermission"
                  :value="item"
                  show-icon
                  x-small
                  :show-file-name="false"
                  v-on="{ ...tooltip }"
                />
              </template>
              <span>{{ $t('asp.importedFileDownloadTitle') }}</span>
            </v-tooltip>

            <v-tooltip bottom>
              <template v-slot:activator="{ on: tooltip }">
                <doc-downloader
                  v-if="item.exportedFile.blobId && item.exportedFile.blobId !== null && hasAspImportPermission"
                  :value="item.exportedFile"
                  show-icon
                  x-small
                  :show-file-name="false"
                  v-on="{ ...tooltip }"
                />
              </template>
              <span>{{ $t('asp.exportedFileDownloadTitle') }}</span>
            </v-tooltip>
            <signing-button
              v-if="hasAspBenefitUpdatePermission && !item.isSigned && item.recordsCount > 0 && item.isActive"
              bottom
              small
              :disabled="loading"
              @click="onSignClick(item)"
            />
            <button-tip
              v-if="hasAspBenefitUpdatePermission && item.isSigned && item.recordsCount > 0 && item.isActive"
              icon
              icon-name="mdi-file-sign"
              icon-color="error"
              tooltip="asp.unsign"
              bottom
              iclass=""
              small
              @click="onUnsignClick(item)"
            />

            <button-tip
              v-if="hasAspImportPermission"
              icon
              icon-name="mdi-delete"
              icon-color="red"
              tooltip="buttons.delete"
              bottom
              iclass=""
              small
              @click="deleteFile(item.id)"
            />
          </button-group>
        </template>
      </grid>
    </v-card-text>
    <confirm-dlg ref="confirm" />
  </v-card>
</template>

<script>
import DocDownloader from '@/components/common/DocDownloader.vue';
import Grid from "@/components/wrappers/grid.vue";
import Constants from "@/common/constants.js";
import { mapGetters } from "vuex";
import { config } from "@/common/config";
import { Permissions, Months } from '@/enums/enums';

export default {
name: "ImportedMonthlyBenefitsList",
components: {
  DocDownloader,
  Grid
},
props: {
  institutionId: {
      type: Number,
      required: false,
      default: 0
  },
},
data() {
    return {
      importedFiles: [],
      search: "",
      loading: false,
      APIBaseUrl: config.apiBaseUrl,
      dateAndTimeFormat: Constants.DATE_AND_TIME_FORMAT,
      dateFormat: Constants.DATE_FORMAT,
      expandedItems: [],
      headers: [
        {
          text: this.$t('asp.headers.schoolYear'),
          value: "schoolYear",
        },
        {
          text: this.$t('asp.headers.month'),
          value: "month",
        },
        {
          text: this.$t('asp.headers.createdDate'),
          value: "createdDate",
        },
        {
          text: this.$t('asp.headers.isActive'),
          value: "isActive",
        },
        {
          text: this.$t('asp.headers.fromDate'),
          value: "fromDate",
        },
        {
          text: this.$t('asp.headers.toDate'),
          value: "toDate",
        },
        {
          text: this.$t('asp.headers.fileStatusCheck'),
          value: "fileStatusCheck",
        },
        {
          text: this.$t('asp.headers.importCompleted'),
          value: "importCompleted",
        },
        {
          text: this.$t('asp.headers.recordsCount'),
          value: "recordsCount",
        },
        {
          text: this.$t('asp.headers.forReview'),
          value: "forReview",
        },
        {
          text: this.$t('asp.headers.isSigned'),
          value: "isSigned",
        },
        {
          text: this.$t('asp.headers.signedDate'),
          value: "signedDate",
        },
        { text: "", value: "actions", sortable: false },
      ],
      unprocessedAspConfirmsheaders: [
        {
          text: this.$t('asp.unprocessedAspConfirmsheaders.institutionCode'),
          value: "institutionId",
        },
        {
          text: this.$t('asp.unprocessedAspConfirmsheaders.pin'),
          value: "pin",
        },
        {
          text: this.$t('asp.unprocessedAspConfirmsheaders.firstName'),
          value: "firstName",
        },
        {
          text: this.$t('asp.unprocessedAspConfirmsheaders.middleName'),
          value: "middleName",
        },
        {
          text: this.$t('asp.unprocessedAspConfirmsheaders.lastName'),
          value: "lastName",
        },
        {
          text: this.$t('asp.unprocessedAspConfirmsheaders.notExcused'),
          value: "notExcused",
        },
        {
          text: this.$t('asp.unprocessedAspConfirmsheaders.daysCount'),
          value: "days",
        },
        {
          text: this.$t('asp.unprocessedAspConfirmsheaders.error'),
          value: "error",
        },
      ]
    };
},
computed: {
    ...mapGetters(['gridItemsPerPageOptions', 'userDetails', 'hasPermission']),
    hasAspImportPermission() {
      return this.hasPermission(Permissions.PermissionNameForASPImport);
    },
    hasAspBenefitUpdatePermission() {
      return this.hasPermission(Permissions.PermissionNameForASPBenefitUpdate);
    },
},
methods: {
  async onUnsignClick(aspBenefit) {
    this.loading = true;
      try {
          let signingAttributes = {
            aspBenefitsImportId: aspBenefit.id,
          };

          await this.$api.asp.removeAspBenefitsSigningAtrributes(signingAttributes);

          this.$notifier.success('', this.$t('common.unsignSuccess'));
      } catch (error) {
        console.error(error);
        this.$notifier.error(this.$t('common.unsign'), this.$t('common.unsignError'), 5000);
      }

      this.loading = false;
      this.gridReload();
  },
  showMessages(item){
       this.$notifier.modal(this.$t('errors.aspBenefitImport'), item.importFileMessages, "error", { maxWidth: 2048, persistent: true, scrollable: true, showTextInPreTag: true });
  },
  async onSignClick(aspBenefit) {
      this.loading = true;

      const xml = (await this.$api.asp.constructAspBenefitsConfirmationAsXml({aspMonthlyBenefitImportId: aspBenefit.id})).data;
      if(!xml) {
        this.loading = false;
        this.$helper.logError({ action: 'AspBenefitSign', message: 'Empty xml model'}, aspBenefit, this.userDetails);
        return this.$notifier.error(this.$t('common.sign'), 'Empty xml model', 5000);
      }

      try {
        const signingResponse = await this.$api.certificate.signXml(xml);

        if (signingResponse && signingResponse.isError == false && signingResponse.contents) {
          let signingAttributes = {
            aspBenefitsImportId: aspBenefit.id,
            signature: this.$helper.utf8ToBase64(signingResponse.contents)
          };

          try {
            await this.$api.asp.setAspBenefitsSigningAtrributes(signingAttributes);
          } catch (error) {
            // Ако не успее да записи signature (дава странна CORS грешка)
            // ще маркирам импорта като подписан но без signature.
            signingAttributes.signature = null;
            await this.$api.asp.setAspBenefitsSigningAtrributes(signingAttributes);
          }

          this.$notifier.success('', this.$t('common.signSuccess'));
        } else {
          this.$helper.logError({ action: 'AspBenefitsSign', message: signingResponse}, aspBenefit, { userDetails: this.userDetails, xml: xml });
          console.error(signingResponse);
          this.$notifier.error(this.$t('common.sign'), signingResponse?.message ?? this.$t('common.signError'), 5000);
        }

      } catch (error) {
        this.$helper.logError({ action: 'AspBenefitsSign', message: error}, aspBenefit, { userDetails: this.userDetails, xml: xml });
        console.error(error);
        this.$notifier.error(this.$t('common.sign'), this.$t('common.signError'), 5000);
      }

      this.loading = false;
      this.gridReload();
    },
    gridReload() {
      const grid = this.$refs['aspBenefitsGrid' + this._uid];
      if(grid) {
        grid.get();
      }
    },
    showDetails(item) {
      this.$router.push({
        name: "ImportedBenefitFileDetails",
        params: { importedFileId: item.id, year: item.schoolYear, month: Months.filter(x => (x.monthName || '').toLowerCase() === (item.month || '').toLowerCase())[0].value },
      });
    },
    async deleteFile(fileId){
      if(await this.$refs.confirm.open(this.$t('buttons.delete'), this.$t('common.confirm'))) {
          await this.$api.asp.deleteImporedFileRecord(fileId).then(() => {
            this.gridReload();
          })
          .catch((error) => {
            this.$notifier.error('', error);
            console.log(error.response);
          });
      }
    },
    getStatusColor(status){
       if(status === 'В процес на импортиране'){
         return 'cyan accent 4';
       }
       else if(status === 'Грешка'){
          return 'light';
       }else{
          return 'success';
       }
    }
  },
};
</script>
