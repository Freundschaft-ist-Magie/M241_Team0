import { defineStore } from 'pinia';
import { get } from "~/utils/services/base/ApiService";
import RoomData from "~/models/RoomData";

export const useRoomDataStore = defineStore("roomData", () => {
  const _roomBaseUrl = "RoomDatas";
  const _roomDatas = ref([])
  const roomDatas = computed(() => _roomDatas);

  async function GetAll() {
    const _roomDatas: RoomData[] = await get(_roomBaseUrl);

    return _roomDatas;
  }

  return { roomDatas, GetAll };
});