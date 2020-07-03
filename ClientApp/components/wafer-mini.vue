<template>
    <v-container>
      <v-row>
        <v-col class="d-flex flex-column align-end">
          <svg :style="svgRotation" :height="size.fieldHeight" :width="size.fieldWidth" :viewBox="fieldViewBox">      
            <g v-for="(die, key) in dies" :key="die.id">
              <rect :dieIndex="key" :id="die.id" :x="die.x" :y="die.y" :width="die.width" :height="die.height" :fill="die.fill" :fill-opacity="die.fillOpacity" @mouseover="mouseOver" @click="selectDie"/>
            </g>
          </svg>
        </v-col>
        <v-col class="d-flex flex-column align-center">
            <v-btn :color="mode === 'selected' ? 'indigo' : 'grey darken-2'" class="mt-auto" fab x-small dark @click="goToSelected(selectedDies)">
              Вбр
            </v-btn>
            <v-btn :color="mode === 'dirty' ? 'indigo' : 'grey darken-2'" class="mt-auto" fab x-small dark @click="goToDirty(selectedDies)">
              Гдн
            </v-btn>
            <v-btn :color="mode === 'color' ? 'indigo' : 'grey darken-2'" class="mt-auto" fab x-small dark @click="goToColor(selectedDies)">
              Цвт
            </v-btn>
        </v-col>
      </v-row>
    </v-container>
</template>

<script>
  import Loading from 'vue-loading-overlay';
  export default {
    props: ['keyGraphicState', 'avbSelectedDies'],
    components: { Loading },
    data() {
      return {
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

    mounted() {      
      this.dies = _.cloneDeep(this.wafer.formedMapMini.dies)             
      this.initialOrientation = +this.wafer.formedMapMini.orientation;
      this.currentOrientation = this.initialOrientation; 
      this.initialize(this.dies) 
      this.goToDirty(this.selectedDies)
    },

    methods: {

      initialize: function(dies) {
        dies.forEach(die => { 
          die.fill = "#A1887F";
          die.isActive = false
          die.fillOpacity = 1.0
        })
      },

      mouseOver: function(e) {
        if(this.selectedDies.includes(+e.target.id) &&  this.mode === "color") {
          this.$store.dispatch("wafermeas/hoverWaferMini", {dieId: +e.target.id, keyGraphicState: this.keyGraphicState});
        }
      },

      mouseLeave: function() {
        if(this.mode === "color") {
          this.$store.dispatch("wafermeas/unHoverWaferMini")
        }
      },
 
      selectDie(e) {
        e.preventDefault()
        let die = this.dies[+e.currentTarget.attributes.dieIndex.value]
        let dieId = die.id      
        if (die.isActive) {
          let position = this.selectedDies.indexOf(dieId);
          if ( ~position ) {
            this.selectedDies.splice(position, 1);
            this.$store.dispatch("wafermeas/updateSelectedDies", this.selectedDies);
          } 
          else {
            this.selectedDies.push(dieId);
            this.$store.dispatch("wafermeas/updateSelectedDies", this.selectedDies);
          }
        }
      },

      goToDirty: function(selectedDies) {
        this.$store.dispatch("wafermeas/changeKeyGraphicStateMode", {keyGraphicState: this.keyGraphicState, mode: "dirty"});
        this.avbSelectedDies.forEach(avb => {
          let die = this.dies.find(d => d.id === avb)
          die.fill = this.dirtyCells.fullWafer.cells.includes(die.id) 
                     ? selectedDies.includes(die.id) ? "#F50057" : "#580000" 
                     : selectedDies.includes(die.id) ? "#00E676" : "#1B5E20"
          die.fillOpacity = 1.0
          die.isActive = true
        })
      },

      goToSelected: function(selectedDies) {
        this.$store.dispatch("wafermeas/changeKeyGraphicStateMode", {keyGraphicState: this.keyGraphicState, mode: "selected"});
        for (let i = 0; i < this.avbSelectedDies.length; i++) {
          let die = this.dies.find(d => d.id === this.avbSelectedDies[i])
          die.fillOpacity = 1.0
          die.fill = selectedDies.includes(die.id) ? "#3D5AFE" : "#8C9EFF";
          die.isActive = true
        }          
      },

      goToColor: function(selectedDies) {
        this.$store.dispatch("wafermeas/changeKeyGraphicStateMode", {keyGraphicState: this.keyGraphicState, mode: "color"});
        this.avbSelectedDies.forEach(avb => {
          let die = this.dies.find(d => d.id === avb)
          let isSelected = selectedDies.includes(die.id) 
          die.fillOpacity = 1.0
          die.fill = isSelected ? this.dieColors.find(d => d.dieId === die.id).hexColor : "#424242"
          die.isActive = true
        })
      }
    },
    
    watch:
    {
      fieldWidth: {
        immediate: true,
        handler(newVal, oldVal) {
          this.fieldViewBox = `0 0 ${this.size.fieldHeight} ${this.size.fieldWidth}`;
        }
      },

      selectedDies: function(selectedDies) {
        if(this.mode === "dirty") {
          this.goToDirty(selectedDies)
        }

        if(this.mode === "selected") {
          this.goToSelected(selectedDies)
        }

        if(this.mode === "color") {
          this.goToColor(selectedDies)
        }
      }      
    },

    computed:
    {
      selectedDies() {
        return this.$store.getters['wafermeas/selectedDies']
      },

      dirtyCells() {
        return this.$store.getters['wafermeas/getDirtyCellsByGraphic'](this.keyGraphicState)
      },

      mode() {
        return this.$store.getters['wafermeas/getKeyGraphicStateMode'](this.keyGraphicState)
      },

      size() {
        return this.$store.getters['wafermeas/size']("mini")
      },

      wafer() {
        return this.$store.getters['wafermeas/wafer']
      },

      dieColors() {
        return this.$store.getters['wafermeas/dieColors']
      },

      svgRotation() {
        return {
           transform: `rotate(${this.currentOrientation - this.initialOrientation}deg)`         
        }
      }
    }
  }
</script>

<style scoped>
  rect:hover {
    stroke: #fc0;
    stroke-width: 1.2;
  }
</style>