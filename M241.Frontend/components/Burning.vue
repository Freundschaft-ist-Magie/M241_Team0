<script setup lang="ts"></script>

<template>
  <div class="container">
    <h1 class="main-title">🔥 DER RAUM BRENNT 🔥</h1>

    <!-- Spinning Skulls on Fire -->
    <img
      src="https://media1.tenor.com/m/yHk28usHXf8AAAAd/gigi-bye-bye.gif"
      alt="Spinning Fire Skull"
      class="crazy-gif"
      id="skull1"
    />
    <img
      src="https://media1.tenor.com/m/yHk28usHXf8AAAAd/gigi-bye-bye.gif"
      alt="Spinning Fire Skull"
      class="crazy-gif"
      id="skull2"
    />

    <!-- Melting Clocks -->
    <img
      src="https://media.giphy.com/media/3oKIPjHCmuXqdVvak8/giphy.gif"
      alt="Melting Clock"
      class="crazy-gif melting-clock"
      id="clock1"
    />

    <!-- Laser Cats -->
    <img
      src="https://media.giphy.com/media/VbnUQpnihPSIgIXuZv/giphy.gif"
      alt="Laser Cat"
      class="crazy-gif laser-cat"
      id="cat1"
    />

    <!-- Explosions -->
    <img
      src="https://media.giphy.com/media/oe33xf3B50fsc/giphy.gif"
      alt="Explosion"
      class="crazy-gif explosion"
      id="explosion1"
    />

    <!-- Dancing Skeletons -->
    <img
      src="https://media.giphy.com/media/QuxqWk7m9ffxyfoa0a/giphy.gif"
      alt="Dancing Skeleton"
      class="crazy-gif"
      id="dancing-skeleton1"
    />

    <!-- Falling Debris -->
    <img
      src="https://media1.tenor.com/m/jC1B62a9tAQAAAAd/gigi-gigi-slay.gif"
      alt="Falling Brick"
      id="falling-brick"
    />

    <!-- Small Text Boxes -->
    <div class="small-text-box" id="text1">EVERYTHING IS FINE. (probably)</div>
    <div class="small-text-box" id="text2">WOW! MUCH FLAMES! SO BURN!</div>
    <div class="small-text-box" id="text3">MY EYES! THE GOGGLES DO NOTHING! 🔥🔥</div>

    <!-- Warning Signs -->
    <div class="warning-sign" id="warning1">DANGER! MELTDOWN!</div>
    <div class="warning-sign" id="warning2">SYSTEM CRITICAL! ABANDON ALL HOPE!</div>

    <!-- Glitch Overlay -->
    <div class="glitch-overlay"></div>
  </div>
</template>

<style scoped>
.container {
  background-color: #000000 !important;
  position: relative;
  width: 100vw;
  height: 100vh;
  margin: auto;
  background-image: url("https://media.giphy.com/media/3o7譙hW3zsmMv7s4/giphy.gif");
  background-repeat: repeat;
  overflow: hidden;
  color: #ffff00;
  font-family: "Comic Sans MS", "Chalkduster", "Marker Felt", sans-serif;
}

.main-title {
  position: absolute;
  top: 20%;
  left: 50%;
  transform: translateX(-50%);
  font-size: 5vw;
  font-weight: bold;
  text-align: center;
  color: #ff0000;
  background: linear-gradient(45deg, #ff0000, #ff7f00, #ffff00, #ff7f00, #ff0000);
  -webkit-background-clip: text;
  background-clip: text;
  color: transparent;
  text-shadow: 0 0 10px #fff, 0 0 20px #fff, 0 0 30px #ff4500, 0 0 40px #ff4500,
    0 0 50px #ff4500, 0 0 60px #ff4500, 0 0 70px #ff4500;
  animation: crazyflames 0.1s infinite alternate, pulsate 1s infinite;
  z-index: 100;
}

@keyframes crazyflames {
  0% {
    text-shadow: 0 0 5px #fff, 0 0 10px #fff, 0 0 15px #ff4500, 0 0 20px #ff4500,
      0 0 25px #ffd700, 0 0 30px #ff4500, 0 0 35px #ff0000;
  }

  100% {
    text-shadow: 0 0 8px #fff, 0 0 18px #fff, 0 0 28px #ffd700, 0 0 38px #ff4500,
      0 0 48px #ff0000, 0 0 58px #ff4500, 0 0 68px #ffd700;
  }
}

@keyframes pulsate {
  0% {
    transform: translateX(-50%) scale(1);
  }

  50% {
    transform: translateX(-50%) scale(1.05);
  }

  100% {
    transform: translateX(-50%) scale(1);
  }
}

.crazy-gif {
  position: absolute;
  width: 100px;
  height: auto;
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

/* one-time animation or different repeating */

@keyframes spin {
  0% {
    transform: rotate(0deg);
  }

  100% {
    transform: rotate(360deg);
  }
}

@keyframes shake {
  0% {
    transform: translate(1px, 1px) rotate(0deg);
  }

  10% {
    transform: translate(-1px, -2px) rotate(-1deg);
  }

  20% {
    transform: translate(-3px, 0px) rotate(1deg);
  }

  100% {
    transform: translate(1px, -2px) rotate(-1deg);
  }
}

@keyframes flyby {
  0% {
    left: -150px;
    top: 10%;
    transform: scaleX(1);
  }

  49% {
    top: 15%;
  }

  50% {
    left: 110vw;
    top: 20%;
    transform: scaleX(-1);
  }

  51% {
    left: 110vw;
    top: 70%;
    transform: scaleX(-1);
  }

  99% {
    top: 75%;
  }

  100% {
    left: -150px;
    top: 80%;
    transform: scaleX(1);
  }
}

@keyframes popUp {
  0% {
    transform: scale(0.1);
    opacity: 0;
  }

  70% {
    transform: scale(1.2);
    opacity: 1;
  }

  100% {
    transform: scale(1);
    opacity: 1;
  }
}

.small-text-box {
  position: absolute;
  border: 3px dashed #ff00ff;
  background-color: rgba(0, 0, 0, 0.7);
  color: #00ff00;
  padding: 10px;
  font-size: 1.5vw;
  transform: rotate(-5deg);
  animation: blink-border 1s infinite, color-text-panic 2s infinite;
  z-index: 50;
}

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

@keyframes blink-border {
  0% {
    border-color: #ff00ff;
  }

  50% {
    border-color: #00ffff;
  }

  100% {
    border-color: #ff00ff;
  }
}

@keyframes color-text-panic {
  0% {
    color: #00ff00;
  }

  25% {
    color: #ffff00;
  }

  50% {
    color: #ff00ff;
  }

  75% {
    color: #00ffff;
  }

  100% {
    color: #00ff00;
  }
}

@keyframes blink-background {
  0% {
    background-color: yellow;
    color: black;
  }

  50% {
    background-color: red;
    color: white;
  }

  100% {
    background-color: yellow;
    color: black;
  }
}

/* Positioning - absolute chaos! */
/* You'll need to experiment with top/left/right/bottom values */
#skull1 {
  top: 10%;
  left: 10%;
  width: 80px;
}

#skull2 {
  top: 70%;
  left: 80%;
  width: 120px;
  animation-duration: 1.5s;
  /* vary speeds */
}

#clock1 {
  top: 5%;
  right: 5%;
}

#cat1 {
  top: 50%;
  left: -150px;
  /* starts off-screen for flyby animation */
}

#explosion1 {
  top: 40%;
  left: 40%;
  width: 250px;
}

#text1 {
  top: 40%;
  left: 15%;
  transform: rotate(7deg);
  font-size: 1.2vw;
}

#text2 {
  top: 60%;
  right: 10%;
  transform: rotate(-3deg);
  font-size: 1.8vw;
}

#text3 {
  bottom: 5%;
  left: 30%;
  font-size: 1.4vw;
  text-transform: uppercase;
}

#warning1 {
  top: 2%;
  left: 2%;
  transform: rotate(-10deg);
}

#warning2 {
  bottom: 2%;
  right: 2%;
  transform: rotate(8deg);
  font-size: 2.5vw;
}

#dancing-skeleton1 {
  top: 65%;
  left: 5%;
  width: 150px;
  animation: shake 0.3s infinite alternate;
}

#falling-brick {
  top: -100px;
  left: 50%;
  width: 50px;
  animation: fall 3s linear infinite;
  z-index: 200;
}

@keyframes fall {
  0% {
    top: -100px;
    opacity: 1;
  }

  95% {
    opacity: 1;
  }

  100% {
    top: 110vh;
    opacity: 0;
  }
}

.glitch-overlay {
  position: fixed;
  top: 0;
  left: 0;
  width: 100%;
  height: 100%;
  /* https://media.giphy.com/media/l1J9EdzfAJgq15xVm/giphy.gif */
  background-image: url("https://media1.giphy.com/media/v1.Y2lkPTc5MGI3NjExamE3cng4OG13b25sZGlwODZqcXQyMHV2ODdscncxb2Iwd21od2h6bSZlcD12MV9pbnRlcm5hbF9naWZfYnlfaWQmY3Q9Zw/cSfkdR9w2CSI3atokL/giphy.gif");
  opacity: 0.05;
  pointer-events: none;
  z-index: 1000;
  animation: glitch-flicker 5s infinite step-end;
}

@keyframes glitch-flicker {
  0%,
  10%,
  12%,
  20%,
  22%,
  30%,
  32%,
  40%,
  42%,
  50%,
  52%,
  60%,
  62%,
  70%,
  72%,
  80%,
  82%,
  90%,
  92%,
  100% {
    opacity: 0.05;
  }

  11%,
  21%,
  31%,
  41%,
  51%,
  61%,
  71%,
  81%,
  91% {
    opacity: 0;
  }
}
</style>
