// src/app/shared/models/dashboard-summary.model.ts
export interface DashboardSummary {
  username: string;
  todayDate: string; // Comes as ISO string
  caloriesConsumed: number;
  proteinConsumed: number;
  carbsConsumed: number;
  fatConsumed: number;
  dailyCalorieGoal: number;
  dailyProteinGoal: number;
  dailyCarbGoal: number;
  dailyFatGoal: number;
  completedExercisesToday: number;
  currentWeight: number;
  // If your API later provides historical weight data for the chart, it would go here
  // e.g., historicalWeightData?: { date: string; weight: number; }[];
}
export interface MacroGoalInput {
  goalType: string;
  calories: number; // Will be calculated
  protein: number;
  carbs: number;
  fat: number;
}

// Optional: If you want to pre-fill the dialog with existing data
export interface EditMacrosDialogData {
  currentGoal?: MacroGoalInput;
}
export interface MacroGoal {
  goalType: string;
  calories: number;
  protein: number;
  carbs: number;
  fat: number;
}