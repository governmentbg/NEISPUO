<template>
  <div>
    <grid
      :ref="'messagesGrid' + _uid"
      v-model="selectedItems"
      url="/api/message/listMy"
      :headers="headers"
      :title="$t('messages.title')"
      show-expand
      :expanded.sync="expandedItems"
      show-select
    >
      <template v-slot:[`item.sendDate`]="{ item }">
        {{ item.sendDate ? $moment(item.sendDate).format(dateAndTimeFormat) : '' }}
      </template>

      <template v-slot:[`item.isRead`]="{ item }">
        <v-chip
          :color="item.isRead === true ? 'success' : 'error'"
          small
        >
          <yes-no :value="item.isRead" />
        </v-chip>
      </template>

      <template v-slot:[`expanded-item`]="{ item }">
        <td
          :colspan="headers.length"
          class="py-3"
        >
          <!-- eslint-disable-next-line vue/no-v-html -->
          <div v-html="item.content" />
        </td>
      </template>

      <template #footerPrepend>
        <button-tip
          v-if="hasSelectedItems"
          color="error"
          icon-name="mdi-delete"
          icon-color="white"
          tooltip="buttons.deleteSelected"
          text="buttons.deleteSelected"
          bottom
          iclass=""
          small
          @click="deleteSelected()"
        />
        <button-tip
          v-if="hasSelectedItems"
          color="primary"
          icon-name="mdi-archive"
          icon-color="white"
          tooltip="buttons.archiveSelected"
          text="buttons.archiveSelected"
          bottom
          iclass=""
          small
          @click="archiveSelected()"
        />
      </template>

      <template #actions="item">
        <button-group>
          <button-tip
            icon
            icon-name="mdi-eye"
            icon-color="primary"
            tooltip="buttons.review"
            bottom
            iclass=""
            small
            :to="`/message/${item.item.id}/details`"
          />
          <button-tip
            icon
            icon-name="mdi-check"
            icon-color="primary"
            tooltip="buttons.markRead"
            bottom
            iclass=""
            small
            :disabled="item.item.isRead"
            @click="markRead(item.item.id)"
          />
          <button-tip
            icon
            icon-name="mdi-archive"
            icon-color="primary"
            tooltip="buttons.archive"
            bottom
            iclass=""
            small
            @click="archiveMessage(item.item.id)"
          />
          <button-tip
            icon
            icon-name="mdi-delete"
            icon-color="error"
            tooltip="buttons.delete"
            bottom
            iclass=""
            small
            @click="deleteMessage(item.item.id)"
          />
        </button-group>
      </template>
    </grid>

    <v-overlay :value="saving">
      <v-progress-circular
        indeterminate
        size="64"
      />
    </v-overlay>
    <confirm-dlg ref="confirm" />
  </div>
</template>

<script>
import Grid from '@/components/wrappers/grid.vue';
import Constants from '@/common/constants';
import { mapGetters, mapActions } from 'vuex';

export default {
  name: 'MessageComponent',
  components: { Grid },
  data() {
    return {
      saving: false,
      dateAndTimeFormat: Constants.DATE_AND_TIME_FORMAT,
      expandedItems: [],
      selectedItems: [],
      headers: [
        {
          text: this.$t('messages.headers.isRead'),
          value: 'isRead',
        },
        {
          text: this.$t('messages.headers.sender'),
          value: 'senderName',
        },
        {
          text: this.$t('messages.headers.subject'),
          value: 'subject',
        },
        // {
        //   text: this.$t('messages.headers.content'),
        //   value: 'content',
        // },
        {
          text: this.$t('messages.headers.sendDate'),
          value: 'sendDate',
        },
        {
          text: ' ',
          value: "controls",
          sortable: false,
          filterable: false,
          align: 'end',
        },
      ]
    };
  },
  computed: {
    ...mapGetters(['user']),
    hasSelectedItems() {
      return Array.isArray(this.selectedItems) && this.selectedItems.length > 0;
    }
  },
  methods: {
    ...mapActions(['countMyUnreadMessages']),
    refresh() {
      const grid = this.$refs['messagesGrid' + this._uid];
      if (grid) {
        grid.get();
      }
    },
    markRead(id) {
      this.$api.message.markRead(id)
        .then(() => {
          this.$notifier.success('',this.$t('common.saveSuccess'));
          this.refresh();
          this.countMyUnreadMessages();
        })
        .catch(() => {
          this.$notifier.error('', this.$t('common.saveError'));
        });
    },
    archiveMessage(id) {
      this.$api.message.archive(id)
        .then(() => {
          this.$notifier.success('',this.$t('common.saveSuccess'));
          this.refresh();
          this.countMyUnreadMessages();
        })
        .catch(() => {
          this.$notifier.error('',this.$t('common.saveError'));
        });
    },
    async deleteMessage(id) {
      if(await this.$refs.confirm.open(this.$t('buttons.delete'), this.$t('common.confirm'))){
        this.$api.message.delete(id)
        .then(() => {
          this.$notifier.success('',this.$t('common.deleteSuccess'));
          this.refresh();
          this.countMyUnreadMessages();
        })
        .catch(() => {
          this.$notifier.error('',this.$t('common.deleteError'));
        });
      }
    },
    async deleteSelected() {
      if(await this.$refs.confirm.open(this.$t('buttons.deleteSelected'), this.$t('common.confirm'))){
        const model = {
          ids:  this.selectedItems.map(x => x.id)
        };

        this.$api.message.deleteSelected(model)
          .then(() => {
            this.$notifier.success('',this.$t('common.deleteSuccess'));
            this.$helper.clearArray(this.selectedItems);
            this.refresh();
            this.countMyUnreadMessages();
          })
          .catch(() => {
            this.$notifier.error('',this.$t('common.deleteError'));
          });
      }
    },
    async archiveSelected() {
      if(await this.$refs.confirm.open(this.$t('buttons.archiveSelected'), this.$t('common.confirm'))){
        const model = {
          ids:  this.selectedItems.map(x => x.id)
        };

        this.$api.message.archiveSelected(model)
          .then(() => {
            this.$notifier.success('',this.$t('common.saveSuccess'));
            this.$helper.clearArray(this.selectedItems);
            this.refresh();
            this.countMyUnreadMessages();
          })
          .catch(() => {
            this.$notifier.error('',this.$t('common.saveError'));
          });
      }
    }
  },
};
</script>
