<template>
    <v-container>
      <v-row>
        <v-col class="d-flex flex-column align-end">
          <svg :style="svgRotation" :height="size.fieldHeight" :width="size.fieldWidth" :viewBox="fieldViewBox">      
              <g v-for="(die, key) in dies" :key="die.id">
                  <rect :dieIndex="key" :x="die.x" :y="die.y" :width="die.width" :height="die.height" :fill="die.fill" />
              </g>
          </svg>
        </v-col>
        <v-col class="d-flex flex-column align-center">
            <v-btn :color="mode === 'selected' ? 'indigo' : 'grey darken-2'" class="mt-auto" fab x-small dark @click="goToSelected">
              Вбр
            </v-btn>
            <v-btn :color="mode === 'dirty' ? 'indigo' : 'grey darken-2'" class="mt-auto" fab x-small dark @click="goToDirty">
              Гдн
            </v-btn>
            <v-btn :color="mode === 'color' ? 'indigo' : 'grey darken-2'" class="mt-auto" fab x-small dark @click="goToColor">
              Цвт
            </v-btn>
        </v-col>
      </v-row>
    </v-container>
</template>

<script>
  import Loading from 'vue-loading-overlay';
  export default {
    props: ['avbSelectedDies', 'dirtyCells'],
    components: { Loading },
    data() {
      return {
        mode: "",
        dies: [],
        isloading: false,
        activeBtn: 1,
        showNav: false,
        x: 0,
        y: 0,
        initialOrientation: -1,
        currentOrientation: -1,
        fieldViewBox: ""
      }
    },

    mounted()
    {      
      this.dies = _.cloneDeep(this.wafer.formedMapMini.dies)             
      this.initialOrientation = +this.wafer.formedMapMini.orientation;
      this.currentOrientation = this.initialOrientation; 
      this.initialize(this.dies) 
      this.goToDirty()
    },

    methods: {
      
      initialize: function(dies) {
        dies.forEach(cell => cell.fill = "#A1887F")
      },

      goToDirty: function() {
        this.mode = "dirty"
        this.avbSelectedDies.forEach(avb => {
          let die = this.dies.find(d => d.id === avb)
          die.fill = this.dirtyCells.includes(die.id) ? "#E91E63" : "#4CAF50"
        })
      },

      goToSelected: function() {
        this.mode = "selected"
        for (var i = 0; i < this.avbSelectedDies.length; i++) {
          this.dies.find(d => d.id === this.avbSelectedDies[i]).fill = "#8C9EFF";
        }  
        
        for (var i = 0; i < this.selectedDies.length; i++) {            
          this.dies.find(d => d.id === this.selectedDies[i]).fill = "#3D5AFE";           
        }
      },

      goToColor: function() {
        this.mode = "color"
      }
    },
    
    watch:
    {
      fieldWidth: {
        immediate: true,
        handler(newVal, oldVal) {
          this.fieldViewBox = `0 0 ${this.size.fieldHeight} ${this.size.fieldWidth}`;
        }
      }
    },

    computed:
    {
      selectedDies() {
        return this.$store.getters['wafermeas/selectedDies']
      },
      size() {
        return this.$store.getters['wafermeas/size']("mini")
      },
      wafer() {
        return this.$store.getters['wafermeas/wafer']
      },
      svgRotation() {
        return {
           transform: `rotate(${this.currentOrientation - this.initialOrientation}deg)`         
        }
      }
    }
  }
</script>
