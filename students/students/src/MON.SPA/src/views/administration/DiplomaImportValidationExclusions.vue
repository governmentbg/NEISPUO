<template>
  <v-card>
    <v-card-title>
      {{ $t('validationExclusion.title') }}
    </v-card-title>
    <v-card-text>
      <v-data-table
        :headers="headers"
        :items="items"
        :search="search"
        :loading="loading"
        :footer-props="{itemsPerPageOptions: gridItemsPerPageOptions}"
        class="elevation-1"
      >
        <template v-slot:top>
          <v-toolbar
            flat
          >
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

        <template v-slot:[`item.validTo`]="{ item }">
          {{ item.validTo ? $moment.utc(item.validTo).local().format(dateAndTimeFormat) : '' }}
        </template>

        <template v-slot:[`footer.prepend`]>
          <button-group>
            <v-btn
              small
              color="primary"
              @click.stop="onNewRecordClick"
            >
              {{ $t('buttons.newRecord') }}
            </v-btn>
            <v-btn
              small
              color="secondary"
              outlined
              @click.stop="load"
            >
              {{ $t('buttons.reload') }}
            </v-btn>
          </button-group>
        </template>

        <template v-slot:[`item.controls`]="{ item }">
          <button-group>
            <button-tip
              icon
              icon-name="mdi-pencil"
              icon-color="primary"
              tooltip="buttons.edit"
              bottom
              iclass=""
              small
              @click="onEditClick(item)"
            />
            <button-tip
              icon
              icon-name="mdi-delete"
              icon-color="error"
              tooltip="buttons.delete"
              bottom
              iclass=""
              small
              @click="onDeleteClick(item.id)"
            />
          </button-group>
        </template>
      </v-data-table>
    </v-card-text>
    <v-dialog
      v-model="dialog"
      max-width="1200px"
    >
      <form-layout
        v-if="model"
        skip-cancel-prompt
        @on-save="onSave"
        @on-cancel="onCancel"
      >
        <template #title>
          <h3>{{ $t("validationExclusion.editorFormTitle") }}</h3>
        </template>
        <template>
          <v-form
            :ref="'validationExclusionEditorForm' + _uid"
            :disabled="saving"
          >
            <v-row
              dense
            >
              <v-col
                cols="12"
                sm="6"
              >
                <v-text-field
                  :value="model.id"
                  :label="$t('validationExclusion.id')"
                  disabled
                />
              </v-col>
              <v-col
                cols="12"
                sm="6"
              >
                <v-select
                  v-model="model.personalIdTypeId"
                  :label="$t('validationExclusion.headers.pinType')"
                  :items="pinTypeOptions"
                  :rules="[$validator.required(true)]"
                  clearable
                  class="required"
                />
              </v-col>
              <v-col
                cols="12"
                sm="6"
              >
                <v-text-field
                  v-model="model.personalId"
                  :label="$t('validationExclusion.headers.pin')"
                  :rules="[$validator.required(), $validator.numbers(), $validator.min(1)]"
                  class="required"
                />
              </v-col>
              <v-col
                cols="12"
                sm="6"
              >
                <v-text-field
                  v-model="model.series"
                  :label="$t('validationExclusion.headers.series')"
                  :rules="[$validator.required()]"
                  class="required"
                />
              </v-col>
              <v-col
                cols="12"
                sm="6"
              >
                <v-text-field
                  v-model="model.factoryNumber"
                  :label="$t('validationExclusion.headers.factoryNumber')"
                  :rules="[$validator.required()]"
                  class="required"
                />
              </v-col>
              <v-col
                cols="12"
                sm="6"
              >
                <v-text-field
                  v-model="model.institutonId"
                  :label="$t('validationExclusion.headers.institutionCode')"
                  clearable
                />
              </v-col>
              <!-- <v-col
                cols="12"
                sm="6"
              >
                <date-picker
                  id="validTo"
                  ref="validTo"
                  v-model="model.validTo"
                  :show-buttons="false"
                  :scrollable="false"
                  :disabled="saving"
                  :no-title="true"
                  :show-debug-data="false"
                  :label="$t('validationExclusion.headers.validTo')"
                />
              </v-col> -->
            </v-row>
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
    <confirm-dlg ref="confirm" />
  </v-card>
</template>

<script>
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";
import Constants from "@/common/constants.js";

export default {
  name: 'DiplomaImportValidationExclusionsView',
  data() {
    return {
      search: '',
      loading: false,
      saving: false,
      dialog: false,
      model: null,
      items: [],
      pinTypeOptions: [],
      headers: [
        {
          text: this.$t('validationExclusion.id'),
          value: "id",
        },
        {
          text: this.$t('validationExclusion.headers.pinType'),
          value: "personalIdTypeId",
        },
        {
          text: this.$t('validationExclusion.headers.pin'),
          value: "personalId",
        },
        {
          text: this.$t('validationExclusion.headers.series'),
          value: "series",
        },
        {
          text: this.$t('validationExclusion.headers.factoryNumber'),
          value: "factoryNumber",
        },
        {
          text: this.$t('validationExclusion.headers.institutionCode'),
          value: "institutonId",
        },
        {
          text: this.$t('validationExclusion.headers.validTo'),
          value: "validTo",
        },
        {
          text: '',
          value: 'controls',
          align: 'center',
          sortable: false,
          filterable: false
        },
      ],
      dateAndTimeFormat: Constants.DATE_AND_TIME_FORMAT
    };
  },
  computed: {
    ...mapGetters(['hasPermission', 'gridItemsPerPageOptions']),
  },
  mounted() {
    if(!this.hasPermission(Permissions.PermissionNameForStudentDiplomaImportValidationExclusionsManage)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.load();
    this.loadOptions();
  },
  methods: {
    load() {
      this.loading = true;
      this.$api.diplomaValidationExclusion
        .getList()
        .then((response) => {
          this.items = response.data;
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('common.loadError'));
          console.log(error.response);
        })
        .then(() => { this.loading = false; });
    },
    loadOptions() {
      this.$api.lookups
        .getPinTypes()
        .then((response) => {
          this.pinTypeOptions = response.data;
        })
        .catch((error) => {
          this.$notifier.error('', this.$t('errors.pinOptionsLoad'));
          console.log(error);
        });
    },
    async onDeleteClick(id) {
      if(await this.$refs.confirm.open(this.$t('buttons.delete'), this.$t('common.confirm'))){
        this.saving = true;
        this.$api.diplomaValidationExclusion.delete(id)
          .then(() => {
            this.$notifier.success('', this.$t('common.deleteSuccess'));
            this.load();
          })
          .catch(error => {
            this.$notifier.error('',  this.$t('common.deleteError'));
            console.log(error.response);
          })
          .then(() => { this.saving = false; });
      }
    },
    onNewRecordClick() {
      this.model = {};
      this.dialog = true;
    },
    onCancel() {
      this.dialog = false;
      this.model = null;
    },
    onSave() {
      const form = this.$refs['validationExclusionEditorForm' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'));
      }

      this.model.validTo = this.$helper.parseDateToIso(this.model.validTo, '');

      this.saving = true;
      this.$api.diplomaValidationExclusion.addOrUpdate(this.model)
      .then(() => {
        this.load();
        this.$notifier.success('', this.$t('common.saveSuccess'));
        this.onCancel();
        this.load();
      })
      .catch(error => {
        this.$notifier.error('',  this.$t('common.saveError'));
        console.log(error.response);
      })
      .then(() => { this.saving = false; });
    },
    onEditClick(item) {
      this.model = {...item};
      this.dialog = true;
    }
  }
};
</script>
