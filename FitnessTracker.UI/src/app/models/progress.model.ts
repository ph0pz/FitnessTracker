export interface MetricData {
  value: number;
  unit: string;
  average?: number; // Optional average for display
}

export interface UserMetrics {
  weight: number[]; // Array of weights for the chart (or more complex objects with date)
  waistSize: MetricData;
  bodyFat: MetricData;
  // Add more metrics as needed
}
