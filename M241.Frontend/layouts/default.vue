<template>
  <div class="bg-white min-h-screen p-4">
    <MainLayoutHeader />

    <slot />
  </div>
</template>

<style scoped>
.bg {
  background: #141a22;
  background: linear-gradient(
    305deg,
    rgba(20, 26, 34, 1) 0%,
    rgba(0, 96, 185, 1) 0%,
    rgba(175, 216, 245, 1) 100%
  );
}
</style>
