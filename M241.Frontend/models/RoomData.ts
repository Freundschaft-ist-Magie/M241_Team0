import type Room from "./Room";

class RoomData {
  id: number;
  humidity: number;
  temperature: number;
  pressure: number;
  gas: number;
  timeStamp: string;
  room: Room | null;
  roomId: number;

  constructor(
    id: number,
    humidity: number,
    temperature: number,
    pressure: number,
    gas: number,
    timeStamp: string,
    roomId: number,
    room: object
  ) {
    this.id = id;
    this.humidity = humidity;
    this.temperature = temperature;
    this.pressure = pressure;
    this.gas = gas;
    this.timeStamp = timeStamp;
    this.roomId = roomId;
    this.room = room;
  }
}

export default RoomData;
