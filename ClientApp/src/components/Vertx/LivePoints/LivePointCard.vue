<template>
    <v-card class="elevation-8">
        <v-card-title class="d-flex justify-space-between">
            {{ singleCharacteristic.characteristic }}
              <v-chip>
                <span>{{ singleCharacteristic.date }}</span>
            </v-chip>
        </v-card-title>
         <v-card-text class="d-flex">
             <v-chip class="flex-grow-1" large label color="indigo">
                <span>{{ singleCharacteristic.value }}</span>
            </v-chip>          
         </v-card-text>
       
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
