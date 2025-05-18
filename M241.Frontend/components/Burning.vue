<script setup lang="ts"></script>

<template>
  <div class="burning-wrapper">
    <h1 class="main-title">🔥 DER RAUM BRENNT 🔥</h1>

    <img src="https://media1.tenor.com/m/yHk28usHXf8AAAAd/gigi-bye-bye.gif" class="crazy-gif" id="skull1" />
    <img src="https://media1.tenor.com/m/yHk28usHXf8AAAAd/gigi-bye-bye.gif" class="crazy-gif" id="skull2" />
    <img src="https://media.giphy.com/media/3oKIPjHCmuXqdVvak8/giphy.gif" class="crazy-gif melting-clock" id="clock1" />
    <img src="https://media.giphy.com/media/VbnUQpnihPSIgIXuZv/giphy.gif" class="crazy-gif laser-cat" id="cat1" />
    <img src="https://media.giphy.com/media/oe33xf3B50fsc/giphy.gif" class="crazy-gif explosion" id="explosion1" />
    <img src="https://media.giphy.com/media/QuxqWk7m9ffxyfoa0a/giphy.gif" class="crazy-gif" id="dancing-skeleton1" />
    <img src="https://media1.tenor.com/m/jC1B62a9tAQAAAAd/gigi-gigi-slay.gif" id="falling-brick" />

    <div class="small-text-box" id="text1">EVERYTHING IS FINE. (probably)</div>
    <div class="small-text-box" id="text2">WOW! MUCH FLAMES! SO BURN!</div>
    <div class="small-text-box" id="text3">MY EYES! THE GOGGLES DO NOTHING! 🔥🔥</div>

    <div class="warning-sign" id="warning1">DANGER! MELTDOWN!</div>
    <div class="warning-sign" id="warning2">SYSTEM CRITICAL! ABANDON ALL HOPE!</div>

    <div class="glitch-overlay"></div>
  </div>
</template>

<style scoped>
.burning-wrapper {
  position: absolute;
  inset: 0;
  overflow: hidden;
  background-color: #000;
  background-image: url("https://media.giphy.com/media/3o7譙hW3zsmMv7s4/giphy.gif");
  background-repeat: repeat;
  font-family: "Comic Sans MS", "Chalkduster", "Marker Felt", sans-serif;
}

/* Title */
.main-title {
  position: absolute;
  top: 20%;
  left: 50%;
  transform: translateX(-50%);
  font-size: 5vw;
  font-weight: bold;
  background: linear-gradient(45deg, #ff0000, #ff7f00, #ffff00, #ff7f00, #ff0000);
  -webkit-background-clip: text;
  background-clip: text;
  color: transparent;
  text-shadow: 0 0 10px #fff, 0 0 20px #fff, 0 0 30px #ff4500;
  animation: crazyflames 0.1s infinite alternate, pulsate 1s infinite;
  z-index: 100;
}

/* Animations */
@keyframes crazyflames {
  0% { text-shadow: 0 0 5px #fff, 0 0 15px #ff4500; }
  100% { text-shadow: 0 0 10px #ffd700, 0 0 20px #ff4500; }
}

@keyframes pulsate {
  0%, 100% { transform: translateX(-50%) scale(1); }
  50% { transform: translateX(-50%) scale(1.05); }
}

@keyframes spin {
  0% { transform: rotate(0deg); }
  100% { transform: rotate(360deg); }
}

@keyframes shake {
  0%, 100% { transform: translate(1px, 1px) rotate(0deg); }
  10% { transform: translate(-1px, -2px) rotate(-1deg); }
  20% { transform: translate(-3px, 0px) rotate(1deg); }
}

@keyframes fall {
  0% { top: -100px; opacity: 1; }
  95% { opacity: 1; }
  100% { top: 100%; opacity: 0; }
}

@keyframes flyby {
  0% { left: -150px; top: 10%; transform: scaleX(1); }
  50% { left: 100%; top: 20%; transform: scaleX(-1); }
  100% { left: -150px; top: 80%; transform: scaleX(1); }
}

@keyframes popUp {
  0% { transform: scale(0.1); opacity: 0; }
  70% { transform: scale(1.2); opacity: 1; }
  100% { transform: scale(1); opacity: 1; }
}

@keyframes blink-border {
  0%, 100% { border-color: #ff00ff; }
  50% { border-color: #00ffff; }
}

@keyframes color-text-panic {
  0% { color: #00ff00; }
  25% { color: #ffff00; }
  50% { color: #ff00ff; }
  75% { color: #00ffff; }
  100% { color: #00ff00; }
}

@keyframes blink-background {
  0%, 100% { background-color: yellow; color: black; }
  50% { background-color: red; color: white; }
}

@keyframes glitch-flicker {
  0%, 10%, 20%, 30%, 40%, 50%, 60%, 70%, 80%, 90%, 100% { opacity: 0.05; }
  15%, 35%, 55%, 75%, 95% { opacity: 0; }
}

/* General Image Styles */
.crazy-gif {
  position: absolute;
  width: 100px;
  animation: spin 2s linear infinite, shake 0.5s infinite;
}

.melting-clock {
  width: 120px;
  animation: none;
}

.laser-cat {
  width: 150px;
  animation: flyby 10s linear infinite;
}

.explosion {
  width: 200px;
  animation: popUp 0.5s ease-out;
}

#skull1 { top: 10%; left: 10%; width: 80px; }
#skull2 { top: 70%; left: 80%; width: 120px; animation-duration: 1.5s; }
#clock1 { top: 5%; right: 5%; }
#cat1 { top: 50%; left: -150px; }
#explosion1 { top: 40%; left: 40%; width: 250px; }
#dancing-skeleton1 { top: 65%; left: 5%; width: 150px; animation: shake 0.3s infinite alternate; }

#falling-brick {
  position: absolute;
  top: -100px;
  left: 50%;
  width: 50px;
  animation: fall 3s linear infinite;
  z-index: 200;
}

/* Text & Warnings */
.small-text-box {
  position: absolute;
  border: 3px dashed #ff00ff;
  background-color: rgba(0, 0, 0, 0.7);
  padding: 10px;
  font-size: 1.5vw;
  animation: blink-border 1s infinite, color-text-panic 2s infinite;
  z-index: 50;
  color: #00ff00;
}

#text1 { top: 40%; left: 15%; transform: rotate(7deg); font-size: 1.2vw; }
#text2 { top: 60%; right: 10%; transform: rotate(-3deg); font-size: 1.8vw; }
#text3 { bottom: 5%; left: 30%; font-size: 1.4vw; text-transform: uppercase; }

.warning-sign {
  position: absolute;
  background-color: yellow;
  color: black;
  border: 5px solid red;
  font-size: 2vw;
  padding: 15px;
  font-weight: bold;
  text-transform: uppercase;
  animation: blink-background 0.5s infinite;
  z-index: 70;
}

#warning1 { top: 2%; left: 2%; transform: rotate(-10deg); }
#warning2 { bottom: 2%; right: 2%; transform: rotate(8deg); font-size: 2.5vw; }

.glitch-overlay {
  position: absolute;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  background-image: url("https://media1.giphy.com/media/cSfkdR9w2CSI3atokL/giphy.gif");
  opacity: 0.05;
  pointer-events: none;
  z-index: 1000;
  animation: glitch-flicker 5s infinite step-end;
}
</style>
