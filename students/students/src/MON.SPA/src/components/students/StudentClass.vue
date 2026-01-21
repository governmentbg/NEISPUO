<template>
  <v-card>
    <v-card-title :class="model.isOwn ? 'green lighten-4' : ''">
      <div v-if="showTitle">
        {{ model.school }}
      </div>
    </v-card-title>

    <v-card-text>
      <v-row dense>
        <v-col
          cols="12"
          md="3"
          sm="6"
        >
          <c-info
            uid="studentClass.numberInClass"
          >
            <v-text-field
              :value="model.numberInClass"
              :label="$t('studentTabs.numberInClass')"
              readonly
            />
          </c-info>
        </v-col>
        <v-col
          cols="12"
          md="3"
          sm="6"
        >
          <v-text-field
            :value="model.className"
            :label="$t('studentTabs.className')"
            readonly
          />
        </v-col>
        <v-col
          cols="12"
          md="3"
          sm="6"
        >
          <v-text-field
            :value="model.group"
            :label="$t('studentTabs.group')"
            readonly
          />
        </v-col>
        <v-col
          cols="12"
          md="3"
          sm="6"
        >
          <v-text-field
            :value="position"
            :label="$t('studentTabs.position')"
            readonly
          />
        </v-col>
        <v-col
          cols="12"
          md="4"
          sm="6"
        >
          <v-text-field
            :value="model.grade"
            :label="$t('studentTabs.grade')"
            readonly
          />
        </v-col>
        <v-col
          cols="12"
          md="4"
          sm="6"
        >
          <v-text-field
            :value="model.educationForm"
            :label="$t('studentTabs.educationForm')"
            readonly
          />
        </v-col>
        <v-col
          cols="12"
          md="4"
          sm="6"
        >
          <v-text-field
            :value="model.trainingType"
            :label="$t('studentTabs.trainingType')"
            readonly
          />
        </v-col>
        <v-col
          cols="12"
          md="4"
          sm="6"
        >
          <v-text-field
            :value="model.receptionAfter"
            :label="$t('studentTabs.receptionAfter')"
            readonly
          />
        </v-col>
        <v-col
          cols="12"
          md="4"
          ms="6"
        >
          <v-text-field
            :value="model.specialty"
            :label="$t('studentTabs.specialty')"
            readonly
          />
        </v-col>
        <!-- <v-col
          cols="12"
          md="4"
          sm="6"
        >
          <v-text-field
            :value="model.recordedInBookSubjectsPage"
            :label="$t('studentTabs.recordedInBookSubjectsPage')"
            readonly
          />
        </v-col> -->
      </v-row>
    </v-card-text>
    <v-card-actions>
      <v-spacer />
      <v-btn
        v-if="model.isOwn"
        left
        small
        color="primary"
        raised
        @click="onViewClass"
      >
        <v-icon>fa-eye</v-icon>
        {{ $t('studentTabs.viewClass') }}
      </v-btn>
    </v-card-actions>
  </v-card>
</template>

<script>
import Positions from "@/common/positions.js";
export default {
  name: 'StudentClass',
  props: {
    model: {
      type: Object,
      required: true
    },
    showTitle: {
      type: Boolean,
      default() {
        return false;
      }
    }
  },
  data() {
    return {
      position: Positions.filter(x => x.value == this.model.positionId).length >0 ? Positions.filter(x => x.value == this.model.positionId)[0].text: ''
    };
  },
  methods:{
    onViewClass(){
      this.$router.push({ name: 'ClassGroupDetails', params: { id: this.model.classId } });
    }
  }
};
</script>
