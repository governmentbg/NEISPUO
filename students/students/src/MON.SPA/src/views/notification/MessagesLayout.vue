<template>
  <message />
</template>

<script>
import Message from '@/components/notification/Message.vue';
import Constants from '../../common/constants';

export default {
    name: 'MessagesLayout',
    components: {
        Message
    },
    data() {
      return {
        dateFormat: Constants.DATEPICKER_FORMAT,
        dateAndTimeFormat: Constants.DATE_AND_TIME_FORMAT,
      };
    }
};
</script>
