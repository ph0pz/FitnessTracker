
// Define the interface for a single weight log entry from the API
export interface WeightLog {
  logDate: string; // ISO string
  weight: number ;
  waistSizeCm: number;
  bodyFatPercentage: number;
  notes: string;
  // Add an ID or other properties if your API returns them
  id?: string;
}
export interface MetricDetail {
  value: number;
  unit: string;
  average?: number; // Optional average
}

export interface UserMetrics {
  weight: number[]; // Change to array to hold trend data
  waistSize: MetricDetail;
  bodyFat: MetricDetail;
  // Add other metrics as needed
}

// NEW: Interface for a single log entry from the API
export interface WeightLog {
  logDate: string; // Typically ISO string from API (e.g., "2025-07-23T13:14:12.497Z")
  weight: number;
  waistSizeCm: number;
  bodyFatPercentage: number;
  notes: string;
  id?: string; // Optional: if API returns an ID for the log entry
}