<template>
     <v-dialog v-model="state" scrollable :max-width="width + `px`">
      <v-card>
        <v-card-title>{{title}}</v-card-title>
        <v-divider></v-divider>
        <v-card-text style="height:300px;">
            <v-checkbox v-for="item in initialArray" :key="item[id]" v-model="selected" :label="item[labelProp]" :value="item[valueProp]">
            </v-checkbox>
        </v-card-text>
        <v-divider></v-divider>
        <v-card-actions class="d-flex justify-lg-space-between">
          <v-btn color="indigo" @click="close">Отменить</v-btn>
          <v-btn color="success" v-if="selected.length > 0" @click="confirm(selected)">{{confirmText}}</v-btn>
        </v-card-actions>
      </v-card>
    </v-dialog>
</template>

<script>

export default {
  props: {
    initialArray: Array,
    title: String,
    confirmText: String,
    state: {
      type: Boolean,
      default: false,
    },
    width: {
      type: String,
      default: '450',
    },
    keyProp: {
      type: String,
      default: 'id',
    },
    valueProp: {
      type: String,
      default: 'id',
    },
    labelProp: {
      type: String,
      default: 'name',
    },
  },

  data() {
    return {
      selected: [],
    };
  },

  methods: {
    confirm(selected) {
      this.$emit('confirm', [...selected]);
      this.clearSelected();
    },

    close() {
      this.$emit('cancel');
      this.clearSelected();
    },

    clearSelected() {
      this.selected = [];
    },
  },
};
</script>

<style>

</style>
