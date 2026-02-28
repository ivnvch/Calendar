"use client";

import { useQuery } from "@tanstack/react-query";
import { format } from "date-fns";
import { ru } from "date-fns/locale";
import { CalendarDays } from "lucide-react";

import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { ExerciseCard } from "./ExerciseCard";
import { AddExerciseForm } from "./AddExerciseForm";
import { getWorkoutDay } from "@/lib/api";

interface DayDashboardProps {
  selectedDate: Date;
}

export function DayDashboard({ selectedDate }: DayDashboardProps) {
  const dateStr = format(selectedDate, "yyyy-MM-dd");

  const { data: workoutDay, isLoading } = useQuery({
    queryKey: ["workoutDay", dateStr],
    queryFn: () => getWorkoutDay(dateStr),
    retry: (count, error) => {
      if (error.message.includes("404")) return false;
      return count < 2;
    },
  });

  const formattedDate = format(selectedDate, "d MMMM yyyy", { locale: ru });

  return (
    <Card className="h-full">
      <CardHeader className="flex flex-row items-center justify-between space-y-0 pb-4">
        <div className="flex items-center gap-2">
          <CalendarDays className="h-5 w-5 text-muted-foreground" />
          <CardTitle className="text-lg">{formattedDate}</CardTitle>
        </div>
        <AddExerciseForm date={dateStr} />
      </CardHeader>

      <CardContent className="space-y-3">
        {isLoading && (
          <p className="text-sm text-muted-foreground">Загрузка...</p>
        )}

        {!isLoading && (!workoutDay || workoutDay.exercises.length === 0) && (
          <div className="flex flex-col items-center gap-2 py-8 text-center">
            <CalendarDays className="h-10 w-10 text-muted-foreground/40" />
            <p className="text-sm text-muted-foreground">
              Нет упражнений на этот день
            </p>
            <p className="text-xs text-muted-foreground/60">
              Нажмите &quot;Добавить&quot;, чтобы создать тренировку
            </p>
          </div>
        )}

        {workoutDay?.exercises.map((exercise) => (
          <ExerciseCard
            key={exercise.id}
            exercise={exercise}
            date={dateStr}
          />
        ))}
      </CardContent>
    </Card>
  );
}
