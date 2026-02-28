import { clsx, type ClassValue } from "clsx";
import { twMerge } from "tailwind-merge";
import type { ActivityType, ExerciseStatus } from "./types";

export function cn(...inputs: ClassValue[]) {
  return twMerge(clsx(inputs));
}

interface ActivityMeta {
  label: string;
  unit: string;
  color: string;
  bgColor: string;
}

export const ACTIVITY_META: Record<ActivityType, ActivityMeta> = {
  Running: {
    label: "Бег",
    unit: "км",
    color: "text-blue-600",
    bgColor: "bg-blue-100",
  },
  Swimming: {
    label: "Плавание",
    unit: "км",
    color: "text-cyan-600",
    bgColor: "bg-cyan-100",
  },
  Cycling: {
    label: "Велосипед",
    unit: "км",
    color: "text-green-600",
    bgColor: "bg-green-100",
  },
  Yoga: {
    label: "Йога",
    unit: "мин",
    color: "text-violet-600",
    bgColor: "bg-violet-100",
  },
  StrengthTraining: {
    label: "Силовая",
    unit: "раз/подход",
    color: "text-orange-600",
    bgColor: "bg-orange-100",
  },
};

export const ACTIVITY_TYPES: ActivityType[] = [
  "Running",
  "Swimming",
  "Cycling",
  "Yoga",
  "StrengthTraining",
];

export const STATUS_META: Record<
  ExerciseStatus,
  { label: string; variant: "default" | "secondary" | "destructive" | "outline" }
> = {
  Planned: { label: "Запланировано", variant: "outline" },
  InProgress: { label: "В процессе", variant: "secondary" },
  Completed: { label: "Выполнено", variant: "default" },
  Skipped: { label: "Пропущено", variant: "destructive" },
};

export const EXERCISE_STATUSES: ExerciseStatus[] = [
  "Planned",
  "InProgress",
  "Completed",
  "Skipped",
];
