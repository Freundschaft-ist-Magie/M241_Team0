export async function getConfig() {
  const config = await $fetch('/config.json');
  return config;
}