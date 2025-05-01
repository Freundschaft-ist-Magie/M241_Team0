class Room {
  id: number;
  macAddress: string;
  isBurning: boolean;

  constructor(
    id: number,
    macAddress: string,
    isBurning: boolean
  ) {
    this.id = id;
    this.macAddress = macAddress;
    this.isBurning = isBurning;
  }
}

export default Room;
