import { defineStore } from 'pinia';
import { get } from "~/utils/services/base/ApiService";
import RoomData from "~/models/RoomData";

export const useRoomDataStore = defineStore("roomData", () => {
  const _roomBaseUrl = "RoomDatas";
  const _roomDatas = ref([])
  const _roomDatas2 = ref([])
  const roomDatas = computed(() => _roomDatas);

  async function GetAll() {
    const _roomDatas: RoomData[] = await get(_roomBaseUrl);

    return _roomDatas;
  }

  async function GetLast20() {
    const _roomDatas2: RoomData[] = await get(_roomBaseUrl + "?maxPageSize=20");

    return _roomDatas2;
  }

  return { roomDatas, GetAll, GetLast20, _roomDatas2 };
});