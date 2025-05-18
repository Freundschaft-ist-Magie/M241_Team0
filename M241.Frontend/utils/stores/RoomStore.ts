import { defineStore } from 'pinia';
import { get, post, put } from "~/utils/services/base/ApiService";
import Room from "~/models/Room";

export const useRoomStore = defineStore("room", () => {
  const _roomBaseUrl = "Rooms";
  const _rooms = ref([])
  const rooms = computed(() => _rooms);

  async function GetAll() {
    const _rooms: Room[] = await get(_roomBaseUrl);

    return _rooms;
  }

  async function UpdateRoom(room: Room) {
    // separate id from the room object
    const id = room.id;

    // update room values
    const _rooms: Room[] = await put(`${_roomBaseUrl}/${id}`, room);

    console.log("update room to this data", room);

    return _rooms;
  }

  // /Rooms/ping/{macAddress}
  async function PingRoom(macAddress: string) {
    // returns nothing but 200 OK
    const res: string = await get(`${_roomBaseUrl}/ping/${macAddress}`);

    return res;
  }


  return { rooms, GetAll, UpdateRoom, PingRoom };
});