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
    props: ['keyGraphicState', 'avbSelectedDies', 'dirtyCells'],
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
      this.goToDirty()
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
          let position =  this.selectedDies.indexOf(dieId);
          if ( ~position ) {
            this.selectedDies.splice(position, 1);
            this.$store.dispatch("wafermeas/updateSelectedDies", this.selectedDies);
          } 
          else {
            this.selectedDies.push(dieId);
            this.$store.dispatch("wafermeas/updateSelectedDies", this.selectedDies);
          }
          this.goToSelected()
        }
      
      },

      goToDirty: function() {
        this.$store.dispatch("wafermeas/changeKeyGraphicStateMode", {keyGraphicState: this.keyGraphicState, mode: "dirty"});
        this.avbSelectedDies.forEach(avb => {
          let die = this.dies.find(d => d.id === avb)
          die.fill = this.dirtyCells.includes(die.id) ? "#E91E63" : "#4CAF50"
          die.fillOpacity = this.selectedDies.includes(die.id) ? 1.0 : 0.5
          die.isActive = true
        })
      },

      goToSelected: function() {
        this.$store.dispatch("wafermeas/changeKeyGraphicStateMode", {keyGraphicState: this.keyGraphicState, mode: "selected"});
        for (let i = 0; i < this.avbSelectedDies.length; i++) {
          let die = this.dies.find(d => d.id === this.avbSelectedDies[i])
          die.fillOpacity = 1.0;
          die.fill = this.selectedDies.includes(die.id) ? "#3D5AFE" : "#8C9EFF";
          die.isActive = true
        }          
      },

      goToColor: function() {
        this.$store.dispatch("wafermeas/changeKeyGraphicStateMode", {keyGraphicState: this.keyGraphicState, mode: "color"});
        this.avbSelectedDies.forEach(avb => {
          let die = this.dies.find(d => d.id === avb)
          let isSelected = this.selectedDies.includes(die.id) 
          die.fill = isSelected ? this.dieColors.find(d => d.dieId === die.id).hexColor : "#303030"
          die.fillOpacity = isSelected ? 1.0 : 0.5
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

      

      dirtyCells: function() {        
        if(this.mode === "dirty") {
          this.avbSelectedDies.forEach(avb => {
            let die = this.dies.find(d => d.id === avb)
            die.fill = this.dirtyCells.includes(die.id) ? "#E91E63" : "#4CAF50"
            die.isActive = true
          })
        }

        if(this.mode === "selected") {
          for (let i = 0; i < this.avbSelectedDies.length; i++) {
            let die = this.dies.find(d => d.id === this.avbSelectedDies[i])
            die.fill = "#8C9EFF";
            die.isActive = true
          }  
        
          for (let i = 0; i < this.selectedDies.length; i++) {            
            this.dies.find(d => d.id === this.selectedDies[i]).fill = "#3D5AFE";           
          }
        }
      }
    },

    computed:
    {
      selectedDies() {
        return this.$store.getters['wafermeas/selectedDies']
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