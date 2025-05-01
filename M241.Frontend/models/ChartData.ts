import Dataset from "./Dataset"

class ChartData {
  labels: string[];
  datasets: Dataset[];

  constructor(labels: string[], datasets: Dataset[]) {
    this.labels = labels;
    this.datasets = datasets;
  }
}

export default ChartData;