class Notification {
  id: number;
  text: string;
  timestamp: string;
  read: boolean;
  image: string | null;

  constructor(id: number, text: string, timestamp: string, read: boolean, image: string | null) {
    this.id = id;
    this.text = text;
    this.timestamp = timestamp;
    this.read = read;
    this.image = image;
  }
}

export default Notification;