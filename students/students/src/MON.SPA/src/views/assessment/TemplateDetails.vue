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
      <form-layout>
        <template #title>
          <h3>{{ $t('lodAssessmentTemplate.reviewTitle') }}</h3>
        </template>

        <template #default>
          <lod-assessment-template-form
            v-if="model !== null"
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
    </div>
  </div>
</template>

<script>
import LodAssessmentTemplateForm from '@/components/lod/assessment/LodAssessmentTemplateForm.vue';
import { mapGetters } from 'vuex';
import { Permissions } from '@/enums/enums';

export default {
  name: 'AssessmentTemplateDetailsView',
  components: {
    LodAssessmentTemplateForm
  },
  props: {
    id: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      loading: false
    };
  },
  computed: {
    ...mapGetters(['hasPermission'])
  },
  mounted() {
    if(!this.hasPermission(Permissions.PermissionNameForLodAssessmentTemplateRead)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.load();
  },
  methods: {
    load() {
      this.loading = true;
      this.$api.lodAssessmentTemplate.getById(this.id)
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
