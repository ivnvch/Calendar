export type ActivityType =
  | "Running"
  | "Swimming"
  | "Cycling"
  | "Yoga"
  | "StrengthTraining";

export type ExerciseStatus =
  | "Planned"
  | "InProgress"
  | "Completed"
  | "Skipped";

export interface WorkoutDayDetails {
  id: string;
  date: string;
  exercises: ExerciseDetails[];
}

export interface ExerciseDetails {
  id: string;
  activityType: ActivityType;
  status: ExerciseStatus;
  targetValue: number;
  actualValue: number;
  createdAt: string;
}

export interface MonthDayPeriod {
  date: string;
  exerciseCount: number;
}

export interface CreateExerciseDto {
  activityType: ActivityType;
  targetValue: number;
}
