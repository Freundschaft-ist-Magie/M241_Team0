class ChartOptions {
  scales: {
    y: {
      beginAtZero: boolean;
    };
  };
  plugins: {
    legend: {
      display: boolean;
    };
    tooltip: {
      mode: string;
    };
  };
  elements: {
    line: {
      tension: number;
    };
  };
  spanGaps: boolean;

  constructor() {
    this.scales = {
      y: {
        beginAtZero: true,
      },
    };
    this.plugins = {
      legend: {
        display: true,
      },
      tooltip: {
        mode: "index",
      },
    };
    this.elements = {
      line: {
        tension: 0.4, // Adds a slight curve to the line
      },
    };
    this.spanGaps = true; // This connects the line across null values
  }
}

export default ChartOptions;
