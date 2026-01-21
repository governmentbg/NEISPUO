<template>
  <v-btn
    v-bind="{ ...$attrs, ...commonAttrs }"
    v-on="$listeners"
  >
    <template
      v-for="(_, scopedSlotName) in $scopedSlots"
      #[scopedSlotName]="slotData"
    >
      <slot
        :name="scopedSlotName"
        v-bind="slotData"
      />
    </template>
    <template
      v-for="(_, slotName) in $slots"
      #[slotName]
    >
      <slot :name="slotName" />
    </template>
  </v-btn>
</template>

<script>
export default {
    name: "ButtonBase",
    inheritAttrs: false,
    computed: {
    commonAttrs() {
      return {

      };
    }
  }
};
</script>
