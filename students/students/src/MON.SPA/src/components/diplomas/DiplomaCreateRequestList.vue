<template>
  <div>
    <grid
      :ref="'diplomaCreateRequestGrid_' + _uid"
      url="/api/diplomaCreateRequest/list"
      :headers="headers"
      :title="hideTitle ? '' : $t('diplomas.createRequest.listTitle')"
    >
      <template v-slot:[`item.isGranted`]="item">
        <v-chip
          :color="item.isGranted === true ? 'success' : 'secondary'"
          outlined
          small
        >
          <yes-no :value="item.isGranted" />
        </v-chip>
      </template>

      <template v-slot:[`item.pin`]="{ item }">
        {{ `${item.pin} - ${item.pinType}` }}
      </template>

      <template v-slot:[`item.registrationDate`]="{ item }">
        {{ item.registrationDate ? $moment(item.registrationDate).format(dateFormat) : '' }}
      </template>

      <template v-slot:[`item.createDate`]="{ item }">
        {{ item.createDate ? $moment(item.createDate).format(dateTimeFormat) : '' }}
      </template>

      <template v-slot:[`item.modifyDate`]="{ item }">
        {{ item.modifyDate ? $moment(item.modifyDate).format(dateTimeFormat) : '' }}
      </template>

      <template v-slot:[`item.diplomaIsSigned`]="{ item }">
        <v-chip
          v-if="!!item.diplomaId"
          :color="item.diplomaIsSigned === true ? 'success' : 'error'"
          outlined
          small
        >
          <yes-no :value="item.diplomaIsSigned" />
        </v-chip>
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
            v-if="hasManagePermission && (!!item.item.diplomaId === false) && userInstitutionId && userInstitutionId === item.item.requestingInstitutionId"
            icon
            icon-color="primary"
            icon-name="mdi-pencil"
            iclass=""
            tooltip="buttons.edit"
            bottom
            small
            :to="`/diploma/createRequest/${item.item.id}/edit`"
          />
          <button-tip
            v-if="hasManagePermission && (!!item.item.diplomaId === false) && userInstitutionId && userInstitutionId === item.item.requestingInstitutionId"
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
            v-if="item.item.personId && (!!item.item.diplomaId === false)"
            icon
            icon-color="primary"
            icon-name="mdi-certificate"
            iclass=""
            tooltip="diplomas.createRequest.createTitle"
            bottom
            small
            @click="onDiplomaListClick(item.item.personId)"
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
import Grid from '@/components/wrappers/grid.vue';
import Constants from '@/common/constants.js';
import { Permissions } from '@/enums/enums';
import { mapGetters } from 'vuex';

export default {
  name: 'DiplomaCreateRequestListComponent',
  components: {
    Grid
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
      dateTimeFormat: Constants.DATE_AND_TIME_FORMAT,
      dateFormat: Constants.DATEPICKER_FORMAT,
      headers: [
        {text: this.$t('diplomas.createRequest.headers.personName'), value: "personName", sortable: true},
        {text: this.$t('diplomas.createRequest.headers.pin'), value: "pin", sortable: true},
        {text: this.$t('diplomas.createRequest.headers.requestingInstitution'), value: "requestingInstitutionName", sortable: true},
        {text: this.$t('diplomas.createRequest.headers.currentInstitution'), value: "currentInstitutionName", sortable: true},
        {text: this.$t('diplomas.createRequest.headers.basicDocument'), value: "basicDocumentName", sortable: true},
        {text: this.$t('diplomas.createRequest.headers.registrationNumber'), value: "registrationNumber", sortable: true},
        {text: this.$t('diplomas.createRequest.headers.registrationNumberYear'), value: "registrationNumberYear", sortable: true},
        {text: this.$t('diplomas.createRequest.headers.registrationDate'), value: "registrationDate", sortable: true},
        {text: this.$t('diplomas.createRequest.headers.diploma'), value: "diplomaId", sortable: true},
        {text: this.$t('diplomas.isSignedStatus'), value: "diplomaIsSigned", sortable: true},
        {
          text: this.$t("diplomas.status"),
          value: "tags",
          sortable: false,
          filterable: false,
        },
        {text: '', value: 'controls', filterable: false, sortable: false, align: 'end'},
      ]
    };
  },
  computed: {
    ...mapGetters(['hasPermission', 'userInstitutionId']),
    hasManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForDiplomaCreateRequestManage);
    },
  },
  methods: {
    async deleteItem(item) {
      if(await this.$refs.confirm.open(this.$t('common.delete'), this.$t('common.confirm'))){
        this.saving = true;

        this.$api.diplomaCreateRequest
          .delete(item.id)
          .then(() => {
            this.$notifier.success('', this.$t('common.deleteSuccess'));
            this.gridReload();
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
    gridReload() {
      const grid = this.$refs[`diplomaCreateRequestGrid_${this._uid}`];
      if (grid) {
        grid.get();
      }
    },
    onDiplomaListClick(personId) {
      let routeData = this.$router.resolve({ name: 'StudentDiplomasList', params: { id: personId } });
      window.open(routeData.href, '_blank');
    }
  }
};
</script>
