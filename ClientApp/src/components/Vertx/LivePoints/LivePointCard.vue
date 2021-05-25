<template>
    <v-card class="elevation-8">
        <v-card-title class="d-flex">
           <v-card class="d-flex elevation-8">
            <v-card-title class="d-flex flex-grow-1">
              <span>{{ singleCharacteristic.characteristic }}</span>
            </v-card-title>
           </v-card>
        </v-card-title>
         <v-card-text class="d-flex">
             <v-chip class="flex-grow-1" large label color="indigo">
                <span>{{ singleCharacteristic.value + ' ' + singleCharacteristic.unit }}</span>
            </v-chip>
         </v-card-text>
        <v-card-actions class="d-flex">
           <v-chip class="flex-grow-1" label>
                <span>{{ singleCharacteristic.date }}</span>
            </v-chip>
        </v-card-actions>
    </v-card>
</template>

<script>

export default {
  props: ['singleCharacteristic'],
  data() {
    return {

    };
  },
};

</script>
