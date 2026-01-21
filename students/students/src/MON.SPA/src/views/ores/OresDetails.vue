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
          <h3>{{ $t('ores.reviewTitle') }}</h3>
        </template>

        <template #default>
          <ores-form
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
      <ores-students
        :ores-id="oresId"
        class="mt-4"
      />
    </div>
  </div>
</template>

<script>
import OresForm from '@/components/ores/OresForm.vue';
import OresStudents from '@/components/ores/OresStudents.vue';
import { OresModel } from "@/models/oresModel";
import { mapGetters } from 'vuex';
import { Permissions } from '@/enums/enums';

export default {
  name: 'OresEditView',
  components: {
    OresForm,
    OresStudents
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
    },
    oresId: {
      type: Number,
      required: true,
    },
  },
  data() {
    return {
      loading: true,
      model: null
    };
  },
  computed: {
    ...mapGetters(['hasPermission', 'turnOnOresModule']),
    hasReadPermission() {
      return this.turnOnOresModule && this.hasPermission(Permissions.PermissionNameForOresRead);
    }
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
    backClick() {
      this.$router.go(-1);
    },
  }
};
</script>
