<template>
  <div>
    <v-card>
      <v-card-title>
        {{ $t("asp.importedData") }}
      </v-card-title>

      <v-dialog
        v-model="editDialog"
        persistent
        max-width="900px"
        :retain-focus="false"
      >
        <v-form
          @submit.stop.prevent="validate"
        >
          <v-card>
            <v-card-title>
              {{ $t('asp.confirmTitle') }}
            </v-card-title>
            <v-card-text>
              <v-row>
                <v-col
                  cols="12"
                  sm="4"
                >
                  <v-text-field
                    v-model="benefitForEdit.publicEduNumber"
                    :label="$t('common.publicEduNumber')"
                    :disabled="true"
                    :value="benefitForEdit.publicEduNumber"
                  />
                </v-col>
                <v-col
                  cols="12"
                  sm="4"
                >
                  <autocomplete
                    id="aspStatus"
                    ref="aspStatus"
                    v-model="benefitForEdit.aspStatus"
                    api="/api/lookups/GetAspStatusOptions"
                    :label="$t('asp.headers.aspStatus')"
                    :placeholder="$t('buttons.search')"
                    hide-no-data
                    hide-selected
                    disabled
                  />
                </v-col>
                <v-col
                  cols="12"
                  sm="4"
                >
                  <combo
                    ref="neispuoStatus"
                    v-model="benefitForEdit.neispuoStatus"
                    api="/api/lookups/GetMonStatusOptions"
                    :label="$t('asp.headers.neispuoStatusName')"
                    :placeholder="$t('buttons.search')"
                    :return-object="true"
                    hide-no-data
                    hide-selected
                    disable-typing
                    :rules="[$validator.required()]"
                    item-text="text"
                    item-value="text"
                    class="required"
                    single-line
                  />
                </v-col>
              </v-row>
              <v-row>
                <v-col
                  cols="12"
                  sm="4"
                >
                  <v-text-field
                    v-model="benefitForEdit.firstName"
                    :label="$t('studentTabs.firstName')"
                    :disabled="true"
                    :value="benefitForEdit.firstName"
                  />
                </v-col>
                <v-col
                  cols="12"
                  sm="4"
                >
                  <v-text-field
                    v-model="benefitForEdit.middleName"
                    :label="$t('studentTabs.middleName')"
                    :disabled="true"
                    :value="benefitForEdit.middleName"
                  />
                </v-col>
                <v-col
                  cols="12"
                  sm="4"
                >
                  <v-text-field
                    v-model="benefitForEdit.lastName"
                    :label="$t('studentTabs.lastName')"
                    :disabled="true"
                    :value="benefitForEdit.lastName"
                  />
                </v-col>
              </v-row>
              <v-row>
                <v-col
                  cols="12"
                  sm="6"
                >
                  <v-text-field
                    v-if="(benefitForEdit.aspStatus && benefitForEdit.aspStatus.value == ASPStatus.Absence)"
                    v-model.number="benefitForEdit.absenceCount"
                    outlined
                    filled
                    :label="$t('asp.headers.absenceCount')"
                    :value="benefitForEdit.absenceCount"
                    type="number"
                    disabled
                    :rules="[$validator.min(0)]"
                  />
                </v-col>

                <v-col
                  cols="12"
                  sm="6"
                >
                  <v-text-field
                    v-if="(benefitForEdit.aspStatus && benefitForEdit.aspStatus.value == ASPStatus.Absence)"
                    v-model.number="benefitForEdit.absenceCorrection"
                    outlined
                    :filled="neispuoConfirmed || (benefitForEdit.aspStatus && benefitForEdit.aspStatus.value != ASPStatus.Absence)"
                    :label="$t('asp.headers.absenceCorrection')"
                    :value="benefitForEdit.absenceCorrection"
                    type="number"
                    clearable
                    :disabled="neispuoConfirmed || (benefitForEdit.aspStatus && benefitForEdit.aspStatus.value != ASPStatus.Absence)"
                    :rules="benefitForEdit.neispuoStatus && benefitForEdit.neispuoStatus.value === NEISPUOStatus.Rejected ? [$validator.required(true), $validator.min(0)] : [$validator.min(0)]"
                    :class="(benefitForEdit.neispuoStatus && benefitForEdit.neispuoStatus.value === NEISPUOStatus.Rejected ? 'required' : '')"
                  />
                </v-col>
                <v-col
                  cols="12"
                  sm="6"
                >
                  <v-text-field
                    v-if="(benefitForEdit.aspStatus && benefitForEdit.aspStatus.value == ASPStatus.NonVisiting)"
                    v-model.number="benefitForEdit.daysCount"
                    outlined
                    filled
                    :label="$t('asp.headers.daysCount')"
                    :value="benefitForEdit.daysCount"
                    type="number"
                    disabled
                    :rules="[$validator.min(0)]"
                  />
                </v-col>
                <v-col
                  cols="12"
                  sm="6"
                >
                  <v-text-field
                    v-if="(benefitForEdit.aspStatus && benefitForEdit.aspStatus.value == ASPStatus.NonVisiting)"
                    v-model.number="benefitForEdit.daysCorrection"
                    outlined
                    :filled="neispuoConfirmed || (benefitForEdit.aspStatus && benefitForEdit.aspStatus.value != ASPStatus.NonVisiting)"
                    :label="$t('asp.headers.daysCorrection')"
                    :value="benefitForEdit.daysCorrection"
                    :disabled="neispuoConfirmed || (benefitForEdit.aspStatus && benefitForEdit.aspStatus.value != ASPStatus.NonVisiting)"
                    type="number"
                    clearable
                    :rules="benefitForEdit.neispuoStatus && benefitForEdit.neispuoStatus.value === NEISPUOStatus.Rejected ? [$validator.required(true), $validator.min(0)] : [$validator.min(0)]"
                    :class="(benefitForEdit.neispuoStatus && benefitForEdit.neispuoStatus.value === NEISPUOStatus.Rejected ? 'required' : '')"
                  />
                </v-col>
              </v-row>
              <v-row>
                <v-textarea
                  v-model="benefitForEdit.reason"
                  counter
                  outlined
                  prepend-icon="mdi-comment"
                  :label="$t('asp.reason')"
                  autocomplete="description"
                />
              </v-row>
            </v-card-text>
            <v-card-actions
              class="justify-center"
            >
              <v-btn
                ref="submit"
                raised
                color="primary"
                type="submit"
                :disabled="sending"
              >
                <v-icon left>
                  fas fa-save
                </v-icon>
                {{ $t('buttons.saveChanges') }}
              </v-btn>

              <v-btn
                raised
                color="error"
                :disabled="sending"
                @click="editDialog = false; get();"
              >
                <v-icon left>
                  fas fa-times
                </v-icon>
                {{ $t('buttons.cancel') }}
              </v-btn>
            </v-card-actions>
          </v-card>
        </v-form>
      </v-dialog>

      <v-card-text>
        <asp-session-info-details
          v-if="hasAspImportPermission"
          :value="aspAskingSession"
          is-confirmation-session
        />

        <mon-session-info-details
          v-if="hasAspImportPermission"
          :value="monAspResponseSession"
          is-confirmation-session
        />

        <v-card>
          <v-simple-table>
            <template v-slot:default>
              <thead>
                <tr>
                  <th>Кампания</th>
                  <th>{{ $t('asp.headers.isActive') }}</th>
                  <th>{{ $t('asp.importedRecordsCount') }}</th>
                  <th>{{ $t('asp.headers.fromDate') }}</th>
                  <th>{{ $t('asp.headers.toDate') }}</th>
                  <th v-if="hasAspUpdateBenefitPermission">
                    {{ $t('asp.headers.isSigned') }}
                  </th>
                  <th v-if="hasAspUpdateBenefitPermission">
                    {{ $t('asp.headers.signedDate') }}
                  </th>
                </tr>
              </thead>
              <tbody>
                <tr>
                  <td>{{ fileMetaData.schoolYear }}/ {{ fileMetaData.month }}</td>
                  <td>
                    <v-chip
                      :class="fileMetaData.isActive === true ? 'success': 'light'"
                      small
                    >
                      <yes-no
                        :value="fileMetaData.isActive"
                      />
                    </v-chip>
                  </td>
                  <td>
                    {{ fileMetaData.recordsCount }}
                  </td>
                  <td>
                    {{ fileMetaData.fromDate }}
                  </td>

                  <td>
                    {{ fileMetaData.toDate }}
                  </td>
                  <td v-if="hasAspUpdateBenefitPermission">
                    <v-chip
                      :class="fileMetaData.isSigned === true ? 'success': 'light'"
                      small
                    >
                      <yes-no
                        :value="fileMetaData.isSigned"
                      />
                    </v-chip>
                  </td>
                  <td v-if="hasAspUpdateBenefitPermission">
                    {{
                      fileMetaData.signedDate ? $moment(fileMetaData.signedDate).format(dateAndTimeFormat) : ""
                    }}
                  </td>
                </tr>
              </tbody>
            </template>
          </v-simple-table>
        </v-card>
      </v-card-text>
    </v-card>

    <v-card class="mt-5">
      <v-card-text>
        <grid
          :ref="'inReviewGrid' + _uid"
          url="/api/asp/GetImportedBenefitsDetails"
          file-export-name="Потвърждаване на прекратяването на месечните помощи."
          :headers="headers"
          :title="$t('asp.importedData')"
          :search="search"
          :file-exporter-extensions="['xlsx', 'csv', 'txt']"
          :filter="{ importedFileId: importedFileId, statusFilter: statusFilter }"
        >
          <template #subtitle>
            <v-row>
              <v-radio-group
                v-model="statusFilter"
                row
              >
                <v-radio
                  label="Всички"
                  :value="-1"
                />
                <v-radio
                  label="За преглед"
                  :value="0"
                />
                <v-radio
                  label="Потвърдени"
                  :value="1"
                />
                <v-radio
                  label="Отказани"
                  :value="2"
                />
              </v-radio-group>
            </v-row>
          </template>

          <template v-slot:[`item.actions`]="{ item }">
            <button-group>
              <button-tip
                icon
                icon-color="primary"
                icon-name="mdi-information"
                iclass=""
                tooltip="student.details"
                top
                small
                :to="`/student/${item.personId}/details`"
              />
              <button-tip
                v-if="item.id && hasAspUpdateBenefitPermission && !fileMetaData.isSigned && fileMetaData.isActive"
                icon
                icon-color="primary"
                iclass=""
                icon-name="mdi-pencil"
                small
                tooltip="buttons.edit"
                bottom
                @click="editItem(item)"
              />
            </button-group>
          </template>

          <v-overlay :value="sending">
            <v-progress-circular
              indeterminate
              size="64"
            />
          </v-overlay>
        </grid>
      </v-card-text>
      <confirm-dlg ref="confirm" />
    </v-card>
  </div>
</template>

<script>
import { MonthlyBenefitsViewModel } from '@/models/asp/monthlyBenefitsViewModel.js';
import {MonthlyBenefit} from '@/models/asp/monthlyBenefit.js';
import { mapGetters } from 'vuex';
import { Permissions, NEISPUOStatus, ASPStatus } from '@/enums/enums';
import Autocomplete from '@/components/wrappers/CustomAutocomplete.vue';
import Constants from '@/common/constants.js';
import Grid from '@/components/wrappers/grid.vue';
import AspSessionInfoDetails from '@/components/asp/AspSessionInfoDetails.vue';
import MonSessionInfoDetails from '@/components/asp/MonSessionInfoDetails.vue';


export default {
  name: 'ImportedMonthlyBenefitsDetails',
  components: { Autocomplete, Grid, AspSessionInfoDetails, MonSessionInfoDetails },
  props: {
    importedFileId: {
      type: Number,
      required: true,
    },
    gridStatusFilter: {
      type: Number,
      default() {
        return undefined;
      }
    },
    year: {
      type: Number,
      default() {
        return undefined;
      },
    },
    month: {
      type: Number,
      default() {
        return undefined;
      },
    },
  },
  data() {
    return {
      benefitForEdit: new MonthlyBenefit(),
      dateAndTimeFormat: Constants.DATE_AND_TIME_FORMAT,
      dateFormat: Constants.DATE_FORMAT,
      search: '',
      statusFilter: this.gridStatusFilter || 0, //За преглед - 0, Потвърдени - 1, Отказвам, няма основания - 2
      sending: false,
      editDialog: false,
      fileMetaData: new MonthlyBenefitsViewModel({}),
      neispuoConfirmed: false,
      ASPStatus: ASPStatus,
      NEISPUOStatus: NEISPUOStatus,
      headers: [
        {
          text: this.$t("asp.headers.institutionCode"),
          value: "currentInstitutionId",
        },
        {
          text: this.$t("asp.headers.institutionName"),
          value: "institutionName",
        },
        {
          text: this.$t("asp.headers.basicClassName"),
          value: "basicClassName",
          sortable: false
        },
        {
          text: this.$t("asp.headers.currentClass"),
          value: "currentClassName",
          sortable: false
        },
        {
          text: this.$t("student.headers.firstName"),
          value: "firstName",
        },
        {
          text: this.$t("student.headers.middleName"),
          value: "middleName",
        },
        {
          text: this.$t("student.headers.lastName"),
          value: "lastName",
        },
        {
          text: this.$t("student.headers.publicEduNumber"),
          value: "publicEduNumber",
        },
        {
          text: this.$t("asp.headers.aspStatus"),
          value: "aspStatus.text",
        },
        {
          text: this.$t("asp.headers.absenceCount"),
          value: "absenceCount",
        },
        {
          text: this.$t("asp.headers.absenceCorrection"),
          value: "absenceCorrection",
        },
        {
          text: this.$t("asp.headers.daysCount"),
          value: "daysCount",
        },
        {
          text: this.$t("asp.headers.daysCorrection"),
          value: "daysCorrection",
        },
        {
          text: this.$t("asp.headers.neispuoStatusName"),
          value: "neispuoStatus.text",
          sortable: false
        },
        { text: "", value: "actions", sortable: false },
      ],
      aspAskingSession: undefined,
      monAspResponseSession: undefined,
    };
  },
  computed:{
      ...mapGetters(['hasPermission']),
      hasAspUpdateBenefitPermission() {
        return this.hasPermission(Permissions.PermissionNameForASPBenefitUpdate);
      },
      hasAspImportPermission() {
        return this.hasPermission(Permissions.PermissionNameForASPImport);
      }
  },
  watch: {
    "benefitForEdit.neispuoStatus": function (value) {
        if(value && value.value === NEISPUOStatus.Confirmed) {
            this.neispuoConfirmed = true;
            this.benefitForEdit.absenceCorrection = 0;
            this.benefitForEdit.daysCorrection = 0;
        }
        else{
            this.neispuoConfirmed = false;
        }
    }
  },
mounted() {
    this.checkAspAskingSession();
    this.checkMonAspResponseSession();

    this.$studentHub.$on('asp-campaign-modified', this.checkMonAspResponseSession);

    this.$api.asp.getImportedBenefitsFileMetaData(this.importedFileId).then((response) => {
        if(response.data){
        this.fileMetaData = new MonthlyBenefitsViewModel(response.data, this.$moment);
      }
    }).catch((error) => {
        this.$notifier.error('', this.$t('errors.importedBenefitsDetailsLoad'));
        console.log(error);
    }).finally(() => {
        this.sending = false;
    });
  },
  destroyed() {
    this.$studentHub.$off('asp-campaign-modified');
  },
  methods: {
    editItem(item){
      this.benefitForEdit = item;
      this.editDialog = true;
    },
    async get() {
      const grid = this.$refs['inReviewGrid' + this._uid];
      if(grid) {
        grid.get();
      }
    },
    async validate() {
      let hasErrors = this.$validator.validate(this);
      const neispuoStatus = this.benefitForEdit.neispuoStatus;
      const aspStatus = this.benefitForEdit.aspStatus;
      if (neispuoStatus && neispuoStatus.value == NEISPUOStatus.Rejected ) {
        // При отказ бройката на коригираните отсъствия/ дни трябва да се различава от началната
        if (aspStatus && aspStatus.value == ASPStatus.Absence) {
          if (this.benefitForEdit.absenceCorrection === null || this.benefitForEdit.absenceCorrection === undefined || this.benefitForEdit.absenceCorrection < 0 || this.benefitForEdit.absenceCount === this.benefitForEdit.absenceCorrection) {
            this.$notifier.error('', this.$t('asp.errors.neispuoRejectedCountMustDiffer'));
            hasErrors = true;
            return;
          } else {

            // В случай, че е отказано потвърждение броят отсъствия трябва да е по-малко от 5 за ученик, 3 за дете в ДГ
            if (this.benefitForEdit.basicClassId > 0){
              if (this.benefitForEdit.absenceCorrection === null || this.benefitForEdit.absenceCorrection === undefined || this.benefitForEdit.absenceCorrection >= 5 || this.benefitForEdit.absenceCorrection < 0) {
                this.$notifier.error('', this.$t('asp.errors.neispuoRejectedAbsenceSchoolCount'));
                hasErrors = true;
                return;
              }
            }
            else{
              if (this.benefitForEdit.absenceCorrection === null || this.benefitForEdit.absenceCorrection === undefined ||this.benefitForEdit.absenceCorrection > 3 || this.benefitForEdit.absenceCorrection < 0) {
                this.$notifier.error('', this.$t('asp.errors.neispuoRejectedAbsenceKGCount'));
                hasErrors = true;
                return;
              }
            }
          }

        } else if (aspStatus && aspStatus.value == ASPStatus.NonVisiting) {
          if (this.benefitForEdit.daysCorrection === null || this.benefitForEdit.daysCorrection === undefined || this.benefitForEdit.daysCorrection < 0 || this.benefitForEdit.daysCount === this.benefitForEdit.daysCorrection) {
            this.$notifier.error('', this.$t('asp.errors.neispuoRejectedCountMustDiffer'));
            hasErrors = true;
            return;
          }
        }
      }

      if(hasErrors) {
        this.$notifier.error('', this.$t('validation.hasErrors', 5000));
        return;
      }

      const payload = {
          id: this.benefitForEdit.id,
          absenceCorrection: this.benefitForEdit.absenceCorrection,
          daysCorrection:  this.benefitForEdit.daysCorrection,
          reason: this.benefitForEdit.reason,
          neispuoStatus: neispuoStatus,
          aspMonthlyBenefitsImportId: this.importedFileId,
          daysCount: this.benefitForEdit.daysCount
      };

      this.sending = true;

      this.$api.asp.updateBenefit(payload)
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'));
          this.editDialog = false;
        })
        .catch((error) => {
          console.log(error.response);
          this.$notifier.error('', this.$t('errors.updateBenefit', 5000));
        })
        .finally(() => {
          this.get();
          this.sending = false;
        });
    },
    async checkAspAskingSession() {
      this.aspAskingSession == undefined;
      if (!this.year || !this.month) {
        return;
      }

      try {
        this.aspAskingSession  = (await this.$api.absenceCampaign.getAspSession(this.year, this.month, 'ПОТВЪРЖДЕНИЕ'))?.data;
      } catch (error) {
        console.log(error);
      }
    },
    async checkMonAspResponseSession() {
      this.monAspResponseSession == undefined;
      if (!this.year || !this.month) {
        return;
      }

      try {
        this.monAspResponseSession  = (await this.$api.asp.getMonSession(this.year, this.month, 'ПОТВЪРЖДЕНИЕ'))?.data;
      } catch (error) {
        console.log(error);
      }
    }
  },
};
</script>
