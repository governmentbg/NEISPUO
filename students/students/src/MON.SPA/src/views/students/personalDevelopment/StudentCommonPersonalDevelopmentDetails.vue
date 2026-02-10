<template>
  <div>
    <form-layout
      :disabled="loading"
    >
      <template #title>
        <h3>{{ $t('commonPersonalDevelopment.reviewTitle') }}</h3>
      </template>
      <template #default>
        <common-personal-development-support-form
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
import CommonPersonalDevelopmentSupportForm from '@/components/students/CommonPersonalDevelopmentSupportForm.vue';
import { Permissions } from "@/enums/enums";
import { mapGetters } from 'vuex';

export default {
  name: 'StudentCommonPersonalDevelopmentDetailsView',
  components: {
    CommonPersonalDevelopmentSupportForm
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
      this.$api.studentCommonPDS.getById(this.id)
        .then((response) => {
          if (response.data) {
            this.model = response.data;
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
