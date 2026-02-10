<template>
  <v-card dense>
    <v-card-title>
      <v-btn
        v-if="dualFormCompanyManagePermission"
        small
        color="primary"
        @click="add"
      >
        {{ $t('company.addDualFormBtn') }}
      </v-btn>
    </v-card-title>
    <v-card-text>
      <v-row dense>
        <v-col
          v-for="item in model.dualFormCompanies"
          :key="item.uid"
          cols="12"
          :md="Math.max(12 / model.dualFormCompanies.length, 6)"
          :xl="Math.max(12 / model.dualFormCompanies.length, 4)"
        >
          <student-class-dual-form-company
            :value="item"
            @delete="onDelete"
          />
        </v-col>
      </v-row>
    </v-card-text>
  </v-card>
</template>

<script>
import StudentClassDualFormCompany from '@/components/students/class/StudentClassDualFormCompany.vue';
import { StudentClassDualFormCompanyModel } from '@/models/studentClass/studentClassDualFormCompanyModel';

export default {
  name: 'StudentClassDualFormCompanyManager',
  components: {
    StudentClassDualFormCompany
  },
  props: {
    value: {
      type: Object,
      required: true
    }
  },
  data() {
    return {
      model: this.value,
    };
  },
  computed: {
    dualFormCompanyManagePermission() {
      return true;
    }
  },
  methods: {
    add() {
      if(!this.model.dualFormCompanies) {
        this.$set(this.model, 'dualFormCompanies', []);
      }
      this.model.dualFormCompanies.push(new StudentClassDualFormCompanyModel());
    },
    onDelete(item) {
      if(!this.model.dualFormCompanies) {
        return;
      }
      const index = this.model.dualFormCompanies.findIndex((x) => x.uid === item.uid);
      if (index > -1) {
        this.model.dualFormCompanies.splice(index, 1);
      }
    }
  }
};
</script>
