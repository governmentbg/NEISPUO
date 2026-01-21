<template>
  <div>
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
            @click="printDialog = false"
          >
            <v-icon>mdi-close</v-icon>
          </v-btn>
          <v-toolbar-title>Печат</v-toolbar-title>
          <v-spacer />
          <v-chip>
            <v-icon
              left
              color="success"
            >
              mdi-numeric-1-circle-outline
            </v-icon>
            <div>
              <v-icon small>
                fa-arrows-alt-h
              </v-icon> {{ left1Margin }}mm
              <v-icon small>
                fa-arrows-alt-v
              </v-icon> {{ top1Margin }}mm
            </div>
          </v-chip>
          <v-chip>
            <v-icon
              left
              color="success"
            >
              mdi-numeric-2-circle-outline
            </v-icon>
            <div>
              <v-icon small>
                fa-arrows-alt-h
              </v-icon> {{ left2Margin }}mm
              <v-icon small>
                fa-arrows-alt-v
              </v-icon> {{ top2Margin }}mm
            </div>
          </v-chip>
        </v-toolbar>
        <v-card-text>
          <print-component
            v-if="printDialog == true"
            :id="printId"
            :left1-margin="left1Margin"
            :top1-margin="top1Margin"
            :left2-margin="left2Margin"
            :top2-margin="top2Margin"
            :report-name="reportName"
            :is-duplicate="isDuplicate"
          />
        </v-card-text>
        <v-card-actions class="mt-0 pt-0">
          <v-spacer />
          <button-tip
            color="error"
            icon-name="fas fa-times"
            text="buttons.close"
            raised
            @click="closePrintDialog()"
          />
        </v-card-actions>
      </v-card>
    </v-dialog>
    <v-dialog v-model="certificateDialog">
      <v-card v-if="certificateData">
        <v-card-title>{{ $t("diplomas.certificate") }}</v-card-title>
        <v-card-text>
          <v-row>
            <v-col
              cols="12"
              md="2"
            >
              {{ $t("certificate.isValid") }}
            </v-col>
            <v-col
              cols="12"
              md="10"
            >
              {{ certificateData.isValid | yesNo }}
            </v-col>
          </v-row>
          <v-row>
            <v-col
              cols="12"
              md="2"
            >
              {{ $t("certificate.signedDate") }}
            </v-col>
            <v-col
              cols="12"
              md="10"
            >
              {{ certificateData.signedDate | date }}
            </v-col>
          </v-row>
          <certificate-details :certificate="certificateData.certificate" />
        </v-card-text>
        <v-card-actions>
          <v-spacer />
          <v-btn
            outlined
            color="primary"
            @click="certificateDialog = false"
          >
            {{ $t("buttons.close") }}
          </v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
    <grid
      v-if="schoolYearLoaded"
      :ref="'diplomaListGrid' + _uid"
      v-model="selectedItems"
      url="/api/diploma/list"
      :headers="headers"
      :title="title"
      :file-export-name="title"
      :file-exporter-extensions="['xlsx', 'csv', 'txt']"
      :filter="{
        year: gridFilters.schoolYear,
        basicDocuments: gridFilters.basicDocuments,
        filterForSigning: gridFilters.filterForSigning,
        isValidation: isValidation,
        isEqualization: isEqualization,
        personId: personId,
        isSigned: customFilter.isSigned,
        pinFilter: customFilter.pinFilter
          ? customFilter.pinFilter.filter
          : null,
        pinFilterOp: customFilter.pinFilter ? customFilter.pinFilter.op : null,
        nameFilter: customFilter.nameFilter
          ? customFilter.nameFilter.filter
          : null,
        nameFilterOp: customFilter.nameFilter
          ? customFilter.nameFilter.op
          : null,
        seriesFilter: customFilter.seriesFilter
          ? customFilter.seriesFilter.filter
          : null,
        seriesFilterOp: customFilter.seriesFilter
          ? customFilter.seriesFilter.op
          : null,
        factoryNumberFilter: customFilter.factoryNumberFilter
          ? customFilter.factoryNumberFilter.filter
          : null,
        factoryNumberFilterOp: customFilter.factoryNumberFilter
          ? customFilter.factoryNumberFilter.op
          : null,
        institutionIdFilter: customFilter.institutionIdFilter
          ? customFilter.institutionIdFilter.filter
          : null,
        institutionIdFilterOp: customFilter.institutionIdFilter
          ? customFilter.institutionIdFilter.op
          : null,
        basicDocumentTypeFilter: customFilter.basicDocumentTypeFilter
          ? customFilter.basicDocumentTypeFilter.filter
          : null,
        basicDocumentTypeFilterOp: customFilter.basicDocumentTypeFilter
          ? customFilter.basicDocumentTypeFilter.op
          : null,
        schoolYearFilter: customFilter.schoolYearFilter
          ? customFilter.schoolYearFilter.filter
          : null,
        schoolYearFilterOp: customFilter.schoolYearFilter
          ? customFilter.schoolYearFilter.op
          : null,
        regionNameFilter: customFilter.regionNameFilter
          ? customFilter.regionNameFilter.filter
          : null,
        regionNameFilterOp: customFilter.regionNameFilter
          ? customFilter.regionNameFilter.op
          : null,
      }"
      show-select
      selectable-key="canBeSigned"
      show-expand
      :expanded.sync="expandedItems"
      :ref-key="isInStudentLayout ? null : refKey"
      :debounce="1500"
      @pagination="clearSelections"
    >
      <template #subtitle>
        <v-row dense>
          <v-col
            cols="12"
            sm="6"
            md="2"
          >
            <school-year-selector
              v-model="gridFilters.schoolYear"
              item-text="value"
            />
          </v-col>
          <v-col
            cols="12"
            sm="7"
          >
            <autocomplete
              v-model="gridFilters.basicDocuments"
              api="/api/lookups/GetBasicDocumentTypes"
              :label="$t('diplomas.basicDocumentTypeName')"
              :defer-options-loading="false"
              chips
              small-chips
              deletable-chips
              clearable
              multiple
              :filter="{ isValidation: isValidation, isRuoDoc: isEqualization }"
            />
          </v-col>
          <v-col
            cols="12"
            sm="6"
            md="2"
          >
            <v-checkbox
              v-model="gridFilters.filterForSigning"
              label="За подписване"
            />
          </v-col>
          <v-col
            cols="12"
            sm="6"
            md="1"
            class="d-flex justify-end align-center"
          >
            <v-btn
              small
              color="error"
              outlined
              @click="clearFilters"
            >
              <v-icon left>
                fas fa-times
              </v-icon>
              {{ $t("buttons.clear") }}
            </v-btn>
          </v-col>
        </v-row>
      </template>

      <template v-slot:[`item.registrationDate`]="{ item }">
        {{
          item.registrationDate
            ? $moment(item.registrationDate).format(dateFormat)
            : ""
        }}
      </template>

      <template #footerPrepend>
        <v-btn
          v-if="hasDiplomaSignPermission"
          small
          color="error"
          @click.stop="clearSelections"
        >
          <v-icon
            left
            color="white"
          >
            mdi-close
          </v-icon>
          <span class="ml-1">{{ $t("buttons.clearSelected") }}</span>
        </v-btn>
        <v-btn
          v-if="hasDiplomaSignPermission"
          small
          color="primary"
          @click.stop="onSignSelectedClick"
        >
          <v-icon
            left
            color="white"
          >
            mdi-file-sign
          </v-icon>
          <span class="ml-1">{{ $t("buttons.signSelected") }}</span>
        </v-btn>
        <v-btn
          v-if="hasDiplomaManagePermission"
          small
          color="primary"
          @click.stop="onDiplomaCreateClick"
        >
          {{
            isValidation || isEqualization
              ? $t("validationDocument.createFormTitle")
              : $t("menu.diplomas.createDiploma")
          }}
        </v-btn>
        <v-btn
          v-if="validationError"
          color="error"
          small
          @click="errorsDialog = true"
        >
          <v-icon
            left
            color="white"
          >
            mdi-alert-circle-outline
          </v-icon>
          Виж детайли на последната грешка
        </v-btn>
      </template>

      <template v-slot:[`item.isSigned`]="{ item }">
        <v-chip
          v-if="item.isNotSignable === true"
          color="success"
          outlined
          small
        >
          {{ $t("diplomas.isNotSignableStatus") }}
        </v-chip>
        <v-chip
          v-else
          :color="item.isSigned === true ? 'success' : 'error'"
          outlined
          small
        >
          <yes-no :value="item.isSigned" />
        </v-chip>
      </template>

      <template v-slot:[`item.personalId`]="{ item }">
        {{ `${item.personalId} - ${item.personalIdTypeName}` }}
      </template>

      <template v-slot:[`expanded-item`]="{ item }">
        <td :colspan="headers.length">
          <vue-json-pretty
            path="res"
            :data="item"
            show-length
          />
        </td>
      </template>

      <template v-slot:[`item.tags`]="{ item }">
        <v-chip
          v-for="(tag, index) in item.tags"
          :key="index"
          :color="tag.color || 'light'"
          small
          class="ma-1"
        >
          {{ $t(tag.localizationKey) }}
        </v-chip>
      </template>

      <template #actions="item">
        <button-group>
          <button-tip
            v-if="hasDiplomaReadPermission"
            icon
            icon-name="mdi-eye"
            icon-color="primary"
            tooltip="buttons.review"
            bottom
            iclass=""
            small
            @click="onDetailsClick(item.item.id, personId || item.item.personId)"
          />
          <button-tip
            v-if="hasDiplomaManagePermission && item.item.canBeEdit"
            icon
            icon-name="mdi-pencil"
            icon-color="primary"
            tooltip="buttons.edit"
            bottom
            iclass=""
            small
            @click="onEditClick(item.item.id, personId || item.item.personId)"
          />
          <button-tip
            v-if="hasDiplomaReadPermission"
            icon
            icon-name="mdi-file-image"
            icon-color="primary"
            tooltip="diplomas.attachedDocuments"
            bottom
            iclass=""
            small
            @click="onAttachedDocuments(item.item)"
          />
          <button-tip
            v-if="hasDiplomaManagePermission && item.item.canBeSetAsEditable"
            icon
            icon-name="mdi-pencil"
            icon-color="primary"
            tooltip="diplomas.setAsEditable"
            bottom
            iclass=""
            small
            @click="onSetAsEditableClick(item.item.id)"
          />
          <button-tip
            v-if="hasDiplomaManagePermission && item.item.canBeSetAsEditable"
            icon
            icon-name="mdi-cancel"
            icon-color="error"
            tooltip="diplomas.annulment"
            bottom
            iclass=""
            small
            @click="onAnnulmentClick(item.item.id)"
          />
          <button-tip
            v-if="hasDiplomaManagePermission && item.item.canBeDeleted"
            icon
            icon-name="mdi-delete"
            icon-color="error"
            tooltip="buttons.delete"
            bottom
            iclass=""
            small
            @click="onDeleteClick(item.item.id)"
          />
          <button-tip
            v-if="item.item.isNotSignable === false"
            icon
            icon-name="fas fa-certificate"
            iclass=""
            color="normal"
            icon-color="primary"
            tooltip="diplomas.showCertificate"
            bottom
            small
            :disabled="!item.item.isSigned"
            @click="onShowCertificate(item)"
          />
          <button-tip
            v-if="!isInStudentLayout && item.item.personId"
            icon
            icon-color="primary"
            icon-name="fas fa-info-circle"
            iclass=""
            tooltip="student.details"
            top
            small
            :to="`/student/${item.item.personId}/details`"
          />
          <button-tip
            v-if="hasDiplomaReadPermission && item.item.reportFormPath"
            icon
            icon-color="primary"
            icon-name="fa fa-print"
            iclass=""
            tooltip="diplomas.printForm"
            top
            small
            @click="onPrintFormClick(item.item)"
          />
          <button-tip
            icon
            icon-color="primary"
            icon-name="fas fa-file-word"
            iclass=""
            tooltip="diplomas.generateApplicationFile"
            bottom
            small
            @click="generateApplicationFile(item.item)"
          />
        </button-group>
      </template>

      <template v-slot:[`header.isSigned`]="{ header }">
        {{ header.text }}
        <bool-header-filter v-model="customFilter.isSigned" />
      </template>
      <template v-slot:[`header.personalId`]="{ header }">
        {{ header.text }}
        <text-header-filter v-model="customFilter.pinFilter" />
      </template>
      <template v-slot:[`header.fullName`]="{ header }">
        {{ header.text }}
        <text-header-filter v-model="customFilter.nameFilter" />
      </template>
      <template v-slot:[`header.series`]="{ header }">
        {{ header.text }}
        <text-header-filter v-model="customFilter.seriesFilter" />
      </template>
      <template v-slot:[`header.factoryNumber`]="{ header }">
        {{ header.text }}
        <text-header-filter v-model="customFilter.factoruNumberFilter" />
      </template>
      <template v-slot:[`header.institutionId`]="{ header }">
        {{ header.text }}
        <text-header-filter v-model="customFilter.institutionIdFilter" />
      </template>
      <template v-slot:[`header.basicDocumentType`]="{ header }">
        {{ header.text }}
        <text-header-filter v-model="customFilter.basicDocumentTypeFilter" />
      </template>
      <template v-slot:[`header.schoolYearName`]="{ header }">
        {{ header.text }}
        <text-header-filter v-model="customFilter.schoolYearFilter" />
      </template>
      <template v-slot:[`header.ruoRegName`]="{ header }">
        {{ header.text }}
        <text-header-filter v-model="customFilter.regionNameFilter" />
      </template>
    </grid>
    <prompt-dlg
      ref="setAsEditablePrompt"
      persistent
    >
      <template>
        <v-textarea
          v-model="setAsEditableReason"
          :label="$t('diplomas.setAsEditableReason')"
          outlined
          filled
          auto-grow
          clearable
          :rules="[$validator.required()]"
          class="required"
        />
      </template>
    </prompt-dlg>
    <prompt-dlg
      ref="annulmentPrompt"
      persistent
    >
      <template>
        <v-textarea
          v-model="annulmentReason"
          :label="$t('diplomas.diplomaAnnullationReason')"
          outlined
          filled
          auto-grow
          clearable
          :rules="[$validator.required()]"
          class="required"
        />
      </template>
    </prompt-dlg>
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
    <confirm-dlg ref="confirm" />
    <prompt-dlg
      ref="templatePrompt"
      persistent
    >
      <template>
        <v-row dense>
          <c-info uid="diplomaTemplate.diplomaBasicDocumentDropdown">
            <autocomplete
              id="diplomaBasicDocumentDropdown"
              :ref="'DiplomaBasicDocumentDropdown_' + _uid"
              v-model="selectedBasicDocumentId"
              api="/api/lookups/GetBasicDocumentTypes"
              :label="$t('diplomas.templateSelector.basicDocument')"
              :placeholder="$t('common.choose')"
              clearable
              :defer-options-loading="false"
              :filter="{
                schemaSpecified: true,
                isValidation: isValidation,
                isRuoDoc: isEqualization,
                filterByDetailedSchoolType: true
              }"
              @change="diplomaBasicDocumentChange"
            />
          </c-info>
        </v-row>
        <v-row dense>
          <c-info uid="diplomaTemplate.diplomaTemplateDropdown">
            <autocomplete
              id="diplomaTemplateDropdown"
              :ref="'DiplomaTemplateDropdown_' + _uid"
              v-model="selectedTemplateId"
              api="/api/diplomaTemplate/DropdownOptions"
              :label="$t('diplomas.templateSelector.template')"
              :placeholder="$t('common.choose')"
              clearable
              :defer-options-loading="true"
              persistent-hint
              :hint="defferedLoadingHint"
              :filter="{ basicDocumentId: selectedBasicDocumentId }"
              @change="diplomaTemplateChange"
            />
          </c-info>
        </v-row>
        <v-row dense>
          <c-info
            v-if="filteredBasicClassOptions && filteredBasicClassOptions.length > 0"
            uid="diplomaTemplate.diplomaBasicDocumentDropdown"
          >
            <autocomplete
              v-model="selectedBasicClassId"
              :items="filteredBasicClassOptions"
              :label="$t('recognition.basicClass')"
              :placeholder="$t('common.choose')"
              :disabled="selectedTemplate && selectedBasicClassId && selectedTemplate.basicClassId === selectedBasicClassId"
              clearable
              :defer-options-loading="false"
            />
          </c-info>
        </v-row>
      </template>
    </prompt-dlg>

    <prompt-dlg
      ref="printTemplatePrompt"
    >
      <template>
        <v-row dense>
          <c-info uid="diplomaTemplate.diplomaTemplateDropdown">
            <autocomplete
              id="printTemplateDropdown"
              v-model="selectedPrintTemplateId"
              api="/api/printTemplate/DropdownOptions"
              :label="$t('diplomas.printTemplateSelector.template')"
              :placeholder="$t('common.choose')"
              clearable
              :defer-options-loading="false"
              persistent-hint
              :hint="defferedLoadingHint"
              :filter="{ basicDocumentId: selectedDiplomaBasicDocumentId }"
              :rules="[$validator.required()]"
              class="required"
            />
          </c-info>
        </v-row>
        <v-card>
          <v-card-title>
            Отместване
            <v-spacer />
            <button-tip
              icon
              icon-name="fa-sync"
              icon-color="primary"
              tooltip="buttons.reset"
              bottom
              iclass=""
              small
              @click="resetMargins()"
            />
          </v-card-title>
          <v-card-text>
            <v-row>
              <v-col class="pr-4">
                <v-slider
                  v-model="left1Margin"
                  label="1 стр. отляво"
                  class="align-center"
                  :max="20"
                  :min="-20"
                  prepend-icon="fa-arrows-alt-h"
                  hide-details
                >
                  <template v-slot:append>
                    <v-text-field
                      v-model="left1Margin"
                      class="mt-0 pt-0"
                      hide-details
                      single-line
                      type="number"
                      suffix="mm"
                      style="width: 100px"
                    />
                  </template>
                </v-slider>
              </v-col>
            </v-row>
            <v-row>
              <v-col class="pr-4">
                <v-slider
                  v-model="top1Margin"
                  class="align-center"
                  label="1 стр. отгоре"
                  :max="20"
                  :min="-20"
                  hide-details
                  prepend-icon="fa-arrows-alt-v"
                >
                  <template v-slot:append>
                    <v-text-field
                      v-model="top1Margin"
                      class="mt-0 pt-0"
                      hide-details
                      single-line
                      suffix="mm"
                      type="number"
                      style="width: 100px"
                    />
                  </template>
                </v-slider>
              </v-col>
            </v-row>
            <v-row>
              <v-col class="pr-4">
                <v-slider
                  v-model="left2Margin"
                  label="2 стр. отляво"
                  class="align-center"
                  :max="20"
                  :min="-20"
                  prepend-icon="fa-arrows-alt-h"
                  hide-details
                >
                  <template v-slot:append>
                    <v-text-field
                      v-model="left2Margin"
                      class="mt-0 pt-0"
                      hide-details
                      single-line
                      type="number"
                      suffix="mm"
                      style="width: 100px"
                    />
                  </template>
                </v-slider>
              </v-col>
            </v-row>
            <v-row>
              <v-col class="pr-4">
                <v-slider
                  v-model="top2Margin"
                  class="align-center"
                  label="2 стр. отгоре"
                  :max="20"
                  :min="-20"
                  hide-details
                  prepend-icon="fa-arrows-alt-v"
                >
                  <template v-slot:append>
                    <v-text-field
                      v-model="top2Margin"
                      class="mt-0 pt-0"
                      hide-details
                      single-line
                      suffix="mm"
                      type="number"
                      style="width: 100px"
                    />
                  </template>
                </v-slider>
              </v-col>
            </v-row>
          </v-card-text>
        </v-card>
      </template>
    </prompt-dlg>

    <v-overlay :value="signing">
      <v-row justify="center">
        <v-progress-circular
          :value="signingProgressPercentage"
          color="primary"
          size="128"
          width="13"
        >
          <h2 class="white--text">
            {{ `${signingProgressCount}/${selectedItems.length}` }}
          </h2>
        </v-progress-circular>
      </v-row>
      <div class="text-center mt-5">
        <h3>{{ selectedItemName }}</h3>
      </div>
    </v-overlay>
    <v-dialog
      v-model="errorsDialog"
      fullscreen
      hide-overlay
      transition="dialog-bottom-transition"
    >
      <v-toolbar
        color="red"
        outlined
      >
        <v-btn
          icon
          dark
          @click="errorsDialog = false"
        >
          <v-icon>mdi-close</v-icon>
        </v-btn>
        <v-spacer />
        <v-toolbar-items>
          <v-btn
            dark
            text
            @click="errorsDialog = false"
          >
            {{ $t('buttons.close') }}
          </v-btn>
        </v-toolbar-items>
      </v-toolbar>

      <api-error-details
        :value="validationError"
      />
    </v-dialog>
  </div>
</template>

<script>
import Grid from "@/components/wrappers/grid.vue";
import Constants from "@/common/constants.js";
import SchoolYearSelector from "@/components/common/SchoolYearSelector";
import Autocomplete from "@/components/wrappers/CustomAutocomplete.vue";
import ApiErrorDetails from '@/components/admin/ApiErrorDetails.vue';
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";
import CertificateDetails from "@/components/certificate/CertificateDetails.vue";
import PrintComponent from "@/components/print/PrintComponent.vue";
import VueJsonPretty from "vue-json-pretty";
import "vue-json-pretty/lib/styles.css";
import BoolHeaderFilter from "@/components/wrappers/grid/BoolHeaderFilter.vue";
import TextHeaderFilter from "@/components/wrappers/grid/TextHeaderFilter.vue";

export default {
  name: "DiplomaList",
  components: {
    Grid,
    SchoolYearSelector,
    Autocomplete,
    VueJsonPretty,
    PrintComponent,
    CertificateDetails,
    BoolHeaderFilter,
    TextHeaderFilter,
    ApiErrorDetails
  },
  props: {
    institutionId: {
      type: Number,
      required: false,
      default: null,
    },
    personId: {
      type: Number,
      default() {
        return null;
      },
    },
    isValidation: {
      type: Boolean,
      default() {
        return false;
      },
    },
    isEqualization: {
      type: Boolean,
      default() {
        return false;
      },
    },
    title: {
      type: String,
      default() {
        return undefined;
      },
    },
  },
  data() {
    return {
      dateTimeFormat: Constants.DATE_AND_TIME_FORMAT,
      dateFormat: Constants.DATEPICKER_FORMAT,
      refKey: "institutionDiplomaList",
      defaultGridFilter: {
        schoolYear: null,
        filterForSigning: false,
        basicDocuments: []
      },
      filterForSigning: false,
      setAsEditableReason: null,
      annulmentReason: null,
      saving: false,
      signing: false,
      signingProgressCount: 0,
      selectedBasicDocumentId: null,
      selectedDiplomaBasicDocumentId: null,
      isDuplicate: false,
      selectedBasicClassId: null,
      selectedPrintTemplateId: null,
      selectedTemplateId: null,
      left1Margin: 0,
      top1Margin: 0,
      left2Margin: 0,
      top2Margin: 0,
      defferedLoadingHint: this.$t("common.comboSearchHint", [
        Constants.SEARCH_INPUT_MIN_LENGTH,
      ]),
      schoolYearLoaded: false,
      printDialog: false,
      errorsDialog: false,
      certificateDialog: false,
      certificateData: null,
      selectedItems: [],
      expandedItems: [],
      selectedItemName: '',
      validationError: null,
      selectedBasicDocument: null,
      selectedTemplate: null,
      basicClassOptions: [],
      customFilter: {
        isSigned: null,
        pinFilter: null,
        nameFilter: null,
        seriesFilter: null,
        factoruNumberFilter: null,
        institutionIdFilter: null,
        basicDocumentTypeFilter: null,
        schoolYearFilter: null,
        regionNameFilter: null,
      },
    };
  },
  computed: {
    ...mapGetters(["hasPermission", "hasStudentPermission"]),
    headers() {
      const headers = [
        {
          text: this.$t("diplomas.headers.personName"),
          value: "fullName",
        },
        // {
        //   text: this.$t("diplomas.headers.pinType"),
        //   value: "personalIdTypeName",
        // },
        {
          text: this.$t("diplomas.headers.pin"),
          value: "personalId",
        },
        {
          text: this.$t("diplomas.series"),
          value: "series",
          visible: !this.isEqualization || this.isInStudentLayout,
        },
        {
          text: this.$t("diplomas.factoryNumber"),
          value: "factoryNumber",
          visible: !this.isEqualization || this.isInStudentLayout,
        },
        {
          text: this.$t("diplomas.additionalDocument.registrationNumber"),
          value: "registrationNumberTotal",
        },
        {
          text: this.$t("diplomas.additionalDocument.registrationNumberYear"),
          value: "registrationNumberYear",
        },
        {
          text: this.$t("diplomas.registrationDate"),
          value: "registrationDate",
        },
        {
          // Добавено е само, за да може да присъства в item
          text: this.$t("diplomas.basicDocumentTypeName"),
          value: "basicDocumentId",
          align: " d-none",
        },
        {
          text: this.$t("diplomas.basicDocumentTypeName"),
          value: "basicDocumentType",
        },
        {
          text: this.$t("diplomas.institutionCode"),
          value: "institutionId",
          visible: !this.isEqualization || this.isInStudentLayout,
        },
        {
          text: this.$t("institution.headers.region"),
          value: "ruoRegName",
          visible: this.isEqualization && !this.isInStudentLayout,
        },
        {
          text: this.$t("diplomas.isSignedStatus"),
          value: "isSigned",
        },
        {
          text: this.$t("diplomas.status"),
          value: "tags",
          sortable: false,
          filterable: false,
        },
        {
          text: this.$t("diplomas.schoolYear"),
          value: "schoolYearName",
        },
        {
          text: this.$t("diplomas.yearGraduated"),
          value: "yearGraduated",
          visible: !this.isEqualization || this.isInStudentLayout,
        },
        {
          text: "",
          value: "controls",
          filterable: false,
          sortable: false,
          align: "end",
        },
        {
          text: "",
          value: "printActions",
          align: "center",
          sortable: false,
        },
      ];

      return headers.filter(x => x.visible !== false);
    },
    isInStudentLayout() {
      return !!this.personId;
    },
    selectedItemsIds() {
      return this.selectedItems.map((x) => x.id);
    },
    hasDiplomaReadPermission() {
      return (
        this.hasStudentPermission(Permissions.PermissionNameForStudentDiplomaRead) ||
        this.hasPermission(
          Permissions.PermissionNameForInstitutionDiplomaRead
        ) ||
        this.hasPermission(Permissions.PermissionNameForAdminDiplomaRead) ||
        this.hasPermission(Permissions.PermissionNameForMonHrDiplomaRead) ||
        this.hasPermission(
          Permissions.PermissionNameForStudentDiplomaByCreateRequestRead
        )
      );
    },
    hasDiplomaManagePermission() {
      if(this.isEqualization) {
        return this.hasPermission(Permissions.PermissionNameForAdminDiplomaManage)
          || this.hasPermission(Permissions.PermissionNameForRuoHrDiplomaManage);
      }

      return this.personId
        ? this.hasStudentPermission(Permissions.PermissionNameForStudentDiplomaManage)
          || this.hasStudentPermission(Permissions.PermissionNameForStudentDiplomaByCreateRequestManage)
        : this.hasPermission(Permissions.PermissionNameForInstitutionDiplomaManage)
          || this.hasPermission(Permissions.PermissionNameForAdminDiplomaManage)
          || this.hasPermission(Permissions.PermissionNameForMonHrDiplomaManage);
    },
    hasDiplomaSignPermission() {
      return (
        this.hasDiplomaManagePermission &&
        this.selectedItems &&
        this.selectedItems.length > 0
      );
    },
    signingProgressPercentage() {
      if (!this.selectedItems || !Array.isArray(this.selectedItems)) {
        return 0;
      }

      return (this.signingProgressCount / this.selectedItems.length) * 100;
    },
    gridFilters: {
      get() {
        if (this.refKey in this.$store.state.gridFilters) {
          return this.$store.state.gridFilters[this.refKey] || {};
        } else {
          return this.defaultGridFilter;
        }
      },
      set(value) {
        if (this.refKey in this.$store.state.gridFilters) {
          this.$store.commit("updateGridFilter", {
            options: value,
            refKey: this.refKey,
          });
        } else {
          return (this.defaultGridFilter = value);
        }
      },
    },
    filteredBasicClassOptions() {
      if(!Array.isArray(this.basicClassOptions) || this.basicClassOptions.length === 0) {
        return [];
      }

      if(this.selectedTemplate && this.selectedTemplate.basicClassId) {
        return this.basicClassOptions.filter(x => x.value === this.selectedTemplate.basicClassId);
      }

      if(this.selectedBasicDocument && Array.isArray(this.selectedBasicDocument.basicClassList)
        && this.selectedBasicDocument.basicClassList.length > 0) {
          return this.basicClassOptions.filter(x => this.selectedBasicDocument.basicClassList.includes(x.value));
      }

      return [];
    }
  },
  watch: {
    selectedPrintTemplateId: async function (value) {
      if (value) {
        const printTemplateId = value.replace(".trdp", "");
        const parsed = Number.parseInt(printTemplateId);
        if (!Number.isNaN(parsed)) {
          let printTemplate = await this.$api.printTemplate.getPrintTemplate(
            printTemplateId
          );
          this.left1Margin = printTemplate.data.left1Margin;
          this.top1Margin = printTemplate.data.top1Margin;
          this.left2Margin = printTemplate.data.left2Margin;
          this.top2Margin = printTemplate.data.top2Margin;
        } else {
          const margins = await this.$api.printTemplate.getDefaultMargins(
            this.selectedDiplomaBasicDocumentId,
            printTemplateId
          );
          if (margins && margins.data) {
            this.left1Margin = margins.data.left1Margin;
            this.top1Margin = margins.data.top1Margin;
            this.left2Margin = margins.data.left2Margin;
            this.top2Margin = margins.data.top2Margin;
          }
        }
      } else {
        this.left1Margin = 0;
        this.top1Margin = 0;
        this.left2Margin = 0;
        this.top2Margin = 0;
      }
    },
  },
  async created() {
    if (this.isInStudentLayout) {
      this.schoolYearLoaded = true;
    } else {
      try {
        if(!this.gridFilters.schoolYear) {
          const currentYear = Number(
            (await this.$api.institution.getCurrentYear(this.institutionId))?.data
          );
          if (!isNaN(currentYear)) {
            this.gridFilters.schoolYear = currentYear;
            this.refresh();
          } else {
            this.$helper.getYear();
          }
        }

      } catch (error) {
        console.log(error);
        this.$helper.getYear();
      } finally {
        this.schoolYearLoaded = true;
      }
    }
  },
  mounted() {
    this.loadBasicClassOptions();
  },
  methods: {
    refresh() {
      const grid = this.$refs["diplomaListGrid" + this._uid];
      if (grid) {
        grid.get();
      }
    },
    clearFilters() {
      this.gridFilters = this.defaultGridFilter;
    },
    async checkForLocalServer() {
      let hasLocalServerError = false;
      await this.$api.localServer
        .version()
        .then(() => {
          hasLocalServerError = false;
        })
        .catch(() => {
          hasLocalServerError = true;
        });

      return hasLocalServerError;
    },
    async onSignSelectedClick() {
      this.signingProgressCount = 0;
      this.selectedItemName = "";
      this.signing = true;
      this.validationError = null;
      const hasLocalServerError = await this.checkForLocalServer();
      this.signing = false;
      if (hasLocalServerError) {
        this.$notifier.modalError(
          this.$t('menu.localServer'),
          this.$t('errors.scan.localServerError')
        );
        return;
      }

      if (this.selectedItems && this.selectedItems.length > 0) {
        this.signing = true;
        let thumbprint = null;
        try {
          for (const item of this.selectedItems) {
            this.signingProgressCount += 1;
            this.selectedItemName = item.fullName;

            let xmlContents = (
              await this.$api.diploma.constructDiplomaByIdAsXml(item.id)
            ).data;
            let response = await this.$api.certificate.signXmlThumbprint(
              xmlContents,
              thumbprint
            );

            if (response && response.isError == false) {
              let signature = response.contents;
              thumbprint = response.thumbprint;
              if (
                (thumbprint &&
                  thumbprint.toUpperCase() ==
                    "6C943294128E6D5455F7CA6B0CE9E4A1F179BFFB") ||
                signature.toUpperCase().indexOf("KONTRAX") > 0
              ) {
                this.$notifier.modalError(
                  this.$t('localServer.demoMode'),
                  this.$t('localServer.demoModeError')
                );
                this.signing = false;
                this.refresh();
                return;
              }
              const payload = {
                diplomaId: item.id,
                confirmedStepNumber: 2, // подписване, финализиране, публична
                signature: this.$helper.utf8ToBase64(signature),
              };

              try {
                await this.$api.diploma.updateDiplomaFinalizationSteps(payload);
              } catch(error) {
                const {message, errors} = this.$helper.parseError(error.response);
                if(errors) {
                  this.validationError = { date: new Date(), ...error.response.data } ;
                  this.$notifier.modalError(message, errors);
                } else {
                  this.$notifier.error('', this.$t('diplomas.updateDiplomaFinalizationStepsErrorMsg'));
                }
              }
            } else {
              // Грешка при подписване
              this.$notifier.error("", this.$t("common.signError"));
            }
          }
        } finally {
          this.signing = false;
          this.signingProgressCount = 0;
          this.selectedItemName = "";
          this.refresh();
        }
      }
    },
    clearSelections() {
      this.$helper.clearArray(this.selectedItems);
      this.$helper.clearArray(this.expandedItems);
    },
    async onSetAsEditableClick(diplomaId) {
      if (
        await this.$refs.setAsEditablePrompt.open(
          "",
          this.$t("diplomas.setAsEditable")
        )
      ) {
        if (!this.setAsEditableReason) {
          return this.$notifier.error(
            "",
            `${this.$t("diplomas.setAsEditableReasonError")}`
          );
        }

        this.saving = true;
        this.$api.diploma
          .setAsEditable({
            diplomaId: diplomaId,
            reason: this.setAsEditableReason,
          })
          .then(() => {
            this.refresh();
            this.$notifier.success("", this.$t("common.saveSuccess"));
          })
          .catch(() => {
            this.$notifier.error("", this.$t("common.saveError"));
          })
          .finally(() => {
            this.setAsEditableReason = null;
            this.saving = false;
          });
      } else {
        this.setAsEditableReason = null;
      }
    },
    async onAnnulmentClick(diplomaId) {
      if (
        await this.$refs.annulmentPrompt.open("", this.$t("diplomas.annulment"))
      ) {
        if (!this.annulmentReason) {
          return this.$notifier.error(
            "",
            `${this.$t("diplomas.annulmentReasonError")}`
          );
        }

        this.saving = true;
        this.$api.diploma
          .anullDiploma({
            diplomaId: diplomaId,
            cancellationReason: this.annulmentReason,
          })
          .then(() => {
            this.refresh();
            this.$notifier.success("", this.$t("common.saveSuccess"));
          })
          .catch(() => {
            this.$notifier.error("", this.$t("common.saveError"));
          })
          .finally(() => {
            this.annulmentReason = null;
            this.saving = false;
          });
      } else {
        this.annulmentReason = null;
      }
    },
    async onDeleteClick(diplomaId) {
      if (
        await this.$refs.confirm.open(
          this.$t("buttons.delete"),
          this.$t("common.confirm")
        )
      ) {
        this.saving = true;

        this.$api.diploma
          .delete(diplomaId)
          .then(() => {
            this.$notifier.success("", this.$t("common.deleteSuccess"));
            this.refresh();
          })
          .catch(() => {
            this.$notifier.error("", this.$t("common.deleteError"));
          })
          .then(() => {
            this.saving = false;
          });
      }
    },
    async onDiplomaCreateClick() {
      if (
        await this.$refs.templatePrompt.open(
          "",
          this.$t("diplomas.templateSelector.title"),
          { width: 800 }
        )
      ) {
        if (!this.selectedTemplateId && !this.selectedBasicDocumentId) {
          return this.$notifier.error(
            "",
            `${this.$t("diplomas.templateSelector.missingTemplateSelection")}`
          );
        }

        const templateId = this.selectedTemplateId;
        const basicDocumentId = this.selectedBasicDocumentId;
        const basicClassId = this.selectedBasicClassId;
        this.selectedBasicDocumentId = null;
        this.selectedTemplateId = null;
        this.selectedBasicClassId = null;

        if(this.isInStudentLayout) {
          return this.$router.push({
            name: "StudentDiplomaCreate",
            params: {
              personId: this.personId,
            },
            query: {
              templateId: templateId,
              basicDocumentId: basicDocumentId,
              basicClassId: basicClassId,
            },
          });
        } else {
          return this.$router.push({
            name: "DiplomaCreate",
            query: {
              templateId: templateId,
              basicDocumentId: basicDocumentId,
              basicClassId: basicClassId,
            },
          });
        }
      } else {
        this.selectedBasicDocumentId = null;
        this.selectedTemplateId = null;
      }
    },
    onDetailsClick(diplomaId, personId) {
      if (this.isInStudentLayout) {
        this.$router.push({
          name: "StudentDiplomaReview",
          params: { id: personId, diplomaId: diplomaId },
        });
      } else {
        this.$router.push({
          name: "DiplomaReview",
          params: { id: personId, diplomaId: diplomaId },
        });
      }
    },
    onEditClick(diplomaId, personId) {
      if (this.isInStudentLayout) {
        this.$router.push({
          name: "StudentDiplomaEdit",
          params: { id: personId, diplomaId: diplomaId },
        });
      } else {
        this.$router.push({
          name: "DiplomaEdit",
          params: { id: personId, diplomaId: diplomaId },
        });
      }
    },
    async generateApplicationFile(item) {
      await this.$api.diploma
        .generateApplicationFile(item.id)
        .then((response) => {
          const url = window.URL.createObjectURL(new Blob([response.data]));
          const link = document.createElement("a");
          link.href = url;
          link.setAttribute("download", `Рег_кн_изд_док_${item.fullName}.docx`);
          document.body.appendChild(link);
          link.click();
        });
    },
    onAttachedDocuments(item) {
      if (this.isValidation) {
        this.$router.push({
          name: "ValidationImages",
          params: { id: item.id, details: item },
        });
      } else {
        this.$router.push({
          name: "DiplomaImages",
          params: { id: item.id, details: item },
        });
      }
    },
    async onPrintFormClick(item) {
      this.selectedDiplomaBasicDocumentId = item.basicDocumentId;
      if (item.reportFormPath) {
        this.selectedPrintTemplateId = item.reportFormPath;
      }
      if (
        await this.$refs.printTemplatePrompt.open(
          "",
          this.$t("diplomas.printTemplateSelector.title"),
          { width: 800 }
        )
      ) {
        if (!this.selectedPrintTemplateId) {
          return this.$notifier.error(
            "",
            `${this.$t(
              "diplomas.printTemplateSelector.missingTemplateSelection"
            )}`
          );
        }

        await this.$api.basicDocument.getById(item.basicDocumentId).then((response) => {
          if (response) {
            this.isDuplicate = response.data.isDuplicate;
          }
        });

        // Ако сме избрали данните за шаблона по подразбиране, запазваме отместванията
        // if (this.selectedPrintTemplateId == item.reportFormPath)
        {
          this.$api.printTemplate.setDefaultMargins({
            left1Margin: this.left1Margin,
            top1Margin: this.top1Margin,
            left2Margin: this.left2Margin,
            top2Margin: this.top2Margin,
            basicDocumentId: this.selectedDiplomaBasicDocumentId,
            reportForm: this.selectedPrintTemplateId,
          });
        }

        this.printId = item.id.toString();
        this.reportName = this.selectedPrintTemplateId; //, item.reportFormPath;
        this.printDialog = true;
      }
    },
    closePrintDialog() {
      this.printDialog = false;
    },
    async onShowCertificate(item) {
      var vm = this;
      this.$api.diploma.getDiplomaSigningData(item.item.id).then((response) => {
        vm.certificateDialog = true;
        vm.certificateData = response.data;
      });
    },
    resetMargins() {
      this.left1Margin = 0;
      this.top1Margin = 0;
      this.left2Margin = 0;
      this.top2Margin = 0;
    },
    diplomaBasicDocumentChange(basicDocumentId) {
      if(!basicDocumentId && !this.selectedTemplateId) {
        this.selectedBasicClassId = null;
        return;
      }

      const selector = this.$refs[`DiplomaBasicDocumentDropdown_${this._uid}`];
       if (selector) {
        const selectedItem = selector.getOptionsList().find(x => x.value === basicDocumentId);
        this.selectedBasicDocument = selectedItem;
      }
    },
    diplomaTemplateChange(templateId) {
      if(!templateId && !this.selectedBasicDocumentId) {
        this.selectedBasicClassId = null;
        return;
      }
      const selector = this.$refs[`DiplomaTemplateDropdown_${this._uid}`];
       if (selector) {
        const selectedItem = selector.getOptionsList().find(x => x.value === templateId);
        this.selectedTemplate = selectedItem;
        if(selectedItem && selectedItem.basicClassId) {
          this.selectedBasicClassId = selectedItem.basicClassId;
        }
      }
    },
    loadBasicClassOptions() {
      this.$api.lookups.getBasicClassOptions({minId: 1, maxId: 13})
      .then((result) => {
        if(result) {
          this.basicClassOptions = result.data;
        }
      })
      .catch((error) => {
        console.log(error.response);
      });
    }
  },
};
</script>
