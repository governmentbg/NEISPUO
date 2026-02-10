<template>
  <v-card>
    <v-card-title>
      <v-icon left>
        fa-child
      </v-icon>{{ this.$tc('student.title', 2) }}
    </v-card-title>
    <v-card-text>
      <v-treeview
        :items="items"
        :load-children="loadChildren"
        :open.sync="open"
        color="primary"
        open-on-click
        transition
        dense
      >
        <template v-slot:append="{ item }">
          {{ item.total }}
          <!-- <v-chip
            v-if="item.total"
            color="primary"
            outlined
            small
          >
            {{ item.total }}
          </v-chip> -->
        </template>
      </v-treeview>
    </v-card-text>
  </v-card>
</template>

<script>
export default {
  name: 'StudentStats',
  data() {
    return {
      open: [],
      items: [
        {
          name: this.$t('dashboards.totalStudentsCountLabel'),
          isRootLeaf: true,
          total: 0,
          children: []
        }
      ]
    };
  },
  mounted() {
    this.loadStudentsCount();
  },
  methods: {
    loadStudentsCount() {
      this.$api.admin.getStudentsCount()
      .then((response) => {
        if(response.data) {
          this.items[0].total = response.data;
        }
      })
      .catch((error) => {
        console.log(error.response);
      });
    },
    async loadChildren(item) {
      if(item.isRootLeaf) {
        const data = (await this.$api.admin.getStudentsCountGroupByClassType()).data;
        if (data) {
          item.children.push(...data);
        }
      } else {
        const data = (await this.$api.admin.getStudentsCountByClassType(item.classTypeId)).data;
        if (data) {
          item.children.push(...data);
        }
      }
    }
  }
};
</script>
