class Dataset {
  label: string;
  data: (number | null)[];
  fill: boolean;
  backgroundColor: string;
  borderColor: string;
  tension: number;

  constructor(
    label: string,
    data: (number | null)[],
    fill: boolean = false,
    color: string = '#000000',
    tension: number = 0.1
  ) {
    this.label = label;
    this.data = data;
    this.fill = fill;
    this.backgroundColor = color;
    this.borderColor = color;
    this.tension = tension;
  }
}

export default Dataset;