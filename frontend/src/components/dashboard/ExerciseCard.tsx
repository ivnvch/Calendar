"use client";

import { useMutation, useQueryClient } from "@tanstack/react-query";

import { Card, CardContent } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";

import type { ExerciseDetails, ExerciseStatus } from "@/lib/types";
import { updateExerciseStatus } from "@/lib/api";
import {
  ACTIVITY_META,
  EXERCISE_STATUSES,
  STATUS_META,
  cn,
} from "@/lib/utils";

interface ExerciseCardProps {
  exercise: ExerciseDetails;
  date: string;
}

export function ExerciseCard({ exercise, date }: ExerciseCardProps) {
  const queryClient = useQueryClient();
  const meta = ACTIVITY_META[exercise.activityType];

  const mutation = useMutation({
    mutationFn: (newStatus: string) =>
      updateExerciseStatus(date, exercise.id, newStatus),
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ["workoutDay", date] });
      queryClient.invalidateQueries({ queryKey: ["monthPeriod"] });
    },
  });

  const statusMeta = STATUS_META[exercise.status];

  return (
    <Card>
      <CardContent className="flex items-center gap-4 p-4">
        <div
          className={cn(
            "flex h-10 w-10 shrink-0 items-center justify-center rounded-lg text-sm font-bold",
            meta.bgColor,
            meta.color
          )}
        >
          {meta.label.slice(0, 2)}
        </div>

        <div className="flex-1 min-w-0">
          <div className="flex items-center gap-2">
            <span className="font-medium">{meta.label}</span>
            <Badge variant={statusMeta.variant} className="text-xs">
              {statusMeta.label}
            </Badge>
          </div>
          <p className="text-sm text-muted-foreground">
            {exercise.actualValue > 0
              ? `${exercise.actualValue} / ${exercise.targetValue} ${meta.unit}`
              : `Цель: ${exercise.targetValue} ${meta.unit}`}
          </p>
        </div>

        <Select
          value={exercise.status}
          onValueChange={(value) => mutation.mutate(value)}
          disabled={mutation.isPending}
        >
          <SelectTrigger className="w-[150px]">
            <SelectValue />
          </SelectTrigger>
          <SelectContent>
            {EXERCISE_STATUSES.map((status: ExerciseStatus) => (
              <SelectItem key={status} value={status}>
                {STATUS_META[status].label}
              </SelectItem>
            ))}
          </SelectContent>
        </Select>
      </CardContent>
    </Card>
  );
}
