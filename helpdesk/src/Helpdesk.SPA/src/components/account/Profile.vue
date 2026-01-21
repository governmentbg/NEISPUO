<template>
  <div>
    <v-card v-if="userDetails">
      <v-card-title>
        <v-icon left>
          mdi-account-circle-outline
        </v-icon>
        {{ user.profile.sub }}
      </v-card-title>
      <v-card-subtitle>
        {{ $t("account.role", { role: userInfo.roleName }) }}
      </v-card-subtitle>
      <v-card-text>
        <v-row dense>
          <v-col
            v-if="userInfo.institution"
            cols="12"
          >
            <v-icon left>
              mdi-town-hall
            </v-icon>{{
              `${$t("account.institution", {
                institution: userInfo.institution,
              })}${userInfo.instType ? ` / ${userInfo.instType}` : ""}`
            }}
          </v-col>
          <v-col
            v-if="userInfo.address"
            cols="12"
          >
            <v-icon left>
              mdi-map-marker-outline
            </v-icon>{{ $t("account.address", { address: userInfo.address }) }}
          </v-col>
          <v-col
            v-if="userInfo.isLeadTeacher"
            cols="12"
          >
            <v-icon left>
              mdi-account-star-outline
            </v-icon>
            {{ $t("common.leadTeacher") }}
            <v-chip
              v-for="classGroup in userInfo.leadTeacherClassGroups"
              :key="classGroup.id"
            >
              {{ classGroup.name }}
            </v-chip>
          </v-col>
        </v-row>
        <v-expansion-panels
          v-if="extended"
          class="pt-2"
        >
          <v-expansion-panel>
            <v-expansion-panel-header>
              {{ $t("profile.userData") }}
            </v-expansion-panel-header>
            <v-expansion-panel-content>
              <pre>{{ userJson }}</pre>
            </v-expansion-panel-content>
          </v-expansion-panel>
        </v-expansion-panels>
      </v-card-text>
    </v-card>
  </div>
</template>

<script>
import { UserInfo } from "@/models/account/userInfo";
import { mapGetters } from "vuex";

export default {
  name: "AccountProfile",
  props: {
    extended: {
      type: Boolean,
      required: false,
      default: true,
    },
  },
  computed: {
    ...mapGetters(["user", "userDetails"]),

    userJson() {
      return this.user ? JSON.stringify(this.user, null, 2) : "";
    },
    userInfo() {
      return new UserInfo(this.userDetails);
    },
  },
};
</script>
