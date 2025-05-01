class SensorData {
  value: number;
  timestamp: string;

  constructor(value: number, timestamp: string) {
    this.value = value;
    this.timestamp = timestamp;
  }
}

export default SensorData;