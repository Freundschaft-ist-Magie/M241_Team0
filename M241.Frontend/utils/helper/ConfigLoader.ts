export async function getConfig(): Promise<any> {
  const config = await $fetch('/config.json');
  return config;
}