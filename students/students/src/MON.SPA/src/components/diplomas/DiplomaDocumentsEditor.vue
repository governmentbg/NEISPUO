<template>
  <div>
    <validation-errors-details
      :value="validationErrors"
    />
    <v-alert
      class="mt-5"
      border="left"
      colored-border
      type="info"
      elevation="2"
    >
      Като първи свързан документ въведете:
      <ul>
        <li>Данни за оригинала, ако въвеждате дубликат на диплома</li>
        <li>Данни за документа, към който е приложението, ако въвеждате приложение на диплома</li>
      </ul>
    </v-alert>
    <v-form
      :ref="`additionalDocumentssForm_${_uid}`"
      :disabled="disabled"
    >
      <v-card-subtitle>
        <v-btn
          v-if="hasManagePermission"
          small
          color="primary"
          :disabled="disabled"
          @click.stop="onAdditionalDocumentAdd"
        >
          <v-icon left>
            mdi-plus
          </v-icon>
          {{ $t("buttons.newDocument") }}
        </v-btn>
        <span v-if="hasOriginalDocumentSearchPermission">
          <v-btn
            v-if="hasSelectedPerson"
            small
            color="primary"
            class="ml-2"
            @click.stop="onSearchForOriginalDocument"
          >
            <v-icon left>
              mdi-file-search
            </v-icon>
            Търсене на оригинален документ
          </v-btn>
          <span v-else>
            <v-tooltip bottom>
              <template v-slot:activator="{ on: tooltip }">
                <span v-on="{ ...tooltip }">
                  <v-btn
                    small
                    color="primary"
                    class="ml-2"
                    disabled
                  >
                    <v-icon
                      left
                    >
                      mdi-file-search
                    </v-icon>
                    {{ $t('diplomas.additionalDocument.searchBtn') }}
                  </v-btn>
                </span>
              </template>
              <span> Липсва въведен идентификатор на лицето </span>
            </v-tooltip>
          </span>
        </span>
      </v-card-subtitle>
      <v-card
        v-for="(item, index) in value.diplomaData.additionalDocuments"
        :key="index"
        class="mb-2"
        outlined
      >
        <v-card-title
          class="pb-0"
        >
          <v-spacer />
          <button-tip
            v-if="hasManagePermission"
            icon
            icon-name="mdi-close-thick"
            icon-color="error"
            iclass="mx-2"
            tooltip="buttons.delete"
            bottom
            fab
            :disabled="disabled"
            @click="onAdditionalDocumentDelete(item.uid)"
          />
        </v-card-title>
        <v-card-text
          class="py-0"
        >
          <diploma-additional-document-editor
            :value="value.diplomaData.additionalDocuments[index]"
            :main-basic-documents="value.mainBasicDocuments"
          />
        </v-card-text>
      </v-card>
    </v-form>
    <confirm-dlg ref="additionalDocumentConfirm" />
    <v-dialog
      v-model="dialog"
      min-width="800"
      persistent
    >
      <v-card>
        <v-toolbar
          color="primary"
          dark
        >
          <v-btn
            icon
            dark
            @click="dialog = false"
          >
            <v-icon>mdi-close</v-icon>
          </v-btn>
          <v-toolbar-title>{{ $t('diplomas.additionalDocument.searchTitle') }}</v-toolbar-title>
          <v-spacer />
          <v-toolbar-items>
            <v-btn
              dark
              text
              @click="dialog = false"
            >
              {{ $t('buttons.close') }}
            </v-btn>
          </v-toolbar-items>
        </v-toolbar>
        <v-card-text>
          <v-card
            v-for="(item, index) in originalDocuments"
            :key="index"
            class="my-5"
            color="grey lighten-4"
          >
            <v-card-text dense>
              <v-row dense>
                <v-col
                  cols="12"
                  md="6"
                >
                  <v-text-field
                    :value="item.basicDocumentName"
                    :label="$t('diplomas.additionalDocument.basicDocument')"
                    readonly
                    dense
                  />
                </v-col>
                <v-col
                  cols="12"
                  md="6"
                >
                  <v-text-field
                    :value="item.institutionDetails"
                    :label="$t('diplomas.additionalDocument.institution')"
                    readonly
                    dense
                  />
                </v-col>
                <v-col
                  cols="12"
                  md="6"
                >
                  <v-text-field
                    :value="item.series"
                    :label="$t('diplomas.additionalDocument.series')"
                    readonly
                    dense
                  />
                </v-col>
                <v-col
                  cols="12"
                  md="6"
                >
                  <v-text-field
                    :value="item.factoryNumber"
                    :label="$t('diplomas.additionalDocument.factoryNumber')"
                    readonly
                    dense
                  />
                </v-col>
                <v-col
                  cols="12"
                  md="6"
                >
                  <v-text-field
                    :value="item.registrationNumber"
                    :label="$t('diplomas.additionalDocument.registrationNumber')"
                    readonly
                    dense
                  />
                </v-col>
                <v-col
                  cols="12"
                  md="6"
                >
                  <v-text-field
                    :value="item.registrationNumberYear"
                    :label="$t('diplomas.additionalDocument.registrationNumberYear')"
                    readonly
                    dense
                  />
                </v-col>
                <v-col
                  cols="12"
                  md="6"
                >
                  <v-text-field
                    :value="item.registrationDate ? $moment(value.fromDate).format(dateFormat) : ''"
                    :label="$t('diplomas.additionalDocument.registrationDate')"
                    readonly
                    dense
                  />
                </v-col>
              </v-row>
            </v-card-text>
            <v-card-actions dense>
              <v-spacer />
              <v-btn
                raised
                color="primary"
                @click="onSelectOriginalDocument(item)"
              >
                {{ $t('buttons.select') }}
              </v-btn>
            </v-card-actions>
          </v-card>
        </v-card-text>

        <v-card-actions dense>
          <v-spacer />
          <v-btn
            color="primary"
            text
            @click="dialog = false"
          >
            {{ $t('buttons.close') }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
  </div>
</template>

<script>
import ValidationErrorsDetails from '@/components/common/ValidationErrorsDetails';
import DiplomaAdditionalDocumentEditor from '@/components/diplomas/DiplomaAdditionalDocumentEditor';
import { DiplomaAdditionalDocumentModel } from '@/models/diploma/diplomaAdditionalDocumentModel';
import { Permissions } from "@/enums/enums";
import { mapGetters } from 'vuex';
import Constants from '@/common/constants.js';

export default {
  name: 'DiplomaDocumentsEditorComponent',
  components: {
    DiplomaAdditionalDocumentEditor,
    ValidationErrorsDetails
  },
  props: {
    value: {
      type: Object,
      default() {
        return null;
      }
    },
    disabled: {
      type: Boolean,
      default() {
        return false;
      }
    },
    personId: {
      type: Number,
      default() {
        return null;
      }
    },
  },
  data() {
    return {
      validationErrors: [],
      dialog: false,
      originalDocuments: [],
      dateFormat: Constants.DATEPICKER_FORMAT,
    };
  },
  computed: {
    ...mapGetters(['hasPermission']),
    hasManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForInstitutionDiplomaManage)
        || this.hasPermission(Permissions.PermissionNameForAdminDiplomaManage)
        || this.hasPermission(Permissions.PermissionNameForMonHrDiplomaManage)
        || this.hasPermission(Permissions.PermissionNameForRuoHrDiplomaManage);
    },
    hasOriginalDocumentSearchPermission() {
      return !this.disabled && this.hasManagePermission && this.value.mainBasicDocuments.length > 0 && !this.value.id;
    },
    hasSelectedPerson() {
      return this.personId
        || (this.value && this.value.diplomaData && this.value.diplomaData.generalDataModel
          && this.value.diplomaData.generalDataModel.personalId && this.value.diplomaData.generalDataModel.personalIdType);
    },

  },
  methods: {
    validate() {
      this.validationErrors = [];
      const form = this.$refs[`additionalDocumentssForm_${this._uid}`];
      let isValid = false;
      if (form) {
        isValid = form.validate();
        this.validationErrors = this.$helper.getValidationErrorsDetails(form);
      }

      return isValid;
    },
    onAdditionalDocumentAdd() {
      if (!this.value|| !this.value.diplomaData) return;

      this.value.diplomaData.additionalDocuments.push(new DiplomaAdditionalDocumentModel());
    },
    async onAdditionalDocumentDelete(uid) {
      if (!this.value || !this.value.diplomaData.additionalDocuments) {
        return;
      }

      if(await this.$refs.additionalDocumentConfirm.open('', this.$t('common.confirm'))) {
        const index = this.value.diplomaData.additionalDocuments.findIndex(x => x.uid === uid);
        this.value.diplomaData.additionalDocuments.splice(index, 1);
      }
    },
    async onSearchForOriginalDocument() {
      this.originalDocuments = [];
      if (!this.value || !this.value.diplomaData) return;

      const { data } = await this.$api.diploma.getOriginalDocuments(this.personId, this.value.diplomaData.generalDataModel?.personalId,
        this.value.diplomaData.generalDataModel?.personalIdType, this.value.mainBasicDocuments,);
      if(data.length === 0) {
        return this.$notifier.error(this.$t('diplomas.additionalDocument.searchTitle'), this.$t('common.loadError'));
      }

      if(data.length === 1) {
        this.value.diplomaData.additionalDocuments.push(new DiplomaAdditionalDocumentModel(data[0]));
        return this.$notifier.success(this.$t('diplomas.additionalDocument.searchTitle'), this.$t('common.loadSuccess'));
      }

      if(data.length > 1) {
        this.originalDocuments = [...data];
        this.dialog = true;
      }
    },
    onSelectOriginalDocument(item) {
      if (!this.value || !this.value.diplomaData || !this.value.diplomaData.additionalDocuments) return;

      const index = this.value.diplomaData.additionalDocuments.findIndex((x) => x.mainDiplomaId === item.mainDiplomaId);
      if (index > -1) {
        this.value.diplomaData.additionalDocuments.splice(index, 1);
      }

      this.value.diplomaData.additionalDocuments.push(new DiplomaAdditionalDocumentModel(item));
      this.dialog = false;
      return this.$notifier.success(this.$t('diplomas.additionalDocument.searchTitle'), this.$t('common.loadSuccess'));
    },
  }
};
</script>
