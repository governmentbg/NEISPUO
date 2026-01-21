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
          <h3>{{ $t('sop.reviewTitle') }}</h3>
        </template>

        <template #default>
          <sop-form
            v-if="model !== null"
            :ref="'sopForm' + _uid"
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
import SopForm from '@/components/students/SopForm.vue';
import { StudentSopModel } from '@/models/studentSopModel';
import { Permissions } from "@/enums/enums";
import { mapGetters } from "vuex";

export default {
  name: 'StudentSopDetails',
  components: {
    SopForm
  },
  props: {
    perosnId: {
      type: Number,
      required: true
    },
    sopId: {
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
    if(!this.hasStudentPermission(Permissions.PermissionNameForStudentSopRead)) {
      return this.$router.push('/errors/AccessDenied');
    }

    this.load();
  },
  methods: {
    load() {
      this.loading = true;
      this.$api.studentSOP.getById(this.sopId)
      .then(response => {
        if(response.data) {
          this.model = new StudentSopModel(response.data);
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