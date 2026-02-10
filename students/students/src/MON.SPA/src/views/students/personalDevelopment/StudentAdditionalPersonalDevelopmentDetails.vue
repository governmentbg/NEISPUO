<template>
  <div>
    <form-layout
      :disabled="loading"
    >
      <template #title>
        <h3>{{ $t('additionalPersonalDevelopment.reviewTitle') }}</h3>
      </template>
      <template #default>
        <additional-personal-development-support-form
          v-if="model != null"
          v-model="model"
          disabled
        />
      </template>
      <template #actions>
        <v-spacer />
        <v-btn
          raised
          color="primary"
          @click.stop="backClick"
        >
          <v-icon left>
            fas fa-chevron-left
          </v-icon>
          {{ $t('buttons.back') }}
        </v-btn>
      </template>
    </form-layout>
    <v-overlay :value="loading">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
  </div>
</template>

<script>
import AdditionalPersonalDevelopmentSupportForm from '@/components/students/AdditionalPersonalDevelopmentSupportForm.vue';
import { Permissions } from "@/enums/enums";
import { mapGetters } from 'vuex';
import Constants from '@/common/constants.js';

export default {
  name: 'StudentAdditionalPersonalDevelopmentDetailsView',
  components: {
    AdditionalPersonalDevelopmentSupportForm
  },
  props: {
    personId: {
      type: Number,
      required: true
    },
    id: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      loading: false,
      model: null
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission']),
    hasReadPermission() {
      return this.hasStudentPermission(
        Permissions.PermissionNameForStudentPersonalDevelopmentRead
      );
    },
  },
  mounted() {
    if(!this.hasReadPermission) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.load();
  },
  methods: {
    load() {
      this.loading = true;
      this.$api.studentAdditionalPDS.getById(this.id)
        .then((response) => {
          if (response.data) {
            this.model = response.data;
            this.model.orderDate = this.model.orderDate ? this.$moment(this.model.orderDate).format(Constants.DATEPICKER_FORMAT) : this.model.orderDate;
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
    backClick() {
      this.$router.go(-1);
    },
  }
};
</script>
