import type {
  CreateExerciseDto,
  MonthDayPeriod,
  WorkoutDayDetails,
} from "./types";

const API_BASE = process.env.NEXT_PUBLIC_API_URL ?? "http://localhost:5149/api";

async function request<T>(url: string, options?: RequestInit): Promise<T> {
  const response = await fetch(`${API_BASE}${url}`, {
    headers: { "Content-Type": "application/json" },
    ...options,
  });

  if (!response.ok) {
    const body = await response.json().catch(() => null);
    throw new Error(body?.errorList?.[0]?.message ?? `HTTP ${response.status}`);
  }

  if (response.status === 204) return undefined as T;

  return response.json();
}

export function getWorkoutDay(date: string): Promise<WorkoutDayDetails> {
  return request<WorkoutDayDetails>(`/workout-days/${date}`);
}

export function getMonthPeriod(
  year: number,
  month: number
): Promise<MonthDayPeriod[]> {
  return request<MonthDayPeriod[]>(
    `/workout-days/period?year=${year}&month=${month}`
  );
}

export function createWorkoutDay(
  date: string,
  exercises: CreateExerciseDto[]
): Promise<string> {
  return request<string>("/workout-days", {
    method: "POST",
    body: JSON.stringify({ command: { date, exercises } }),
  });
}

export function updateExerciseStatus(
  date: string,
  exerciseId: string,
  status: string
): Promise<void> {
  return request<void>(
    `/workout-days/${date}/exercises/${exerciseId}/status`,
    {
      method: "PATCH",
      body: JSON.stringify({ status }),
    }
  );
}
