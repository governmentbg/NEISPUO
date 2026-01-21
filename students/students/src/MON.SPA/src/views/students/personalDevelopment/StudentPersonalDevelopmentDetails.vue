<template>
  <div>
    <div
      v-if="loading"
    >
      <v-progress-linear
        v-if="loading"
        indeterminate
        color="primary"
      />
    </div>
    <div v-else>
      <form-layout>
        <template #title>
          <h3>{{ $t('personalDevelopment.reviewTitle') }}</h3>
        </template>

        <template #default>
          <personal-development-support-form
            v-if="model !== null"
            :ref="'personalDevelopmentSupportForm' + _uid"
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
import PersonalDevelopmentSupportForm from '@/components/students/PersonalDevelopmentSupportForm.vue';
import { PersonalDevelopmentSupportModel } from '@/models/personalDevelopmentSupportModel.js'; 
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'StudentPersonalDevelopmentDetails',
  components: {
    PersonalDevelopmentSupportForm
  },
  props: {
    perosnId: {
      type: Number,
      required: true
    },
    pdId: {
      type: Number,
      required: true
    }
  },
  data() {
    return {
      loading: true,
      model: null
    };
  },
  computed: {
    ...mapGetters(['hasStudentPermission']),
  },
  mounted() {
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentPersonalDevelopmentRead)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.load();
  },
  methods: {
    load() {
      this.loading = true;
      this.$api.studentPDS.getById(this.pdId)
      .then(response => {
        if(response.data) {
          this.model = new PersonalDevelopmentSupportModel(response.data);
        }
      })
      .catch(error => {
        this.$notifier.error('', this.$t('documents.studentSopLoadErrorMessage', 5000));
        console.log(error.response);
      })
      .then(()=> { this.loading = false; });
    },
    backClick() {
      this.$router.go(-1);
    }
  }
};
</script>