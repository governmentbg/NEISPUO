<template>
  <div>
    <div v-if="loading">
      <v-progress-linear
        v-if="loading"
        indeterminate
        color="primary"
      />
    </div>
    <div v-else>
      <form-layout
        :disabled="saving"
        @on-save="onSave"
        @on-cancel="onCancel"
      >
        <template #title>
          <h3>{{ $t('ores.editTitle') }}</h3>
        </template>

        <template #default>
          <ores-form
            v-if="model !== null"
            :ref="'oresEditForm' + _uid"
            v-model="model"
            :disabled="saving"
          />
        </template>
      </form-layout>
      <v-overlay :value="saving">
        <v-progress-circular
          indeterminate
          size="64"
        />
      </v-overlay>
      <confirm-dlg ref="editConfirm" />
    </div>
  </div>
</template>

<script>
import OresForm from '@/components/ores/OresForm.vue';
import { OresModel } from "@/models/oresModel";
import { mapGetters } from 'vuex';
import { Permissions } from '@/enums/enums';

export default {
  name: 'OresEditView',
  components: {
    OresForm
  },
  props: {
    oresId: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      loading: true,
      saving: false,
      model: null
    };
  },
  computed: {
    ...mapGetters(['hasPermission', 'turnOnOresModule']),
    hasManagePermission() {
      return this.turnOnOresModule && this.hasPermission(Permissions.PermissionNameForOresManage);
    }
  },
  mounted() {
    if(!this.hasManagePermission) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.load();
  },
  methods: {
    load() {
      this.loading = true;
      this.$api.ores.getById(this.oresId)
        .then((response) => {
          if (response.data) {
            this.model = new OresModel(response.data, this.$moment);
          }
        })
        .catch((error) => {
          this.$notifier.success('', this.$t('common.loadError'), 5000);
          console.log(error.response);
        })
        .then(() => {
          this.loading = false;
        });
    },
    async onSave() {
      const form = this.$refs['oresEditForm' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      }

      this.model.startDate = this.$helper.parseDateToIso(this.model.startDate, '');
      this.model.endDate = this.$helper.parseDateToIso(this.model.endDate, '');

      const isConfirmed = this.model.personId
        ? true
        : await this.$refs.editConfirm.open('', this.$t('ores.editPrompt', {
            name: this.model.classId
              ? this.$t('student.class').toLocaleLowerCase()
              : this.$t('common.institution').toLocaleLowerCase()
          }));

      if(!isConfirmed) return;

      this.saving = true;
      this.$api.ores.update(this.model)
        .then(() => {
          this.$notifier.success('', this.$t('common.saveSuccess'), 5000);
          this.onCancel();
        })
        .catch((error) => {
          this.$notifier.error(this.$t('common.save'), error?.response?.data?.message ?? this.$t('common.error'), 7000);
          console.log(error.response);
        })
        .then(() => { this.saving = false; });
    },
    onCancel() {
      this.$router.go(-1);
    },
  }
};
</script>
