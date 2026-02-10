<template>
  <v-card
    flat
  >
    <v-card-title>{{ $t('appSettings.absenceImportType.title') }}</v-card-title>
    <v-card-text>
      <v-row
        v-if="hasAbsenceManagePermission"
        dense
      >
        <c-info
          uid="appSettings.absenceImportType"
        >
          <v-radio-group
            v-model="importType"
            row
            dense
            @change="onChange"
          >
            <v-radio
              :label="$t('appSettings.absenceImportType.schoolBook')"
              value="1"
            />
            <v-radio
              :label="$t('appSettings.absenceImportType.file')"
              value="2"
            />
            <v-radio
              :label="$t('appSettings.absenceImportType.manual')"
              value="3"
            />
          </v-radio-group>
        </c-info>
      </v-row>
      <v-alert
        v-else
        outlined
        type="error"
        prominent
        dense
      >
        <h4>{{ $t('errors.401') }}</h4>
      </v-alert>
    </v-card-text>
    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </v-card>
</template>

<script>
import { Permissions, AppSettingKeys } from '@/enums/enums';
import { mapGetters } from 'vuex';
export default {
  name: 'AbsenceImportTypeSetting',
  data() {
    return {
      loading: false,
      saving: false,
      appSettingKey: AppSettingKeys.AbsenceImportType,
      importType: null
    };
  },
  computed: {
    ...mapGetters(['hasPermission']),
    hasAbsenceManagePermission() {
      return this.hasPermission(Permissions.PermissionNameForStudentAbsenceManage);
    }
  },
  mounted() {
    this.loadAppSetting();
  },
  methods: {
    loadAppSetting() {
      this.loading = true;
      this.$api.administration.getTenantAppSetting(this.appSettingKey)
      .then((response) => {
        if(response.data) {
          this.importType = response.data;
        }
      })
      .catch((error) => {
        console.log(error.response);
        this.$notifier.error('', this.$t('errors.load'));
      })
      .then(() => {
        this.loading = false;
      });
    },
    onChange(val) {
      this.saving = true;
      const payload = { key: this.appSettingKey, value: val };
      this.$api.administration.setTenantAppSetting(payload)
      .then(() => {
        this.$notifier.success('', this.$t('common.saveSuccess'));
      })
      .catch((error) => {
        console.log(error.response);
        this.$notifier.error('', this.$t('common.saveError'));
      })
      .then(() => {
        this.saving = false;
      });
    }
  }
};
</script>
