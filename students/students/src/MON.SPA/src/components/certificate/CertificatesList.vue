<template>
  <div>
    <grid
      v-if="hasReadPermission"
      :ref="'certificatesList' + _uid"
      url="/api/certificate/GetCertificates"
      :file-export-name="$t('certificate.certificates')"
      :headers="headers"
      :title="$t('certificate.title')"
      :file-exporter-extensions="['xlsx', 'csv', 'txt']"
    >
      <template v-slot:[`item.certificateType`]="props">
        {{ props.item.certificateType === 1 ? "Root" : "Intermediate" }}
      </template>

      <template v-slot:[`item.notBefore`]="{ item }">
        {{
          item.notBefore ? $moment(item.notBefore).format(dateTimeFormat) : ""
        }}
      </template>
      <template v-slot:[`item.notAfter`]="{ item }">
        {{ item.notAfter ? $moment(item.notAfter).format(dateTimeFormat) : "" }}
      </template>
      <template v-slot:[`item.isValid`]="props">
        <v-chip
          :class="props.item.isValid === true ? 'success' : 'light'"
          small
        >
          <yes-no :value="props.item.isValid" />
        </v-chip>
      </template>

      <template #actions="item">
        <button-group>
          <button-tip
            v-if="hasManagePermission"
            icon
            icon-name="mdi-pencil"
            icon-color="primary"
            tooltip="buttons.edit"
            bottom
            iclass=""
            small
            :to="`/administration/certificates/${item.item.id}/edit`"
          />
          <button-tip
            icon
            icon-name="mdi-file-download"
            icon-color="primary"
            iclass=""
            small
            tooltip="buttons.download"
            bottom
            raised
            @click="download(item.item)"
          />

          <button-tip
            icon
            icon-name="mdi-delete"
            icon-color="error"
            tooltip="buttons.delete"
            bottom
            iclass=""
            small
            @click="deleteItem(item.item)"
          />
        </button-group>
      </template>

      <template #footerPrepend>
        <button-group>
          <v-btn
            small
            color="primary"
            :to="`/administration/certificates/create`"
          >
            {{ $t("buttons.newRecord") }}
          </v-btn>
        </button-group>
      </template>

      <v-overlay :value="saving">
        <v-progress-circular
          indeterminate
          size="64"
        />
      </v-overlay>
    </grid>

    <confirm-dlg ref="confirm" />
  </div>
</template>

<script>
import Constants from "@/common/constants.js";
import Grid from "@/components/wrappers/grid.vue";
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";
import { saveAs } from "file-saver";

export default {
  name: "CertificatesList",
  components: {
    Grid,
  },
  data() {
    return {
      saving: false,
      dateTimeFormat: Constants.DATE_AND_TIME_FORMAT,
      headers: [
        {
          text: this.$t("certificate.name"),
          value: "name",
          sortable: true,
        },
        {
          text: this.$t("certificate.isValid"),
          value: "isValid",
          sortable: true,
        },
        {
          text: this.$t("certificate.description"),
          value: "description",
          sortable: true,
        },
        {
          text: this.$t("certificate.type"),
          value: "certificateType",
          sortable: true,
        },
        {
          text: this.$t("certificate.issuer"),
          value: "issuer",
          sortable: true,
        },
        {
          text: this.$t("certificate.subject"),
          value: "subject",
          sortable: true,
        },
        {
          text: this.$t("certificate.validFrom"),
          value: "notBefore",
          sortable: true,
        },
        {
          text: this.$t("certificate.validTo"),
          value: "notAfter",
          sortable: true,
        },
        {
          text: this.$t("certificate.thumbprint"),
          value: "thumbprint",
          sortable: true,
        },
        {
          text: this.$t("certificate.serialNumber"),
          value: "serialNumber",
          sortable: true,
        },
        {
          text: '',
          value: "controls",
          filterable: false,
          sortable: false,
          align: 'end',
        },
      ],
    };
  },
  computed: {
    ...mapGetters(['hasPermission']),
    hasReadPermission() {
      return this.hasPermission(Permissions.PermissionNameForCertificatesRead);
    },
    hasManagePermission() {
      return this.hasPermission(
        Permissions.PermissionNameForCertificatesManage
      );
    },
  },
  mounted() {},
  destroyed() {},
  methods: {
    gridReload() {
      const grid = this.$refs["certificatesList" + this._uid];
      if (grid) {
        grid.get();
      }
    },
    async deleteItem(item) {
      if (
        await this.$refs.confirm.open(
          this.$t("common.delete"),
          this.$t("common.confirm")
        )
      ) {
        this.saving = true;

        this.$api.certificate
          .delete(item.id)
          .then(() => {
            this.gridReload();
          })
          .catch((error) => {
            this.$notifier.error("", this.$t("certificate.deleteError"));
            console.log(error.response.data.message);
          })
          .finally(() => {
            this.saving = false;
          });
      }
    },
    download(item) {
      this.$api.certificate
        .download(item.id)
        .then((response) => {
          var blob = new Blob([response.data], {
            type: "application/x-x509-ca-cert",
          });
          saveAs(blob, item.name + ".cer");
        })
        .catch((error) => {
          this.$notifier.error("", this.$t("certificate.downloadError"));
          console.log(error.response.data.message);
        });
    },
  },
};
</script>
