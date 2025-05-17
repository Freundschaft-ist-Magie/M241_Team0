class Room {
  id?: number;
  name: string;
  macAddress: string;
  isBurning: boolean;

  constructor(
    id: number,
    name: string,
    macAddress: string,
    isBurning: boolean
  ) {
    this.id = id;
    this.name = name;
    this.macAddress = macAddress;
    this.isBurning = isBurning;
  }
}

export default Room;
