<template>
  <div>
    <v-alert
      v-if="!disabled && model.periodTypeId === PersonalDevelopmentSupportPeriodType.LongTerm"
      border="top"
      colored-border
      type="info"
      elevation="2"
    >
      При избор на етап е задължително да попълните учебната година, до която (включително) ще се осигурява ДПЛР.
    </v-alert>
    <v-alert
      v-if="!disabled && hasResourceSupportNeed"
      border="top"
      colored-border
      type="info"
      elevation="2"
    >
      <ul>
        При избор на вид ДПЛР „Ресурсно подпомагане“, „Рехабилитация на слуха и говора“ или „Зрителна рехабилитация“:
        <li>трябва да се посочи вида на институцията, която ще осъществява ресурсното подпомагане</li>
        <li>поне един от посочените специалисти трябва да е ресурсен учител, а при избрана „Рехабилитация на слуха и говора“ и/или „Зрителна рехабилитация“ – специалист по сензорни увреждания</li>
        <li>не се допуска посочването на повече от един специалист от всеки вид (напр. не е възможно въвеждането на повече от един ресурсен учител)</li>
        <li>задължително е да се въведат часовете, с които се работи седмично с детето/ученика съгласно плана за подкрепа</li>
        <li>взетите часове от всеки специалист от ЕПЛР се изчисляват от взетите теми, отбелязани в електронния дневник</li>
      </ul>
    </v-alert>
    <v-tabs
      v-model="tab"
      centered
      center-active
    >
      <v-tab key="basicData">
        <h5>Общи</h5>
        <v-icon
          v-if="tabsValidationErrors && tabsValidationErrors[baiscDataFormRefKey] && tabsValidationErrors[baiscDataFormRefKey].length > 0"
          right
          color="error"
        >
          mdi-alert
        </v-icon>
      </v-tab>
      <v-tab key="dplr">
        <h5>{{ $t('additionalPersonalDevelopment.title') }}</h5>
        <v-icon
          v-if="tabsValidationErrors && tabsValidationErrors[dplrFormRefKey] && tabsValidationErrors[dplrFormRefKey].length > 0"
          right
          color="error"
        >
          mdi-alert
        </v-icon>
      </v-tab>
      <v-tab
        v-if="!!model.id"
        key="dplrControl"
      >
        <h5>{{ $t('additionalPersonalDevelopment.schoolBookData.title') }}</h5>
      </v-tab>
    </v-tabs>

    <v-tabs-items
      v-model="tab"
    >
      <v-tab-item
        key="basicData"
        class="pt-3"
        eager
      >
        <v-form
          :ref="baiscDataFormRefKey"
          :disabled="disabled"
        >
          <v-row dense>
            <v-col>
              <c-text-field
                v-if="disabled"
                v-model="model.schoolYearName"
                :label="$t('common.schoolYear')"
                disabled
                dense
                persistent-placeholder
                outlined
              />
              <school-year-selector
                v-else
                v-model="model.schoolYear"
                :label="$t('common.schoolYear')"
                :rules="[$validator.required()]"
                class="required"
                persistent-placeholder
                outlined
                dense
              />
            </v-col>
            <v-col
              v-if="model.periodTypeId === PersonalDevelopmentSupportPeriodType.LongTerm"
            >
              <c-text-field
                v-if="disabled"
                v-model="model.finalSchoolYearName"
                :label="$t('common.schoolYear')"
                disabled
                dense
                persistent-placeholder
                outlined
              />
              <school-year-selector
                v-else
                v-model="model.finalSchoolYear"
                :label="$t('additionalPersonalDevelopment.finalSchoolYear')"
                :rules="[$validator.required()]"
                class="required"
                persistent-placeholder
                outlined
                dense
              />
            </v-col>
            <v-col>
              <c-text-field
                v-model="model.orderNumber"
                :label="$t('resourceSupport.numberLabel')"
                :disabled="disabled"
                dense
                persistent-placeholder
                outlined
                :rules="disabled ? [] : [$validator.required()]"
                :class="disabled ? '' : 'required'"
              />
            </v-col>
            <v-col>
              <date-picker
                id="orderDate"
                ref="orderDate"
                v-model="model.orderDate"
                :label="$t('resourceSupport.reportDate')"
                :show-buttons="false"
                :scrollable="false"
                no-title
                :show-debug-data="false"
                :rules="[$validator.required()]"
                class="required"
                persistent-placeholder
                outlined
                dense
              />
            </v-col>
          </v-row>
          <v-row dense>
            <v-col
              cols="12"
              md="6"
            >
              <c-text-field
                v-if="disabled"
                v-model="model.periodTypeName"
                :label="$t('additionalPersonalDevelopment.periodType')"
                disabled
                dense
                persistent-placeholder
                outlined
              />
              <c-autocomplete
                v-else
                v-model="model.periodTypeId"
                api="/api/lookups/GetSupportPeriodOptions"
                :label="$t('additionalPersonalDevelopment.periodType')"
                :rules="[$validator.required()]"
                class="required"
                clearable
                dense
                persistent-placeholder
                outlined
                :defer-options-loading="false"
              />
            </v-col>
            <v-col
              cols="12"
              md="6"
            >
              <c-text-field
                v-if="disabled"
                v-model="model.studentTypeName"
                :label="$t('additionalPersonalDevelopment.studetType')"
                disabled
                dense
                persistent-placeholder
                outlined
              />
              <c-autocomplete
                v-else
                v-model="model.studentTypeId"
                api="/api/lookups/GetStudentTypeOptions"
                :label="$t('additionalPersonalDevelopment.studetType')"
                :rules="[$validator.required()]"
                class="required"
                clearable
                dense
                persistent-placeholder
                outlined
                :defer-options-loading="false"
                @change="model.sop = []"
              />
            </v-col>
          </v-row>
          <v-card
            v-if="model.studentTypeId === PersonalDevelopmentSupportStudentType.Sop"
            outlined
          >
            <v-card-title>
              {{ `${$t('sop.sopTitle')} / ${$t('sop.subtitle')}` }}
            </v-card-title>

            <v-card-text>
              <v-alert
                v-if="!disabled"
                border="top"
                colored-border
                type="info"
                elevation="2"
              >
                При избор на вид ученик „със СОП“, на който се предоставя ДПЛР, е задължително да се посочи вид СОП.
              </v-alert>
              <sop
                v-model="model.sop"
                :disabled="disabled"
              />
            </v-card-text>
          </v-card>
          <v-alert
            v-if="!disabled"
            border="top"
            colored-border
            type="info"
            elevation="2"
            class="mt-2"
          >
            <ul>
              При избор на вид ДПЛР „Ресурсно подпомагане“, „Рехабилитация на слуха и говора“ и „Зрителна рехабилитация“ задължително трябва да прикачите следните файлове:
              <li>Заповед/документ от РЦПППО за определяне на ДПЛР</li>
              <li>Карта за оценка на индивидуалните потребности на детето/ученика</li>
              <li>План за подкрепа на детето/ученика</li>
            </ul>
          </v-alert>

          <v-alert
            v-if="!disabled"
            border="top"
            colored-border
            type="warning"
            elevation="2"
            class="mt-2"
          >
            Общият размер на прикачените файлове не бива да надвишава 20 МВ.
          </v-alert>

          <v-card
            outlined
            class="my-1"
          >
            <v-card-title>
              {{ $t('additionalPersonalDevelopment.documents.order') }}
            </v-card-title>
            <v-card-text>
              <file-manager
                v-model="model.orders"
                :disabled="disabled"
              />
            </v-card-text>
          </v-card>
          <v-card
            outlined
            class="my-1"
          >
            <v-card-title>
              {{ $t('additionalPersonalDevelopment.documents.scorecard') }}
            </v-card-title>
            <v-card-text>
              <file-manager
                v-model="model.scorecards"
                :disabled="disabled"
              />
            </v-card-text>
          </v-card>
          <v-card
            outlined
            class="my-1"
          >
            <v-card-title>
              {{ $t('additionalPersonalDevelopment.documents.plan') }}
            </v-card-title>
            <v-card-text>
              <file-manager
                v-model="model.plans"
                :disabled="disabled"
              />
            </v-card-text>
          </v-card>
        </v-form>
      </v-tab-item>
      <v-tab-item
        key="dplr"
        class="pt-3"
        eager
      >
        <v-form
          :ref="dplrFormRefKey"
          :disabled="disabled"
        >
          <v-card
            v-for="(item, index) in model.items"
            :key="item.uid"
            class="ma-1"
            elevation="3"
          >
            <v-alert
              outlined
              :color="item.isSuspended ? 'error' : 'secondary'"
              class="pa-0"
            >
              <v-card-title>
                {{ $t('student.menu.additionalPersonalDevelopment') }}
                <v-spacer />

                <v-chip
                  v-if="item.isSuspended"
                  color="error"
                  small
                  label
                >
                  {{ $t('additionalPersonalDevelopment.suspended') }}
                </v-chip>
                <span v-else>
                  <button-tip
                    v-if="hasManagePermission && !disabled && item.id"
                    icon
                    icon-name="mdi-cancel"
                    icon-color="error"
                    tooltip="buttons.suspend"
                    bottom
                    iclass="mx-2"
                    small
                    @click="onSuspend(item)"
                  />
                  <button-tip
                    v-if="!disabled"
                    icon
                    icon-name="mdi-delete"
                    icon-color="error"
                    tooltip="buttons.delete"
                    bottom
                    iclass="mx-2"
                    small
                    @click="onRemove(index)"
                  />
                </span>
              </v-card-title>
              <v-card-text
                class="py-1"
              >
                <v-alert
                  v-if="item.isSuspended"
                  border="left"
                  colored-border
                  type="error"
                  elevation="2"
                >
                  <v-row dense>
                    <v-col>
                      <c-text-field
                        :value="item.suspensionDate ? $moment(item.suspensionDate).format(dateFormat) : ''"
                        :label="$t('additionalPersonalDevelopment.suspensionDate')"
                        disabled
                        dense
                        persistent-placeholder
                        outlined
                      />
                    </v-col>
                    <v-col>
                      <c-textarea
                        :value="item.suspensionReason"
                        :rows="5"
                        :label="$t('additionalPersonalDevelopment.suspensionReason')"
                        outlined
                        dense
                        persistent-placeholder
                        disabled
                      />
                    </v-col>
                  </v-row>
                  <file-manager
                    v-model="item.suspensionDocuments"
                    disabled
                  />
                </v-alert>
                <v-row
                  dense
                >
                  <v-col
                    cols="12"
                    md="6"
                  >
                    <c-text-field
                      v-if="disabled || item.isSuspended"
                      v-model="item.typeName"
                      :label="$t('lod.supportType')"
                      disabled
                      dense
                      persistent-placeholder
                      outlined
                    />
                    <c-select
                      v-else
                      v-model="item.typeId"
                      :items="filteredAdditionalSupportTypeOptions"
                      :label="$t('lod.supportType')"
                      :rules="[$validator.required()]"
                      class="required"
                      clearable
                      dense
                      persistent-placeholder
                      outlined
                      @change="onTypeChange(index)"
                    >
                      <template v-slot:selection="data">
                        {{ data.item.text }}
                      </template>
                    </c-select>
                  </v-col>
                  <v-col
                    cols="12"
                    md="6"
                  >
                    <c-textarea
                      v-model="item.details"
                      :label="$t('common.detailedInformation')"
                      :disabled="disabled || item.isSuspended"
                      outlined
                      dense
                      persistent-placeholder
                      clearable
                    />
                  </v-col>
                </v-row>
              </v-card-text>

              <v-card-text
                v-if="hasResourceSupportNeed"
                class="py-1"
              >
                <v-alert
                  v-if=" (item.resourceSupport=== 0)"
                  color="warning"
                  border="left"
                  colored-border
                  elevation="2"
                >
                  {{ $t('resourceSupport.emptyResourceSupportsError') }}
                </v-alert>
                <resource-support
                  v-model="item.resourceSupport"
                  :disabled="disabled || item.isSuspended"
                  :disable-add="!NeedForResourceSupport.includes(item.typeId)"
                />
              </v-card-text>
            </v-alert>
          </v-card>

          <v-card-actions
            v-if="!disabled"
            dense
            class="pa-0"
          >
            <button-tip
              icon-name="mdi-plus"
              icon-color="secondary"
              color="secondary"
              text="buttons.lodAdditionalSupportTypeAddTooltip"
              tooltip="buttons.lodAdditionalSupportTypeAddTooltip"
              iclass=""
              outlined
              bottom
              small
              @click="onAdd"
            />
          </v-card-actions>
        </v-form>
      </v-tab-item>
      <v-tab-item
        v-if="!!model.id"
        key="dplrControl"
        class="pt-3"
      >
        <resource-support-school-book-info
          :person-id="model.personId"
          :school-year="model.schoolYear"
          class="ma-1"
        />
      </v-tab-item>
    </v-tabs-items>
    <v-dialog
      v-model="suspendDialog"
      max-width="800px"
      persistent
    >
      <form-layout
        skip-cancel-prompt
        @on-save="onSuspendSave"
        @on-cancel="suspendDialog = false"
      >
        <template #title>
          Прекратяване
        </template>
        <template>
          <v-row dense>
            <v-col>
              <c-text-field
                v-model="editedItem.typeName"
                :label="$t('lod.supportType')"
                disabled
                dense
                persistent-placeholder
                outlined
              />
            </v-col>
          </v-row>
          <v-form
            :ref="'suspendForm' + _uid"
          >
            <v-row
              dense
            >
              <!-- <t>Note: Because this mirrors the native Date parameters, months, hours, minutes, seconds, and milliseconds are all zero indexed. Years and days of the month are 1 indexed.</t> -->
              <v-col>
                <date-picker
                  id="suspensionDate"
                  ref="suspensionDate"
                  v-model="editedItem.suspensionDate"
                  :label="$t('additionalPersonalDevelopment.suspensionDate')"
                  :show-buttons="false"
                  :scrollable="false"
                  no-title
                  :show-debug-data="false"
                  :rules="[$validator.required()]"
                  class="required"
                  persistent-placeholder
                  outlined
                  dense
                  :min="$moment({ year: editedItem.schoolYear, month: 8, day: 15}).format()"
                />
              </v-col>
              <v-col>
                <c-textarea
                  v-model="editedItem.suspensionReason"
                  :rows="5"
                  :label="$t('additionalPersonalDevelopment.suspensionReason')"
                  outlined
                  dense
                  persistent-placeholder
                  clearable
                  :rules="[$validator.required()]"
                  class="required"
                />
              </v-col>
            </v-row>
            <file-manager
              v-model="editedItem.documents"
            />
          </v-form>
        </template>
      </form-layout>
    </v-dialog>
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </div>
</template>

<script>
import SchoolYearSelector from '@/components/common/SchoolYearSelector';
import FileManager from '@/components/common/FileManager.vue';
import CAutocomplete from '@/components/wrappers/CustomAutocomplete.vue';
import Sop from './SopComponent.vue';
import ResourceSupport from '@/components/tabs/resourceSupport/ResourceSupportComponent.vue';
import { PersonalDevelopmentSupportPeriodType, PersonalDevelopmentSupportStudentType, AdditipnalPersonalDevelopmentSupportType } from '@/enums/enums';
import { mapGetters } from 'vuex';
import { Permissions } from "@/enums/enums";
import Constants from "@/common/constants.js";
import Helper from '@/components/helper.js';
import ResourceSupportSchoolBookInfo from '@/components/tabs/resourceSupport/ResourceSupportSchoolBookInfo.vue';
import isEqual from 'lodash.isequal';

export default {
  name: 'AdditionalPersonalDevelopmentSupportForm',
  components: {
    SchoolYearSelector,
    FileManager,
    CAutocomplete,
    Sop,
    ResourceSupport,
    ResourceSupportSchoolBookInfo
  },
  props: {
    value: {
      type: Object,
      default: null
    },
    disabled: {
      type: Boolean,
      default: false
    },
    isCreation: {
      type: Boolean,
      default: false
    },
    isEdit: {
      type: Boolean,
      default: false
    },
  },
  data() {
    return {
      model: this.value,
      helper: Helper,
      dateFormat: Constants.DATEPICKER_FORMAT,
      tab: null,
      saving: false,
      baiscDataFormRefKey: `basicDataForm_${this._uid}`,
      dplrFormRefKey: `dplrForm_${this._uid}`,
      tabsValidationErrors: {},
      additionalSupportTypeOptions: [],
      suspendDialog: false,
      editedItem: {},
      PersonalDevelopmentSupportPeriodType: PersonalDevelopmentSupportPeriodType,
      PersonalDevelopmentSupportStudentType: PersonalDevelopmentSupportStudentType,
      AdditipnalPersonalDevelopmentSupportType: AdditipnalPersonalDevelopmentSupportType,
      NeedForResourceSupport: [AdditipnalPersonalDevelopmentSupportType.ResourceSupport, AdditipnalPersonalDevelopmentSupportType.HearingAndSpeechRehabilitation, AdditipnalPersonalDevelopmentSupportType.VisualRehabilitation],
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission']),
    hasManagePermission() {
      return this.hasStudentPermission(Permissions.PermissionNameForStudentPersonalDevelopmentManage);
    },
    hasResourceSupportNeed() {
      return this.model.items && this.model.items.some(x => this.NeedForResourceSupport.includes(x.typeId));
    },
    filteredAdditionalSupportTypeOptions() {
      return this.additionalSupportTypeOptions.filter(x => {
        if(this.model.items.some(e => e.typeId === x.value)) {
          x.disabled = true;
        } else {
          x.disabled = false;
        }
        return x;
      });
    },
  },
  watch: {
    value() {
      this.model = this.value;
    },
    "model.studentTypeId": function (newValue, oldValue) {
      if(!isEqual(oldValue, newValue) && newValue == PersonalDevelopmentSupportStudentType.Sop) {
        if(this.isCreation || (this.isEdit && this.model.sop.length === 0)) {
          this.loadStudentSop();
        }
      }
    },
    "model.schoolYear": function (newValue, oldValue) {
      if(!isEqual(oldValue, newValue) && this.model.studentTypeId == PersonalDevelopmentSupportStudentType.Sop) {
        if(this.isCreation || (this.isEdit && this.model.sop.length === 0)) {
          this.loadStudentSop();
        }
      }
    },
  },
  mounted() {
    this.loadOptions();
  },
  methods: {
    loadOptions() {
      this.$api.lookups.getAdditionalSupportTypeOptions()
        .then((response) => {
          this.additionalSupportTypeOptions = response.data;
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.additionalSupportTypeOptionsLoad'));
          console.log(error.response);
        });
    },
    loadStudentSop() {
      this.saving = true;
      this.$api.studentAdditionalPDS.getSopForPerson(this.model.personId, this.model.schoolYear)
        .then((response) => {
          if(response.data && response.data.length > 0) {
            response.data.forEach(x => {
              this.model.sop.push(x);
            });

          }
        })
        .catch(() => {
          this.$notifier.error('', this.$t('common.loadError'));
        })
        .finally(() => {
          this.saving = false;
        });
    },
    validate() {
      this.tabsValidationErrors = {};

      const form = this.$refs[this.baiscDataFormRefKey];
      let isValid = form ? form.validate() : false;
      this.tabsValidationErrors[this.baiscDataFormRefKey] = this.$helper.getValidationErrorsDetails(form);

      const dplrForm = this.$refs[this.dplrFormRefKey];
      if (dplrForm) {
        // dplrForm може да не съществува, ако таба не е кликнат, поради отложеното рендериране на v-tabs
        const dplrFormIsValid = dplrForm.validate();
        this.tabsValidationErrors[this.dplrFormRefKey] = this.$helper.getValidationErrorsDetails(dplrForm);
        isValid = isValid && dplrFormIsValid;


        this.model.items.filter(x => this.NeedForResourceSupport.includes(x.typeId)).forEach(x => {
          if(!x.resourceSupport || x.resourceSupport.length === 0) {
            isValid = false;
            const error = this.$t('resourceSupport.emptyResourceSupportsError');
            this.tabsValidationErrors[this.dplrFormRefKey].push(error);
            this.$notifier.error('', error);
          }
        });
      }
      return isValid;
    },
    onAdd() {
      this.model.items.push({
        uid: this.$uuid.v4(),
        resourceSupport: []
      });
    },
    onRemove(index) {
      if (!this.model.items) return; // Няма от какво да махаме
      this.model.items.splice(index, 1);
    },
    onTypeChange(index) {
      const item = this.model.items[index];
      if(!item) return;

      if(this.NeedForResourceSupport.includes(item.typeId)) {
        if(item.resourceSupport.length === 0) {
          item.resourceSupport.push({
            resourceSupportSpecialists: []
          });
        }
      } else {
        item.resourceSupport = [];
      }
    },
    onSuspend(item) {
      this.editedItem = Object.assign({ schoolYear: this.model.schoolYear, documents: [] }, item);
      this.suspendDialog = true;
    },
    onSuspendSave() {
      const form = this.$refs['suspendForm' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      }

      this.editedItem.suspensionDate = this.$helper.parseDateToIso(this.editedItem.suspensionDate, '');

      this.saving = true;
      this.$api.studentAdditionalPDS.suspendAdditionalPersonalDevelopmentSupport(this.editedItem)
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          this.$emit('suspended', this.editedItem);
        })
        .catch((error) => {
          const { message, errors } = this.$helper.parseError(error.response);
          this.$notifier.modalError(message, errors);
          this.$helper.logError({ action: "AdditionalPersonalDevelopmentSuspension", message: message }, errors, this.userDetails);
        })
        .finally(() => {
          this.saving = false;
          this.suspendDialog = false;
          this.$nextTick(() => {
              this.editedItem = Object.assign({}, {});
            });
        });

    }
  }
};
</script>
