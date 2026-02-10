<template>
  <div>
    <form-layout
      :disabled="saving"
      @on-save="onSave"
      @on-cancel="onCancel"
    >
      <template #title>
        <h3>{{ $t('ores.createTitle') }}</h3>
      </template>

      <template #default>
        <ores-form
          v-if="model !== null"
          :ref="'oresCreateForm' + _uid"
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
  </div>
</template>

<script>
import OresForm from '@/components/ores/OresForm.vue';
import { OresModel } from "@/models/oresModel";
import { mapGetters } from 'vuex';
import { Permissions } from '@/enums/enums';

export default {
  name: 'OresCreateView',
  components: {
    OresForm
  },
  props: {
    personId: {
      type: Number,
      default() {
        return null;
      }
    },
    classId: {
      type: Number,
      default() {
        return null;
      }
    },
    institutionId: {
      type: Number,
      default() {
        return null;
      }
    }
  },
  data() {
    return {
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

    this.init();
  },
  methods: {
    init() {
      this.model = new OresModel({
        personId: this.personId,
        classId: this.classId,
        institutionId: this.institutionId,
        startDate: new Date()
      }, this.$moment);
    },
    onSave() {
      const form = this.$refs['oresCreateForm' + this._uid];
      const isValid = form.validate();

      if(!isValid) {
        return this.$notifier.error('', this.$t('validation.hasErrors'), 5000);
      }

      this.model.startDate = this.$helper.parseDateToIso(this.model.startDate, '');
      this.model.endDate = this.$helper.parseDateToIso(this.model.endDate, '');

      this.saving = true;
      this.$api.ores.create(this.model)
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
