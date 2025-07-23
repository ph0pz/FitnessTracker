export interface Meal {
  id: string;
  date?: string; // YYYY-MM-DD
  mealName: string;
  time?: string; // HH:MM AM/PM
  name: string;
  calories: number;
  protein: number;
  carbs: number;
  fat: number;
  notes?: string; // Optional field for additional notes
}
export interface SavedItem {
  id: string;
  name?: string;
  calories: number;
  protein: number;
  carbs: number;
  fat: number;
  notes?: string;
}